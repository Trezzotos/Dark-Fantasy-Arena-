using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewPerkData", menuName = "Inventory/Perks/Perk Data")]
[System.Serializable]
public class PerkData : PickableData
{
    public string perkName;

    // image.sprite = perkImage     works and it is great
    public Sprite perkImage;
}
