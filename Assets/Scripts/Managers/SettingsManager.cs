using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    private readonly Dictionary<string, int> supportedResolutions = new()
    {
        {"1280x720",0},
        {"1920x1080",1},
        {"2560x1440",2},
        {"2880x1800",3},
        {"3840x2160",4},
    };
    public void Awake()
    {
        if (instance)
        {
            Destroy(gameObject); 
            return;
        }
        instance = this;
        InitPlayerPrefs();
        DontDestroyOnLoad(gameObject);
    }

    public static SettingsManager GetIstance()
    {
        return instance;
    }
    
    public void InitPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1f);
        }
        if (!PlayerPrefs.HasKey("resolution"))
        {
            string playerDefaultResolution = Screen.width + "x" + Screen.height;

            if (!supportedResolutions.ContainsKey(playerDefaultResolution))
            {
                playerDefaultResolution = supportedResolutions.Keys.First();
            }

            PlayerPrefs.SetString("resolution", playerDefaultResolution);
        }
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("volume");
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }
    public void SetResolution(string resolution)
    {
        PlayerPrefs.SetString("resolution", resolution);
        PlayerPrefs.Save();
    }

    public int GetResolutionIndex()
    {
        return supportedResolutions[PlayerPrefs.GetString("resolution")]; 
    }
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
}
