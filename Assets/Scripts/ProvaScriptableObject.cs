using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class ProvaScriptableObject : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public bool isUsable;
    public string description;
}
