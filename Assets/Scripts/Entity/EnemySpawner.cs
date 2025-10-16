using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool pool;
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnWave(int roundNumber)
    {
        int enemiesToSpawn = CalculateEnemiesForRound(roundNumber);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            string enemyType = GetEnemyTypeForRound(roundNumber);
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        //    var enemy = pool.GetEnemy(enemyType);
       //        enemy.transform.position = spawnPoint.position;
        }
    }

    private int CalculateEnemiesForRound(int round)
    {
        // Formula di difficoltà
        return 5 + Mathf.FloorToInt(Mathf.Pow(round, 1.5f));
    }

    private string GetEnemyTypeForRound(int round)
    {
        // Sblocca nemici nuovi man mano
        if (round < 3) return "Basic";
        if (round < 6) return Random.value < 0.7f ? "Basic" : "Fast";
        return Random.value < 0.5f ? "Tank" : "Fast";
    }
    /*
    public Enemy[] enemyPool;
    [Tooltip("Enemies / sec")]
    public int spawnFreq = 10;

    [Header("Zombie properties")]
    public float minZHealth = 1;
    public float maxZHealth = 10;
    [Space]     // these values will be determined by the GameManager based on the level and the difficoulty
    public float minZDamage = 1;
    public float maxZDamage = 5;

    void Start()
    {
        // will be invoked by GameManager
        StartSpawn();
    }

    public void StartSpawn()
    {
        StartCoroutine(nameof(Spawn));
    }

    IEnumerator Spawn()
    {
        Instantiate(enemyPool[Random.Range(0, enemyPool.Length)], transform.position, transform.rotation);
        yield return new WaitForSeconds(spawnFreq);
        StartCoroutine(nameof(Spawn));
    }*/
}
