using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    [Header("Stats")]
    public int magCapacity = 9;
    public float reloadTime = 0.8f;
    public float range = 5;

    float timeToShoot;
    float mag;

    void Start()
    {
        mag = magCapacity;
        timeToShoot = 0;
    }

    void Update()
    {
        timeToShoot -= Time.deltaTime;
    }

    public void TryShoot(Vector3 direction)
    {
        if (timeToShoot > 0) return;    // not ready to shoot again
        // if (mag <= 0) return;   // no more bullets
        if (direction != Vector3.zero) return;  // player has to be moving

        Shoot(direction);
    }

    void Shoot(Vector3 direction)
    {
        timeToShoot = reloadTime;   // we only have 1 arrow at a time
        // mag--;

        // non riesco a disegnare il ray :(
        Debug.DrawRay(transform.position, direction * range, Color.red, 0.5f);
        Debug.Log(mag + " / " + magCapacity);
        // raycast
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, range, LayerMask.NameToLayer("ArrowHittable")))
        {
            Debug.DrawRay(transform.position, direction * range, Color.yellow, 0.5f);
        }
    }

    public void Reload()
    {
        // UI: cerchietto ricarica
        // Aspetta il tempo di ricarica
        mag = magCapacity;
    }
}
