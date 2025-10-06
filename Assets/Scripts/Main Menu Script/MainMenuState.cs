using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : MenuState {
    public MainMenuState(MenuStateMachine m) : base(m) {}
    public override void Enter() {
        EventBus.Publish("UI/ShowPanel", "Main");
    }
}
