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
        gameOverPanel.SetActive(state == GameState.GAMEOVER);
        levelDialog.SetActive(state == GameState.SHOPPING);
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

    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.UpdateGameState(GameState.STARTING);
    }

    public void OnQuitButton()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.UpdateGameState(GameState.GAMEOVER);
    }

    public void OnContinueButton()
    {
        // 👉 Chiude il dialogo e rimette il gioco in PLAYING
        levelDialog.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.PLAYING);
    }
}
