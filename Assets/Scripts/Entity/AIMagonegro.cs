using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

public class AIMagonegro : AIBase
{
    [Header("Weapon (physical Spell)")]
    public GameObject spellPrefab;       // prefab che contiene lo script Spell + SpriteRenderer + Rigidbody2D + Collider2D
    public SpellData spellData;          // ScriptableObject con i dati della spell (baseDamage, effect, ecc.)

    [Header("Weapon / prediction")]
    public float range = 7f;                     // raggio di ingaggio
    public float projectileSpeed = 8f;           // usato per la predizione; controlla la moveSpeed della Spell
    public float maxPredictionTime = 3f;         // limite per la radice positiva
    public float maxInterceptDistance = 20f;     // protezione da soluzioni troppo lontane

    [Header("Projectile runtime")]
    public float spellLifeTime = 6f;             // durata della spell istanziata (override)
    public int overrideDamage = -1;              // se >=0 sovrascrive spellData.baseDamage

    void Start()
    {
        AIStart();
        if (spellPrefab == null) Debug.LogError("AIMagonegro: missing spellPrefab!");
        if (spellData == null) Debug.LogWarning("AIMagonegro: no SpellData assigned; spawn comunque la spell ma senza dati.");
    }

    void Update()
    {
        AIUpdate();
        if (player == null) return;

        float dist = Vector3.Distance(player.transform.position, transform.position);

        if (timeToHit <= 0f && dist <= range)
        {
            Vector2 shooterPos = transform.position;
            Vector2 targetPos = player.transform.position;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector2 targetVel = playerRb != null ? playerRb.velocity : Vector2.zero;

            Vector2 interceptPoint;
            bool hasSolution = PredictInterceptPoint(shooterPos, targetPos, targetVel, projectileSpeed, maxPredictionTime, out interceptPoint);

            Vector2 aimPoint = hasSolution ? interceptPoint : targetPos;
            if ((aimPoint - shooterPos).magnitude > maxInterceptDistance) aimPoint = targetPos;

            Vector2 shootDir = (aimPoint - shooterPos).normalized;
            SpawnSpell(shooterPos, shootDir);
            timeToHit = hitRate;
        }
    }

    void SpawnSpell(Vector2 origin, Vector2 direction)
{
    if (spellPrefab == null) return;

    GameObject go = Instantiate(spellPrefab, origin, Quaternion.identity);
    Spell spell = go.GetComponent<Spell>();

    Collider2D spellCol = go.GetComponent<Collider2D>();
    Collider2D myCol = GetComponent<Collider2D>();

    if (spell != null)
    {
        // inizializza come prima (passando spellData o fallback)
        if (spellData != null) spell.Initialize(spellData, direction);
        else
        {
            SpellData fallback = ScriptableObject.CreateInstance<SpellData>();
            fallback.spellName = "fallback";
            fallback.baseDamage = overrideDamage >= 0 ? overrideDamage : damage;
            fallback.effect = SpellData.EffectType.ONESHOT;
            spell.Initialize(fallback, direction);
        }

        // runtime overrides
        spell.moveSpeed = projectileSpeed;
        spell.life = spellLifeTime;

        // evita friendly fire: imposta ownerTag e ownerCollider e ignora collisione specifica
        spell.ownerTag = this.gameObject.tag;
        if (myCol != null)
        {
            spell.ownerCollider = myCol;
            if (spellCol != null)
            {
                Physics2D.IgnoreCollision(spellCol, myCol, true);
            }
        }

        // override damage tramite runtime copy (se richiesto)
        if (overrideDamage >= 0)
        {
            SpellData runtimeData = ScriptableObject.CreateInstance<SpellData>();
            runtimeData.spellName = spellData != null ? spellData.spellName : "runtime";
            runtimeData.sprite = spellData != null ? spellData.sprite : null;
            runtimeData.spriteTint = spellData != null ? spellData.spriteTint : Color.white;
            runtimeData.effect = spellData != null ? spellData.effect : SpellData.EffectType.ONESHOT;
            runtimeData.baseDamage = overrideDamage;
            runtimeData.totalHits = spellData != null ? spellData.totalHits : 1;
            runtimeData.timeBetweenHits = spellData != null ? spellData.timeBetweenHits : 0.1f;
            runtimeData.damageIncrement = spellData != null ? spellData.damageIncrement : 0f;
            runtimeData.radius = spellData != null ? spellData.radius : 0f;

            spell.Initialize(runtimeData, direction);

            // ri-applica runtime overrides e ownership
            spell.moveSpeed = projectileSpeed;
            spell.life = spellLifeTime;
            spell.ownerTag = this.gameObject.tag;
            if (myCol != null && spellCol != null) Physics2D.IgnoreCollision(spellCol, myCol, true);
        }
    }
    else
    {
        // fallback: applica velocità se non trovi Spell
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction * projectileSpeed;
        // non dimenticare che in questo caso non esiste la logica di danno automatica
    }
}


    // Predizione dell'intercetta (identica alla versione già discussa)
    bool PredictInterceptPoint(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVel, float projectileSpeed, float maxTime, out Vector2 interceptPoint)
    {
        interceptPoint = targetPos;

        Vector2 r = targetPos - shooterPos;
        float r2 = Vector2.Dot(r, r);
        float v2 = Vector2.Dot(targetVel, targetVel);
        float s2 = projectileSpeed * projectileSpeed;
        float rv = Vector2.Dot(r, targetVel);

        float a = v2 - s2;
        float b = 2f * rv;
        float c = r2;

        float t = -1f;

        if (Mathf.Abs(a) < 1e-6f)
        {
            if (Mathf.Abs(b) > 1e-6f)
            {
                float tl = -c / b;
                if (tl > 0f && tl <= maxTime) t = tl;
            }
        }
        else
        {
            float disc = b * b - 4f * a * c;
            if (disc >= 0f)
            {
                float sqrtD = Mathf.Sqrt(disc);
                float t1 = (-b + sqrtD) / (2f * a);
                float t2 = (-b - sqrtD) / (2f * a);

                float best = float.MaxValue;
                if (t1 > 0f && t1 <= maxTime && t1 < best) best = t1;
                if (t2 > 0f && t2 <= maxTime && t2 < best) best = t2;

                if (best != float.MaxValue) t = best;
            }
        }

        if (t > 0f)
        {
            interceptPoint = targetPos + targetVel * t;
            return true;
        }

        return false;
    }
}
