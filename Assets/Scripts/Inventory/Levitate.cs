using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitate : MonoBehaviour
{
    public float height = .25f;
    public float speed = .25f;

    float startPos;
    int direction;
    
    void Start()
    {
        startPos = transform.position.y;
        direction = 1;
    }

    // custom animation
    void Update()
    {
        if (transform.position.y >= startPos + height) direction = -1;
        if (transform.position.y <= startPos) direction = 1;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + direction * speed * Time.deltaTime
        );
    }
}
