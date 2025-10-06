using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensure we have the components we need
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Entity : MonoBehaviour
{
    public float maxHealth = 50;
    public float damage = 10;

    [Space]
    public float movementSpeed = 1;

    protected float health;

    protected Rigidbody2D rb;

    // Declared virtual so it can be overridden.
    public virtual void Heal(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth;
    }

    // Declared virtual so it can be overridden.
    public virtual void FullyHeal()
    {
        health = maxHealth;
    }

    public float GetDamage()
    {
        return damage;
    }

    // Declared virtual so it can be overridden.
    public virtual void Hit(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
