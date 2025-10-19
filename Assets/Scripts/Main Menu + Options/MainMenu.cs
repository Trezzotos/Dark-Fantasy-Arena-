using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject difficultyPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    private void Start()
    {
        // Se non c’è un salvataggio, disabilita il pulsante Continue
        if (!SaveSystem.SaveFileExists())
        {
            continueButton.GetComponent<Image>().color = Color.gray;
            continueButton.interactable = false;
        }

        // All’avvio mostra solo il main menu
        mainMenuPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    // 👉 Chiamato dal pulsante "New Game"
    public void NewGame()
    {
        if (SaveSystem.SaveFileExists())
        {
            Debug.LogWarning("Esiste già un salvataggio, verrà sovrascritto se scegli una difficoltà.");
        }

        // Nascondo il menu principale e mostro la scelta difficoltà
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    // 👉 Chiamato dai pulsanti di difficoltà (es. Easy/Normal/Hard)
    public void SelectDifficulty(int difficulty)
    {
        // Genera un nuovo salvataggio vuoto con la difficoltà scelta
        SaveSystem.GenerateEmptySaveFile(difficulty);

        // Notifica il GameManager (se già presente in scena)
        if (GameManager.Instance != null)
        {
            StatsManager.Instance.currentDifficulty = difficulty;
            Debug.Log($"Difficoltà selezionata: {difficulty}");
        }

        // Carica la scena di gioco
        SceneManager.LoadScene("Game");
    }

    public void Continue()
    {
        SceneManager.LoadScene("Game");
    }

    public void OptionMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Back()
    {
        mainMenuPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }
}
