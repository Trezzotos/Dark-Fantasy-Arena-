using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Spells/Spell Data")]

public class SpellData : ScriptableObject
{
    public string spellName;
    public Sprite sprite;
    public Color spriteTint = Color.white;

    public enum EffectType
    {
        ONESHOT,
        MULTIPLE,
        INCREMENTAL
    }

    public EffectType effect;

    // --- Proprietà specifiche per gli effetti ---

    // Condivise tra tutti i tipi
    public float baseDamage;

    // Condivise (usate da MULTIPLE e INCREMENTAL)
    public int totalHits;
    public float timeBetweenHits;


    // INCREMENTAL usa proprietà specifiche
    public float damageIncrement;
}