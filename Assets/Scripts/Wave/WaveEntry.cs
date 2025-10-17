using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public EnemyType enemyType;    // Tipo di nemico da spawnare
    public GameObject prefab;      // Prefab associato
    public float spawnDelay;       // Ritardo prima dello spawn (in secondi)
    public int quantity;           // Numero di istanze da spawnare
}
