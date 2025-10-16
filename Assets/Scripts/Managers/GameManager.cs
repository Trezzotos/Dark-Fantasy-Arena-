using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GameObject startUI;
    public GameObject gameOverUI;
    public GameObject gamePausedUI;

    public enum GameState
    {
        STARTING,
        PLAYING,
        SHOPPING,
        PAUSED,
        GAMEOVER
    }
    public GameState gameState = GameState.STARTING;

    public void StartGame()
    {   
        startUI.SetActive(true);
        gameState = GameState.PLAYING;
        EntityManager.Instance.SetDifficoulty(EntityManager.Difficoulty.EASY);  // Da settare dal Main Menu
        EntityManager.Instance.PrepareLevel();   // spawn everything
        startUI.SetActive(false);
    }

    public void PauseGame()
    {
        gameState = GameState.PAUSED;
   //     Time.timeScale = 0;
        gamePausedUI.SetActive(true);
    }
/*
    public void ResumeGame()
    {
        gameState = GameState.PLAYING;
        Time.timeScale = 1;
        gamePausedUI.SetActive(false);
    }
*/
    public void Gameover()
    {
        gameState = GameState.GAMEOVER;
        gameOverUI.SetActive(true);
    }
/*
    public void Restart()
    {
        gameState = GameState.STARTING;
        gameOverUI.SetActive(false);
        startUI.SetActive(true);
        EntityManager.Instance.ClearLevel();
    }
*/
    void Awake()
    {
        // Se non esiste ancora un'istanza, questa è l'istanza.
        if (Instance == null)
        {
            Instance = this;
            // impedisce che l'oggetto venga distrutto al cambio di scena.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se esiste già un'istanza, distruggi questo oggetto (perché è un duplicato).
            Destroy(gameObject);
        }
    }
}
