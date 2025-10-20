using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Tooltip("Exit button has to be clicked twice within this time to exit shop")]
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
        int selected;
        List<int> l = new List<int>();
        foreach (ShopButton button in buttons)
        {
            do
            {
                selected = Random.Range(0, buttons.Length);
            } while (l.Contains(selected));
            l.Add(selected);

            button.Set(items[selected]);
        }

        dialogLabel.text = "";
    }

    public void ShopExit()
    {
        // check for the double click
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
        // check for the double click
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

            lastClicked = null;
        }
        else
        {
            dialogLabel.text = data.description + "\nPrice: " + data.price;
            lastClickTime = Time.time;
            lastClicked = data;
        }
    }
}
