using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identità")]
    public string enemyID;              // Es: "Basic", "Fast", "Tank"
    public Sprite sprite;
    public Color spriteTint = Color.white;

    [Header("Statistiche base")]
    public float health = 50f;
    public float movementSpeed = 1f;
    public float damage = 5f;
    public float hitRate = 1;
    //float timeToHit = 0;  TO IMPLEMENT

    [Header("Spawn")]
    public int spawnWeight = 1;         // Per calcolo probabilità di spawn

    // Metodo utile se vuoi aggiornare dinamicamente lo sprite
    public void UpdateSprite(Sprite newSprite)
    {
        sprite = newSprite;
    }
}
