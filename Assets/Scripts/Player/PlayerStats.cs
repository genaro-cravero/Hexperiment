using Health;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private CharacterData _playerData;
        private IDamageable _playerHealth;
        private const float MIN_FIRE_COOLDOWN = 0.2f;

        [Header("Improvables")]
        [HideInInspector] public float moveSpeed;
        [HideInInspector] public float fireCooldown;
        [HideInInspector] public float damage;
        [HideInInspector] public float maxHealth;

        [HideInInspector] public float gravity;
        [HideInInspector] public float gravityAcceleration;

        private void Awake()
        {
            _playerHealth = GetComponent<IDamageable>();

            moveSpeed = _playerData.moveSpeed;
            fireCooldown = _playerData.attackCoolDown;
            damage = _playerData.attackDamage;
            maxHealth = _playerData.initialHealth;

            gravity = _playerData.gravity;
            gravityAcceleration = _playerData.gravityAcceleration;
        }

        public void IncreaseMoveSpeed(float perc)
        {
            var multiplier = 1 + (perc / 100f);
            moveSpeed *= multiplier;
        }

        public void IncreaseFireRate(float perc)
        {
            var multiplier = 1f + (perc / 100f);
            fireCooldown /= multiplier;
            fireCooldown = Mathf.Clamp(fireCooldown, MIN_FIRE_COOLDOWN, fireCooldown);
        }

        public void IncreaseDamage(float amount)
        {
            damage += amount;
        }

        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            _playerHealth.IncreaseMaxHealth(amount);
        }
    }
}