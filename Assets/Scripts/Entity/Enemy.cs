using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float movementSpeed = 1;
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
        hbInitialScale = healthBar.localScale;
        FullyHeal();
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GAMEOVER) Die();
        if (GameManager.Instance.gameState != GameManager.GameState.PLAYING) return;
        
        // go towards the selected player
        direction = player.transform.position - transform.position;

        timeToHit -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // apply movement
        rb.velocity = direction.normalized * movementSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (timeToHit > 0) return;
        if (collision.transform.tag != "Player") return;

        player.Hit(damage);
        // animazione?
        timeToHit = hitRate;
    }
}
