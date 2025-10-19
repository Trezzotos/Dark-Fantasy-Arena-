using System;
using UnityEngine;

public enum GameState
{
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

    public int difficolta;

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
        PrepareGame();
    }

    public void PrepareGame()
{
    UpdateGameState(GameState.STARTING);
    LoadGameData();

    if (StatsManager.Instance != null)
    {
        Level = StatsManager.Instance.currentLevel;
        OnLevelChanged?.Invoke(Level);
    }
    else
    {
        Debug.LogWarning("StatsManager non trovato, imposto livello di default = 1");
        Level = 1;
        OnLevelChanged?.Invoke(Level);
    }
}


    private void Update()
    {
        if (gameState == GameState.PLAYING && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PAUSED);
        else if (gameState == GameState.PAUSED && Input.GetKeyDown(KeyCode.Escape))
            UpdateGameState(GameState.PLAYING);
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;
        Time.timeScale = (newState == GameState.PLAYING) ? 1f : 0f;
        OnGameStateChanged?.Invoke(newState);

        HandleMusic(newState);
    }

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
        {
            inventory.ApplyLoadedInventoryData(loadedData.inventory);
        }
        else
        {
            Debug.LogWarning("Inventory non trovato in scena!");
        }

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ApplyGameStats(loadedData.gameStats);
        }
        else
        {
            Debug.LogWarning("StatsManager non trovato in scena!");
        }
    }
    else
    {
        Debug.Log("Nessun salvataggio trovato, si parte da zero.");
    }
}


    private void SaveGameData()
    {
        InventoryData inventoryData = FindObjectOfType<Examples.Observer.Inventory>().GetCurrentInventoryData();
        GameSaveData saveData = new GameSaveData(inventoryData, StatsManager.Instance.GetCurrentGameStats());
        SaveSystem.SaveGame(saveData);
    }
}
