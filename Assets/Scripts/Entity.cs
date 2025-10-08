using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Ensure we have the components we need
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Entity : MonoBehaviour
{
<<<<<<< Updated upstream:Assets/Scripts/Entity.cs
    public float maxHealth = 50;
    public float damage = 10;

    [Space]
    public float movementSpeed = 1;

=======
    [Header("References")]
    public Transform healthbar;

    [Header("Stats")]
    public float maxHealth = 50;
    public float damage = 10;

>>>>>>> Stashed changes:Assets/Scripts/Entity/Entity.cs
    protected float health;

    protected Rigidbody2D rb;

    protected Vector3 hbInitialScale;

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

    protected void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void UpdateHealthBar()
    {
        healthbar.localScale.Set(math.remap(0, maxHealth, 0, hbInitialScale.x, health), hbInitialScale.y, hbInitialScale.z);  // mappa la vita in valori [0, 1]
    }
}
