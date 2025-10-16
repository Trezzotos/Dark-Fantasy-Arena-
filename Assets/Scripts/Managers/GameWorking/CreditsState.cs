using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsState : IGameState
{
    private readonly GameManagerHandler handler;
    public CreditsState(GameManagerHandler h) => handler = h;

    public void OnEnter()
    {
        SceneManager.LoadScene("Credits");
        handler.SetupCredits();
    }
    public void OnExit()    => handler.CleanupCredits();
    public void OnUpdate()  { /* scroll automatico o input per skip */ }
}
