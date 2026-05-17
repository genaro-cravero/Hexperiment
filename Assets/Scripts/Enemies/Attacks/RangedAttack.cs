using Health;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace Enemy
{
    public class RangedAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private LayerMask _shootLayer;
        [SerializeField] private VisualEffect _muzzleVFX;

        [SerializeField] private BulletData _bulletData;
        private IObjectPool<Bullet> _bulletPool;

        private float _damage = 1f;
        private float _fireRate = 1f;
        private float _lastAttackTime;
        private const float ROT_THRESHOLD = 0.15f;

        private bool _initialized;
        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        private HealthComponent _health;
        private Transform _player;
        private CharacterData _data;
        private Vector3 _lastPlayerPosition;
        private Vector3 _playerVelocity;
        private float _lastVelocityTime;
        private ICharacterAnimator _cAnimator;

        private const float PREDICTION_BASE = 0.75f;
        private float _predictionAccuracy;

        private void OnDisable()
        {
            if (_health != null)
                _health.OnDie -= HandleDie;

            StopAllCoroutines();
            _isAttacking = false;
        }
        public void Initialize(EnemyContext context)
        {
            _fireRate = context.enemyData.attackCoolDown;
            _damage = context.enemyData.attackDamage;
            _player = context.player;
            _data = context.enemyData;
            _cAnimator = context.cAnimator;

            _lastPlayerPosition = _player.position;
            _lastVelocityTime = Time.time;
            _bulletPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(_bulletPrefab),
                actionOnGet: bullet =>
                {
                    bullet.SetParameters(_shootLayer, _damage, false);
                    bullet.gameObject.SetActive(true);
                },
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                maxSize: 20
            );
            _predictionAccuracy = Mathf.Lerp(PREDICTION_BASE, 1f, WaveManager.Instance.TotalWavesProgress);
            _health = GetComponent<HealthComponent>();
            StartCoroutine(WaitForInitializeHealth());    

            _initialized = true;
        }
        private IEnumerator WaitForInitializeHealth()
        {
            if (!_health) yield break;

            yield return new WaitUntil(() => _health.IsInitialized);

            _health.OnDie += HandleDie;
        }
        public void Attack()
        {
            if (!_initialized || IsAttacking || !_health.IsAlive)
                return;

            _isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            while (Time.time < _lastAttackTime + _fireRate)
            {
                if (!_health.IsAlive) HandleDie();

                yield return RotateToTarget();
            }

            var attackAnimName = "Attack";
            _muzzleVFX.Play();
            _cAnimator.Play(attackAnimName);

            var bullet = _bulletPool.Get();
            bullet.transform.SetPositionAndRotation(_shootPoint.position, _shootPoint.rotation);
            bullet.Init(_bulletPool);

            yield return null;
            yield return new WaitUntil(() => _cAnimator.IsAnimationFinished(attackAnimName));
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }

        private IEnumerator RotateToTarget()
        {
            Vector3 targetDirection = CalculateTargetDir() - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (Quaternion.Angle(transform.rotation, targetRotation) > ROT_THRESHOLD)
            {
                targetDirection = CalculateTargetDir() - transform.position;
                targetDirection.y = 0f;
                targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _data.rotationSpeed * 360f * Time.deltaTime);
                yield return null;
            }
        }

        private Vector3 CalculateTargetDir()
        {
            UpdatePlayerVelocity();

            Vector3 target = _player.position;
            float bulletSpeed = Mathf.Max(0.01f, _bulletData.speed);
            float distToPlayer = Vector3.Distance(_player.position, transform.position);
            float timeToTarget = distToPlayer / bulletSpeed;

            float accuracy = Mathf.Clamp01(_predictionAccuracy);
            target += _playerVelocity * timeToTarget * accuracy;
            target.y = 0f;
            return target;
        }

        private void UpdatePlayerVelocity()
        {
            float deltaTime = Time.time - _lastVelocityTime;
            if (deltaTime <= 0f)
                return;

            Vector3 currentPosition = _player.position;
            _playerVelocity = (currentPosition - _lastPlayerPosition) / deltaTime;
            _lastPlayerPosition = currentPosition;
            _lastVelocityTime = Time.time;
        }

        private void HandleDie()
        {
            StopAllCoroutines();
            _isAttacking = false;
            if (_muzzleVFX)
                _muzzleVFX.Stop();
            enabled = false;
        }
    }
}
