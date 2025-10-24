using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveManager : MonoBehaviour
{

    [Serializable]
    private class Wave
    {   
        public GameObject[] structuresPrefabs;   // prefab dello spawner da instanziare
        public int enemyCount;          // quanti nemici deve generare lo spawner
        public float spawnInterval;         // intervallo tra uno spawn e l'altro
    }

    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform[] spawnPoints;
    private List<Structure> activeStructures = new List<Structure>();

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

            // attendi fine wave: nessun nemico e nessuno spawner vivo
            yield return new WaitUntil(() =>
                EnemyManager.Instance.ActiveEnemyCount == 0 && activeStructures.Count == 0);

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

        if (spawnPoints == null)
        {
            Debug.LogError("No spawn points available for this wave.");
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
            {
                Debug.LogWarning("Spawn point is null, skipping.");
                continue;
            }

            var structureGameobject = Instantiate(
                wave.structuresPrefabs[UnityEngine.Random.Range(0, wave.structuresPrefabs.Length)],
                point.position,
                point.rotation
            );

            var structure = structureGameobject.GetComponent<Structure>();
            if (structure == null)
            {
                Debug.LogError("Il nuovo prefab deve avere lo script Structure.");
                Destroy(structureGameobject);
                continue;
            }

            if (structure is EnemySpawner spawner)
            {
                // Configurazioni per questa wave
                SpawnerPrefabSetup(spawner, wave);
            }
            else if (structure is Tesla tesla)
            {
                // modifica i parametri del danno
            }

            structure.OnStructureDestroyed += HandleStructureDestoyed;
            activeStructures.Add(structure);
        }
    }


    private void CleanupSpawners()
    {
        foreach (var s in activeStructures)
        {
            if (s != null)
            {
                s.OnStructureDestroyed -= HandleStructureDestoyed;
                s.Deactivate();
                Destroy(s.gameObject);
            }
        }
        activeStructures.Clear();
    }

    private void HandleStructureDestoyed(Structure structure)
    {
        if (activeStructures.Contains(structure))
            activeStructures.Remove(structure);
    }
    
    private void SpawnerPrefabSetup(EnemySpawner spawner, Wave wave)
    {
        // spawner.prefabIndexToSpawn = wave.prefabIndex;
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

        spawner.Activate();
    }

}

