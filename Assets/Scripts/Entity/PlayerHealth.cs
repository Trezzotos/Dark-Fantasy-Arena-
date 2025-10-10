using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Entity
{
    [Tooltip("HP / Sec")]
    public float regenFactor = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
        hbInitialScale = healthBar.localScale;
        FullyHeal();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.PLAYING) return;
        if (health < maxHealth && regenFactor != 0)
        {
            Heal(regenFactor * Time.deltaTime);
        }
    }

    protected override void Die()
    {
        GameManager.Instance.Gameover();
        // eventuali animazioni
    }

    protected override void UpdateHealthBar()
    {
        // this is an Image, not a sprite
        healthBar.GetComponent<Image>().fillAmount = math.remap(0, maxHealth, 0, hbInitialScale.x, health);
    }
}
