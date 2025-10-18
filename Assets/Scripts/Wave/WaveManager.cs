using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveManager : MonoBehaviour
{

    [Serializable]
    private class Wave
    {   
        public GameObject spawnerPrefab;   // prefab dello spawner da instanziare
        public Transform[] spawnerPoints;   // punti in cui instanziare gli spawner
        public int prefabIndex;         // indice del tipo di nemico (se serve)
        public int enemyCount;          // quanti nemici deve generare lo spawner
        public float spawnInterval;         // intervallo tra uno spawn e l'altro
    }

    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;
    private List<EnemySpawner> activeSpawners = new List<EnemySpawner>();

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
            // 1) Instanzia gli spawner della wave
            SpawnWaveSpawners(wave);

            // 2) Attiva gli spawner per iniziare a generare nemici
            foreach (var spawner in activeSpawners) spawner.Activate();

            // 3) Attendi fine wave: nessun nemico e nessuno spawner vivo
            yield return new WaitUntil(() =>
                EnemyManager.Instance.ActiveEnemyCount == 0 && activeSpawners.Count == 0);

            // 👉 qui invece di aspettare solo un tempo fisso,
            // segnali al GameManager che il livello è finito
            GameManager.Instance.NextLevel();

            // 👉 aspetti che il giocatore prema "Continua"
            yield return new WaitUntil(() => GameManager.Instance.gameState == GameState.PLAYING);
        }

        waveRoutine = null;
    }



    private void SpawnWaveSpawners(Wave wave)
    {
        CleanupSpawners(); // sicurezza: pulisci lista dei riferimenti

        foreach (var point in spawnPoints)
        {
            var spawnerGO = Instantiate(wave.spawnerPrefab, point.position, point.rotation);
            var spawner = spawnerGO.GetComponent<EnemySpawner>();
            if (spawner == null)
            {
                Debug.LogError("Il prefab dello spawner deve avere lo script EnemySpawner.");
                Destroy(spawnerGO);
                continue;
            }

            // Configurazioni per questa wave
            spawnerPrefabSetup(spawner, wave);

            // Subscribe alla distruzione
            spawner.OnSpawnerKilled += HandleSpawnerKilled;

            activeSpawners.Add(spawner);
        }
    }

    private void CleanupSpawners()
    {
        foreach (var s in activeSpawners)
        {
            if (s != null)
            {
                s.OnSpawnerKilled -= HandleSpawnerKilled;
                s.Deactivate();
                Destroy(s.gameObject);
            }
        }
        activeSpawners.Clear();
    }

    private void HandleSpawnerKilled(EnemySpawner spawner)
    {
        if (activeSpawners.Contains(spawner))
            activeSpawners.Remove(spawner);
    }
    
    private void spawnerPrefabSetup(EnemySpawner spawner, Wave wave)
    {
        spawner.prefabIndexToSpawn = wave.prefabIndex;
        spawner.maxSpawnCount = wave.enemyCount;
        spawner.spawnInterval = wave.spawnInterval;
    }
}

