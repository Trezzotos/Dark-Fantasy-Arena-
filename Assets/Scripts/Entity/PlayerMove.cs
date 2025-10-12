using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("InputActionAsset")]
    public InputActionAsset InputActions;

    [Header("InputAction")]
    private InputAction m_moveAction;
    private InputAction m_pauseActionPlayer;
    private InputAction m_pauseActionUI;

    [Header("Vectors")]
    private Vector2 m_moveAmt;

    [Header("Stats")]
    public float walkSpeed = 3f;
    public float sprintSpeedMultiplier = 1.5f;

    private Vector2 moveInput = Vector2.zero;
    internal Vector2 lastDirection = Vector2.left;
    Rigidbody2D m_rigidbody;
    SpriteRenderer sp;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable(); //for the Input system
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable(); //for the Input system
    }

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        //Because we have two actions with the same name we need to specify whitch Action Map it belongs to
        m_pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        m_pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        m_rigidbody = GetComponent<Rigidbody2D>();
      //  sp.flipX = true;   // sx
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        DisplayPause();
    }

    private void DisplayPause()
    {   
        if (GameManager.Instance.gameState != GameManager.GameState.PLAYING) 
            return;
        if (m_pauseActionPlayer.WasPressedThisFrame())
            GameManager.Instance.PauseGame();
        
        else if (m_pauseActionUI.WasPressedThisFrame())
            GameManager.Instance.ResumeGame();
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Walking();
    }
    
    void Walking()
    {
        //Set speed Here
        m_rigidbody.MovePosition(m_rigidbody.position + moveInput * m_moveAmt.y * walkSpeed * Time.deltaTime);
    }

}