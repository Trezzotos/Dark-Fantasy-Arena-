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

        // sei sicuro di voler eliminare il vecchio salvataggio (se non morto)

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
