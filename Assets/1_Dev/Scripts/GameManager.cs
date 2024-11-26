using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI earthquakeMagnitudeText; 
    public Slider earthquakeSlider; 
    public float earthquakeMagnitude = 0.2f;
    public GameObject UiPanel;

    private void Start()
    {
        earthquakeSlider.minValue = 0;
        earthquakeSlider.maxValue = 10000;
        earthquakeSlider.value = earthquakeMagnitude;

        UpdateEarthquakeMagnitudeText(earthquakeMagnitude);
    }

    private void Update() {
        if (Input.GetKeyDown("h")){
            HideButton();
        }
    }

    public void Earthquake()
    {
        BlockPhysicsEvents.TriggerEarthquake();
    }

    public void HideButton(){
        if (UiPanel.activeSelf)
        {
            UiPanel.SetActive(false);
        }
        else
        {
            // Eğer kapalıysa, aç
            UiPanel.SetActive(true);
        }
    }

    public void GameRestart()
    {
        BlockPhysicsEvents.ClearAllEvents();
        SceneManager.LoadScene("0_GameScene");
    }

    // Slider ile tetiklenen fonksiyon
    public void OnEarthquakeMagnitudeChanged()
    {
        earthquakeMagnitude = earthquakeSlider.value;
        BlockPhysicsEvents.TriggerPower(earthquakeMagnitude);
        UpdateEarthquakeMagnitudeText(earthquakeMagnitude);
    }

    private void UpdateEarthquakeMagnitudeText(float magnitude)
    {
        earthquakeMagnitudeText.text = (magnitude / 1000).ToString("F1");
    }
}
