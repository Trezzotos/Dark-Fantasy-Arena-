using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public event Action<ItemData> ButtonPressed = delegate { };

    [SerializeField] Image image;
    [SerializeField] TMP_Text label;

    ItemData data;
    public ItemData CurrentData => data; // proprietà pubblica per leggere l'item corrente

    public void Set(ItemData itemData)
    {
        data = itemData;

        PickableData pickableData = itemData.item;
        if (pickableData is SpellData spellData)
        {
            image.sprite = spellData.sprite;
            label.text = spellData.spellName;
        }
        else if (pickableData is PerkData perkData)
        {
            image.sprite = perkData.perkImage;
            label.text = perkData.perkName;
        }
    }

    public void OnButtonClick()
    {
        ButtonPressed.Invoke(data);
    }
}
