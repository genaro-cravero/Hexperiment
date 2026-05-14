using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class MeleeAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private LayerMask _playerLayer;
        private float _attackRadius;
        private float _damage = 1f;

        private float _attackCooldown = 1f;
        private float _lastAttackTime;

        private bool _initialized;

        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        private ICharacterAnimator _cAnimator;

        public void Initialize(EnemyContext context)
        {
            _initialized = true;
            _damage = context.enemyData.attackDamage;
            _attackRadius = context.enemyData.attackDistance + 0.4f;
            _cAnimator = context.cAnimator;
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
            var hitCount = Physics.OverlapSphereNonAlloc(transform.position, _attackRadius, hits, _playerLayer);
            var attackAnimName = "Attack";
            _cAnimator.Play(attackAnimName);
            if (hitCount > 0)
            {
                if (hits[0].TryGetComponent(out Health.IDamageable damageable))
                {
                    damageable.TakeDamage(_damage, gameObject, true);
                }
            }
            yield return null;
            yield return new WaitUntil(() => _cAnimator.IsAnimationFinished(attackAnimName));
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }


    }
}
