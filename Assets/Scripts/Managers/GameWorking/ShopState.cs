using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopState : IGameState {
    public bool IsOverlay => true;
    public void Enter(IGameManagerHandler h) {
        Time.timeScale = 0f;
        h.ShowShop();
    }
    public void Exit(IGameManagerHandler h) {
        h.HideShop();
        Time.timeScale = 1f;
    }
    public void Tick(IGameManagerHandler h) {
        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.PopState();
    }
}
