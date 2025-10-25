using System.Collections.Generic;
using UnityEngine;
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [Tooltip("Prefabs da inserire nella pool")]
    [SerializeField] private GameObject[] prefabsToPool;

    [Tooltip("Numero di istanze per prefab")]
    [SerializeField] private int amountToPool = 10;

    private List<GameObject> pooledObjects;
    private int poolSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pooledObjects = new List<GameObject>();
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (var prefab in prefabsToPool)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject instance = Instantiate(prefab, transform);
                instance.SetActive(false);
                pooledObjects.Add(instance);
            }
        }

        poolSize = pooledObjects.Count;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < poolSize; i++)
        {
            int index = Random.Range(0, poolSize);
            if (!pooledObjects[index].activeInHierarchy)
                return pooledObjects[index];
        }

        return null; // pool piena
    }
}
