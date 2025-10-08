using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Sprite))]

public class Spell : MonoBehaviour
{
    public float range = 3;
    public float speed = 1;
    public float damage = 5;    // DA RIMPIAZZARE CON CLASSE SpellEffect

    Vector3 startPos;
    Vector2 direction;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * (Vector3)direction;
        if (Vector3.Magnitude(transform.position - startPos) >= range)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Distrugge l'oggetto colpito (se vuoi che solo i nemici vengano distrutti, qui devi filtrare per tag)
        if (collision.transform.TryGetComponent(out Entity e))
        {
            e.Hit(damage);  // applica lo SpellEffect
        }


        // Distrugge il proiettile
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }
}

