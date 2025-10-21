using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] coins;
    float dropProbability;

    void Start()
    {
        dropProbability = 1 / (float)(1 + StatsManager.Instance.currentDifficulty * 2);
        print($"Drop probability: {dropProbability}");
    }

    void OnEnable()
    {
        EnemyManager.Instance.OnEnemyDefeated += Drop;
    }

    void OnDisable()
    {
        EnemyManager.Instance.OnEnemyDefeated -= Drop;
    }

    void Drop(Vector2 position)
    {
        if (Random.Range(0f, 1f) <= dropProbability)
        {
            GameObject coin = Instantiate(coins[UnityEngine.Random.Range(0, coins.Length)], position, Quaternion.identity);
        }
    }
}
