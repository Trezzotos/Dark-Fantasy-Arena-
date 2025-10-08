using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float movementSpeed = 1;
    [Tooltip("How close the enemy must be to hit")]
    public float hitRange = .25f;
    [Tooltip("How many seconds the enemy has to wait before being able to hit again")]
    public float hitRate = 1;

    PlayerHealth player;
    Vector2 direction = Vector2.zero;
    float timeToHit = 0;

    void Start()
    {
        player = FindAnyObjectByType<PlayerHealth>();  // also great for 2+ players
        rb = GetComponent<Rigidbody2D>();   // granted by Entity
        
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
        initialHbScale = healthBar.localScale;
    }

    void Update()
    {
        // go towards the selected player
        direction = player.transform.position - transform.position;

        timeToHit -= Time.deltaTime;
        if (timeToHit > 0) return;

        // if the enemy is close enough, attack
        if (direction.magnitude <= hitRange)
        {
            player.Hit(damage);
            // animazione?
            timeToHit = hitRate;
        }
    }

    void FixedUpdate()
    {
        // apply movement
        rb.velocity = direction.normalized * movementSpeed;
    }
}
