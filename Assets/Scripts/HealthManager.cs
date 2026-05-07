using System;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class HealthManager
    {
        private float _currentHealth;
        private float _maxHealth;
        private Slider _healthBar;

        public bool IsAlive { get; private set; }

        public Action OnDamage;
        public Action OnDie;

        public HealthManager(float maxHealth, Slider healthBar)
        {
            IsAlive = true;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _healthBar = healthBar;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            OnDamage?.Invoke();
            if (_currentHealth <= 0)
            {
                Die();
            }
            UpdateHealthBar();
        }

        private void Die()
        {
            IsAlive = false;
            OnDie?.Invoke();
        }

        private void UpdateHealthBar()
        {
            if (_healthBar)
            {
                if(IsAlive)
                    _healthBar.value = _currentHealth / _maxHealth;
                else
                    _healthBar.gameObject.SetActive(false);
            }
        }

    }
}
