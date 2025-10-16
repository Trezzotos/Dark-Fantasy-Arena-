using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : IGameState {
    private readonly GameManagerHandler handler;
    public GameplayState(GameManagerHandler h) => handler = h;

    public void OnEnter()
    {
        handler.SetupGameplay();
    }
    public void OnExit()    => handler.CleanupGameplay();
    public void OnUpdate()  { /* logica di gioco continua */ }
}

