using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingState : IGameState
{
    private readonly GameManagerHandler handler;
    public LoadingState(GameManagerHandler h) => handler = h;

    public void OnEnter()
    {
        SceneManager.LoadScene("Loading");
        handler.SetupLoading();
    }
    public void OnExit()    => handler.CleanupLoading();
    public void OnUpdate()  { /* monitor progresso caricamento */ }
}
