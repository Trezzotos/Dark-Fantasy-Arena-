using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItemData", menuName = "Shop/Item Data")]


public class ItemData : ScriptableObject
{
    public PickableData item;
    public int price;
    public string description;
}
