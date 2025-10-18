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
    public static event Action<int> OnLevelChanged;   // 👉 nuovo evento

    public GameState gameState { get; private set; }
    public int Level { get; private set; } = 1;       // 👉 parte da livello 1

    [Header("UI")]
    [SerializeField] private GameObject levelDialog;  // 👉 assegna un pannello UI in Inspector
    [SerializeField] private TMPro.TextMeshProUGUI levelText; // 👉 testo per mostrare il livello

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

        // Mostra la finestra di dialogo
        if (levelDialog != null)
        {
            levelDialog.SetActive(true);
            if (levelText != null)
                levelText.text = $"Livello {Level}";
        }

        // Metti il gioco in pausa mentre mostri la finestra
        UpdateGameState(GameState.SHOPPING);
    }

    // 👉 Metodo chiamato da un bottone UI "Continua"
    public void ContinueGame()
    {
        if (levelDialog != null)
            levelDialog.SetActive(false);

        UpdateGameState(GameState.PLAYING);
    }
    
}
