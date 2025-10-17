using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

// This is the class that acts as the 'Subject' in our Observer pattern.

namespace Examples.Observer
{
    public class Health : MonoBehaviour
    {
        public event Action<float> Damaged = delegate { };
        public event Action<float> Healed = delegate { };
        public event Action Killed = delegate { };

        [SerializeField] float _startingHealth = 100;
        public float StartingHealth => _startingHealth;

        [SerializeField] float _maxHealth = 100;
        public float MaxHealth => _maxHealth;

        public bool _canHeal;
        [SerializeField] float _regenWaitTime = .1f;
        public float RegenWaitTime
        {
            get => _regenWaitTime;
            set
            {
                if (value <= 0)
                {
                    _regenWaitTime = 0;
                    _canHeal = false;
                    // Coroutine will stop itself, we do not disturb it
                }
                else
                {
                    _regenWaitTime = value;
                    if (RegenAmountPerTime > 0) _canHeal = true;
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
                    _canHeal = false;
                    // Coroutine will stop itself, we do not disturb it
                }
                else
                {
                    _regenAmountPerTime = value;
                    if (_regenWaitTime > 0) _canHeal = true;
                    StartCoroutine(Regen());
                }
            }
        }

        float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                // ensure we can't go above our max health
                if (value > _maxHealth)
                {
                    value = _maxHealth;
                }
                _currentHealth = value;
            }
        }

        private void Awake()
        {
            CurrentHealth = _startingHealth;
            _canHeal = false;
        }

        public void Heal(float amount)
        {
            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth)
            {
                amount = CurrentHealth - MaxHealth;     // actual heal amount
                CurrentHealth = MaxHealth;
                _canHeal = false;   // Coroutine will stop itself, we do not disturb it
            }
            Healed.Invoke(amount);
        }

        public void TakeDamage(float amount)
        {
            CurrentHealth -= amount;
            Damaged.Invoke(amount);
            if (!_canHeal)
            {
                _canHeal = true;
                StartCoroutine(Regen());
            }

            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            Killed.Invoke();
            StopAllCoroutines();
            GameManager.Instance.UpdateGameState(GameState.GAMEOVER);
            Destroy(gameObject);   
        }

        private IEnumerator Regen()
        {
            Heal(RegenAmountPerTime);
            yield return new WaitForSeconds(RegenWaitTime);
            if (_canHeal) StartCoroutine(Regen());
        }
    }
}