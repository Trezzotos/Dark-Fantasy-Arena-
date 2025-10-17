using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Game/Wave Data", order = 1)]
public class WaveData : ScriptableObject
{
    [Tooltip("Lista di nemici da spawnare in questo wave")]
    public WaveEntry[] entries;
}
