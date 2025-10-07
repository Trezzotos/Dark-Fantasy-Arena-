using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : Entity
{
    [Header("Commands")]
    public KeyCode shoot = KeyCode.E;
    public KeyCode reload = KeyCode.R;  // remove?

    [Space]
    public Crossbow crossbow;
    public Image healthBarValue;

    private PlayerMove mp;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mp = GetComponent<PlayerMove>();
        if (TryGetComponent(out Crossbow c)) crossbow = c;
        FullyHeal();
    }

    // Update is called once per frame
    void Update()
    {
        // handle shooting
        if (Input.GetKey(shoot)) crossbow.TryShoot(mp.mov);
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