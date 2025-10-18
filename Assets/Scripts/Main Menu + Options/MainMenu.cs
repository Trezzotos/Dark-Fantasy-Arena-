using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button continueButton;

    void Start()
    {
        if (!SaveSystem.SaveFileExists())
        {
            continueButton.GetComponent<Image>().color = Color.gray;
            continueButton.enabled = false;
        }
    }

    public void PlayGame()
    {
        // da settare la difficoltà

        // quando scegli la difficoltà, crea salvataggio vuoto

        if (SaveSystem.SaveFileExists()) Debug.LogWarning("Elimino il vecchio salvataggio");
        
        int difficulty = 1;
        SaveSystem.GenerateEmptySaveFile(difficulty);
        SceneManager.LoadScene("Game");
    }

    public void OptionMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void Continue()
    {
        SceneManager.LoadScene("Game");
    }

}
