using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

public class AIMagonegro : AIBase
{
    [Header("These values override weapon settings")]
    public float range = 7;

    [SerializeField] RaycastShoot weapon;

    void Start()
    {
        AIStart();      // detect player and stuff

        weapon.range = range;
        weapon.damage = damage;
        weapon.reloadTime = 0;  // weapon must not handle hit rate
        // weapon.SetLayerMask(LayerMask.GetMask("EnemyHittable"));
        weapon.SetLayerMask(LayerMask.GetMask("EnemyHittable"));
    }

    void Update()
    {
        AIUpdate();     // walking and stuff
        print(Vector3.Distance(player.transform.position, transform.position));

        if (timeToHit <= 0 && Vector3.Distance(player.transform.position, transform.position) <= range)
        {
            // we're close enough and can shoot => we shoot. >:c
            weapon.TryShoot(player.transform.position - transform.position);
            timeToHit = hitRate;
        }
    }
}
