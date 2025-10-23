using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Examples.Observer;

public class EnemySpawner : Structure
{
    public float spawnInterval = 1.5f;
    // public int prefabIndexToSpawn = 0;
    public int maxSpawnCount = 3;

    int spawned = 0;
    // private Health health;

    protected override void Awake()
    {
        base.Awake();

        // in più gli aggiungo
        spawned = 0;
    }

    private IEnumerator SpawnLoop()
    {
        while (active && spawned < maxSpawnCount)
        {
            int maxInstances = 3 * GameManager.Instance.Level; // esempio: 3 * livello
            int currentInstances = EnemyManager.Instance.ActiveEnemyCount;

            if (currentInstances < maxInstances)
            {
                EnemyManager.Instance.SpawnEnemy(transform.position);
                spawned++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public override void Activate()
    {
        base.Activate();

        // solo per EnemySpawner
        StartCoroutine(SpawnLoop());
    }
}
