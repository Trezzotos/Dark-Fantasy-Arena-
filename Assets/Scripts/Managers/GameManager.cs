using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MAINMENU,
    STARTING,
    PLAYING,
    SHOPPING,
    PAUSED,
    GAMEOVER
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnLevelChanged;

    public GameState gameState { get; private set; }
    public int Level { get; private set; } = 1;
    public int Difficolta { get; private set; }

    LabelTimer gameoverTimer;

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

    void Start()
    {
        UpdateGameState(GameState.MAINMENU);
    }

    private void Update()
    {
        if (gameState == GameState.PLAYING && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PAUSED);
        else if (gameState == GameState.PAUSED && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PLAYING);
    }

    public void PrepareGame()
    {
        UpdateGameState(GameState.STARTING);
        LoadGameData();

        if (StatsManager.Instance != null)
        {
            Level = StatsManager.Instance.currentLevel;
            Difficolta = StatsManager.Instance.currentDifficulty;
            OnLevelChanged?.Invoke(Level);
        }

    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;

        Debug.Log("Stato Gioco: "+gameState);

        if (gameState == GameState.GAMEOVER)
        {
            SaveSystem.DeleteGame();
            gameoverTimer.StopTimer();
        }
        
        if (gameState == GameState.PAUSED)
        {
            Time.timeScale = 0f;
            gameoverTimer.StopTimer();
        }
        else if (gameState == GameState.PLAYING)//
        {
            Time.timeScale = 1f;
            if (gameoverTimer)
            {
                if (gameoverTimer.GetwasStarted())
                    gameoverTimer.ResumeTimer();
                else
                {
                    gameoverTimer.SetTimer(76 + 24 / StatsManager.Instance.currentDifficulty * StatsManager.Instance.currentLevel);
                    gameoverTimer.StartTimer();
                } 
            }
            else Debug.LogWarning("Gameover timer unreferenced, make sure to put one in scene");
        }
        else if (gameState == GameState.SHOPPING)
        {
            gameoverTimer.StopTimer();
        }

        HandleMusic(newState);

        OnGameStateChanged?.Invoke(newState);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneChange;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneChange;
    }

    private void HandleSceneChange(Scene scene, LoadSceneMode mode)
    {
        // Quando si entra nella scena di gioco, prepara lo stato di gioco
        if (scene.name == "Game")
        {
            Timer[] timers = FindObjectsOfType<Timer>();
            foreach (Timer timer in timers)
            {
                if (timer is LabelTimer labelTimer)
                {
                    gameoverTimer = labelTimer;
                    gameoverTimer.TimerEnded += TimerExpired;
                    break;
                }
            }
            PrepareGame();
            // Assicura timeScale corretto alla partenza della partita
            Time.timeScale = 1f;
        }
    }

    public void NextLevel()
    {
        Level++;
        OnLevelChanged?.Invoke(Level);
        SaveGameData();
        UpdateGameState(GameState.SHOPPING);
    }

    private void LoadGameData()
    {
        GameSaveData loadedData = SaveSystem.LoadGame();

        if (loadedData != null)
        {
            var inventory = FindObjectOfType<Examples.Observer.Inventory>();
            if (inventory != null)
                inventory.ApplyLoadedInventoryData(loadedData.inventory);

            if (StatsManager.Instance != null)
                StatsManager.Instance.ApplyGameStats(loadedData.gameStats);
        }
        else
        {
            Debug.LogError("Nessun salvataggio trovato (NON dovrebbe succedere!)");
        }
    }

    private void SaveGameData()
    {
        var inventoryComp = FindObjectOfType<Examples.Observer.Inventory>();
        if (inventoryComp == null)
        {
            Debug.LogWarning("SaveGameData: Inventory component non trovato, skip save.");
            return;
        }

        InventoryData inventoryData = inventoryComp.GetCurrentInventoryData();
        GameSaveData saveData = new GameSaveData(inventoryData, StatsManager.Instance.GetCurrentGameStats());
        SaveSystem.SaveGame(saveData);
    }

    private void HandleMusic(GameState state)
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        float current = am.GetVolume(); // valore corrente (quello salvato o l'ultimo impostato)

        switch (state)
        {
            case GameState.STARTING:
                // non forzare 1f; riapplica il valore attuale se necessario
                am.SetVolume(current, save: false);
                break;

            case GameState.PLAYING:
            case GameState.SHOPPING:
                // ripristina il volume corrente (fade verso il valore utente)
                am.FadeTo(current, 1.5f, saveAtEnd: false);
                break;

            case GameState.PAUSED:
                // abbassa il volume rispetto al valore corrente (es. 30% del valore utente)
                am.FadeTo(Mathf.Clamp01(current * 0.3f), 1.5f, saveAtEnd: false);
                break;

            case GameState.GAMEOVER:
                am.StopMusic();
                break;
        }
    }


    void TimerExpired()
    {
        UpdateGameState(GameState.GAMEOVER);
        // non so se serva altro
    }
}
