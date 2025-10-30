using System;
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

    [Space]
    public KeyCode pause = KeyCode.Escape;

    internal Vector2 lastDirection = Vector2.left;
    // Direzione raw con componenti -1,0,1; mantiene la direzione anche quando il giocatore smette di muoversi
    internal Vector2 lastDirectionRaw = Vector2.left;

    Vector2 mov = Vector2.zero;
    SpriteRenderer sp;
    Rigidbody2D rb;

    const string PREF_CONTROL_SCHEME = "ControlScheme";

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        sp.flipX = true;

        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        ChangeCommands(scheme);
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING
            && GameManager.Instance.gameState != GameState.SHOPPING)
        {
            lastDirection = Vector2.zero;
            lastDirectionRaw = Vector2.zero;
            mov = Vector2.zero;
            return;
        }

        Vector2 raw = Vector2.zero;

        if (Input.GetKey(up)) raw.y = 1;
        else if (Input.GetKey(down)) raw.y = -1;

        if (Input.GetKey(right))
        {
            raw.x = 1;
            if (sp.flipX) sp.flipX = false;
        }
        else if (Input.GetKey(left))
        {
            raw.x = -1;
            if (!sp.flipX) sp.flipX = true;
        }

        // Aggiorna ultima direzione raw solo se ricevi input (così resta memorizzata quando ti fermi)
        if (raw != Vector2.zero)
        {
            lastDirectionRaw = raw;
            lastDirection = raw.normalized;
        }

        // mov normalizzato per la fisica
        mov = raw.normalized;
    }

    void FixedUpdate()
    {
        float vel = movementSpeed;
        if (Input.GetKey(sprint)) vel *= sprintSpeedMultiplier;
        rb.velocity = vel * mov;
    }

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
            default:
                up = KeyCode.W;
                down = KeyCode.S;
                left = KeyCode.A;
                right = KeyCode.D;
                sprint = KeyCode.LeftShift;
                break;
        }
    }
}
