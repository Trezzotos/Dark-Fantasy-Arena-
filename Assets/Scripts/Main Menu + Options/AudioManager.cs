using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Main Menu Track (single)")]
    public AudioClip mainMenuTrack; // singola traccia per Main Menu

    [Header("Playlists (assign in inspector)")]
    public AudioClip[] gameTracks;
    public AudioClip[] shopTracks;

    const string PREF_MUSIC_VOLUME = "MusicVolume";
    const float DEFAULT_VOLUME = 1f;

    // stato per le playlist
    class PlaylistState
    {
        public AudioClip[] clips;
        public int trackIndex = 0;
        public float time = 0f;
        public bool paused = true;
    }

    private Dictionary<string, PlaylistState> states = new Dictionary<string, PlaylistState>();
    private string currentScene = null;
    private Coroutine trackCoroutine = null;

    // campo privato nella classe (se non esiste già)
    private Coroutine fadeCoroutine = null;

    // Metodo pubblico da aggiungere
    public void FadeTo(float targetVolume, float duration, bool saveAtEnd = true)
    {
        if (musicSource == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(targetVolume, duration, saveAtEnd));
    }

    // Coroutine privata da aggiungere
    private IEnumerator FadeRoutine(float targetVolume, float duration, bool saveAtEnd)
    {
        float startVolume = musicSource.volume;
        float time = 0f;
        targetVolume = Mathf.Clamp01(targetVolume);

        // evita divisione per zero
        if (duration <= 0f)
        {
            musicSource.volume = targetVolume;
            if (saveAtEnd)
            {
                PlayerPrefs.SetFloat(PREF_MUSIC_VOLUME, musicSource.volume);
                PlayerPrefs.Save();
            }
            fadeCoroutine = null;
            yield break;
        }

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


    void Awake()
    {
        // singleton persistente
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

        // inizializza stati per le scene interessate
        states.Clear();

        // Main Menu: singola clip trasformata in array per uniformità
        states["MainMenu"] = new PlaylistState
        {
            clips = (mainMenuTrack != null) ? new AudioClip[] { mainMenuTrack } : new AudioClip[0],
            trackIndex = 0,
            time = 0f,
            paused = true
        };

        states["Game"] = new PlaylistState
        {
            clips = (gameTracks != null) ? gameTracks : new AudioClip[0],
            trackIndex = 0,
            time = 0f,
            paused = true
        };

        states["Shop"] = new PlaylistState
        {
            clips = (shopTracks != null) ? shopTracks : new AudioClip[0],
            trackIndex = 0,
            time = 0f,
            paused = true
        };

        // Alias: fare in modo che Settings, Credits e Ranking usino lo stesso stato di MainMenu
        string[] mainMenuAliases = new string[] { "Settings", "Credits", "Ranking" };
        foreach (var alias in mainMenuAliases)
        {
            // assegna la stessa istanza di PlaylistState del MainMenu
            states[alias] = states["MainMenu"];
        }

        float savedVol = PlayerPrefs.GetFloat(PREF_MUSIC_VOLUME, DEFAULT_VOLUME);
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(savedVol);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // applica playlist in base alla scena attiva all'avvio
        Scene active = SceneManager.GetActiveScene();
        if (active != null)
            SwitchToScene(active.name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchToScene(scene.name);
    }

    // pubbliche
    public void SetVolume(float v, bool save = true)
    {
        if (musicSource == null) return;
        musicSource.volume = Mathf.Clamp01(v);
        if (save)
        {
            PlayerPrefs.SetFloat(PREF_MUSIC_VOLUME, musicSource.volume);
            PlayerPrefs.Save();
        }
    }

    public float GetVolume()
    {
        if (musicSource == null) return DEFAULT_VOLUME;
        return musicSource.volume;
    }

    public int CurrentTrackIndex
    {
        get
        {
            if (currentScene != null && states.ContainsKey(currentScene)) return states[currentScene].trackIndex;
            return -1;
        }
    }

    public bool IsPlaying => musicSource != null && musicSource.isPlaying;

    // core
    void SwitchToScene(string sceneName)
    {
        if (currentScene == sceneName) return;

        // salva e pausa stato corrente
        if (!string.IsNullOrEmpty(currentScene) && states.ContainsKey(currentScene))
        {
            PausePlaylist(currentScene);
        }

        currentScene = sceneName;

        if (states.ContainsKey(sceneName))
        {
            PlayOrResumePlaylist(sceneName);
        }
        else
        {
            StopMusic();
        }
    }

    void PausePlaylist(string sceneName)
    {
        if (!states.ContainsKey(sceneName) || musicSource == null) return;

        var st = states[sceneName];

        if (musicSource.clip != null && st.clips != null && st.clips.Length > 0)
        {
            for (int i = 0; i < st.clips.Length; i++)
            {
                if (st.clips[i] == musicSource.clip)
                {
                    st.trackIndex = i;
                    st.time = Mathf.Clamp(musicSource.time, 0f, musicSource.clip.length);
                    break;
                }
            }
        }

        st.paused = true;

        if (musicSource.isPlaying)
            musicSource.Pause();

        if (trackCoroutine != null)
        {
            StopCoroutine(trackCoroutine);
            trackCoroutine = null;
        }
    }

    void PlayOrResumePlaylist(string sceneName)
    {
        if (!states.ContainsKey(sceneName) || musicSource == null) return;

        var st = states[sceneName];

        if (st.clips == null || st.clips.Length == 0)
        {
            StopMusic();
            return;
        }

        // uniformità: per Main Menu l'array ha 1 clip e verrà loopata
        st.trackIndex = Mathf.Clamp(st.trackIndex, 0, st.clips.Length - 1);
        AudioClip clipToPlay = st.clips[st.trackIndex];
        if (clipToPlay == null)
        {
            StopMusic();
            return;
        }

        // se è la stessa clip attuale e non è stata cambiata, e la sorgente è in pausa (cioè resume), riprendi
        bool clipMatches = musicSource.clip == clipToPlay;

        musicSource.clip = clipToPlay;

        float startTime = st.paused ? st.time : 0f;
        startTime = Mathf.Clamp(startTime, 0f, clipToPlay.length);

        // Special-case Main Menu: loop singola traccia usando AudioSource.loop = true
        if (st.clips.Length == 1)
        {
            musicSource.time = startTime;
            musicSource.loop = true;
            musicSource.Play();
            st.paused = false;

            // non serve coroutine watcher per singola loop gestita da AudioSource
            if (trackCoroutine != null) { StopCoroutine(trackCoroutine); trackCoroutine = null; }
            return;
        }

        // per playlist multi-clip
        musicSource.time = startTime;
        musicSource.loop = false; // loop a livello di playlist
        musicSource.Play();
        st.paused = false;

        if (trackCoroutine != null) StopCoroutine(trackCoroutine);
        trackCoroutine = StartCoroutine(TrackWatcherCoroutine(sceneName));
    }

    IEnumerator TrackWatcherCoroutine(string sceneName)
    {
        while (true)
        {
            if (musicSource == null || musicSource.clip == null) yield break;
            if (!states.ContainsKey(sceneName)) yield break;
            var st = states[sceneName];

            float remaining = musicSource.clip.length - musicSource.time;
            if (remaining <= 0.01f) remaining = 0.01f;

            float t = 0f;
            while (t < remaining)
            {
                if (currentScene != sceneName) yield break;
                if (states.ContainsKey(sceneName) && states[sceneName].paused) yield break;

                t += Time.unscaledDeltaTime;
                yield return null;
            }

            // clip finita: avanza all'indice successivo e loop nella playlist
            st.trackIndex = (st.trackIndex + 1) % st.clips.Length;
            st.time = 0f;
            AudioClip nextClip = st.clips[st.trackIndex];
            if (nextClip == null) yield break;

            musicSource.clip = nextClip;
            musicSource.time = 0f;
            musicSource.Play();
        }
    }

    // utilità
    public void PlaySpecificTrackInScene(string sceneName, int trackIndex)
    {
        if (!states.ContainsKey(sceneName)) return;
        var st = states[sceneName];
        if (st.clips == null || trackIndex < 0 || trackIndex >= st.clips.Length) return;
        st.trackIndex = trackIndex;
        st.time = 0f;
        st.paused = false;

        if (currentScene == sceneName) PlayOrResumePlaylist(sceneName);
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        if (trackCoroutine != null)
        {
            StopCoroutine(trackCoroutine);
            trackCoroutine = null;
        }
    }
}
