using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject optionsPanel;

    void Start() {
        ShowMainMenu();
    }

    public void ShowMainMenu() {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ShowOptionsMenu() {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void StartGame() {
        IMenuCommand startCommand = new StartGameCommand();
        startCommand.Execute();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
