using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GameObject gameOverUI;

    public enum GameState
    {
        PLAYING,
        SHOPPING,
        GAMEOVER
    }
    public GameState gameState;

    public void Gameover()
    {
        gameState = GameState.GAMEOVER;
        gameOverUI.SetActive(true);
    }

    void Start()
    {
        gameState = GameState.PLAYING;
    }

    void Awake()
    {
        // Se non esiste ancora un'istanza, questa è l'istanza.
        if (Instance == null)
        {
            Instance = this;
            // impedisce che l'oggetto venga distrutto al cambio di scena.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se esiste già un'istanza, distruggi questo oggetto (perché è un duplicato).
            Destroy(gameObject);
        }
    }
}