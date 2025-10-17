using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public EnemyType type;          // Chiave di lookup
        public GameObject prefab;       // Prefab da istanziare
        public int initialSize = 10;    // Numero di oggetti da prewarmare
    }

    [SerializeField]
    private Pool[] pools;                                    // Configurazione via Inspector
    private Dictionary<EnemyType, Queue<GameObject>> poolMap;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inizializza le code per ogni pool
        poolMap = new Dictionary<EnemyType, Queue<GameObject>>();
        foreach (var p in pools)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < p.initialSize; i++)
            {
                var go = Instantiate(p.prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
            poolMap.Add(p.type, q);
        }
    }

    /// <summary>
    /// Prende in prestito un oggetto dal pool; se la coda è vuota, istanzia un nuovo prefab.
    /// </summary>
    public GameObject Get(EnemyType type)
    {
        if (!poolMap.ContainsKey(type))
        {
            Debug.LogWarning($"[PoolManager] Nessun pool registrato per il tipo {type}");
            return null;
        }

        var queue = poolMap[type];
        GameObject instance;

        if (queue.Count > 0)
        {
            instance = queue.Dequeue();
        }
        else
        {
            // Se la coda è esaurita, crea un nuovo oggetto
            var prefab = System.Array.Find(pools, x => x.type == type)?.prefab;
            instance = Instantiate(prefab, transform);
        }

        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// Restituisce l’istanza al pool disattivandola e ri-queueandola.
    /// </summary>
    public void Release(EnemyType type, GameObject instance)
    {
        instance.SetActive(false);

        if (!poolMap.ContainsKey(type))
        {
            Debug.LogWarning($"[PoolManager] Nessun pool registrato per il tipo {type}; oggetto distrutto.");
            Destroy(instance);
            return;
        }

        poolMap[type].Enqueue(instance);
    }

    /// <summary>
    /// Disattiva tutte le istanze ancora in pool (utile, ad es., su GAMEOVER).
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var kv in poolMap)
        {
            foreach (var go in kv.Value)
            {
                go.SetActive(false);
            }
        }
    }
}
