using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class AIEnemy : MonoBehaviour
{
    public float movementSpeed = 1;
    [Tooltip("How many seconds the enemy has to wait before being able to hit again")]
    public float hitRate = 1;
    public int damage = 5;

    private GameObject player;
    Vector2 direction = Vector2.zero;
    float timeToHit = 0;
    Rigidbody2D rb;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = TrovaIlPiùVicino(players);
        rb = GetComponent<Rigidbody2D>();   // granted by Entity
    }

    void Update()
    {
        // if (GameManager.Instance.gameState == GameManager.GameState.GAMEOVER) Die(); // IVAN??
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

        Health health = collision.transform.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage(damage);

            // animazione?
            timeToHit = hitRate;
        }
    }
    GameObject TrovaIlPiùVicino(GameObject[] players)
{
    float minDistance = Mathf.Infinity;
    GameObject closest = null;

    foreach (GameObject p in players)
    {
        float dist = Vector2.Distance(transform.position, p.transform.position);
        if (dist < minDistance)
        {
            minDistance = dist;
            closest = p;
        }
    }

    return closest;
}
}