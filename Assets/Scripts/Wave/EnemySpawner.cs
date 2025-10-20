using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Examples.Observer;

public class EnemySpawner : MonoBehaviour
{
    public event Action<EnemySpawner> OnSpawnerKilled;

    public float spawnInterval = 1.5f;
    public int prefabIndexToSpawn = 0;
    public int maxSpawnCount = 3;

    int spawned = 0;
    bool active = false;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Examples.Observer.Health>();
        if (health == null) Debug.LogError("EnemySpawner richiede un Health sullo stesso GameObject.");
    }

    private void OnEnable()
    {
        if (health != null)
            health.Killed += HandleKilled;
    }

    private void OnDisable()
    {
        if (health != null)
            health.Killed -= HandleKilled;
    }

    public void Activate()
    {
        active = true;
        spawned = 0;
        StartCoroutine(SpawnLoop());
    }

    public void Deactivate()
    {
        active = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        while (active && spawned < maxSpawnCount)
        {
            int maxInstances = 3 * GameManager.Instance.Level; // esempio: 3 * livello
            int currentInstances = EnemyManager.Instance.ActiveEnemyCount;

            if (currentInstances < maxInstances)
            {
                EnemyManager.Instance.SpawnEnemy(prefabIndexToSpawn, transform.position);
                spawned++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    // Solo per lo spawner
    private void HandleKilled()
    {
        Deactivate();
        OnSpawnerKilled?.Invoke(this);
    }
}
