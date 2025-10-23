using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source per la musica")]
    public AudioSource musicSource;

    [Header("Tracce musicali disponibili")]
    public AudioClip[] musicTracks;

    private int currentTrackIndex = 0;
    private Coroutine fadeCoroutine;

    const string PREF_MUSIC_VOLUME = "MusicVolume";
    const string PREF_MUSIC_TRACK = "MusicTrack";
    const float DEFAULT_VOLUME = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this) Destroy(gameObject);
            return;
        }

        if (musicSource == null)
            Debug.LogError("[AudioManager] musicSource non assegnato nell'Inspector", this);

        // Carica impostazioni
        currentTrackIndex = PlayerPrefs.GetInt(PREF_MUSIC_TRACK, currentTrackIndex);
        float savedVol = PlayerPrefs.GetFloat(PREF_MUSIC_VOLUME, DEFAULT_VOLUME);
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(savedVol);
    }

    void Start()
    {
        // Assicura che la traccia corrente sia impostata e avviata
        if (musicSource != null && musicTracks != null && musicTracks.Length > 0)
        {
            currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, musicTracks.Length - 1);
            if (musicSource.clip == null || musicSource.clip != musicTracks[currentTrackIndex])
                musicSource.clip = musicTracks[currentTrackIndex];

            musicSource.loop = true;
            if (!musicSource.isPlaying) musicSource.Play();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        // Se la sorgente è stata persa o disabilitata in una scena nuova, ricrea comportamento consistente
        if (musicSource == null) return;

        if (musicSource.clip == null && musicTracks != null && musicTracks.Length > 0)
        {
            currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, musicTracks.Length - 1);
            musicSource.clip = musicTracks[currentTrackIndex];
            musicSource.loop = true;
        }

        if (!musicSource.isPlaying) musicSource.Play();
    }

    public int CurrentTrackIndex => currentTrackIndex;
    public bool IsPlaying => musicSource != null && musicSource.isPlaying;

    public void PlayMusic(int index)
    {
        if (musicSource == null || musicTracks == null || musicTracks.Length == 0) return;
        if (index < 0 || index >= musicTracks.Length) return;

        currentTrackIndex = index;
        musicSource.clip = musicTracks[index];
        musicSource.loop = true;
        musicSource.Play();

        PlayerPrefs.SetInt(PREF_MUSIC_TRACK, currentTrackIndex);
        PlayerPrefs.Save();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }

    public void SetVolume(float volume, bool save = true)
    {
        if (musicSource == null) return;
        musicSource.volume = Mathf.Clamp01(volume);
        if (save)
        {
            PlayerPrefs.SetFloat(PREF_MUSIC_VOLUME, musicSource.volume);
            PlayerPrefs.Save();
        }
    }

    public void FadeTo(float targetVolume, float duration, bool saveAtEnd = true)
    {
        if (musicSource == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetVolume, duration, saveAtEnd));
    }

    private IEnumerator FadeRoutine(float targetVolume, float duration, bool saveAtEnd)
    {
        float startVolume = musicSource.volume;
        float time = 0f;
        targetVolume = Mathf.Clamp01(targetVolume);

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;

        if (saveAtEnd)
        {
            PlayerPrefs.SetFloat(PREF_MUSIC_VOLUME, musicSource.volume);
            PlayerPrefs.Save();
        }

        fadeCoroutine = null;
    }
}
