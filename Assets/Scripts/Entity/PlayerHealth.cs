using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Entity
{
    [Tooltip("HP / Sec")]
    public float regenFactor = 0;

    [Header("References")]
    public Image healthBarValue;

    // Start is called before the first frame update
    void Start()
    {
        FullyHeal();
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHealth)
        {
            Heal(regenFactor * Time.deltaTime);
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

    protected override void Die()
    {
        Debug.Log("Sei morto!");
        healthBarValue.fillAmount = 0;
        base.Die();
    }
}
