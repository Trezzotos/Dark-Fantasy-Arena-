using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Ensure we have the components we need
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Entity : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50;
    public float damage = 10;

    [Space]

    [Header("References")]
    public Transform healthBar;

    protected float health;
    protected Rigidbody2D rb;
    protected Vector3 hbInitialScale;

    void Start()
    {
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
        hbInitialScale = healthBar.localScale;
        FullyHeal();
    }

    // Declared virtual so it can be overridden.
    public virtual void Heal(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth;
        UpdateHealthBar();
    }

    // Declared virtual so it can be overridden.
    public virtual void FullyHeal()
    {
        Heal(maxHealth);
    }

    public float GetDamage()
    {
        return damage;
    }

    // Declared virtual so it can be overridden.
    public virtual void Hit(float damage)
    {
        health -= damage;
        UpdateHealthBar();
        if (health <= 0) Die();
    }

    // Declared virtual so it can be overridden.
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void UpdateHealthBar()
    {
        // this is a sprite, not an image
        healthBar.localScale = new Vector3(math.remap(0, maxHealth, 0, hbInitialScale.x, health), hbInitialScale.y, hbInitialScale.z);  // mappa la vita in valori [0, 1]
    }
}
