using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
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
 
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;   
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void StartGame()
    {
        gameState = GameState.PLAYING;
        EntityManager.Instance.SetDifficoulty(EntityManager.Difficoulty.EASY);  // Da settare dal Main Menu
        EntityManager.Instance.PrepareLevel();   // spawn everything
        startUI.SetActive(false);
    }

    public void PauseGame()
    {
        gameState = GameState.PAUSED;
        Time.timeScale = 0;
        gamePausedUI.SetActive(true);
    }

    public void ResumeGame()
    {
        gameState = GameState.PLAYING;
        Time.timeScale = 1;
        gamePausedUI.SetActive(false);
    }

    public void Gameover()
    {
        gameState = GameState.GAMEOVER;
        gameOverUI.SetActive(true);
    }

    public void Restart()
    {
        gameState = GameState.STARTING;
        gameOverUI.SetActive(false);
        startUI.SetActive(true);
        EntityManager.Instance.ClearLevel();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerHealth>().FullyHeal();
        }
    }

    public void BackToMenu()
    {   
        //gameState = GameState.PLAYING;
        SceneManager.LoadScene(sceneNames.MainMenu);
    }

}