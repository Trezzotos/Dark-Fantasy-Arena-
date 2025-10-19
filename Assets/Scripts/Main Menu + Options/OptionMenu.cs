using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Riferimenti")]
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    void Start()
    {
        // Ottieni tutte le risoluzioni supportate
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        LoadSettings();
    }

    // 🔊 Volume
    public void OnVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
        PlayerPrefs.Save();
    }

    // 🖥️ Fullscreen
    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    // 📺 Risoluzione
    public void OnResolutionChanged(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    private void LoadSettings()
    {
        // Volume
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        volumeSlider.value = volume;
        AudioManager.Instance.SetVolume(volume);

        // Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        // Risoluzione
        currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        OnResolutionChanged(currentResolutionIndex);
    }
}
