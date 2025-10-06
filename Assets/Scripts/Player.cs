using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public float sprintSpeedMultiplier = 1.5f;

    [Header("Commands")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode shoot = KeyCode.E;
    public KeyCode reload = KeyCode.R;

    [Space]
    public Gun gun;
    public Image healthBarValue;

    Vector2 mov;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (TryGetComponent(out Gun g)) gun = g;
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

        // shooting
        if (Input.GetKey(shoot)) gun.TryShoot();
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

    public override void Heal(float amount)
    {
        base.Heal(amount);
        healthBarValue.fillAmount = math.remap(0, maxHealth, 0, 1, health); // mappa la vita in valori [0, 1]
    }

    public override void FullyHeal()
    {
        base.FullyHeal();
        healthBarValue.fillAmount = math.remap(0, maxHealth, 0, 1, health); // mappa la vita in valori [0, 1]
    }

    public override void Hit(float damage)
    {
        base.Hit(damage);
        healthBarValue.fillAmount = math.remap(0, maxHealth, 0, 1, health); // mappa la vita in valori [0, 1]
    }
}