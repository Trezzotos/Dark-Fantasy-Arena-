using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public float timeToDoubleClick = 5f;

    [Header("References")]
    public TMP_Text coinsLabel;
    public TMP_Text dialogLabel;
    public ShopButton[] buttons;

    [Header("Items")]
    public ItemData[] items;

    GameSaveData saveData;
    InventoryData inventory;
    float lastClickTime = 0f;
    ItemData lastClicked;

    // map per tracciare le delegate registrate su ogni bottone (per poterle rimuovere correttamente)
    private Dictionary<ShopButton, System.Action<ItemData>> handlers = new Dictionary<ShopButton, System.Action<ItemData>>();

    void Start()
{
    saveData = SaveSystem.LoadGame();

    if (saveData != null && saveData.inventory != null)
    {
        inventory = saveData.inventory;
    }
    else
    {
        // Fallback: crea un InventoryData vuoto con i param richiesti:
        var emptySpells = new System.Collections.Generic.List<SpellData>();
        var emptyPerks = new System.Collections.Generic.List<PerkData>();
        int startMoney = 0;

        inventory = new InventoryData(emptySpells, emptyPerks, startMoney);

        // Prova a recuperare gli stats correnti dal StatsManager, se disponibile
        GameStatsData statsFallback = null;
            statsFallback = StatsManager.Instance.GetCurrentGameStats();
        
        // Costruisci correttamente il GameSaveData con i param richiesti
        saveData = new GameSaveData(inventory, statsFallback);
    }

    if (coinsLabel != null) coinsLabel.text = "Coins: " + inventory.money;

    Random.InitState((int) Time.time);
    InitializeButtons();
    if (dialogLabel != null) dialogLabel.text = "";
}


    void InitializeButtons()
    {
        if (buttons == null || buttons.Length == 0) return;
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("[ShopManager] Nessun item disponibile");
            foreach (var b in buttons) if (b != null) b.gameObject.SetActive(false);
            return;
        }

        List<int> usedIndices = new List<int>();
        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            if (btn == null) continue;

            int selected;
            int attempts = 0;
            do
            {
                selected = Random.Range(0, items.Length);
                attempts++;
                // se abbiamo esaurito tutti gli items unici, permetti ripetizioni
                if (attempts > items.Length + 2) break;
            } while (usedIndices.Contains(selected) && usedIndices.Count < items.Length);

            usedIndices.Add(selected);
            btn.Set(items[selected]);
            btn.gameObject.SetActive(true);
        }
    }

    // Exit: doppio click su Exit (non legato a fakeBtn)
    public void ShopExit()
    {
        // se lastClicked è null interpretalo come primo click su exit
        if (lastClicked == null && Time.time - lastClickTime < timeToDoubleClick)
        {
            // conferma uscita
            if (saveData != null)
            {
                saveData.inventory = inventory;
                SaveSystem.SaveGame(saveData);
            }
            SceneManager.LoadScene("Game");
        }
        else
        {
            lastClickTime = Time.time;
            lastClicked = null; // segnala che abbiamo "cliccato exit" come primo click
            if (dialogLabel != null) dialogLabel.text = "Is that all, stranger?";
        }
    }

    void OnEnable()
    {
        // registra delegate uniche per ogni bottone
        if (buttons == null) return;

        handlers.Clear();
        foreach (ShopButton button in buttons)
        {
            if (button == null) continue;
            // crea closure che lega il bottone alla callback
            System.Action<ItemData> handler = (data) => OnButtonPressed(button, data);
            handlers[button] = handler;
            button.ButtonPressed += handler;
        }
    }

    void OnDisable()
    {
        // rimuovi le delegate registrate
        if (buttons == null) return;

        foreach (ShopButton button in buttons)
        {
            if (button == null) continue;
            if (handlers.TryGetValue(button, out var handler))
            {
                button.ButtonPressed -= handler;
            }
        }
        handlers.Clear();
    }

    // wrapper che riceve il bottone sorgente e l'item: utile per debug e mapping univoco
    void OnButtonPressed(ShopButton sourceButton, ItemData data)
    {
        // debug rapido
        // Debug.Log($"[ShopManager] Button pressed: {sourceButton.name} -> {data?.name}");

        BuyItem(data);
    }

    public void BuyItem(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[ShopManager] BuyItem chiamata con data null");
            return;
        }

        // se è il secondo click sullo stesso item entro il tempo, acquista
        if (data == lastClicked && Time.time - lastClickTime < timeToDoubleClick)
        {
            if (inventory.money < data.price)
            {
                if (dialogLabel != null) dialogLabel.text = "Not enough cash, stranger";
                return;
            }

            PickableData pickableData = data.item;
            if (pickableData is SpellData spell)
            {
                inventory.spells.Add(spell);
            }
            else if (pickableData is PerkData perk)
            {
                inventory.perks.Add(perk);
            }
            inventory.money -= data.price;
            if (coinsLabel != null) coinsLabel.text = "Coins: " + inventory.money;
            if (dialogLabel != null) dialogLabel.text = "Eh eh eh, thank you";

            // Rimpiazza TUTTI i bottoni con nuovi item
            ReplaceAllButtons();

            lastClicked = null;
        }
        else
        {
            if (dialogLabel != null) dialogLabel.text = data.description + "\nPrice: " + data.price;
            lastClickTime = Time.time;
            lastClicked = data;
        }
    }

    void ReplaceAllButtons()
    {
        if (buttons == null || items == null) return;

        // crea lista di candidate iniziale (copie degli items disponibili)
        List<ItemData> candidates = new List<ItemData>(items);

        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            if (btn == null) continue;

            if (candidates.Count == 0)
            {
                // non ci sono più items unici da mostrare
                btn.gameObject.SetActive(false);
                continue;
            }

            int idx = Random.Range(0, candidates.Count);
            ItemData chosen = candidates[idx];
            candidates.RemoveAt(idx);

            btn.Set(chosen);
            btn.gameObject.SetActive(true);
        }
    }
}
