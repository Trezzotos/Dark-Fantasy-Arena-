using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy[] enemyPool;
    public int spawnCount = 0;

    [Header("Zombie properties")]
    public float minHealth = 1;
    public float maxHealth = 10;
    [Space]     // these values will be determined by the GameManager based on the level and the difficoulty
    public float minDamage = 1;
    public float maxDamage = 5;

    void Start()
    {
        // debug only
        StartCoroutine("Spawn");
    }

    public IEnumerator Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(enemyPool[Random.Range(0, enemyPool.Length)], transform.position, transform.rotation);
            yield return new WaitForSeconds(2);
        }
    }
}
