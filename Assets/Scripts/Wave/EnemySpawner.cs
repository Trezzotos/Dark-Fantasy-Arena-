using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Examples.Observer;

public class EnemySpawner : Structure
{
    public float spawnInterval = 1.5f;
    public int maxSpawnCount = 3;

    int spawned = 0;
    private Coroutine spawnRoutine;

    protected override void Awake()
    {
        base.Awake();
        spawned = 0;
    }

    private IEnumerator SpawnLoop()
    {
        while (active && spawned < maxSpawnCount)
        {
            int maxInstances = 3 * Mathf.Max(1, GameManager.Instance.Level);
            int currentInstances = EnemyManager.Instance.ActiveEnemyCount;

            if (currentInstances < maxInstances)
            {
                // invece di spawnare direttamente, accodiamo l'azione a WaveManager
                // WaveManager eseguirà l'azione serialmente per evitare spike simultanei
                WaveManager.Instance.RequestSpawn(() =>
                {
                    // questa action viene eseguita da WaveManager.ProcessSpawnQueue
                    // qui manteniamo la logica di registrazione/spawn originale
                    EnemyManager.Instance.SpawnEnemy(transform.position);
                });

                spawned++;
            }

            // se il gioco è in PAUSE e vuoi che spawnLoop rispetti il Time.timeScale,
            // usare WaitForSeconds(spawnInterval) invece di WaitForSecondsRealtime
            yield return new WaitForSeconds(spawnInterval);
        }

        spawnRoutine = null;
    }

    public override void Activate()
    {
        base.Activate();

        spawned = 0;
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

}
