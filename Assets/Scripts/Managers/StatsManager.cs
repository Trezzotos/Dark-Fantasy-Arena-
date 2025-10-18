using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public int currentLevel;
    public int enemiesKilled;
    public int structuresDestroyed;
    public int currentDifficulty;    // will be initialized when starting the new game
    public float playTimeSeconds;

    bool isTimePassing = false;
    float beginCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Signal subscription
    void OnEnable()
    {
        GameManager.OnGameStateChanged += CountTime;
        GameManager.OnLevelChanged += UpdateLevel;
        EnemyManager.Instance.OnEnemyDefeated += OnEnemyKilled;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= CountTime;
        GameManager.OnLevelChanged -= UpdateLevel;
        EnemyManager.Instance.OnEnemyDefeated -= OnEnemyKilled;
    }

    // Signal handling
    void OnEnemyKilled()
    {
        enemiesKilled++;
        print(enemiesKilled);
    }

    void UpdateLevel(int level)
    {
        currentLevel = level;
    }

    void CountTime(GameState gameState)
    {
        if (!isTimePassing && gameState == GameState.PLAYING)
        {
            isTimePassing = true;
            beginCount = Time.time;
        }

        else if (isTimePassing && gameState != GameState.PLAYING)
        {
            isTimePassing = false;
            playTimeSeconds += Time.time - beginCount;
        }
    }

    // GameSave
    public GameStatsData GetCurrentGameStats()
    {
        return new GameStatsData(currentLevel, enemiesKilled, structuresDestroyed, currentDifficulty, math.floor(playTimeSeconds));
    }

    public void ApplyGameStats(GameStatsData loadedData)
    {
        if (loadedData != null)
        {
            currentLevel = loadedData.currentLevel;
            enemiesKilled = loadedData.enemiesKilled;
            currentDifficulty = loadedData.currentDifficulty;
            playTimeSeconds = loadedData.playTimeSeconds;
        }
    }
}