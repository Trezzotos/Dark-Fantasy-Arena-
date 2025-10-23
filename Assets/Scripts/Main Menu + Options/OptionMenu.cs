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
    public TMP_Dropdown resolutionDropdown;

    [Header("Controls UI")]
    public Button controlSchemeButton;
    public TMP_Text controlSchemeLabel;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    bool initializing = false;

    const string PREF_CONTROL_SCHEME = "ControlScheme";

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        LoadSettings();
    }

    void OnEnable()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        if (controlSchemeButton != null)
            controlSchemeButton.onClick.AddListener(OnControlSchemeButton);
    }

    void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

        if (controlSchemeButton != null)
            controlSchemeButton.onClick.RemoveListener(OnControlSchemeButton);
    }

    public void OnVolumeChanged(float value)
    {
        if (initializing) return;
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(value);
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
        if (index < 0 || index >= resolutions.Length) return;
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    // Bottone Controls: cicla gli schemi, salva e applica ai PlayerMove presenti
    public void OnControlSchemeButton()
    {
        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        scheme = (scheme + 1) % 2;
        PlayerPrefs.SetInt(PREF_CONTROL_SCHEME, scheme);
        PlayerPrefs.Save();
        UpdateControlSchemeLabel(scheme);
        Debug.Log("[OptionsMenu] ControlScheme changed to " + scheme);
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

        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(volume);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(volume, save: false);

        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        Screen.fullScreen = isFullscreen;

        currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        currentResolutionIndex = Mathf.Clamp(currentResolutionIndex, 0, resolutions.Length - 1);
        if (resolutionDropdown != null)
        {
            resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }
        Resolution res = resolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // Controls: carica schema e aggiorna UI, applica ai player
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
