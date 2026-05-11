using Enemy;
using Health;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerHealthReaction : MonoBehaviour
    {

        private HealthComponent _health;
        private PlayerMovement _movement;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _movement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            _health.OnDamage += HandleDamage;
            _health.OnDie += HandleDie;
            _health.OnPush += HandlePush;
        }

        private void OnDisable()
        {
            _health.OnDamage -= HandleDamage;
            _health.OnDie -= HandleDie;
            _health.OnPush -= HandlePush;
        }

        private void HandleDamage(float damage, GameObject source)
        {
            //VFX and SFX for player damage
        }

        private void HandlePush(float damage, GameObject source)
        {
            Vector3 direction = transform.position - source.transform.position;
            _movement.Knockback(direction, damage);
        }

        private void HandleDie()
        {
            GameManager.Instance.LoseGame();
        }
    }
}
