using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class Tesla : Entity
{
    [Header("Stats")]
    [Tooltip("How much time it has to wait to be able to shoot again")]
    public float shootFreq = 1.5f;

    Transform target;
    float timeToShoot;
    LineController lineController;

    // Start is called before the first frame update
    void Start()
    {
        lineController = transform.GetComponentInChildren<LineController>();
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
        hbInitialScale = healthBar.localScale;
        FullyHeal();
        timeToShoot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.PLAYING) return;

        timeToShoot -= Time.deltaTime;

        if (!target) return;
        if (timeToShoot > 0) return;

        timeToShoot = shootFreq;
        lineController.DrawLine(target.position);
        target.GetComponent<PlayerHealth>().Hit(damage);
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
