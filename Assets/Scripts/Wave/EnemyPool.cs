using System.Collections.Generic;
using UnityEngine;

// Wrapper compatibile: mantiene lo stesso nome, campi e metodi usati dal resto del progetto
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    // Manteniamo i campi pubblici per compatibilità con inspector/scripting esterno
    public List<GameObject> pooledObjects;
    public GameObject[] objectsToPool;
    [Tooltip("Per object amount")]
    public int amountToPool = 10;

    int poolSize;

    // Pool generica concreta per GameObject (usiamo Component GameObject tramite prefab.transform)
    GenericPool<Transform> internalPool = new GenericPool<Transform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        poolSize = (objectsToPool != null) ? amountToPool * objectsToPool.Length : 0;
    }

    void Start()
    {
        // Inizializziamo la internalPool con i prefab come Transform
        Transform[] prefabsAsTransforms;
        if (objectsToPool != null && objectsToPool.Length > 0)
        {
            prefabsAsTransforms = new Transform[objectsToPool.Length];
            for (int i = 0; i < objectsToPool.Length; i++)
                prefabsAsTransforms[i] = objectsToPool[i].transform;
        }
        else
        {
            prefabsAsTransforms = new Transform[0];
        }

        internalPool.Initialize(prefabsAsTransforms, amountToPool);

        // Manteniamo pooledObjects esattamente come prima per compatibilità
        var all = internalPool.GetAll();
        pooledObjects = new List<GameObject>(all.Count);
        foreach (var t in all) pooledObjects.Add(t.gameObject);
    }

    // Firma compatibile: restituisce GameObject o null
    public GameObject GetPooledObject()
    {
        var t = internalPool.GetRandomInactive();
        return t != null ? t.gameObject : null;
    }

    // Se in altri punti del progetto si usa questa proprietà, continuiamo a esporla
    public List<GameObject> GetPooledObjectsList()
    {
        return pooledObjects;
    }

    // Metodo opzionale per restituire un oggetto alla pool (comportamento esplicito)
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        internalPool.ReturnToPool(obj.transform);
    }
}
