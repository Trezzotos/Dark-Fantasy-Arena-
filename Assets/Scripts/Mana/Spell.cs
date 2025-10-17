using UnityEngine;

// Rappresenta il proiettile fisico nel gioco.
// Gestisce il movimento, l'aspetto e la logica iniziale della spell.

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Spell : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocità di movimento predefinita
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private SpellData data;
    private Vector2 launchDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f; // La spell non è influenzata dalla gravità
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Migliora la rilevazione di collisioni veloci
        }
    }

    private void FixedUpdate()
    {
        if (rb != null && launchDirection != Vector2.zero)
        {
            rb.velocity = launchDirection.normalized * moveSpeed;
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
        rb.velocity = launchDirection * moveSpeed;
        
        // Opzionale: Ruota la spell in base alla direzione di lancio
        RotateToDirection();

        // Avvia la logica di timeout/distruzione (se necessario)
        // Ad esempio: Destroy(gameObject, 5f); 
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

    private void RotateToDirection()
    {
        if (launchDirection != Vector2.zero)
        {
            // Calcola l'angolo in base al vettore direzione
            float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
            // Applica la rotazione Z al Transform
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // logica di contatto

        Destroy(gameObject);
    }
}