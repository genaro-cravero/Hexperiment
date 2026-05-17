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
        public Action<float, GameObject> OnPush;
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

        public void TakeDamage(float damage, GameObject source, bool push)
        {
            if (!IsAlive) return;
            if (!GameManager.Instance.IsGameplayActive) return;

            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Die();
                UpdateHealthBar();
                return;
            }
            OnDamage?.Invoke(damage, source);
            if(push)
            {
                OnPush?.Invoke(damage, source);
            }
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
        public void IncreaseMaxHealth(float amount)
        {
            _maxHealth += amount;
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
                    _healthText.text = $"{_currentHealth:0.##} / {_maxHealth:0.##}";
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
