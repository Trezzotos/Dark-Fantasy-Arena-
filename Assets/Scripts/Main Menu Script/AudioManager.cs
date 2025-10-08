using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips Registrati")]
    [SerializeField] private List<AudioClip> musicClips;
    [SerializeField] private List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;

    // Singleton design pattern
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // evita duplicati
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        musicDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();

        foreach (var clip in musicClips)
            musicDict[clip.name] = clip;

        foreach (var clip in sfxClips)
            sfxDict[clip.name] = clip;
    }

    // Riproduzione musica
    public void PlayMusic(string clipName, bool loop = true)
    {
        if (musicDict.TryGetValue(clipName, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Clip musicale '{clipName}' non trovata!");
        }
    }

    // Effetti sonori
    public void PlaySFX(string clipName)
    {
        if (sfxDict.TryGetValue(clipName, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{clipName}' non trovato!");
        }
    }

    // Volume controlli
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
