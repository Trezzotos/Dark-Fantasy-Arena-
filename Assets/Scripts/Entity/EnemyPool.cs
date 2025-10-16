using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{/*
    [SerializeField] private List<Enemy> enemyPrefabs;
    private Dictionary<string, Queue<Enemy>> pool = new();

    public Enemy GetEnemy(string type)
    {
        if (!pool.ContainsKey(type))
            pool[type] = new Queue<Enemy>();

        if (pool[type].Count > 0)
        {
            var enemy = pool[type].Dequeue();
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        var newEnemy = Instantiate(enemyPrefabs.Find(e => e.Type == type));
        newEnemy.Pool = this;
        return newEnemy;
    }

    public void ReturnToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        pool[enemy.Type].Enqueue(enemy);
    }*/
}
