using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public event Action<Vector2> OnEnemyDefeated = delegate { };

    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemyCount
    {
        get
        {
            int count = 0;
            foreach (var go in activeEnemies)
                if (go != null && go.activeInHierarchy)
                    count++;
            return count;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnEnemy(Vector3 position)
    {
        GameObject obj = EnemyPool.Instance.GetPooledObject();
        if (!obj)
        {
            Debug.LogWarning("Pool piena");     // rimuovere?
            return;
        }

        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
        obj.SetActive(true);

        // reset health so UI updates when reused from pool
        var health = obj.GetComponent<Examples.Observer.Health>();
        if (health != null)
        {
            health.ResetToFull();

            // applica 0 danno per forzare l'evento Damaged e aggiornare eventuali UI
            // health.TakeDamage(0f);
        }

        activeEnemies.Add(obj);
    }


    public void DespawnEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        enemy.SetActive(false);
        activeEnemies.Remove(enemy);
        OnEnemyDefeated.Invoke(enemy.transform.position);
    }

    public void SpawnRandomEnemy(Vector3 position)
    {
        SpawnEnemy(position);
    }

    public void PauseAllEnemies()
    {
        foreach (var go in activeEnemies)
            if (go != null)
                go.SetActive(false);
    }

    public void ResumeAllEnemies()
    {
        foreach (var go in activeEnemies)
            if (go != null)
                go.SetActive(true);
    }

    public void ClearEnemies()
    {
        foreach (var go in activeEnemies)
        {
            if (go != null)
                go.SetActive(false);
        }
        activeEnemies.Clear();
    }
}
