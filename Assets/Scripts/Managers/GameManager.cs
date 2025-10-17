using System;
using UnityEngine;

public enum GameState
{
    STARTING,
    PLAYING,
    SHOPPING,    // gestito altrove
    PAUSED,
    GAMEOVER
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<GameState> OnGameStateChanged;

    public GameState gameState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGameState(GameState.STARTING);
    }

    private void Update()
    {
        // Gestione rapida della pausa con Escape
        if (gameState == GameState.PLAYING && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PAUSED);
        else if (gameState == GameState.PAUSED && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PLAYING);
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;

        // timeScale: 1 solo se PLAYING, altrimenti 0
        Time.timeScale = (newState == GameState.PLAYING) ? 1f : 0f;

        OnGameStateChanged?.Invoke(newState);
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

*/