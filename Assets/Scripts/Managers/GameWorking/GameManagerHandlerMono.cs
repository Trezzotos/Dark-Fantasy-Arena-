using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerHandlerMono : MonoBehaviour
{   
    // Logica specifica di ogni stato
    public void SetupMainMenu()      { Debug.Log("MainMenu: setup UI e pulsanti"); }
    public void CleanupMainMenu()    { Debug.Log("MainMenu: pulisco risorse"); }

    public void SetupLoading()       { Debug.Log("Loading: avvio caricamento dati"); }
    public void CleanupLoading()     { Debug.Log("Loading: fine caricamento"); }

    public void SetupGameplay()      { Debug.Log("Gameplay: inizializzo player e nemici"); }
    public void CleanupGameplay()    { Debug.Log("Gameplay: salvo stati"); }

    public void SetupPause()         { Time.timeScale = 0; Debug.Log("Pause: gioco in pausa"); }
    public void CleanupPause()       { Time.timeScale = 1; Debug.Log("Pause: riprendo gioco"); }

    public void SetupGameOver()      { Debug.Log("GameOver: mostro schermo finale"); }
    public void CleanupGameOver()    { Debug.Log("GameOver: reset variabili"); }

    public void SetupCredits()       { Debug.Log("Credits: scroll testo"); }
    public void CleanupCredits()     { Debug.Log("Credits: torno a menu"); }
}
