using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Enemy
{
    public class RangedAttack : MonoBehaviour, IEnemyAttack
    {
        private float _damage = 1f;

        private float _attackCooldown = 1f;
        private float _lastAttackTime;

        private bool _initialized;

        private bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        private Transform _player;
        private CharacterData _data;

        public void Initialize(EnemyContext context)
        {
            _attackCooldown = context.enemyData.attackCoolDown;
            _damage = context.enemyData.attackDamage;
            _player = context.player;
            _data = context.enemyData;
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
            Debug.Log("Shooted");

            yield return RotateToPlayer();

            yield return new WaitForSeconds(0.3f); //Simulate visual attack delay
            _isAttacking = false;
            _lastAttackTime = Time.time;
        }

        private IEnumerator RotateToPlayer()
        {
            Quaternion targetRotation = _player.rotation;

            while (transform.rotation != targetRotation)
            {
                Debug.Log("Rotating to player");
                Vector3 targetDirection = _player.position - transform.position;
                targetDirection.y = 0f;
                targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    targetRotation, _data.moveSpeed);
                yield return null;
            }
        }
    }
}
