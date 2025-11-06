using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Riferimenti")]
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Slider SFXSlider;
    public TMP_Dropdown resolutionDropdown;

    [Header("Controls UI")]
    public Button controlSchemeButton;
    public TMP_Text controlSchemeLabel;
    private Resolution[] allResolutions;
    private List<Resolution> uniqueResolutions;
    private int currentResolutionIndex = 0;
    bool initializing = false;

    const string PREF_CONTROL_SCHEME = "ControlScheme";

    void Start()
    {
        allResolutions = Screen.resolutions;

       
        uniqueResolutions = new List<Resolution>();      // Filtra risoluzioni duplicate (stessa width x height) mantenendo l'ordine
        var seen = new HashSet<string>();
        for (int i = 0; i < allResolutions.Length; i++)
        {
            string key = allResolutions[i].width + "x" + allResolutions[i].height;
            if (!seen.Contains(key))
            {
                seen.Add(key);
                uniqueResolutions.Add(allResolutions[i]);
            }
        }

        resolutionDropdown.ClearOptions();
        var options = new List<string>();

        currentResolutionIndex = 0;
        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            string option = uniqueResolutions[i].width + "x" + uniqueResolutions[i].height;
            options.Add(option);

            if (uniqueResolutions[i].width == Screen.currentResolution.width &&
                uniqueResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        LoadSettings();
    }

    void OnEnable()
    {
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (SFXSlider != null) SFXSlider.onValueChanged.AddListener(OnSFXChanged);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        if (controlSchemeButton != null)
            controlSchemeButton.onClick.AddListener(OnControlSchemeButton);
    }

    void OnDisable()
    {
        if (volumeSlider != null) volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        if (SFXSlider != null) SFXSlider.onValueChanged.RemoveListener(OnSFXChanged);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

        if (controlSchemeButton != null)
            controlSchemeButton.onClick.RemoveListener(OnControlSchemeButton);
    }

    public void OnVolumeChanged(float value)
    {
        if (initializing) return;
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void OnSFXChanged(float value)
    {
        if (initializing) return;
        if (SFXManager.Instance != null) SFXManager.Instance.SetVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        if (initializing) return;
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnResolutionChanged(int index)
    {
        if (initializing) return;
        if (index < 0 || uniqueResolutions == null || index >= uniqueResolutions.Count) return;
        Resolution res = uniqueResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void OnControlSchemeButton()
    {
        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        scheme = (scheme + 1) % 2;
        PlayerPrefs.SetInt(PREF_CONTROL_SCHEME, scheme);
        PlayerPrefs.Save();
        UpdateControlSchemeLabel(scheme);
        ApplyControlSchemeToPlayers(scheme);
    }

    void UpdateControlSchemeLabel(int scheme)
    {
        if (controlSchemeLabel == null) return;
        switch (scheme)
        {
            case 0: controlSchemeLabel.text = "SCHEME 1"; break;
            case 1: controlSchemeLabel.text = "SCHEME 2"; break;
            default: controlSchemeLabel.text = "SCHEME " + (scheme + 1); break;
        }
    }

    void ApplyControlSchemeToPlayers(int scheme)
    {
        var players = FindObjectsOfType<PlayerMove>();
        foreach (var p in players)
            p.ChangeCommands(scheme);
    }

    private void LoadSettings()
    {
        initializing = true;

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(musicVolume);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(musicVolume, save: false);

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (SFXSlider != null) SFXSlider.SetValueWithoutNotify(sfxVolume);
        if (SFXManager.Instance != null) SFXManager.Instance.SetVolume(sfxVolume, save: false);

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        Screen.fullScreen = isFullscreen;

        // Carica e clampa l'indice usando la lista di risoluzioni uniche
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        currentResolutionIndex = Mathf.Clamp(savedIndex, 0, Mathf.Max(0, uniqueResolutions.Count - 1));
        if (resolutionDropdown != null)
        {
            resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }

        if (uniqueResolutions != null && uniqueResolutions.Count > 0)
        {
            Resolution res = uniqueResolutions[currentResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        int controlScheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        UpdateControlSchemeLabel(controlScheme);
        ApplyControlSchemeToPlayers(controlScheme);

        initializing = false;
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
