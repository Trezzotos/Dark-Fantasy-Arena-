using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Inventory/Spells/Spell Data")]

public class SpellData : ScriptableObject
{
    public string spellName;
    public Sprite sprite;
    public Color spriteTint = Color.white;
    public bool areaDamage = false;

    public enum EffectType
    {
        ONESHOT,
        MULTIPLE,
        INCREMENTAL
    }

    public EffectType effect;

    // Shared between all types
    public float baseDamage;

    // MULTIPLE and INCREMENTAL shared properties
    public int totalHits;
    public float timeBetweenHits;

    // INCREMENTAL specific properties
    public float damageIncrement;

    // AREA specific properties
    public float radius;
}