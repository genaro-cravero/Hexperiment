using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackState : IEnemyState
    {
        private Enemy _enemy;
        private Transform _player => _enemy.Context.player;
        private IEnemyState _chaseState => _enemy.Context.chaseState;
        private EnemyCombat _enemyCombat => _enemy.Context.enemyCombat;
        private EnemyMovement _movement => _enemy.Context.movement;
        private IEnemyAttack _attack;

        public EnemyAttackState(Enemy enemy)
        {
            _enemy = enemy;
            _attack = _enemy.GetComponent<IEnemyAttack>();
            _attack.Initialize(_enemy.Context);
        }

        public void Enter()
        {
            _enemy.StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            _attack.Attack();
            yield return null;
            yield return new WaitUntil(() => !_attack.IsAttacking);

            while (!_enemyCombat.IsPlayerInSight())
            {
                _movement.MoveTo(_player.position);
                yield return null;
            }

            _movement.Stop();

            if (!_enemyCombat.IsInAttackRange())
            {
                _enemy.ChangeState(_chaseState);
                yield break;
            }

            _enemy.StartCoroutine(AttackCoroutine());
        }

        public void Exit()
        {
            _enemy.StopAllCoroutines();
        }

        public void Update(){ }
    }
}