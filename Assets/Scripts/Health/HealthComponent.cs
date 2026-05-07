using System;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private Slider _healthBar;

        private HealthManager _health;

        public bool IsAlive => _health.IsAlive;

        public event Action<float, GameObject> OnDamage
        {
            add => _health.OnDamage += value;
            remove => _health.OnDamage -= value;
        }

        public event Action OnDie
        {
            add => _health.OnDie += value;
            remove => _health.OnDie -= value;
        }

        private void Awake()
        {
            _health = new HealthManager(_characterData.initialHealth, _healthBar);
        }

        public void TakeDamage(float damage, GameObject source)
        {
            _health.TakeDamage(damage, source);
        }
    }
}
