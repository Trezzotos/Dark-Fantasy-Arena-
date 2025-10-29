using Examples.Observer;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CastSpell : MonoBehaviour
{
    public Image UISpell;
    public GameObject spellPrefab;
    public Sprite noSpellSprite;
    [Space]
    public KeyCode spellKey = KeyCode.K;
    public KeyCode spellPlus = KeyCode.L;
    public KeyCode spellMinus = KeyCode.J;

    PlayerMove playerMove;
    [SerializeField] Inventory inventory;
    [SerializeField] int selected = 0;
    [SerializeField] int availableSpells;


    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        inventory = GetComponent<Inventory>();
        
        availableSpells = inventory.spells.Count;
        if (availableSpells <= 0) UISpell.sprite = noSpellSprite;
        else UISpell.sprite = inventory.spells.ToArray()[selected].sprite;
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;
        if (availableSpells <= 0) return;

        if (Input.GetKeyDown(spellKey))
        {   
            GameObject obj = Instantiate(spellPrefab, transform.position, Quaternion.identity);
            
            Spell spell = obj.GetComponent<Spell>();
            spell.Initialize(inventory.spells.ToArray()[selected], playerMove.lastDirection);

            inventory.spells.RemoveAt(selected);
            availableSpells--;

            if (availableSpells <= 0) UISpell.sprite = noSpellSprite;
        }

        else if (Input.GetKeyDown(spellMinus))
        {
            selected--;
            if (selected < 0) selected = availableSpells - 1;
            UISpell.sprite = inventory.spells.ToArray()[selected].sprite;
        }

        else if (Input.GetKeyDown(spellPlus))
        {
            selected = (++selected) % availableSpells;
            UISpell.sprite = inventory.spells.ToArray()[selected].sprite;
        }
    }
}
