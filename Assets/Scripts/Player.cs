using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : Entity
{
    public float sprintSpeedMultiplier = 1.5f;

    [Header("Commands")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftShift;
   // public Gun gun;
    private SpriteRenderer sp;


    Vector2 mov;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FullyHeal();
        mov = Vector2.zero;
    }

    // Update is called once per frame
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
        }
        else if (Input.GetKey(left))
        {
            mov.x = -1;
        }

        // Apply movement
        // characterController.Move(mov * movementSpeed * Time.deltaTime);
        rb.velocity = movementSpeed * Time.deltaTime * mov.normalized;
    }

    void FixedUpdate()
    {
        float vel = movementSpeed;
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
