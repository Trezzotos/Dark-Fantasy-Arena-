using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameCommand : IMenuCommand {
    public void Execute()
    {
        EventBus.Publish("Menu/StartGame");
        SceneManager.LoadScene(1);
    }
}
