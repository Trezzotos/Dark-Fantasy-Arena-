using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using UnityEngine;

public class AIMagonegro : AIBase
{
    [Header("These values override weapon settings")]
    public float range = 7;

    [SerializeField] RaycastShoot weapon;

    [Header("Prediction")]
    [Tooltip("Stima della velocità del proiettile (units/s). Regola per far corrispondere il comportamento di RaycastShoot")]
    public float projectileSpeedEstimate = 20f;
    [Tooltip("Massimo tempo di predizione (s) ammesso per cercare l'intercetta")]
    public float maxPredictionTime = 3f;
    [Tooltip("Soglia minima per considerare valido il punto predetto (se troppo lontano si usa fallback)")]
    public float maxInterceptDistance = 20f;

    Rigidbody2D playerRb;

    void Start()
    {
        AIStart();      // detect player and stuff

        if (weapon == null) weapon = GetComponentInChildren<RaycastShoot>();
        if (weapon == null) Debug.LogError("RaycastShoot non trovato nei figli di AIMagonegro!");

        weapon.range = range;
        weapon.damage = damage;
        weapon.reloadTime = 0;  // weapon must not handle hit rate
        weapon.SetLayerMask(LayerMask.GetMask("EnemyHittable"));

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        AIUpdate();     // walking and stuff

        if (player == null) return;
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // debug optional
        // print(dist);

        if (timeToHit <= 0 && dist <= range)
        {
            Vector2 shooterPos = transform.position;
            Vector2 targetPos = player.transform.position;
            Vector2 targetVel = playerRb != null ? playerRb.velocity : Vector2.zero;

            Vector2 interceptPoint;
            bool hasSolution = PredictInterceptPoint(shooterPos, targetPos, targetVel, projectileSpeedEstimate, maxPredictionTime, out interceptPoint);

            // fallback: se soluzione non valida, mira alla posizione attuale
            Vector2 aimPoint = hasSolution ? interceptPoint : targetPos;

            // ulteriore fallback: se il punto predetto è troppo distante, usa posizione attuale
            if ((aimPoint - (Vector2)shooterPos).magnitude > maxInterceptDistance)
            {
                aimPoint = targetPos;
            }

            Vector2 shootDir = (aimPoint - shooterPos).normalized;
            weapon.TryShoot(shootDir);
            timeToHit = hitRate;
        }
    }

    // Predizione dell'intercetta: restituisce true e l'interceptPoint se esiste t>0 <= maxTime
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
            //serve a controllare se il coefficiente a è praticamente uguale a zero, con una piccola tolleranza numerica.
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
