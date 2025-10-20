using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject TutorialPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button rankingButton;

    private void Start()
    {
        // Se non c’è un salvataggio, disabilita il pulsante Continue
        if (!SaveSystem.SaveFileExists())
        {
            continueButton.GetComponent<Image>().color = Color.gray;
            continueButton.interactable = false;

            rankingButton.GetComponent<Image>().color = Color.gray;
            rankingButton.interactable = false;
            return;
        }

        // All’avvio mostra solo il main menu
        mainMenuPanel.SetActive(true);
        difficultyPanel.SetActive(false);
        TutorialPanel.SetActive(false);
    }

    //  Chiamato dal pulsante "New Game"
    public void NewGame()
    {
        if (SaveSystem.SaveFileExists())
        {
            Debug.LogWarning("Esiste già un salvataggio, verrà sovrascritto se scegli una difficoltà.");
        }

        // Nascondo il menu principale e il tutorial e mostro il tutorial
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        TutorialPanel.SetActive(true);
    }

    //  Chiamato dai pulsanti di difficoltà (es. Easy/Normal/Hard)
    public void SelectDifficulty(int difficulty)
    {
        SaveSystem.GenerateEmptySaveFile(difficulty);
        SceneManager.LoadScene("Game");
    }

    public void Continue()
    {
        SceneManager.LoadScene("Game");
    }

    public void OptionMenu()
    {
        SceneManager.LoadScene("Settings");
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

    public void ShowTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void HideTutorial()
    {
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
        TutorialPanel.SetActive(false);
    }
    
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
}
