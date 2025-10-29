using Examples.Observer;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CastSpell : MonoBehaviour
{
    const string PREF_CONTROL_SCHEME = "ControlScheme";

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

        availableSpells = inventory != null ? inventory.spells.Count : 0;
        if (availableSpells <= 0) UISpell.sprite = noSpellSprite;
        else UISpell.sprite = inventory.spells.ToArray()[selected].sprite;

        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        ChangeCommands(scheme);
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
            else
            {
                selected = Mathf.Clamp(selected, 0, availableSpells - 1);
                UISpell.sprite = inventory.spells.ToArray()[selected].sprite;
            }
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

    public void ChangeCommands(int index)
    {
        switch (index)
        {
            // Set 0: movement WASD, shoot I, spells J K L
            case 0:
                spellKey = KeyCode.K;
                spellPlus = KeyCode.L;
                spellMinus = KeyCode.J;
                break;
            // Set 1: movement arrows, shoot E, spells Z X C
            case 1:
                spellKey = KeyCode.X;
                spellPlus = KeyCode.C;
                spellMinus = KeyCode.Z;
                break;
            default:
                spellKey = KeyCode.K;
                spellPlus = KeyCode.L;
                spellMinus = KeyCode.J;
                break;
        }
    }
}
