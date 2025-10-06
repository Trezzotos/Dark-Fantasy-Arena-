using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    [Space]
    public float bulletSpeed = 10f;
    [Tooltip("Bullets / Sec")]
    public float rate = 25;
    public int magCapacity = 9;

    float timeToShoot;
    float mag;

    void Start()
    {
        mag = magCapacity;
    }

    void Update()
    {
        timeToShoot -= Time.deltaTime;
    }

    public void TryShoot()
    {
        if (timeToShoot > 0) return;    // not ready to shoot again
        if (mag <= 0) return;   // no more bullets
        Shoot();
    }

    void Shoot()
    {
        timeToShoot = 1 / rate;
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.velocity = bulletSpawnPoint.right * bulletSpeed;
    }

    public void Reload()
    {
        // UI: cerchietto ricarica
        // Aspetta il tempo di ricarica
        mag = magCapacity;
    }
}
