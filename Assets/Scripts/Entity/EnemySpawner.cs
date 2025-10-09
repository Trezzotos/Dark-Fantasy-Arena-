using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Entity
{
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
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
        hbInitialScale = healthBar.localScale;
        FullyHeal();

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
    }

    protected override void Die()
    {
        StopAllCoroutines();
        base.Die();
    }
}
