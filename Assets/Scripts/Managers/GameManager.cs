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

    public GameState gameState { get; set; }
    public int Level { get; private set; } = 1;

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
        Level = StatsManager.Instance.currentLevel;
        OnLevelChanged?.Invoke(Level);
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
    }

    // 👉 Metodo per passare al livello successivo
    public void NextLevel()
    {
        Level++;
        OnLevelChanged?.Invoke(Level);
        SaveGameData();

        // 👉 Non gestisce più la UI, solo lo stato
        UpdateGameState(GameState.SHOPPING);
    }

    private void LoadGameData()
    {
        GameSaveData loadedData = SaveSystem.LoadGame();

        if (loadedData != null)
        {
            // La Find è un po' una pezza, non va assolutamente bene
            FindObjectOfType<Examples.Observer.Inventory>().ApplyLoadedInventoryData(loadedData.inventory);

            StatsManager.Instance.ApplyGameStats(loadedData.gameStats);
        }
    }

    private void SaveGameData()
    {
        InventoryData inventoryData = FindObjectOfType<Examples.Observer.Inventory>().GetCurrentInventoryData();

        GameSaveData saveData = new GameSaveData(inventoryData, StatsManager.Instance.GetCurrentGameStats());

        SaveSystem.SaveGame(saveData);
    }

}
