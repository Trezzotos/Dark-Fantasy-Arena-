using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);  // evita duplicati
            return;
        }

        audioSource = GetComponent<AudioSource>();
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        ChangeVolume(SettingsManager.GetIstance().GetVolume());
        audioSource.Play();
    }

    public static AudioManager GetIstance()
    {
        return instance;
    }

    public void SetMusic(string track)
    {
        audioSource.clip = Resources.Load<AudioClip>(track);
        audioSource.Play();
    }

    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }
}