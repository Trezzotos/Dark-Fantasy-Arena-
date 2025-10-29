using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    const string PREF_CONTROL_SCHEME = "ControlScheme";

    [Header("Commands")]
    public KeyCode shootKey = KeyCode.E;

    [Space]
    [SerializeField] private RaycastShoot weapon;

    private PlayerMove pm;

    void Start()
    {
        pm = GetComponent<PlayerMove>();
        weapon = GetComponentInChildren<RaycastShoot>();

        if (pm == null) Debug.LogError("PlayerMove non trovato!");
        if (weapon == null) Debug.LogError("RaycastShoot non trovato nei figli!");

        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        ChangeCommands(scheme);
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        if (Input.GetKey(shootKey)) weapon.TryShoot(pm.lastDirection);
    }

    public void ChangeCommands(int index)
    {
        switch (index)
        {
            // Set 0: WASD movement (handled in PlayerMove), shoot = I
            case 0:
                shootKey = KeyCode.I;
                break;
            // Set 1: Arrow movement (handled in PlayerMove), shoot = E
            case 1:
                shootKey = KeyCode.E;
                break;
            default:
                shootKey = KeyCode.I;
                break;
        }
    }
}
