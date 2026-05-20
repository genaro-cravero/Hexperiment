using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemy
{
    public class MeleeAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private LayerMask _playerLayer;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _hitVfxPrefab;

        [Header("Audio")]
        [SerializeField] private AudioClip[] _hitClips;
        [SerializeField, Range(0, 1)] private float _hitVolume = 0.5f;

        private float _attackRadius;
        private float _damage = 1f;

        private float _attackCooldown = 1f;
        private float _lastAttackTime;

        private bool _initialized;

        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        private Transform _player;
        private ICharacterAnimator _cAnimator;

        public void Initialize(EnemyContext context)
        {
            _initialized = true;
            _damage = context.enemyData.attackDamage;
            _attackRadius = context.enemyData.attackDistance + 0.4f;
            _cAnimator = context.cAnimator;
            _player = context.player;
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
                Quaternion rot = Quaternion.LookRotation(_player.position - transform.position);
                VfxManager.Instance.Play(_hitVfxPrefab, _player.position, rot);
                SoundFXManager.Instance.PlaySound(_hitClips, transform.position, _hitVolume);
            }
            yield return null;
            yield return new WaitUntil(() => _cAnimator.IsAnimationFinished(attackAnimName));
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }


    }
}
