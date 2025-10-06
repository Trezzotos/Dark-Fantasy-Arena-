using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuState {
    protected MenuStateMachine machine;
    protected MenuState(MenuStateMachine m) => machine = m;
    public virtual void Enter() {}
    public virtual void Exit() {}
}