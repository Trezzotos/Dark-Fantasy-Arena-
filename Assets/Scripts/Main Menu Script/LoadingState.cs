using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingState : MenuState {
    public LoadingState(MenuStateMachine m) : base(m) {}
    public override void Enter() {
        EventBus.Publish("UI/ShowPanel", "Loading");
        EventBus.Publish("Game/LoadRequested");
    }
}