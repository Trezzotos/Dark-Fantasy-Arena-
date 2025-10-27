using System;
using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(NavMeshAgent))]

public class AIBase : MonoBehaviour
{
    public float movementSpeed = 1;
    [Tooltip("How many seconds the enemy has to wait before being able to hit again")]
    public float hitRate = 1;
    public int damage = 5;

    protected GameObject player;
    protected Vector2 direction = Vector2.zero;
    protected float timeToHit = 0;
    protected Rigidbody2D rb;
    protected NavMeshAgent agent;

    protected void AIStart()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = FindClosest(players);
    }

    protected void AIUpdate()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        agent.SetDestination(player.transform.position);
        timeToHit -= Time.deltaTime;
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
}
