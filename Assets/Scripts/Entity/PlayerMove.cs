using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3;
    public float sprintSpeedMultiplier = 1.5f;

    [Header("Commands")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftShift;

    bool facingRight = true;
    internal Vector2 mov = Vector2.zero;    // lo metto global perchè lo utilizzo in FixedUpdate
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // granted by Entity
    }

    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
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
            if (facingRight)    // look forwards
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                facingRight = false;
            }
        }
        else if (Input.GetKey(left))
        {
            mov.x = -1;
            if (!facingRight)   // look forwards
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                facingRight = true;
            }
        }
    }

    void FixedUpdate()
    {
        float vel = speed;
        if (Input.GetKey(sprint)) vel *= sprintSpeedMultiplier;
        rb.velocity = vel * mov.normalized;
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
