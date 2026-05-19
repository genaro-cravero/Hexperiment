using Health;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class BossAttack : MonoBehaviour, IEnemyAttack
    {
        [Header("Shoot Settings")]
        [SerializeField] private BulletHell _bulletHell;

        private float _fireRate = 1f;
        private float _lastAttackTime;
        private const float ROT_THRESHOLD = 0.15f;

        [Header("Melee Settings")]
        [SerializeField] private float _attackRadius = 2f;
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private Transform _meleePoint;
        [SerializeField] private float _meleeDamage = 5f;
        private const float DISTANCE_TO_GET_CLOSE = 8f;
        private const float MELEE_RANGE = 3f;
        private EnemyMovement _movement;
        private bool _justMeleeAttacked;
        private WaitForSeconds _meleeCooldown = new(3f);

        [Header("Initialization")]
        private bool _initialized;
        private bool _isAttacking;
        private HealthComponent _health;
        private CharacterData _data;
        private EnemyCombat _enemyCombat;
        private ICharacterAnimator _cAnimator;
        private int _stages = 3;
        private int _currentStage;
        private float _hpPerStage;
        private float _currentStageHp;
        public bool IsAttacking => _isAttacking;

        [Header("Player References")]
        private Transform _player;

        public void Initialize(EnemyContext context)
        {
            _fireRate = context.enemyData.attackCoolDown;
            _player = context.player;
            _data = context.enemyData;
            _movement = context.movement;
            _enemyCombat = context.enemyCombat;
            _cAnimator = context.cAnimator;

            _health = GetComponent<HealthComponent>();
            _hpPerStage = _data.initialHealth / _stages;
            _currentStageHp = _hpPerStage;
            StartCoroutine(SuscribeToEvents());


            _initialized = true;
        }
        
        private void OnDisable()
        {
            _health.OnDie -= StopAllAttacks;
            _health.OnDamage -= ChangeStage;
        }
        public void Attack()
        {
            if (!_initialized || IsAttacking)
                return;

            _isAttacking = true;
            if (!_justMeleeAttacked && IsPlayerClose())
            {
                StartCoroutine(GetClose());
                return;
            }
            else
            {
                _movement.SetStoppingDistance(_data.attackDistance);
                StartCoroutine(Shootcoroutine());
            }
        }

        private IEnumerator Shootcoroutine()
        {
            while (Time.time < _lastAttackTime + _fireRate)
            {
                yield return null;
            }
            yield return RotateToTarget();
            _bulletHell.gameObject.SetActive(true);
            var animName = "Attack";
            _cAnimator.Play(animName);
            while (!IsPlayerClose() && _enemyCombat.IsInAttackRange())
            {
                if(_cAnimator.IsAnimationFinished(animName))
                {
                    _cAnimator.Play(animName);
                }
                yield return null;
            }
            _bulletHell.gameObject.SetActive(false);
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }

        private IEnumerator RotateToTarget()
        {
            Vector3 targetDirection = _player.position - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (Quaternion.Angle(transform.rotation, targetRotation) > ROT_THRESHOLD)
            {
                targetDirection = _player.position - transform.position;
                targetDirection.y = 0f;
                targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _data.rotationSpeed * 360f * Time.deltaTime);
                yield return null;
            }
        }


        #region Melee
        private IEnumerator GetClose()
        {
            _cAnimator.Play("Move");
            _movement.SetStoppingDistance(MELEE_RANGE);
            while (!IsPlayerClose(true))
            {
                _movement.ForceMoveTo(_player.position);
                yield return null;
            }
            if (!IsPlayerClose())
            {
                _isAttacking = false;
                yield break;
            }
            StartCoroutine(MeleeAttackCoroutine());
        }

        private IEnumerator MeleeAttackCoroutine()
        {
            Collider[] hits = new Collider[1];
            var hitCount = Physics.OverlapSphereNonAlloc(_meleePoint.position, _attackRadius, hits, _playerLayer);

            if (hitCount > 0)
            {
                if (hits[0].TryGetComponent(out Health.IDamageable damageable))
                {
                    damageable.TakeDamage(_meleeDamage, gameObject, true);
                }
            }

            yield return new WaitForSeconds(0.3f);

            _justMeleeAttacked = true;
            _isAttacking = false;
            _lastAttackTime = Time.time;
            yield return _meleeCooldown;
            _justMeleeAttacked = false;
        }


        private bool IsPlayerClose(bool meleeDistance = false)
        {
            float distance = Vector3.Distance(transform.position, _player.position);
            float targetDist = meleeDistance ? MELEE_RANGE : DISTANCE_TO_GET_CLOSE;
            return distance <= targetDist;
        }
        #endregion

        private void ChangeStage(float damage, GameObject source)
        {
            if (!_health.IsAlive) return;

            _currentStageHp -= damage;
            if(_currentStageHp <= 0)
            {
                _currentStageHp = _hpPerStage;
                _currentStage++;

                _bulletHell.ChangeStage(_currentStage);
            }
        }

        private IEnumerator SuscribeToEvents()
        {
            yield return new WaitForSeconds(0.3f);
            _health.OnDie += StopAllAttacks;
            _health.OnDamage += ChangeStage;
        }

        private void StopAllAttacks()
        {
            StopAllCoroutines();
            _bulletHell.gameObject.SetActive(false);
        }
    }
}