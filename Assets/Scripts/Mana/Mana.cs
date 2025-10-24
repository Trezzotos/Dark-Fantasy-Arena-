using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

// This is the class that acts as the 'Subject' in our Observer pattern.

namespace Examples.Observer
{
    public class Mana : MonoBehaviour
    {
        public event Action ManaSpent = delegate { };
        public event Action ManaGained = delegate { };

        [SerializeField] float _startingMana = 100;
        public float StartingMana => _startingMana;

        [SerializeField] float _maxMana = 100;
        public float MaxMana => _maxMana;

        public bool _canBeRestored;
        [SerializeField] float _regenWaitTime = .1f;
        public float RegenWaitTime
        {
            get => _regenWaitTime;
            set
            {
                if (value <= 0)
                {
                    _regenWaitTime = 0;
                    _canBeRestored = false;
                    // Coroutine will stop itself, we do not disturb it
                }
                else
                {
                    _regenWaitTime = value;
                    if (RegenAmountPerTime > 0) _canBeRestored = true;
                    StartCoroutine(Regen());
                }
            }
        }

        [SerializeField] float _regenAmountPerTime = .1f;
        public float RegenAmountPerTime
        {
            get => _regenAmountPerTime;
            set
            {
                if (value <= 0)
                {
                    _regenAmountPerTime = 0;
                    _canBeRestored = false;
                    // Coroutine will stop itself, we do not disturb it
                }
                else
                {
                    _regenAmountPerTime = value;
                    if (_regenWaitTime > 0) _canBeRestored = true;
                    StartCoroutine(Regen());
                }
            }
        }

        float _currentMana;
        public float CurrentMana
        {
            get => _currentMana;
            set
            {
                // ensure we can't go above our max health
                if (value > _maxMana)
                {
                    value = _maxMana;
                }
                _currentMana = value;
            }
        }

        private void Awake()
        {
            CurrentMana = _startingMana;
            _canBeRestored = false;
        }

        public void GainMana(float amount)
        {
            CurrentMana += amount;
            if (CurrentMana > MaxMana)
            {
                amount = CurrentMana - MaxMana;     // actual heal amount
                CurrentMana = MaxMana;
                _canBeRestored = false;   // Coroutine will stop itself, we do not disturb it
            }
            ManaGained.Invoke();
        }

        public void SpendMana(float amount)
        {
            CurrentMana -= amount;
            ManaSpent.Invoke();
            if (!_canBeRestored)
            {
                _canBeRestored = true;
                StartCoroutine(Regen());
            }
        }

        private IEnumerator Regen()
        {
            GainMana(RegenAmountPerTime);
            yield return new WaitForSeconds(RegenWaitTime);
            if (_canBeRestored) StartCoroutine(Regen());
        }
    }
}