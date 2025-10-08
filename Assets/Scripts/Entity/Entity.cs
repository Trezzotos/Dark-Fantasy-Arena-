using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
<<<<<<< HEAD:Assets/Scripts/Entity.cs
using Unity.VisualScripting;
=======
>>>>>>> RefactoringCode:Assets/Scripts/Entity/Entity.cs
using UnityEngine;
using UnityEngine.UI;

// Ensure we have the components we need
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Entity : MonoBehaviour
{
<<<<<<< HEAD:Assets/Scripts/Entity.cs
<<<<<<< Updated upstream:Assets/Scripts/Entity.cs
=======
    public Transform healthBar;
    [Header("Stats")]
>>>>>>> RefactoringCode:Assets/Scripts/Entity/Entity.cs
    public float maxHealth = 50;
    public float damage = 10;

    [Space]

=======
    [Header("References")]
    public Transform healthbar;

    [Header("Stats")]
    public float maxHealth = 50;
    public float damage = 10;

>>>>>>> Stashed changes:Assets/Scripts/Entity/Entity.cs
    protected float health;

    protected Rigidbody2D rb;
    protected Vector3 initialHbScale;

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

    // Declared virtual so it can be overridden.
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void UpdateHealthBar()
    {
<<<<<<< HEAD:Assets/Scripts/Entity.cs
        healthbar.localScale.Set(math.remap(0, maxHealth, 0, hbInitialScale.x, health), hbInitialScale.y, hbInitialScale.z);  // mappa la vita in valori [0, 1]
=======
        // this is a sprite, not an image
        healthBar.localScale = new Vector3(math.remap(0, maxHealth, 0, initialHbScale.x, health), initialHbScale.y, initialHbScale.z);
        Debug.Log(health);
>>>>>>> RefactoringCode:Assets/Scripts/Entity/Entity.cs
    }
}
