using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Stats")]
    public float movementSpeed = 3;
    public float sprintSpeedMultiplier = 1.5f;

    [Header("Commands")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftShift;

    internal Vector2 lastDirection = Vector2.left;
    Vector2 mov = Vector2.zero;
    SpriteRenderer sp;
    Rigidbody2D rb;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();   // granted by Entity

        sp.flipX = false;   // sx
    }


    void Update()
    {
        // Movement handling
        mov = Vector2.zero;

        // y-axis input
        if (Input.GetKey(up)) mov.y = 1;
        else if (Input.GetKey(down)) mov.y = -1;

        // x-axis input
        if (Input.GetKey(right))
        {
            mov.x = 1;
            if (!sp.flipX) sp.flipX = true;    // dx
        }
        else if (Input.GetKey(left))
        {
            mov.x = -1;
            if (sp.flipX) sp.flipX = false;   // sx
        }
        mov.Normalize();

        if (mov != Vector2.zero) lastDirection = mov;
    }

    void FixedUpdate()
    {
        float vel = movementSpeed;
        if (Input.GetKey(sprint)) vel *= sprintSpeedMultiplier;
        rb.velocity = vel * mov;
    }

    // Options -> Controls: 2pt
    public void ChangeCommands(int index)
    {
        switch (index)
        {
            case 0:
                up = KeyCode.W;
                down = KeyCode.S;
                left = KeyCode.A;
                right = KeyCode.D;
                sprint = KeyCode.LeftShift;
                break;
            case 1:
                up = KeyCode.UpArrow;
                down = KeyCode.DownArrow;
                left = KeyCode.LeftArrow;
                right = KeyCode.RightArrow;
                sprint = KeyCode.RightShift;
                break;
        }
    }
}
