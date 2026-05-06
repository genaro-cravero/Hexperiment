using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Enemy
{
    public class RangedAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private LayerMask _shootLayer;
        private IObjectPool<Bullet> _bulletPool;

        private float _damage = 1f;
        private float _fireRate = 1f;
        private float _lastAttackTime;
        private const float ROT_THRESHOLD = 0.3f;

        private bool _initialized;
        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        private Transform _player;
        private CharacterData _data;

        public void Initialize(EnemyContext context)
        {
            _fireRate = context.enemyData.attackCoolDown;
            _damage = context.enemyData.attackDamage;
            _player = context.player;
            _data = context.enemyData;
            _bulletPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(_bulletPrefab),
                actionOnGet: bullet =>
                {
                    bullet.SetParameters(_shootLayer, _damage);
                    bullet.gameObject.SetActive(true);
                },
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                maxSize: 20
            );
            _initialized = true;
        }
        public void Attack()
        {
            if (!_initialized || IsAttacking)
                return;

            _isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            while(Time.time < _lastAttackTime + _fireRate)
            {
                yield return RotateToPlayer();
            }
            var bullet = _bulletPool.Get();
            bullet.transform.SetPositionAndRotation(_shootPoint.position, _shootPoint.rotation);
            bullet.Init(_bulletPool);

            yield return new WaitForSeconds(0.3f); //Simulate visual attack delay
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }

        private IEnumerator RotateToPlayer()
        {
            Vector3 targetDirection = _player.position - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (Quaternion.Angle(transform.rotation, targetRotation) > ROT_THRESHOLD)
            {
                targetDirection = _player.position - transform.position;
                targetDirection.y = 0f;
                targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    targetRotation, _data.moveSpeed);
                yield return null;
            }
        }
    }
}
