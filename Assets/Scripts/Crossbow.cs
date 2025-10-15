using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [Header("Stats")]
    public float reloadTime = 0.8f;
    public float range = 5;
    public float beamDuration = .25f;

    float timeToShoot = 0;
    RaycastHit2D hit;
    int layerMask;
    LineController lineController;
    Vector3[] positions;

    void Start()
    {
        lineController = transform.GetComponentInChildren<LineController>();
        timeToShoot -= Time.deltaTime;
        layerMask = LayerMask.GetMask("ArrowHittable");
        positions = new Vector3[2];
    }

    public void TryShoot(Vector2 direction)
    {
        if (timeToShoot > 0) return;    // not ready to shoot again

        Shoot(direction);
    }

    void Shoot(Vector3 direction)
    {
        timeToShoot = reloadTime;   // we only have 1 arrow at a time
        positions[0] = transform.position;

        Debug.DrawRay(transform.position, direction * range, Color.red, 0.5f);
        // raycast
        hit = Physics2D.Raycast(transform.position, direction, range, layerMask);
        if (hit)
        {
            positions[1] = hit.transform.position;
            if (hit.transform.TryGetComponent(out Entity e))
            {
                e.Hit(5);
            }
        }
        else positions[1] = transform.position + direction * range;
        lineController.DrawLine(positions[0], positions[1]);
    }

    
}
