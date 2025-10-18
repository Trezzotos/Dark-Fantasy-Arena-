using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveManager : MonoBehaviour
{

    [Serializable]
    private class Wave
    {
        public int prefabIndex;
        public int enemyCount;
        public float spawnInterval;
    }

    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 4f;

    public static event Action OnAllWavesCompleted;

    private int currentWaveIndex = 0;
    private Coroutine waveRoutine;
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.PLAYING:
                EnemyManager.Instance.ResumeAllEnemies();
                if (waveRoutine == null)
                    waveRoutine = StartCoroutine(RunAllWaves());
                break;
            case GameState.PAUSED:
                  EnemyManager.Instance.PauseAllEnemies();
                break;
            case GameState.GAMEOVER:
                 EnemyManager.Instance.ClearEnemies();
                if(waveRoutine != null)
                {
                    StopCoroutine(waveRoutine);
                    waveRoutine = null;
                }
                break;
        }
    }

    private IEnumerator RunAllWaves()
    {
        for (; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            Wave wave = waves[currentWaveIndex];
            yield return StartCoroutine(RunSingleWave(wave));
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        OnAllWavesCompleted?.Invoke();
        waveRoutine = null;
    }
    
    private IEnumerator RunSingleWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            // Attende che il gioco sia in PLAYING (utile se si pausa in mezzo)
            yield return new WaitUntil(() => GameManager.Instance.gameState == GameState.PLAYING);

            // Seleziona spawn point casuale e genera il nemico
            Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            EnemyManager.Instance.SpawnEnemy(wave.prefabIndex, spawn.position);

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // Attende che non ci siano più nemici attivi prima di proseguire
        yield return new WaitUntil(() => EnemyManager.Instance.ActiveEnemyCount == 0);
    }
}

