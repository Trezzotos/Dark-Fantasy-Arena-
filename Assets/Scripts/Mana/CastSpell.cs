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

    // Memorizza l'ultima direzione usata per sparare (componenti possono essere -1,0,1). Viene normalizzata prima dell'uso.
    Vector2 lastShootDirection = Vector2.right; // fallback se non c'è mai stato input

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
            // Determina direzione di sparo: preferisci l'input corrente raw, altrimenti conserva ultima direzione di sparo
            Vector2 dirRaw = playerMove != null ? playerMove.lastDirectionRaw : Vector2.zero;
            if (dirRaw != Vector2.zero)
            {
                lastShootDirection = dirRaw;
            }

            Vector2 shootDir = lastShootDirection.normalized;
            if (shootDir == Vector2.zero) shootDir = Vector2.right;

            GameObject obj = Instantiate(spellPrefab, transform.position, Quaternion.identity);

            Spell spell = obj.GetComponent<Spell>();
            spell.ownerTag = gameObject.tag;
            spell.Initialize(inventory.spells.Keys.ToArray()[selected], shootDir);

            inventory.spells[activeSpell]--;
            UpdateUI();
        }
        else if (Input.GetKeyDown(spellMinus))
        {
            selected--;
            if (selected < 0) selected = Mathf.Max(0, inventory.spells.Count - 1);
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
            case 0:
                spellKey = KeyCode.K;
                spellPlus = KeyCode.L;
                spellMinus = KeyCode.J;
                break;
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
