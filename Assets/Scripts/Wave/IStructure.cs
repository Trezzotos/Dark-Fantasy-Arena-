using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStructure
{
    public event Action<EnemySpawner> OnSpawnerKilled;
    public void HandleKilled();
    public void Deactivate();
}
