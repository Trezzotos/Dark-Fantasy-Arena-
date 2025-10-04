
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;

    void Awake()
    {
        // Dopo 'life' secondi il proiettile si distrugge
        Destroy(gameObject, life);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Distrugge l'oggetto colpito (se vuoi che solo i nemici vengano distrutti, qui devi filtrare per tag)
        Destroy(collision.gameObject);

        // Distrugge il proiettile
        Destroy(gameObject);
    }
}

