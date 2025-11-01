using Examples.Observer;
using Unity.Mathematics;
using UnityEngine;

public class Tesla : Structure
{
    [Header("Stats")]
    [Tooltip("How much time it has to wait to be able to shoot again")]
    public float shootFreq = 1.5f;
    public int damage;
    public float range = 4;

    Transform target;
    float timeToShoot;
    LineController lineController;
    GameObject[] players;
    bool hasTarget = false;
    ParticleSystem ps;

    protected override void Awake()
    {
        base.Awake();

        // in più
        lineController = transform.GetComponentInChildren<LineController>();
        timeToShoot = 0;
        players = GameObject.FindGameObjectsWithTag("Player");
        ps = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        timeToShoot -= Time.deltaTime;
        hasTarget = false;

        for (int i = 0; i < players.Length; i++)
        {
            if (Vector2.Distance(players[i].transform.position, transform.position) < range)
            {
                target = players[i].transform;
                hasTarget = true;
                break;
            }
        }

        if (!hasTarget) return;
        if (timeToShoot > 0) return;

        timeToShoot = shootFreq;
        lineController.DrawLine(target.position);
        Health health = target.GetComponent<Health>();
        if (health) health.TakeDamage(damage);
        SFXManager.Instance.PlayTeslaHit();
        ps.Play();
    }
}
