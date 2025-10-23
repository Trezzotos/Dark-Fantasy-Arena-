using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public event Action<Structure> OnStructureDestroyed;
    private Health health;
    protected bool active = false;

    protected virtual void Awake()
    {
        health = GetComponent<Examples.Observer.Health>();
        if (health == null) Debug.LogError("EnemySpawner richiede un Health sullo stesso GameObject.");
    }

    private void OnEnable()
    {
        if (health != null)
            health.Killed += HandleKilled;
    }

    private void OnDisable()
    {
        if (health != null)
            health.Killed -= HandleKilled;
    }

    public void HandleKilled()
    {
        Deactivate();
        OnStructureDestroyed?.Invoke(this);
    }

    public virtual void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
        StopAllCoroutines();
    }
}
