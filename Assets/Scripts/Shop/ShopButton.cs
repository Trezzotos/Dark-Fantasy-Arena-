using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public event Action<ItemData> ButtonPressed = delegate { };

    [SerializeField] Image image;
    [SerializeField] TMP_Text label;

    ItemData data;

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
