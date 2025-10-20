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

    // Unity functions
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

    // Our functions
    public void PrepareGame()
    {
        UpdateGameState(GameState.STARTING);
        LoadGameData();

        // StatsManager exists because the scene was already loaded
        Level = StatsManager.Instance.currentLevel;
        Difficolta = StatsManager.Instance.currentDifficulty;
        OnLevelChanged?.Invoke(Level);
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;
        // Time.timeScale = (newState == GameState.PLAYING) ? 1f : 0f;

        if (gameState == GameState.GAMEOVER)
            SaveSystem.DeleteGame();

        HandleMusic(newState);

        OnGameStateChanged?.Invoke(newState);
    }

    // Events setup
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
        if (scene.name == "Game") 
        {
            PrepareGame();
        }
    }

    public void NextLevel()
    {
        Level++;
        OnLevelChanged?.Invoke(Level);
        SaveGameData();
        UpdateGameState(GameState.SHOPPING);
    }

    // Savefile Management
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
            Debug.LogError("Nessun salvataggio trovato (NON dovrebbe succedere!)");
    }


    private void SaveGameData()
    {
        InventoryData inventoryData = FindObjectOfType<Examples.Observer.Inventory>().GetCurrentInventoryData();
        GameSaveData saveData = new GameSaveData(inventoryData, StatsManager.Instance.GetCurrentGameStats());
        SaveSystem.SaveGame(saveData);
    }

    // Audio
    private void HandleMusic(GameState state)
    {
        switch (state)
        {
            case GameState.STARTING:
                AudioManager.Instance.PlayMusic(0); // musica menu
                break;

            case GameState.PLAYING:
            case GameState.SHOPPING:
                if (AudioManager.Instance != null &&
                    (AudioManager.Instance.CurrentTrackIndex != 1 || !AudioManager.Instance.IsPlaying))
                {
                    AudioManager.Instance.PlayMusic(1); // musica fight
                }
                // torna al volume pieno
                AudioManager.Instance.FadeTo(1f, 1.5f);
                break;

            case GameState.PAUSED:
                // abbassa il volume ma non ferma la musica
                AudioManager.Instance.FadeTo(0.3f, 1.5f);
                break;

            case GameState.GAMEOVER:
                AudioManager.Instance.StopMusic();
                break;
        }
    }
}
