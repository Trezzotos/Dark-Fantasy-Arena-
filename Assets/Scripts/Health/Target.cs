using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// This class handles the communication between the health slider and the health events

namespace Examples.Observer
{
    [RequireComponent(typeof(Health))]
    public class Target : MonoBehaviour
    {
        public Transform healthBar;
        public Health Health { get; private set; }
        Vector3 hbInitialScale;

        private void Awake()
        {
            Health = GetComponent<Health>();

            if (!healthBar) Debug.LogWarning("Healthbar unreferenced!");
            hbInitialScale = healthBar.localScale;
        }

        private void OnEnable()
        {
            // subscribe to get notified when this health takes damage!
            Health.Damaged += OnHealthChanged;
            Health.Healed += OnHealthChanged;
        }

        private void OnDisable()
        {
            Health.Damaged -= OnHealthChanged;
            Health.Healed -= OnHealthChanged;
        }

        void OnHealthChanged()
        {
            float ratio = (float)Health.CurrentHealth / Health.MaxHealth;
            ratio = math.clamp(ratio, 0, 1);
            
            healthBar.localScale = new Vector3(hbInitialScale.x * ratio, hbInitialScale.y, hbInitialScale.z);
        }
    }
}
