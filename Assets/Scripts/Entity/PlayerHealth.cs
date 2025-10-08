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
<<<<<<< HEAD
        if (!healthbar) Debug.LogWarning("Healthbar unreferenced!");
        hbInitialScale = healthbar.localScale;
=======
        if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
>>>>>>> RefactoringCode
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

    protected override void Die()
    {
        Debug.Log("Sei morto!");
        base.Die();
    }

    protected override void UpdateHealthBar()
    {
<<<<<<< HEAD
        // this is an Image, not a sprite
        healthbar.GetComponent<Image>().fillAmount = math.remap(0, maxHealth, 0, hbInitialScale.x, health);
=======
        // this is an image, not a sprite
        healthBar.GetComponent<Image>().fillAmount = math.remap(0, maxHealth, 0, 1, health);
>>>>>>> RefactoringCode
    }
}
