using Examples.Observer;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CastSpell : MonoBehaviour
{
    public TMP_Text UIText;
    public GameObject spellPrefab;
    [Space]
    public KeyCode spellKey = KeyCode.K;
    public KeyCode spellPlus = KeyCode.L;
    public KeyCode spellMinus = KeyCode.J;

    PlayerMove playerMove;
    Inventory inventory;
    [SerializeField] int selected = 0;
    [SerializeField] int availableSpells;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        inventory = GetComponent<Inventory>();

        availableSpells = inventory.spells.Count;
        if (availableSpells <= 0) UIText.text = "No spell";
        else UIText.text = inventory.spells.ToArray()[selected].spellName;
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;
        if (availableSpells <= 0) return;

        if (Input.GetKeyDown(spellKey))
        {   
            GameObject obj = Instantiate(spellPrefab, transform.position, quaternion.identity);
            
            Spell spell = obj.GetComponent<Spell>();
            spell.Initialize(inventory.spells.ToArray()[selected], playerMove.lastDirection);

            inventory.spells.RemoveAt(selected);
            availableSpells--;

            if (availableSpells <= 0) UIText.text = "No spell";
        }

        else if (Input.GetKeyDown(spellMinus))
        {
            selected--;
            if (selected < 0) selected = availableSpells - 1;
            UIText.text = inventory.spells.ToArray()[selected].spellName;
        }

        else if (Input.GetKeyDown(spellPlus))
        {
            selected = (++selected) % availableSpells;
            UIText.text = inventory.spells.ToArray()[selected].spellName;
        }
    }
}
