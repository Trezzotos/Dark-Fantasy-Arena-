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
    public int currentDifficulty;
    public float playTimeSeconds;
    public int score;
    public string playerName;

    bool isTimePassing = false;
    int scoreIncrement = 100;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isTimePassing) playTimeSeconds += Time.deltaTime;
    }

    // Signal subscription
    void OnEnable()
    {
        GameManager.OnGameStateChanged += CountTime;
        GameManager.OnLevelChanged += UpdateLevel;
        EnemyManager.Instance.OnEnemyDefeated += OnEnemyKilled;
        WaveManager.Instance.OnStructureDestroyed += OnStructureDestoyed;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= CountTime;
        GameManager.OnLevelChanged -= UpdateLevel;
        EnemyManager.Instance.OnEnemyDefeated -= OnEnemyKilled;
        WaveManager.Instance.OnStructureDestroyed -= OnStructureDestoyed;
    }

    // Signal handling
    void OnEnemyKilled(Vector2 position)
    {
        enemiesKilled++;
        score += scoreIncrement;
    }

    void OnStructureDestoyed()
    {
        structuresDestroyed++;
        score += scoreIncrement * 2;
    }

    void UpdateLevel(int level)
    {
        currentLevel = level;
    }

    void CountTime(GameState gameState)
    {
        if (gameState == GameState.PLAYING)
            isTimePassing = true;

        else if (gameState != GameState.PLAYING)
            isTimePassing = false;
    }

    // GameSave
    public GameStatsData GetCurrentGameStats()
    {
        return new GameStatsData(
            currentLevel,
            enemiesKilled,
            structuresDestroyed,
            currentDifficulty,
            math.floor(playTimeSeconds),
            score,
            playerName
        );
    }

    public void ApplyGameStats(GameStatsData loadedData)
    {
        if (loadedData != null)
        {
            currentLevel = loadedData.currentLevel;
            enemiesKilled = loadedData.enemiesKilled;
            structuresDestroyed = loadedData.structuresDestroyed;
            currentDifficulty = loadedData.currentDifficulty;
            playTimeSeconds = loadedData.playTimeSeconds;
            this.score = loadedData.score;
            playerName = loadedData.name;
        }
    }
}