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

        private void Awake()
        {
            _health = new HealthManager(_characterData.initialHealth, _healthBar);
        }

        public void TakeDamage(float damage)
        {
            _health.TakeDamage(damage);
        }
    }
}
