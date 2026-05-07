using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class MeleeAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private LayerMask _playerLayer;
        private float _damage = 1f;

        private float _attackCooldown = 1f;
        private float _lastAttackTime;

        private bool _initialized;

        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;

        public void Initialize(EnemyContext context)
        {
            _initialized = true;
            _damage = context.enemyData.attackDamage;
            _initialized = true;
        }

        public void Attack()
        {
            if (!_initialized || IsAttacking)
                return;
            if (Time.time < _lastAttackTime + _attackCooldown) return;

            _isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            Collider[] hits = new Collider[1];
            var hitCount = Physics.OverlapSphereNonAlloc(transform.position, 1f, hits, _playerLayer);

            if (hitCount > 0)
            {
                if (hits[0].TryGetComponent(out Health.IDamageable damageable))
                {
                    damageable.TakeDamage(_damage, gameObject);
                }
            }

            yield return new WaitForSeconds(0.3f); //Simulate visual attack delay
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }


    }
}
