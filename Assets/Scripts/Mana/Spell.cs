using System;
using System.Collections;
using Examples.Observer;
using Unity.Mathematics;
using UnityEngine;

// Rappresenta il proiettile fisico nel gioco.
// Gestisce il movimento, l'aspetto e la logica iniziale della spell.

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Spell : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocità di movimento predefinita
    public float life = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private SpellData data;
    private Vector2 launchDirection;

    private delegate IEnumerator DamageFunction(Health enemyHealth);
    private DamageFunction damageFunction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f; // La spell non è influenzata dalla gravità
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Migliora la rilevazione di collisioni veloci
        }

        StartCoroutine(DestroyAfterLifetime());
    }

    private void FixedUpdate()
    {
        if (rb != null && launchDirection != Vector2.zero)
        {
            rb.velocity = launchDirection.normalized * moveSpeed;
            sr.enabled = true;  // qualcosa lo disattiva, ma non capisco cosa
        }
    }

    // Inizializza la spell con i dati e la direzione di lancio.
    // Questo metodo DEVE essere chiamato dopo l'instanziazione della spell.
    public void Initialize(SpellData spellData, Vector2 direction)
    {
        if (spellData == null)
        {
            Debug.LogError("SpellData non fornito. Distruggo la spell.");
            Destroy(gameObject);
            return;
        }

        data = spellData;
        launchDirection = direction.normalized; // Assicura che sia normalizzata per un movimento uniforme

        // Imposta l'aspetto visivo
        SetAppearance();

        // Inizializza la fisica
        rb.velocity = (launchDirection * moveSpeed).normalized;

        switch (data.effect)
        {
            case SpellData.EffectType.ONESHOT:
                damageFunction = OneShot;
                break;
            case SpellData.EffectType.MULTIPLE:
                damageFunction = Multiple;
                break;
            case SpellData.EffectType.INCREMENTAL:
                damageFunction = Incremental;
                break;
            default:
                Debug.LogError("SpellData: effect not initialized");
                break;
        }
    }

    // Logica Interna
    private void SetAppearance()
    {
        if (sr != null && data != null)
        {
            sr.sprite = data.sprite;
            sr.color = data.spriteTint;
            gameObject.name = "Spell: " + data.spellName;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out AIEnemy ai))
        {
            StopCoroutine(DestroyAfterLifetime());  // si distruggerà quando avrà finito
            StartCoroutine(damageFunction(other.GetComponent<Health>()));
        }

        // sound effect
        // particle system
        GetComponent<SpriteRenderer>().enabled = false;     // nascondi la spell. si distruggerà dopo
    }

    private IEnumerator OneShot(Health enemyHealth)
    {
        enemyHealth.TakeDamage(data.baseDamage);
        yield return new WaitForSeconds(0);
        Destroy(gameObject);
    }

    private IEnumerator Multiple(Health enemyHealth)
    {
        for (int i = 0; i < data.totalHits; i++)
        {
            enemyHealth.TakeDamage(data.baseDamage);
            yield return new WaitForSeconds(data.timeBetweenHits);
        }
        Destroy(gameObject);
    }

    private IEnumerator Incremental(Health enemyHealth)
    {
        for (int i = 0; i < data.totalHits; i++)
        {
            enemyHealth.TakeDamage(data.baseDamage + data.damageIncrement * i);
            yield return new WaitForSeconds(data.timeBetweenHits);
        }
        Destroy(gameObject);
    }
    
    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(life);
        Destroy(gameObject);
    }
}