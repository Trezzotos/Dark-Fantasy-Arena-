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

    // ownership per evitare friendly fire
    public string ownerTag = null;
    public Collider2D ownerCollider = null;

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
            // usa launchDirection * moveSpeed (non .normalized dopo la moltiplicazione)
            rb.velocity = launchDirection.normalized * moveSpeed;
            if (sr != null) sr.enabled = true;  // se qualcosa lo disattiva, lo riattiviamo
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
    launchDirection = direction.normalized;

    SetAppearance();

    if (rb != null)
    {
        rb.velocity = launchDirection * moveSpeed;
    }

    // ROTAZIONE ISTANTANEA ADATTATA: la sprite "avanti" punta verso +Y, quindi sottrai 90 gradi
    float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg - 90f;
    transform.rotation = Quaternion.Euler(0f, 0f, angle);

    Animator anim = GetComponent<Animator>();
    if (anim != null)
    {
        anim.SetFloat("DirX", launchDirection.x);
        anim.SetFloat("DirY", launchDirection.y);
    }

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
        // ignora il mittente se impostato tramite tag o collider
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag)) return;
        if (ownerCollider != null && other == ownerCollider) return;

        // nascondi la spell e ferma la distruzione programmata
        if (sr != null) sr.enabled = false;
        StopCoroutine(DestroyAfterLifetime());

        // applica danno solo se l'oggetto ha Health
        if (other.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.Killed += DestroySpell;    // se il target morisse prima della fine della spell, distruggi la spell
            StartCoroutine(damageFunction(targetHealth));
        }
        else
        {
            // nessun Health: distruggi la spell
            Destroy(gameObject);
        }
    }

    private IEnumerator OneShot(Health enemyHealth)
    {
        if (enemyHealth != null) enemyHealth.TakeDamage(data.baseDamage);
        yield return null;
        Destroy(gameObject);
    }

    private IEnumerator Multiple(Health enemyHealth)
    {
        if (enemyHealth == null) { Destroy(gameObject); yield break; }

        for (int i = 0; i < data.totalHits; i++)
        {
            enemyHealth.TakeDamage(data.baseDamage);
            yield return new WaitForSeconds(data.timeBetweenHits);
        }
        Destroy(gameObject);
    }

    private IEnumerator Incremental(Health enemyHealth)
    {
        if (enemyHealth == null) { Destroy(gameObject); yield break; }

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

    private void DestroySpell()
    {
        Destroy(gameObject);
    }
}
