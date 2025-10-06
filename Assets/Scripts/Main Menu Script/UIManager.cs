using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject loadingPanel;

    void OnEnable() {
        EventBus.Subscribe(OnEvent);
    }

    void OnDisable() {
        EventBus.Unsubscribe(OnEvent);
    }

    void OnEvent(string topic, object payload) {
        if(topic == "UI/ShowPanel") {
            string panel = payload as string;
            ShowPanel(panel);
        }
    }

    void ShowPanel(string panelName) {
        mainPanel.SetActive(panelName == "Main");
        optionsPanel.SetActive(panelName == "Options");
        loadingPanel.SetActive(panelName == "Loading");
    }
}
