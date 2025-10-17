using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    STARTING,
    PLAYING,
    SHOPPING,    // rimane definito ma gestito altrove
    PAUSED,
    GAMEOVER
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> onGameStateChanged;

    [Header("UI References")]
    [Tooltip("Pannello iniziale con bottone Start")]
    public GameObject startUI;
    [Tooltip("Pannello di pausa con bottone Resume")]
    public GameObject gamePausedUI;
    [Tooltip("Pannello Game Over con bottone Restart")]
    public GameObject gameOverUI;

    public GameState gameState { get; private set; }

    void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        UpdateGameState(GameState.STARTING);
    }

    void Update()
    {
        // Gestione rapido della pausa con il tasto Escape
        if (gameState == GameState.PLAYING && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PAUSED);
        else if (gameState == GameState.PAUSED && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PLAYING);
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;
        Time.timeScale = 1f;  // resetto il timeScale di default

        switch (newState)
        {
            case GameState.STARTING:
                EnterStarting();
                break;

            case GameState.PLAYING:
                EnterPlaying();
                break;

            case GameState.PAUSED:
                EnterPaused();
                break;

            case GameState.GAMEOVER:
                EnterGameOver();
                break;

            case GameState.SHOPPING:
                // TODO: implementazione shop
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        onGameStateChanged?.Invoke(newState);
    }
    private void EnterStarting()
    {
        startUI.SetActive(true);
        gamePausedUI.SetActive(false);
        gameOverUI.SetActive(false);
        Time.timeScale = 0f;
    }

    private void EnterPlaying()
    {
        startUI.SetActive(false);
        gamePausedUI.SetActive(false);
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void EnterPaused()
    {
        gamePausedUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void EnterGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    // Metodi da collegare ai bottoni via Inspector
    public void OnStartButton()
    {
        UpdateGameState(GameState.PLAYING);
    }

    public void OnResumeButton()
    {
        UpdateGameState(GameState.PLAYING);
    }

    public void OnRestartButton()
    {
        // Ricarica la scena corrente
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UpdateGameState(GameState.STARTING);
    }

    public void OnQuitButton()
    {
        // Torna al menù
        SceneManager.LoadScene("MainMenu");
        UpdateGameState(GameState.GAMEOVER);
    }
}

    /*
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
    }
*/