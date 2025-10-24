using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    public List<GameObject> pooledObjects;
    public GameObject[] objectsToPool;
    [Tooltip("Per object amount")]
    public int amountToPool = 10;

    int poolSize;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        poolSize = amountToPool * objectsToPool.Length;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;

        foreach (GameObject obj in objectsToPool)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(obj);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }
    }
    
    public GameObject GetPooledObject()
    {
        int i, count = -1;
        do
        {
            i = Random.Range(0, poolSize);
            count++;
        } while (pooledObjects[i].activeInHierarchy && count < poolSize);

        if (count >= poolSize) return null;     // pool piena
        return pooledObjects[i];
    }
}
