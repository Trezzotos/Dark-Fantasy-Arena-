using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.VisualScripting;
using UnityEngine;

public class AIEnemy : AIBase
{
    void Start()
    {
        AIStart();      // detect player and stuff
    }

    void Update()
    {
        AIUpdate();     // walking and stuff
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (timeToHit > 0) return;
        if (!collision.transform.CompareTag("Player")) return;

        Health health = collision.transform.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage(damage);
            timeToHit = hitRate;
        }
    }
}
