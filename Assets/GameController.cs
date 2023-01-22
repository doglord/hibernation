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

    void Start()
    {
        initStat_Anesthetic = stat_Anesthetic;

        fillbar_Power.value = (stat_Power / max_power);
        fillbar_Vitals.value = stat_Vitals;

        fillbar_Anesthetic.value = stat_Anesthetic;
        slider_Anesthetic.value = stat_Anesthetic;

        ShipCollider.onShipCollision += OnShipCollision;
    }

    void Update()
    {
        // jitter bc he is a mess
        // stat_Vitals += UnityEngine.Random.Range(-idleVarianceStrength, idleVarianceStrength) * Time.deltaTime;
        
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

        

        stat_Power = Mathf.Max(stat_Power - vitalsPowerDraw * Time.deltaTime, 0f);
        stat_Power = Mathf.Max(stat_Power - hullPowerDraw * Time.deltaTime, 0f);

        UpdateDashboard();
    }

    void UpdateDashboard()
    {
        fillbar_Vitals.value = stat_Vitals;
        fillbar_Anesthetic.value = stat_Anesthetic;
        fillbar_Power.value = (stat_Power / max_power);
        
        hullStrengthDisplay.text = $"{currentHullStrengthLevel}";
    }

    void OnGUI()
    {
        GUILayout.Label($"Power: {stat_Power}");
        GUILayout.Label($"Vitals: {stat_Vitals}");
        GUILayout.Label($"Anesthetic: {stat_Anesthetic}");
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
    public void OnHullPanelPressRight()
    {
        currentHullStrengthLevel = Mathf.Min(maxHullStrengthLevel, currentHullStrengthLevel + 1);
    }


    public void OnShipCollision(Asteroid asteroid)
    {
        if(currentHullStrengthLevel < ((int)asteroid.type + 1))
        {
            // hull breach
            Debug.Log("HULL BREACH");
            EndGame();
        }

        Destroy(asteroid.gameObject);
    }

    void EndGame()
    {
        Debug.Log("GAME OVER");
    }
}
