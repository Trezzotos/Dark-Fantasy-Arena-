using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Commands")]
    public KeyCode shootKey = KeyCode.E;
    public KeyCode spellKey = KeyCode.R;

    [Space]
    [SerializeField] private RaycastShoot weapon;

    private PlayerMove pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMove>();
        weapon = GetComponentInChildren<RaycastShoot>();

        if (pm == null) Debug.LogError("PlayerMove non trovato!");
        if (weapon == null) Debug.LogError("RaycastShoot non trovato nei figli!");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        // handle shooting
        if (Input.GetKey(shootKey)) weapon.TryShoot(pm.lastDirection);
    }
}