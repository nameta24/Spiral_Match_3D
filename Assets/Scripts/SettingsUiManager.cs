using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUiManager : MonoBehaviour
{
    public static SettingsUiManager Instance;
    public Sprite toggleOnSprite, toggleOffSprite;
    public Image soundToggleImage, vibrationToggleImage;

    public GameObject settingsUiScreen;
    private const string SoundPref = "SoundEnabled";
    private const string VibrationPref = "VibrationEnabled";


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUiValues();
    }

    void UpdateUiValues()
    {
        // Update sound toggle UI
        bool isSoundEnabled = CheckIfSoundIsEnabled();
        soundToggleImage.sprite = isSoundEnabled ? toggleOnSprite : toggleOffSprite;

        // Update vibration toggle UI
        bool isVibrationEnabled = CheckIfVibrationIsEnabled();
        vibrationToggleImage.sprite = isVibrationEnabled ? toggleOnSprite : toggleOffSprite;
    }

    public void OnSoundButtonPress()
    {
        // Toggle sound setting
        bool isSoundEnabled = !CheckIfSoundIsEnabled();
        PlayerPrefs.SetInt(SoundPref, isSoundEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateUiValues();
        AudioManager.Instance.PlaySound(AudioManager.Instance.uiButton);

    }

    public void OnVibrationButtonPress()
    {
        // Toggle vibration setting
        bool isVibrationEnabled = !CheckIfVibrationIsEnabled();
        PlayerPrefs.SetInt(VibrationPref, isVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
        UpdateUiValues();
        AudioManager.Instance.PlaySound(AudioManager.Instance.uiButton);

    }

    public bool CheckIfSoundIsEnabled()
    {
        return PlayerPrefs.GetInt(SoundPref, 1) == 1; // Default to enabled
    }

    public bool CheckIfVibrationIsEnabled()
    {
        return PlayerPrefs.GetInt(VibrationPref, 1) == 1; // Default to enabled
    }

    public void OnSettingsOpen()
    {
        settingsUiScreen.SetActive(true);
        AudioManager.Instance.PlaySound(AudioManager.Instance.uiButton);

    }

    public void OnSettingsClose()
    {
        settingsUiScreen.SetActive(false);
        AudioManager.Instance.PlaySound(AudioManager.Instance.uiButton);
    }
}