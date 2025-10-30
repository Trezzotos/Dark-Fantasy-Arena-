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

        if (Vector3.Distance(transform.position, player.transform.position) <= minDistance) TryHitPlayer();
        print($"Distance: {Vector3.Distance(transform.position, player.transform.position)} / {minDistance}");
    }

    void TryHitPlayer()
    {
        if (timeToHit > 0) return;

        player.GetComponent<Health>().TakeDamage(damage);
        timeToHit = hitRate;
    }
}
