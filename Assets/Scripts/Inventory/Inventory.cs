using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Observer
{
    public class Inventory : MonoBehaviour
    {
        public List<SpellData> spells;
        public List<Perk> perks;
        public int money;

        private void OnEnable()
        {
            Pickable.ItemPickedUp += AddItemToInventory;
        }

        private void OnDisable()
        {
            Pickable.ItemPickedUp -= AddItemToInventory;
        }

        private void AddItemToInventory(PickableData data)
        {
            if (data is MoneyData moneyData)
            {
                money += moneyData.value;
                print(money);
            }

            // codice modulare: i nemici potrebbero droppare qualsiasi cosa :D
        }
    }
}
