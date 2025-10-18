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

            else
            {
                Debug.LogWarning("Item non riconosciuto!");
            }

        }

        // per il salvataggio
        public InventoryData GetCurrentInventoryData()
        {
            // Restituisce una nuova istanza della classe dati con i valori correnti
            return new InventoryData(this.spells, this.perks, this.money);
        }

        public void ApplyLoadedInventoryData(InventoryData loadedData)
        {
            if (loadedData != null)
            {
                // Usa l'operatore '??' per assicurare che non siano assegnate liste nulle
                spells = loadedData.spells ?? new List<SpellData>();
                perks = loadedData.perks ?? new List<Perk>();
                money = loadedData.money;
            }
        }
    }
}
