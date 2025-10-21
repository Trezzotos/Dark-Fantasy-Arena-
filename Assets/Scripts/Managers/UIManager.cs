using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Pannelli di UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelDialog;
    [SerializeField] private TMPro.TextMeshProUGUI levelText;

    [Header("UI di Gioco")]
    [SerializeField] private GameObject UIPlayer;   // <-- aggiunto

    [Header("Endgame Stats")]
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text enemies;
    [SerializeField] private TMP_Text structures;
    [SerializeField] private TMP_Text seconds;
    [SerializeField] private TMP_Text difficoulty;

    [Header("Endgame Scoreboard")]
    [SerializeField] private TMP_Text[] scoreboardText;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        GameManager.OnLevelChanged += HandleLevelChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        GameManager.OnLevelChanged -= HandleLevelChanged;
    }

    private void HandleLevelChanged(int newLevel)
    {
        if (levelText != null)
            levelText.text = $"Livello {newLevel}";
    }

    private void HandleGameStateChanged(GameState state)
    {
        startPanel.SetActive(state == GameState.STARTING);
        pausePanel.SetActive(state == GameState.PAUSED);
        levelDialog.SetActive(state == GameState.SHOPPING);
        gameOverPanel.SetActive(state == GameState.GAMEOVER);

        // UIPlayer attivo solo in gameplay
        if (UIPlayer != null)
            UIPlayer.SetActive(state == GameState.PLAYING);

        if (state == GameState.GAMEOVER)
            ShowGameover();
    }
    
    private void ShowGameover()
    {
        // Stats
        level.text = $"{StatsManager.Instance.currentLevel}";
        enemies.text = $"{StatsManager.Instance.enemiesKilled}";
        structures.text = $"{StatsManager.Instance.structuresDestroyed}";
        seconds.text = $"{math.ceil(StatsManager.Instance.playTimeSeconds)}";

        difficoulty.text = StatsManager.Instance.currentDifficulty switch
        {
            1 => "Easy",
            2 => "Medium",
            3 => "Hard",
            _ => "I Dunno :(",
        };

        // Scoreboard
        ScoreboardManager.TryAddHighscore(StatsManager.Instance.playerName, StatsManager.Instance.score);
        List<ScoreEntry> scoreboard = ScoreboardManager.GetScoreboard();

        int i = 0;
        foreach (var record in scoreboard)
        {
            scoreboardText[i].text = $"{record.name} {record.score}";
            i++;
        }
    }

    //---- Bottoni via Inspector ----
    public void OnStartButton()
    {
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
    }

    public void OnResumeButton()
    {
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
    }

    public void OnQuitButton()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.UpdateGameState(GameState.MAINMENU);
    }

    public void OnContinueButton()
    {
        levelDialog.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
    }

    public void OnShopButton()
    {
        SceneManager.LoadScene("Shop");
    }
}
