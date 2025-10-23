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
    float lastClickTime = 0;
    ItemData fakeBtn, lastClicked;

    void Start()
    {
        saveData = SaveSystem.LoadGame();
        inventory = saveData.inventory;

        coinsLabel.text = "Coins: " + inventory.money;

        Random.InitState((int) Time.time);
        InitializeButtons();
        dialogLabel.text = "";
    }

    void InitializeButtons()
    {
        List<int> usedIndices = new List<int>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int selected;
            do
            {
                selected = Random.Range(0, items.Length);
            } while (usedIndices.Contains(selected) && usedIndices.Count < items.Length);

            usedIndices.Add(selected);
            buttons[i].Set(items[selected]);
            buttons[i].gameObject.SetActive(true);
        }
    }

    public void ShopExit()
    {
        if (lastClicked == fakeBtn && Time.time - lastClickTime < timeToDoubleClick)
        {
            saveData.inventory = inventory;
            SaveSystem.SaveGame(saveData);
            SceneManager.LoadScene("Game");
        }
        else
        {
            lastClickTime = Time.time;
            lastClicked = fakeBtn;
            dialogLabel.text = "Is that all, stranger?";
        }
    }

    void OnEnable()
    {
        foreach (ShopButton button in buttons)
            button.ButtonPressed += BuyItem;
    }

    void OnDisable()
    {
        foreach (ShopButton button in buttons)
            button.ButtonPressed -= BuyItem;
    }

    public void BuyItem(ItemData data)
    {
        if (data == lastClicked && Time.time - lastClickTime < timeToDoubleClick)
        {
            if (inventory.money < data.price)
            {
                dialogLabel.text = "Not enough cash, stranger";
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
            coinsLabel.text = "Coins: " + inventory.money;
            dialogLabel.text = "Eh eh eh, thank you";

            // Rimpiazza TUTTI i bottoni con nuovi item
            ReplaceAllButtons();

            lastClicked = null;
        }
        else
        {
            dialogLabel.text = data.description + "\nPrice: " + data.price;
            lastClickTime = Time.time;
            lastClicked = data;
        }
    }

    void ReplaceAllButtons()
    {
        // crea lista di candidate iniziale (copie degli items disponibili)
        List<ItemData> candidates = new List<ItemData>(items);

        // se gli items disponibili sono meno dei bottoni, useremo tutti i candidates e poi disabiliteremo i restanti
        // scegli in modo random senza ripetizioni finché possibile
        for (int i = 0; i < buttons.Length; i++)
        {
            if (candidates.Count == 0)
            {
                // non ci sono più items unici da mostrare
                buttons[i].gameObject.SetActive(false);
                continue;
            }

            int idx = Random.Range(0, candidates.Count);
            ItemData chosen = candidates[idx];
            candidates.RemoveAt(idx);

            buttons[i].Set(chosen);
            buttons[i].gameObject.SetActive(true);
        }
    }
}
