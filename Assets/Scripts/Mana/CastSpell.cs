using System.Linq;
using Examples.Observer;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CastSpell : MonoBehaviour
{
    const string PREF_CONTROL_SCHEME = "ControlScheme";

    public Image UISpell;
    public TMP_Text UISpellCount;
    public GameObject spellPrefab;
    [Space]
    public KeyCode spellKey = KeyCode.K;
    public KeyCode spellPlus = KeyCode.L;
    public KeyCode spellMinus = KeyCode.J;

    PlayerMove playerMove;
    [SerializeField] Inventory inventory;
    [SerializeField] int selected = 0;
    SpellData activeSpell;

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        inventory = GetComponent<Inventory>();

        UpdateUI();

        int scheme = PlayerPrefs.GetInt(PREF_CONTROL_SCHEME, 0);
        ChangeCommands(scheme);
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.PLAYING) return;

        if (Input.GetKeyDown(spellKey) && inventory.spells[activeSpell] > 0)
        {
            GameObject obj = Instantiate(spellPrefab, transform.position, Quaternion.identity);

            Spell spell = obj.GetComponent<Spell>();
            spell.ownerTag = gameObject.tag;    
            spell.Initialize(inventory.spells.Keys.ToArray()[selected], playerMove.lastDirection);

            inventory.spells[activeSpell]--;

            UpdateUI();
        }

        else if (Input.GetKeyDown(spellMinus))
        {
            selected--;
            UpdateUI();
        }

        else if (Input.GetKeyDown(spellPlus))
        {
            selected = (++selected) % inventory.spells.Count;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        activeSpell = inventory.spells.Keys.ToArray()[selected];
        UISpell.sprite = activeSpell.sprite;
        UISpellCount.text = "" + inventory.spells[activeSpell];
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
