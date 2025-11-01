using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource sfxSourcePrefab;
    public int initialPoolSize = 8;
    public int maxPoolSize = 0;

    [Range(0f,1f)] public float masterVolume = 1f;
    const string PREF_SFX_VOLUME = "SFXVolume";
    const float DEFAULT_SFX_VOLUME = 1f;

    public AudioClip enemyHit;
    public AudioClip playerHit;
    public AudioClip TeslaHit;
    public AudioClip CollectedMoney;

    private List<AudioSource> pool = new List<AudioSource>();
    private Queue<AudioSource> available = new Queue<AudioSource>();
    private List<AudioSource> inUse = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { if (Instance != this) Destroy(gameObject); return; }

        if (sfxSourcePrefab == null) { }

        masterVolume = PlayerPrefs.GetFloat(PREF_SFX_VOLUME, DEFAULT_SFX_VOLUME);
        masterVolume = Mathf.Clamp01(masterVolume);

        for (int i = 0; i < initialPoolSize; i++) CreatePooledSource();
    }

    void CreatePooledSource()
    {
        if (sfxSourcePrefab == null)
        {
            GameObject go = new GameObject("SFX_AudioSource_Runtime");
            go.transform.SetParent(transform, false);
            var a = go.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.loop = false;
            a.spatialBlend = 0f;
            a.volume = masterVolume;
            pool.Add(a);
            available.Enqueue(a);
            return;
        }

        AudioSource srcInstance = null;
        try
        {
            srcInstance = Instantiate(sfxSourcePrefab, transform);
        }
        catch
        {
            var go = Instantiate(sfxSourcePrefab.gameObject, transform);
            if (go != null) srcInstance = go.GetComponent<AudioSource>();
        }

        if (srcInstance == null) return;

        srcInstance.playOnAwake = false;
        srcInstance.loop = false;
        srcInstance.spatialBlend = 0f;
        srcInstance.volume = masterVolume;
        pool.Add(srcInstance);
        available.Enqueue(srcInstance);
    }

    AudioSource GetFreeSource()
    {
        while (available.Count > 0)
        {
            var s = available.Dequeue();
            if (s == null) continue;
            inUse.Add(s);
            return s;
        }

        if (maxPoolSize <= 0 || pool.Count < maxPoolSize)
        {
            CreatePooledSource();
            if (available.Count > 0)
            {
                var s = available.Dequeue();
                if (s != null)
                {
                    inUse.Add(s);
                    return s;
                }
            }
        }

        if (inUse.Count > 0)
        {
            var s = inUse[0];
            if (s == null) { inUse.RemoveAt(0); return GetFreeSource(); }
            s.Stop();
            s.clip = null;
            inUse.RemoveAt(0);
            available.Enqueue(s);
            var r = available.Dequeue();
            inUse.Add(r);
            return r;
        }

        return null;
    }

    void RecycleSource(AudioSource src)
    {
        if (src == null) return;
        src.Stop();
        src.clip = null;
        if (inUse.Contains(src)) inUse.Remove(src);
        if (!available.Contains(src)) available.Enqueue(src);
    }

    IEnumerator RecycleAfterPlay(AudioSource src, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            if (src == null) yield break;
            if (!src.isPlaying) break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        RecycleSource(src);
    }

    public void SetVolume(float v, bool save = true)
    {
        masterVolume = Mathf.Clamp01(v);
        foreach (var s in pool) if (s != null) s.volume = masterVolume;
        if (save) { PlayerPrefs.SetFloat(PREF_SFX_VOLUME, masterVolume); PlayerPrefs.Save(); }
    }

    public float GetVolume() => Mathf.Clamp01(masterVolume);

    public AudioSource PlayOneShot(AudioClip clip, float volume = -1f, float pitch = 1f)
    {
        if (clip == null) return null;

        float useVolume = (volume < 0f) ? masterVolume : Mathf.Clamp01(volume);

        var src = GetFreeSource();
        if (src == null) return null;

        if (!src.gameObject.activeInHierarchy) src.gameObject.SetActive(true);
        src.enabled = true;

        src.spatialBlend = 0f;
        src.playOnAwake = false;
        src.loop = false;
        src.mute = false;
        src.pitch = Mathf.Approximately(pitch, 0f) ? 1f : pitch;
        src.clip = clip;
        src.volume = Mathf.Max(0.0001f, useVolume);

        if (Camera.main != null) src.transform.position = Camera.main.transform.position;

        src.Play();

        if (!src.isPlaying)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero, useVolume);
            RecycleSource(src);
            return null;
        }

        float dur = clip.length / Mathf.Abs(src.pitch);
        if (dur <= 0.01f) dur = 0.05f;
        StartCoroutine(RecycleAfterPlay(src, dur));
        return src;
    }


    public void PlayEnemyHit(float volume = -1f)  => PlayOneShot(enemyHit, volume);
    public void PlayPlayerHit(float volume = -1f) => PlayOneShot(playerHit, volume);
    public void PlayCollectedMoney(float volume = -1f) => PlayOneShot(CollectedMoney, volume);
    public void PlayTeslaHit(float volume = -1f) => PlayOneShot(TeslaHit, volume);


    public void StopAll()
    {
        foreach (var s in new List<AudioSource>(inUse)) RecycleSource(s);
    }

    void OnDestroy()
    {
        foreach (var s in pool) if (s != null) Destroy(s.gameObject);
        pool.Clear(); available.Clear(); inUse.Clear();
    }
}
