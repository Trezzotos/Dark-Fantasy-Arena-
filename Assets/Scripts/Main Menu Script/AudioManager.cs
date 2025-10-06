using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    [SerializeField] AudioSource musicSource;
    void Awake() {
        if(Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => EventBus.Subscribe(OnEvent);
    void OnDisable() => EventBus.Unsubscribe(OnEvent);

    void OnEvent(string topic, object payload) {
        if(topic == "Menu/StartGame") musicSource.Stop();
        if(topic == "UI/ShowPanel" && (string)payload == "Main") PlayMenuMusic();
    }

    void PlayMenuMusic() => musicSource.Play();
}
