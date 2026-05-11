using Health;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealthReaction : MonoBehaviour
    {
        private HealthComponent _health;
        private EnemyMovement _enemyMovement;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _enemyMovement = GetComponent<EnemyMovement>();
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
            Vector3 pushDirection = (transform.position - source.transform.position).normalized;
        }

        private void HandlePush(float damage, GameObject source)
        {
            _enemyMovement.PushBack(-transform.forward, damage);

        }

        private void HandleDie()
        {
            gameObject.SetActive(false);
        }
    }
}
