using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Pannelli di UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        startPanel.SetActive(state == GameState.STARTING);
        pausePanel.SetActive(state == GameState.PAUSED);
        gameOverPanel.SetActive(state == GameState.GAMEOVER);
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
}
