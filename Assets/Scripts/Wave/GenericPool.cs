using System.Collections.Generic;
using UnityEngine;

// GenericPool non è un MonoBehaviour: può essere riutilizzata senza problemi nell'Editor
public class GenericPool<T> where T : Component
{
    readonly List<T> items = new List<T>();
    int poolSize = 0;

    public IReadOnlyList<T> Items => items;

    public GenericPool() { }

    // Inizializza la pool a partire dai prefabs e dalla quantità per prefab
    public void Initialize(T[] prefabs, int amountPerPrefab)
    {
        items.Clear();
        if (prefabs == null || prefabs.Length == 0 || amountPerPrefab <= 0)
        {
            poolSize = 0;
            return;
        }

        for (int p = 0; p < prefabs.Length; p++)
        {
            T prefab = prefabs[p];
            for (int i = 0; i < amountPerPrefab; i++)
            {
                T instance = Object.Instantiate(prefab);
                instance.gameObject.SetActive(false);
                items.Add(instance);
            }
        }

        poolSize = items.Count;
    }

    // Restituisce un'istanza inattiva o null se la pool è tutta in uso
    public T GetRandomInactive()
    {
        if (items == null || items.Count == 0 || poolSize == 0) return null;

        int i;
        int count = -1;
        do
        {
            i = Random.Range(0, poolSize);
            count++;
        } while (items[i].gameObject.activeInHierarchy && count < poolSize);

        if (count >= poolSize) return null;
        return items[i];
    }

    // Ritorna la lista modificabile (se necessario)
    public List<T> GetAll() => items;

    // Espandi la pool con una nuova istanza (opzionale)
    public T ExpandWith(T prefab)
    {
        T tmp = Object.Instantiate(prefab);
        tmp.gameObject.SetActive(false);
        items.Add(tmp);
        poolSize = items.Count;
        return tmp;
    }

    // Segnala il ritorno in pool (disattiva e lascia l'oggetto nella lista)
    public void ReturnToPool(T obj)
    {
        if (obj == null) return;
        if (!items.Contains(obj)) return;
        obj.gameObject.SetActive(false);
    }
}
