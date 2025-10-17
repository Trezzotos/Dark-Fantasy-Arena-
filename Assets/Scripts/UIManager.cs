using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pannelli di UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    void OnEnable()
    {
        GameManager.onGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.onGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        startPanel.SetActive(state == GameState.STARTING);
        pausePanel.SetActive(state == GameState.PAUSED);
        gameOverPanel.SetActive(state == GameState.GAMEOVER);
    }
}
