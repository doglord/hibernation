using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public class GameController : MonoBehaviour
{
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

    bool GAME_OVER = false;

    public float BaseAsteroidImpact = 0.15f;


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
    System.TimeSpan totalTime;
    float seconds = 0f;
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

        totalTime = System.TimeSpan.FromHours(26);
        // totalTime = System.TimeSpan.FromSeconds(11);
    }

    public AudioSource pulse;
    public TMP_Text timerText;
    bool AutopilotEnabled = true;
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
        stat_Vitals += UnityEngine.Random.Range(-idleVarianceStrength, idleVarianceStrength) * Time.deltaTime;
        
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

        if (stat_Vitals >= 0.66f && !pulse.isPlaying)
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
            EndGame(Ending.Paralyzed);

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

    int maxHullStrengthLevel = 3;
    int currentHullStrengthLevel = 0;
    public void OnHullPanelPressLeft()
    {
        currentHullStrengthLevel = Mathf.Max(0, currentHullStrengthLevel - 1);
    }
    public AudioSource hullRightButton;
    public void OnHullPanelPressRight()
    {
        currentHullStrengthLevel = Mathf.Min(maxHullStrengthLevel, currentHullStrengthLevel + 1);
        if (!hullRightButton.isPlaying)
            hullRightButton.Play();
    }
    public void OnShipCollision(Asteroid asteroid)
    {
        if(currentHullStrengthLevel < ((int)asteroid.type + 1))
        {
            // hull breach
            Debug.Log("HULL BREACH");
            EndGame(Ending.HullBreach);
        }
        else
        {
            Debug.Log("HULL HIT");
            stat_Vitals = Mathf.Min(1f, stat_Vitals + BaseAsteroidImpact * ((int)asteroid.type + 1));
            
            // awake
            if(stat_Vitals >= 1f)
                EndGame(Ending.Awakened);
        }

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

    void EndGame(Ending ending)
    {
        if(GAME_OVER) return;

        GAME_OVER = true;
        Debug.Log("GAME OVER");

        FadePanel.Inst.RunFade(2f);
    }

    public GameObject ALL_IS_LOST;
    void OnFadeOutComplete()
    {
        IEnumerator Delay()
        {
            ALL_IS_LOST.SetActive(true);
            yield return new WaitForSeconds(2f);
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
