using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Health
{
    public class HealthManager
    {
        private float _currentHealth;
        private float _maxHealth;
        private Slider _healthBar;
        private TextMeshProUGUI _healthText;

        public bool IsAlive { get; private set; }

        public Action<float, GameObject> OnDamage;
        public Action OnDie;

        public HealthManager(float maxHealth, Slider healthBar)
        {
            IsAlive = true;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _healthBar = healthBar;
        }
        public HealthManager(float maxHealth, Slider healthBar, TextMeshProUGUI healthText)
        {
            IsAlive = true;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _healthBar = healthBar;
            _healthText = healthText;
        }

        public void TakeDamage(float damage, GameObject source)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Die();
                UpdateHealthBar();
                return;
            }
            OnDamage?.Invoke(damage, source);
            UpdateHealthBar();
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            _currentHealth += amount;
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
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
                if(_healthText)
                    _healthText.text = $"{_currentHealth} / {_maxHealth}";
                if(IsAlive)
                {
                    if(!_healthBar.gameObject.activeSelf)
                        _healthBar.gameObject.SetActive(true);
                    _healthBar.value = _currentHealth / _maxHealth;
                }
                else
                    _healthBar.gameObject.SetActive(false);
            }
        }

    }
}
