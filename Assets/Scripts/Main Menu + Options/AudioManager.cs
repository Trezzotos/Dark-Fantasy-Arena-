using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source per la musica")]
    public AudioSource musicSource;

    [Header("Tracce musicali disponibili")]
    public AudioClip[] musicTracks;

    private int currentTrackIndex = 0;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public int CurrentTrackIndex => currentTrackIndex;
    public bool IsPlaying => musicSource.isPlaying;

    // ▶️ Avvia una traccia specifica
    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicTracks.Length) return;

        currentTrackIndex = index;
        musicSource.clip = musicTracks[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    // ⏹️ Stop
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // 🔊 Cambia volume immediato
    public void SetVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicSource.volume);
    }

    // 🔊 Fade verso un volume target
    public void FadeTo(float targetVolume, float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetVolume, duration));
    }

    private IEnumerator FadeRoutine(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // usa tempo non scalato (così funziona anche in pausa)
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
