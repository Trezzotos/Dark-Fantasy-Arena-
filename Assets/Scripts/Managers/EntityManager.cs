using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{

    public static EntityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.PAUSED)
            StopAllSpawns();
        else if (state == GameState.GAMEOVER)
            ReturnAllToPool();
    }

    public void SpawnWave(WaveData wave)
    {
        StartCoroutine(SpawnRoutine(wave));
    }

    private IEnumerator SpawnRoutine(WaveData wave)
    {
        foreach (var entry in wave.entries)
        {
            yield return new WaitForSeconds(entry.spawnDelay);
            var enemy = PoolManager.Instance.Get(entry.enemyType);
            enemy.Initialize(entry.config);
            enemy.OnDeath += () => PoolManager.Instance.Release(entry.enemyType, enemy);
        }
    }

    private void StopAllSpawns()
    {
        StopAllCoroutines();
    }

    private void ReturnAllToPool()
    {
        PoolManager.Instance.ReleaseAll();
    }
}







/*
    public static EntityManager Instance { get; private set; }

    [Header("Spawn rectangle")]
    public Vector2 minPos;
    public Vector2 maxPos;

    //[Header("References")]
    //public Entity[] structures; Era legato ad Entity, Da sistemare

    public enum Difficoulty
    {
        UNSET,
        EASY,
        MEDIUM,                                 //the difficolty will be set previously
        HARD
    }
    public Difficoulty difficoulty = Difficoulty.UNSET;
    public int level = 1;

    // Questo tipo di advancement non è definitivo (forse)
    public void PrepareLevel()
    {
        return;
      /*  for (int i = 0; i < (int)(level * (int)difficoulty / 2) + 3; i++)
        {
            Entity sorted = structures[Random.Range(0, structures.Length)];

            if (sorted.TryGetComponent(out EnemySpawner sortedSpawner))
            {
                sortedSpawner.minZHealth = 10 + level * (int)difficoulty;
                sortedSpawner.maxZHealth = 30 + level * (int)difficoulty;
                sortedSpawner.minZDamage = 2 + level * (int)difficoulty;
                sortedSpawner.maxZDamage = 5 + level * (int)difficoulty;
            }

            if (sorted.TryGetComponent(out Tesla sortedTesla))
            {
                sortedTesla.damage = 20 + level * (int)difficoulty;
                // sortedTesla.maxHealth = 50 + level * (int)difficoulty;
            }

            Instantiate(sorted, new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y)), Quaternion.identity);
        }
    }

    public void ClearLevel()
    {
        GameObject[] structures = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject structure in structures)
        {
            Destroy(structure);
        }
    }

    public void NextLevel()
    {
        level++;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficoulty(Difficoulty d)
    {
        difficoulty = d;
    }*/