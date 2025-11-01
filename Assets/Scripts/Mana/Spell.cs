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

    // dentro OnTriggerEnter2D, sostituisci la parte che registra l'evento e avvia la coroutine
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag)) return;
        if (ownerCollider != null && other == ownerCollider) return;

        if (sr != null) sr.enabled = false;
        StopCoroutine(DestroyAfterLifetime());

        if (other.TryGetComponent<Health>(out Health targetHealth))
        {   
            SFXManager.Instance.PlayEnemyHit();
            // callback locale che si auto-deregistra
            System.Action deathHandler = null;
            deathHandler = () =>
            {
                // prima di fare qualsiasi cosa, check se questa Spell esiste ancora
                if (this)
                {
                    // ferma tutte le coroutine in corso per evitare accessi successivi
                    StopAllCoroutines();
                    DestroySpell();
                }
                // deregistrati dall'evento (protezione doppia)
                targetHealth.Killed -= deathHandler;
            };

            targetHealth.Killed += deathHandler;

            // avvia la coroutine passando anche la reference al target
            StartCoroutine(RunDamageRoutineSafely(damageFunction, targetHealth, deathHandler));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RunDamageRoutineSafely(DamageFunction damageFunc, Health enemyHealth, System.Action deathHandler)
    {
        // protezione immediata
        if (enemyHealth == null || damageFunc == null) yield break;

        // esegui la routine di danno (OneShot/Multiple/Incremental)
        // ogni routine è già un IEnumerator che usa enemyHealth; usiamola con 'yield return StartCoroutine'
        yield return StartCoroutine(damageFunc(enemyHealth));

        // dopo che la routine finisce, se il target è ancora valido deregistrati
        if (enemyHealth != null)
        {
            enemyHealth.Killed -= deathHandler;
        }

        // assicurati di non distruggere due volte: StopAllCoroutines prima di Destroy
        if (this) Destroy(gameObject);
    }


    private IEnumerator OneShot(Health enemyHealth)
    {
        if (enemyHealth != null) enemyHealth.TakeDamage(data.baseDamage);
        yield return null;
        Destroy(gameObject);
    }

    private IEnumerator Multiple(Health enemyHealth)
    {
        if (enemyHealth == null) yield break;

        for (int i = 0; i < data.totalHits; i++)
        {
            if (!this || enemyHealth == null) yield break; // check protettivo
            enemyHealth.TakeDamage(data.baseDamage);
            yield return new WaitForSeconds(data.timeBetweenHits);
        }
    }

    private IEnumerator Incremental(Health enemyHealth)
    {
        if (enemyHealth == null) yield break;

        for (int i = 0; i < data.totalHits; i++)
        {
            if (!this || enemyHealth == null) yield break;
            enemyHealth.TakeDamage(data.baseDamage + data.damageIncrement * i);
            yield return new WaitForSeconds(data.timeBetweenHits);
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(life);
        Destroy(gameObject);
    }

    private void DestroySpell()
    {
        StopAllCoroutines();
        // non possiamo rimuovere tutte le subscription qui perché non conosciamo i target;
        // le subscription dovrebbero essere rimosse dai rispettivi deathHandler locali.
        Destroy(gameObject);
    }

}
