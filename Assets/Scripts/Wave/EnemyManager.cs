using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private int poolSizePerPrefab = 10; // quanti oggetti per tipo creare in pool

    private Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();
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

        InitializePools();
    }

    private void InitializePools()
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            pools[i] = new Queue<GameObject>();

            for (int j = 0; j < poolSizePerPrefab; j++)
            {
                GameObject obj = Instantiate(enemyPrefabs[i]);
                obj.SetActive(false);
                pools[i].Enqueue(obj);
            }
        }
    }

    public void SpawnEnemy(int prefabIndex, Vector3 position)
    {
        if (!pools.ContainsKey(prefabIndex))
        {
            Debug.LogError($"SpawnEnemy: indice {prefabIndex} fuori range");
            return;
        }

        GameObject obj;
        if (pools[prefabIndex].Count > 0)
        {
            obj = pools[prefabIndex].Dequeue();
        }
        else
        {
            obj = Instantiate(enemyPrefabs[prefabIndex]);
        }

        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);

        // 👉 inizializza AIEnemy con il suo indice
        AIEnemy ai = obj.GetComponent<AIEnemy>();
        if (ai != null)
            ai.Initialize(prefabIndex);

        activeEnemies.Add(obj);
    }


    public void DespawnEnemy(GameObject enemy, int prefabIndex)
    {
        if (enemy == null) return;

        enemy.SetActive(false);
        activeEnemies.Remove(enemy);

        if (pools.ContainsKey(prefabIndex))
            pools[prefabIndex].Enqueue(enemy);
        else
            Destroy(enemy); // fallback
    }

    public void SpawnRandomEnemy(Vector3 position)
    {
        int idx = Random.Range(0, enemyPrefabs.Count);
        SpawnEnemy(idx, position);
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
