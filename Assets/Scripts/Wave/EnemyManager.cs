using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    // 1. Assegna qui in Inspector i prefab dei nemici
    [SerializeField]  private List<GameObject> enemyPrefabs = new List<GameObject>();

    // 2. Registro dei GameObject istanziati
    private readonly List<GameObject> activeEnemies = new List<GameObject>();
    // 2.b Contatore di nemici ancora in scena (non null)
    public int ActiveEnemyCount
    {
        get
        {
            int count = 0;
            foreach (var go in activeEnemies)
                if (go != null)
                    count++;
            return count;
        }
    }
    private void Awake()
    {
        // Singleton minimale
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 3. Spawn di un nemico specifico tramite indice
    public void SpawnEnemy(int prefabIndex, Vector3 position)
    {
        if (prefabIndex < 0 || prefabIndex >= enemyPrefabs.Count) //Pool Object
        {
            Debug.LogError($"SpawnEnemy: indice {prefabIndex} fuori range");
            return;
        }

        GameObject go = Instantiate(enemyPrefabs[prefabIndex], position, Quaternion.identity);
        activeEnemies.Add(go);
    }

    // 4. Spawn casuale tra i prefab
    public void SpawnRandomEnemy(Vector3 position)
    {
        int idx = Random.Range(0, enemyPrefabs.Count);
        SpawnEnemy(idx, position);
    }

    // 5. Metodi per pausa/riprendi/clear
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
            if (go != null)
                Destroy(go);
        activeEnemies.Clear();
    }
}
