using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public class GameController : MonoBehaviour
{
    public AudioSource theme;
    float stat_Power = 0.5f;
    float max_power = 1f;

    public float idleVarianceStrength = 0.5f;
    public float powerGain = 0.2f;
    public Slider fillbar_Power;

    float stat_Vitals = 0.5f;
    public Slider fillbar_Vitals;

    float stat_Anesthetic = 0.5f;
    float initStat_Anesthetic;
    public Slider fillbar_Anesthetic;
    public Slider slider_Anesthetic;
    float vitalsPowerDraw => stat_Anesthetic * AnestheticPowerDrawMultiplier;
    public float AnestheticPowerDrawMultiplier = 0.1f;

    float hullPowerDraw => currentHullStrengthLevel * HullStrengthPowerDrawMultiplier;
    public float HullStrengthPowerDrawMultiplier = 0.25f;

    public float vitalDecayRate = 0.5f;

    public TMP_Text hullStrengthDisplay;
    public TMP_Text powerDisplayText;
    public GameObject asteroidWarningPopup;
    public TMP_Text asteroidWarningSizeText;

    public bool GAME_OVER = false;

    public float BaseAsteroidImpact = 0.15f;

    public static GameController Inst;
    void Awake(){Inst = this;}


    // pressure loss == vacuum
    // electricity
    // vitals

    // autopilot does x to maintain these numbers
    // AUTOPILOT: takes sensor data, evaluates, makes a correction to keep in line


    // autopilot costs power

        // input power from solar

    // power loss == pressure loss == vital loss

    // every turn:
        // ++ power (solar)
        // 
    [HideInInspector]
    public System.TimeSpan totalTime;
    [HideInInspector]
    public float seconds = 0f;
    void Start()
    {
        initStat_Anesthetic = stat_Anesthetic;

        fillbar_Power.value = (stat_Power / max_power);
        fillbar_Vitals.value = stat_Vitals;

        fillbar_Anesthetic.value = stat_Anesthetic;
        slider_Anesthetic.value = stat_Anesthetic;

        ShipCollider.onShipCollision += OnShipCollision;
        AsteroidController.onAsteroidSpawned += OnAsteroidSpawned;
        FadePanel.Inst.onFadeComplete.AddListener(OnFadeOutComplete);

        theme.loop = true;
        theme.Play();
        totalTime = System.TimeSpan.FromHours(11);

        ToggleAutoPilot();
    }

    public AudioSource pulse;
    public TMP_Text timerText;
    public bool AutopilotEnabled = false;
    void Update()
    {
        if(GAME_OVER) return;

        if(System.TimeSpan.FromSeconds(seconds) > totalTime)
        {
            EndGame(Ending.WIN);
            return;
        }


        seconds += Time.deltaTime;

        // jitter bc he is a mess
        var jitter = UnityEngine.Random.Range(-idleVarianceStrength, idleVarianceStrength);
        if(AutopilotEnabled)
        {
            if((jitter < 0f && stat_Vitals < 0.5f) || (jitter > 0f && stat_Vitals > 0.5f))
                jitter = jitter * 0.25f;
        }
        stat_Vitals += jitter * Time.deltaTime;
        
        // gain solar power
        stat_Power = Mathf.Min(stat_Power + powerGain * Time.deltaTime, max_power);

        if(stat_Anesthetic <= initStat_Anesthetic)
        {
            stat_Vitals = Mathf.Max(0f, stat_Vitals - (initStat_Anesthetic - stat_Anesthetic) * vitalDecayRate * Time.deltaTime);
        }
        else if(stat_Vitals < 0.5f && stat_Anesthetic > initStat_Anesthetic)
        {
            stat_Vitals = Mathf.Min(0.5f, stat_Vitals + (stat_Anesthetic - initStat_Anesthetic) * vitalDecayRate * Time.deltaTime);
        }
        else if(stat_Vitals > 0.5f && stat_Anesthetic > initStat_Anesthetic)
        {
            stat_Vitals = Mathf.Max(0.5f, stat_Vitals - (stat_Anesthetic - initStat_Anesthetic) * vitalDecayRate * Time.deltaTime);
        }

        

        stat_Power = Mathf.Max(stat_Power - vitalsPowerDraw * Time.deltaTime, 0f);
        stat_Power = Mathf.Max(stat_Power - hullPowerDraw * Time.deltaTime, 0f);

        if (stat_Vitals <= 0.33f && !pulse.isPlaying)
            pulse.Play();
        if(AutopilotEnabled)
        {
            stat_Power += max_power * 0.05f * Time.deltaTime;
            stat_Anesthetic = initStat_Anesthetic;
            slider_Anesthetic.value = stat_Anesthetic;
        }

        if(stat_Power <= 0f)
            EndGame(Ending.PowerLoss);

        if(stat_Vitals <= 0f)
            EndGame(Ending.Awakened);

        UpdateDashboard();
    }

    float powerDisplayMultiplier = 1000f;
    void UpdateDashboard()
    {
        fillbar_Vitals.value = stat_Vitals;
        fillbar_Anesthetic.value = stat_Anesthetic;
        fillbar_Power.value = (stat_Power / max_power);
        
        hullStrengthDisplay.text = $"{currentHullStrengthLevel}";
        powerDisplayText.text = $"{(stat_Power * powerDisplayMultiplier).ToString("N0")}/{(max_power * powerDisplayMultiplier).ToString("N0")}";
        timerText.text = $"{(totalTime - System.TimeSpan.FromSeconds(seconds)).ToString(@"dd\.hh\:mm\:ss")}";
    }

    void OnGUI()
    {
        // GUILayout.Label($"Power: {stat_Power}");
        // GUILayout.Label($"Vitals: {stat_Vitals}");
        // GUILayout.Label($"Anesthetic: {stat_Anesthetic}");
    }


    public void OnAnestheticSliderValueChanged(float val)
    {
        stat_Anesthetic = slider_Anesthetic.value;
    }

    public AudioSource hullLeftButton;
    int maxHullStrengthLevel = 3;
    int currentHullStrengthLevel = 0;
    public void OnHullPanelPressLeft()
    {
        if (!hullLeftButton.isPlaying && currentHullStrengthLevel > 0)
            hullLeftButton.Play();
        currentHullStrengthLevel = Mathf.Max(0, currentHullStrengthLevel - 1);
    }
    public AudioSource hullRightButton;
    public void OnHullPanelPressRight()
    {
        if (!hullRightButton.isPlaying && currentHullStrengthLevel < 3)
            hullRightButton.Play();
        currentHullStrengthLevel = Mathf.Min(maxHullStrengthLevel, currentHullStrengthLevel + 1);
    }
    public float APPowerHitDrain = 0.1f;
    public void OnShipCollision(Asteroid asteroid)
    {
        if(!AutopilotEnabled)
        {
            if(currentHullStrengthLevel < ((int)asteroid.type + 1))
            {
                EndGame(Ending.HullBreach);
            }
            else
            {
                stat_Vitals = Mathf.Min(1f, stat_Vitals + BaseAsteroidImpact * ((int)asteroid.type + 1) * 0.15f);
            }
        }
        else
        {
            stat_Power = Mathf.Max(0f, stat_Power - BaseAsteroidImpact * ((int)asteroid.type + 1) * 0.15f);
        }
        

        if(stat_Vitals >= 1f)
            EndGame(Ending.Paralyzed);

        Destroy(asteroid.gameObject);
    }
    public void OnAsteroidSpawned(Asteroid asteroid)
    {
        IEnumerator DelayThenStart()
        {
            yield return new WaitForSeconds(2f);
            asteroid.started = true;
            yield return new WaitForSeconds(3f);
            asteroidWarningPopup.SetActive(false);
        }
        StartCoroutine(DelayThenStart());

        asteroidWarningPopup.SetActive(true);
        asteroidWarningSizeText.text = $"[{asteroid.type.ToString()}]";
    }
    public void OnResetAnestheticLevel()
    {
        stat_Anesthetic = initStat_Anesthetic;
        slider_Anesthetic.value = stat_Anesthetic;
    }

    enum Ending
    {
        HullBreach,
        Paralyzed,
        Awakened,
        PowerLoss,
        WIN
    }
    Ending currentEnding;
    public Animator awakenAnimator;
    public AudioSource panicAudio;
    public AudioSource vacuumAudio;
    public AudioSource decompressionChamber;
    void EndGame(Ending ending)
    {
        if(GAME_OVER) return;

        GAME_OVER = true;


        currentEnding = ending;
        
        Debug.Log($"GAME OVER: {ending.ToString()}");

        if(ending == Ending.HullBreach)
        {
            IEnumerator runhullbreach()
            {
                yield return new WaitForSeconds(2f);
                // play sound
                Debug.Log("VACUM");
                vacuumAudio.Play();
                yield return new WaitForSeconds(vacuumAudio.clip.length);
                FadePanel.Inst.RunFade(0f);
            }
            StartCoroutine(runhullbreach());
            // was hit
            // wait a tick
            // play vacuum sound
            // smash to black at the end
        }
        else if(ending == Ending.Awakened)
        {
            IEnumerator runawakened()
            {
                yield return new WaitForSeconds(1f);
                // play animation
                Debug.Log("ANIM");
                awakenAnimator.Play("open");
                decompressionChamber.Play();
                yield return new WaitForSeconds(/*anim duration + beat*/4.5f);
                FadePanel.Inst.RunFade(0f);
            }
            StartCoroutine(runawakened());
            // play animation
            // wait a beat
            // smash to black
            // play scream
        }
        else
            FadePanel.Inst.RunFade(2f);
    }

    public GameObject ALL_IS_LOST;
    void OnFadeOutComplete()
    {
        IEnumerator Delay()
        {
            if(currentEnding == Ending.Awakened)
            {
                Debug.Log("Play scream");
                panicAudio.Play();
                yield return new WaitForSeconds(2f);
            }
            ALL_IS_LOST.SetActive(true);
            yield return new WaitForSeconds(3f);
            ALL_IS_LOST.SetActive(false);
            yield return new WaitForSeconds(1f);
            Bootstrap.Inst.RestartScene();
        }

        StartCoroutine(Delay());
        
    }

    public FakeKnobPanel hullPanel;
    public void ToggleAutoPilot()
    {
        AutopilotEnabled = !AutopilotEnabled;

        if(AutopilotEnabled)
        {
            slider_Anesthetic.interactable = false;
            hullPanel.DisableInteract();
            currentHullStrengthLevel = 0;
        }
        else
        {
            slider_Anesthetic.interactable = true;
            hullPanel.EnableInteract();
        }
    }

    void OnDestroy()
    {
        ShipCollider.onShipCollision -= OnShipCollision;
        AsteroidController.onAsteroidSpawned -= OnAsteroidSpawned;
        FadePanel.Inst.onFadeComplete.RemoveListener(OnFadeOutComplete);
    }
}
