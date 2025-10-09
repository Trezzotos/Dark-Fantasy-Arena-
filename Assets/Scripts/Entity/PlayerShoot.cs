using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Commands")]
    public KeyCode shoot = KeyCode.E;
    public KeyCode reload = KeyCode.R;  // remove?

    [Space]
    public Crossbow crossbow;

    private PlayerMove pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMove>();
        if (TryGetComponent(out Crossbow c)) crossbow = c;
    }

    // Update is called once per frame
    void Update()
    {
        // handle shooting
        if (Input.GetKey(shoot)) crossbow.TryShoot(pm.lastDirection);
    }
}