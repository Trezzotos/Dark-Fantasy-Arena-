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
        // continui finché il gioco non è finito
        while (GameManager.Instance != null && GameManager.Instance.gameState != GameState.GAMEOVER)
        {
            // usa currentWaveIndex come contatore progressivo, ma prendi la definizione tramite modulo
            int templateIndex = waves.Count > 0 ? currentWaveIndex % waves.Count : 0;
            Wave waveTemplate = waves[templateIndex];

            // spawn degli spawner per la wave corrente (con configurazione scalata)
            SpawnWaveSpawners(waveTemplate);

            // attiva gli spawner
            foreach (var spawner in activeSpawners) spawner.Activate();

            // attendi fine wave: nessun nemico e nessuno spawner vivo
            yield return new WaitUntil(() =>
                EnemyManager.Instance.ActiveEnemyCount == 0 && activeSpawners.Count == 0);

            // segnala fine livello e lascia che il GameManager gestisca aumento livello / shop
            GameManager.Instance.NextLevel();

            // aspetta che il giocatore riprenda (continua quando torna in PLAYING)
            yield return new WaitUntil(() => GameManager.Instance.gameState == GameState.PLAYING);

            // avanti la wave-counter (diventa il "prossimo livello/wave")
            currentWaveIndex++;
        }
        waveRoutine = null;
    }




    private void SpawnWaveSpawners(Wave wave)
    {
        CleanupSpawners(); // sicurezza: pulisci lista dei riferimenti

        // Se la wave definisce punti propri, usali; altrimenti usa i spawnPoints globali
        Transform[] pointsToUse = (wave.spawnerPoints != null && wave.spawnerPoints.Length > 0)
            ? wave.spawnerPoints
            : spawnPoints;

        if (pointsToUse == null || pointsToUse.Length == 0)
        {
            Debug.LogError("No spawn points available for this wave.");
            return;
        }

        foreach (var point in pointsToUse)
        {
            if (point == null)
            {
                Debug.LogWarning("Spawn point is null, skipping.");
                continue;
            }

            var spawnerGO = Instantiate(wave.spawnerPrefab, point.position, point.rotation);
            var spawner = spawnerGO.GetComponent<EnemySpawner>();
            if (spawner == null)
            {
                Debug.LogError("Il prefab dello spawner deve avere lo script EnemySpawner.");
                Destroy(spawnerGO);
                continue;
            }

            // Configurazioni per questa wave
            SpawnerPrefabSetup(spawner, wave);

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
    
    private void SpawnerPrefabSetup(EnemySpawner spawner, Wave wave)
    {
        spawner.prefabIndexToSpawn = wave.prefabIndex;
        spawner.spawnInterval = wave.spawnInterval;

        // baseCount è il valore definito nella wave (inteso come count per spawner nella definizione)
        int baseCount = Mathf.Max(1, wave.enemyCount);

        // livello corrente (se GameManager non esiste usiamo 1)
        int level = GameManager.Instance != null ? Mathf.Max(1, GameManager.Instance.Level) : 1;

        // moltiplicatore esponenziale/lineare a scelta — qui uso +15% per livello (modificabile)
        float perLevelIncrease = 0.15f; // 15% per livello
        float multiplier = 1f + perLevelIncrease * (level - 1);

        // assegno il numero scalato per spawner
        spawner.maxSpawnCount = Mathf.Max(1, Mathf.RoundToInt(baseCount * multiplier));
    }

}

