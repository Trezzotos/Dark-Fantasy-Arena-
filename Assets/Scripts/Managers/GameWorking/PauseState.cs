using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : IGameState
{
    private readonly GameManagerHandler handler;
    public PauseState(GameManagerHandler h) => handler = h;

    public void OnEnter()   => handler.SetupPause();
    public void OnExit()    => handler.CleanupPause();
    public void OnUpdate()  { /* ascolta input per resume o quit */ }
}
