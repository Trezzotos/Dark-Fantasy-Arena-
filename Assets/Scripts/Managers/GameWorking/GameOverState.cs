using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : IGameState
{
    private readonly GameManagerHandler handler;
    public GameOverState(GameManagerHandler h) => handler = h;

    public void OnEnter()
    {
        SceneManager.LoadScene("GameOver");
        handler.SetupGameOver();
    }
    public void OnExit()    => handler.CleanupGameOver();
    public void OnUpdate()  { /* attendi input per tornare al menu */ }
}
