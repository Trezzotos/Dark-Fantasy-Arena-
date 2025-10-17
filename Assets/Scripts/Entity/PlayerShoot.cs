using System.Collections;
using System.Collections.Generic;
using Examples.Observer;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Commands")]
    public KeyCode shoot = KeyCode.E;
    public KeyCode shootSpell = KeyCode.T;

    [Space]
    [SerializeField] private RaycastShoot weapon;

    private PlayerMove pm;


    // DA RILOCARE
    public GameObject spellPrefab;
    public SpellData[] availableSpells;
    private int selectedSpell = 0;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMove>();
        weapon = GetComponentInChildren<RaycastShoot>();

        if (pm == null) Debug.LogError("PlayerMove non trovato!");
        if (weapon == null) Debug.LogError("RaycastShoot non trovato nei figli!");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.PLAYING) return;

        // handle shooting
        if (Input.GetKey(shoot)) weapon.TryShoot(pm.lastDirection);
        if (Input.GetKey(shootSpell))
        {
            GameObject spell = Instantiate(spellPrefab, transform.position, Quaternion.identity);
            Spell spellComponent = spell.GetComponent<Spell>();
            if (spellComponent)
            {
                spellComponent.Initialize(availableSpells[selectedSpell], pm.lastDirection);
            }
        }
    }
}