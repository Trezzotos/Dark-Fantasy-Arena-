using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.VisualScripting;
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

    [Header("For the Sprite/EnemyTipe")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    // 👉 Aggiunta per la pool
    public int PrefabIndex { get; private set; }

    public void Initialize(int index)
    {
        PrefabIndex = index;
    }

    void Start()
    {
        AILogic();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        direction = player.transform.position - transform.position;
        timeToHit -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.velocity = direction.normalized * movementSpeed;
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

    GameObject FindClosest(GameObject[] players)
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

    void AILogic()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = FindClosest(players);
    }
}
