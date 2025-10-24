using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles the communication between the mana slider and the mana events

namespace Examples.Observer
{
    [RequireComponent(typeof(Health))]
    public class ManaManager : MonoBehaviour
    {
        public Transform manaBar;
        public Mana Mana { get; private set; }
        Vector3 hbInitialScale;

        private void Awake()
        {
            Mana = GetComponent<Mana>();

            if (!manaBar) Debug.LogWarning("Mana bar unreferenced!");
            hbInitialScale = manaBar.localScale;

            UpdateManaBar();
        }

        private void OnEnable()
        {
            // subscribe to get notified when this health takes damage!
            Mana.ManaGained += UpdateManaBar;
            Mana.ManaSpent += UpdateManaBar;
        }

        private void OnDisable()
        {
            Mana.ManaGained -= UpdateManaBar;
            Mana.ManaSpent -= UpdateManaBar;
        }

        void UpdateManaBar()
        {
            float ratio = (float)Mana.CurrentMana / Mana.MaxMana;

            manaBar.localScale = new Vector3(hbInitialScale.x * ratio, hbInitialScale.y, hbInitialScale.z);
        }
    }
}
