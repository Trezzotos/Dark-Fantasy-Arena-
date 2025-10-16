using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : IGameState
{
    private readonly GameManagerHandler handler;
    public MainMenuState(GameManagerHandler h) => handler = h;

    public void OnEnter()
    {
        SceneManager.LoadScene("MainMenu");
        handler.SetupMainMenu();
    }
    public void OnExit()    => handler.CleanupMainMenu();
    public void OnUpdate()  { /* ascolta input su pulsanti Start, Esci… */ }
}