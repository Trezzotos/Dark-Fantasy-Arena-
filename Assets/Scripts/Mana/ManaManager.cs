using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// This class handles the communication between the health slider and the health events

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
        }

        private void OnEnable()
        {
            // subscribe to get notified when this health takes damage!
            Mana.ManaGained += OnManaChanged;
            Mana.ManaSpent += OnManaChanged;
        }

        private void OnDisable()
        {
            Mana.ManaGained -= OnManaChanged;
            Mana.ManaSpent -= OnManaChanged;
        }

        void OnManaChanged(float amount)
        {
            float ratio = (float)Mana.CurrentMana / Mana.MaxMana;
            
            manaBar.localScale = new Vector3(hbInitialScale.x * ratio, hbInitialScale.y, hbInitialScale.z);
        }
    }
}
