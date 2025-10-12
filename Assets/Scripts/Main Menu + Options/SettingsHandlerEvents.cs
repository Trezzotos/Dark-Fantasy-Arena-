using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class SettingsHandlerEvents : MonoBehaviour
{
    private SettingsManager settingsManager;
    private AudioManager musicManager;
    private Slider volumeSlider;
    private TMP_Dropdown resolutionDropdown;
    private Button switchCmdset;


    private void Awake()
    {
        Toggle fullScreenToggle = GetComponentInChildren<Toggle>();

        settingsManager = SettingsManager.GetIstance();
        musicManager = AudioManager.GetIstance();

        volumeSlider = GetComponentInChildren<Slider>();
        resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();

        volumeSlider.value = settingsManager.GetVolume();
        resolutionDropdown.value = settingsManager.GetResolutionIndex();
        fullScreenToggle.isOn = Screen.fullScreen;

        switchCmdset = GetComponentInChildren<Button>();
    }   

    public void OnControlsChange()
    {
        //
    }

    public void OnVolumeChange()
    {     
        settingsManager.SetVolume(volumeSlider.value);
        musicManager.ChangeVolume(volumeSlider.value);
    }

    public void OnResolutionChange(TMP_Dropdown dropdown)
    {
        string resolution = dropdown.options[dropdown.value].text;
        int width = int.Parse(resolution.Split("x")[0]);
        int height = int.Parse(resolution.Split("x")[1]);

        settingsManager.SetResolution(resolution);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void OnFullScreenChange(Toggle toggle)
    {   
        Screen.fullScreenMode = toggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene(sceneNames.MainMenu);
    }

}
