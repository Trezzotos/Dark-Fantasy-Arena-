using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEditor.EditorTools;
using UnityEngine;

public class Tesla : Structure
{
    [Header("Stats")]
    [Tooltip("How much time it has to wait to be able to shoot again")]
    public float shootFreq = 1.5f;
    public int damage;

    Transform target;
    float timeToShoot;
    LineController lineController;

    protected override void Awake()
    {
        base.Awake();

        // in più
        lineController = transform.GetComponentInChildren<LineController>();
        timeToShoot = 0;

    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        timeToShoot -= Time.deltaTime;

        if (!target) return;
        if (timeToShoot > 0) return;

        timeToShoot = shootFreq;
        lineController.DrawLine(target.position);
        Health health = target.GetComponent<Health>();
        if (health) health.TakeDamage(damage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != "Player") return;
        target = collision.transform;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag != "Player") return;
        target = null;
    }
}
