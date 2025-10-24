using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.Mathematics;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    public ParticleSystem ps;

    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.Damaged += PlayPS;
    }

    void OnDisable()
    {
        health.Damaged -= PlayPS;
    }

    void PlayPS()
    {
        ParticleSystem instance = Instantiate(ps, transform.position, quaternion.identity);
        instance.Play();
        Destroy(instance, instance.main.duration);
    }
}
