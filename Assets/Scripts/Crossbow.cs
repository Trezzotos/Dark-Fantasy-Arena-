using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [Header("Stats")]
    public float reloadTime = 0.8f;
    public float range = 5;

    float timeToShoot = 0;
    RaycastHit2D hit;
    int layerMask;

    void Update()
    {
        timeToShoot -= Time.deltaTime;
        layerMask = LayerMask.GetMask("ArrowHittable");
    }

    public void TryShoot(Vector2 direction)
    {
        if (timeToShoot > 0) return;    // not ready to shoot again

        Shoot(direction);
    }

    void Shoot(Vector3 direction)
    {
        timeToShoot = reloadTime;   // we only have 1 arrow at a time

        Debug.DrawRay(transform.position, direction * range, Color.red, 0.5f);
        // raycast
        hit = Physics2D.Raycast(transform.position, direction, range, layerMask);
        if (hit)
        {
            if (hit.transform.TryGetComponent(out Entity e))
            {
                e.Hit(5);
            }
        }
    }
}
