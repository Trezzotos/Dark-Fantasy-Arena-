using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialMenu : MonoBehaviour
{
    [Header("Background Sets")]
    [SerializeField] private GameObject[] primarySet;   // Set principale (es. posSet1)
    [SerializeField] private GameObject[] secondarySet; // Set alternativo (es. posSet2)

    [Header("Navigation Buttons")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button showOtherSetButton;

    [Header("UI Text References")]
    [SerializeField] private TMP_Text showOtherSetButtonText;   


    private int currentIndex = 0;
    private bool showingSecondarySet = false;

    private void Start()
    {
        // Inizializza listener dei pulsanti
        previousButton.onClick.AddListener(OnPreviousClicked);
        nextButton.onClick.AddListener(OnNextClicked);
        showOtherSetButton.onClick.AddListener(OnShowOtherSetClicked);

        // Aggiorna visibilità iniziale
        UpdateBackgroundVisibility();
        UpdateButtonVisibility();
    }

    /// <summary>
    /// Passa al background precedente.
    /// </summary>
    private void OnPreviousClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateBackgroundVisibility();
            UpdateButtonVisibility();
        }
    }

    /// <summary>
    /// Passa al background successivo o al MainMenu se siamo all’ultimo.
    /// </summary>
    private void OnNextClicked()
    {
        if (currentIndex < GetActiveSet().Length - 1)
        {
            currentIndex++;
            UpdateBackgroundVisibility();
            UpdateButtonVisibility();
        }
        else
        {
            // Carica la scena MainMenu se siamo all’ultimo background
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Mostra l’altro set di background, se disponibile.
    /// </summary>
    private void OnShowOtherSetClicked()
    {
        if (currentIndex < secondarySet.Length)
        {
            showingSecondarySet = !showingSecondarySet;
            UpdateBackgroundVisibility();
            UpdateButtonVisibility();

            // Aggiorna il testo del bottone tramite riferimento
            if (showOtherSetButtonText != null)
            {
                showOtherSetButtonText.text = showingSecondarySet ? "Show Set 1" : "Show Set 2";
            }
        }
        else
        {
            showOtherSetButton.gameObject.SetActive(false);
        }
    }



    /// <summary>
    /// Aggiorna la visibilità dei background in base allo stato attuale.
    /// </summary>
    private void UpdateBackgroundVisibility()
    {
        GameObject[] activeSet = GetActiveSet();
        GameObject[] inactiveSet = showingSecondarySet ? primarySet : secondarySet;

        // Disattiva tutti
        foreach (var bg in primarySet) bg.SetActive(false);
        foreach (var bg in secondarySet) bg.SetActive(false);

        // Attiva solo quello corrente
        if (currentIndex < activeSet.Length)
        {
            activeSet[currentIndex].SetActive(true);
        }
    }

    /// <summary>
    /// Aggiorna la visibilità e l’interattività dei pulsanti.
    /// </summary>
    private void UpdateButtonVisibility()
    {
        GameObject[] activeSet = GetActiveSet();

        // Previous attivo solo se non siamo al primo
        previousButton.gameObject.SetActive(currentIndex > 0);

        // Next sempre attivo (carica MainMenu se siamo all’ultimo)
        nextButton.gameObject.SetActive(true);

        // ShowOtherSet visibile solo se c’è un elemento corrispondente
        bool hasOtherSet = currentIndex < secondarySet.Length;
        showOtherSetButton.gameObject.SetActive(hasOtherSet);
    }

    /// <summary>
    /// Restituisce il set di background attualmente mostrato.
    /// </summary>
    private GameObject[] GetActiveSet()
    {
        return showingSecondarySet ? secondarySet : primarySet;
    }
}
