using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStateMachine {
    MenuState current;
    public void ChangeState(MenuState next) {
        current?.Exit();
        current = next;
        current.Enter();
    }
}