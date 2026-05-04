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

        public EnemyAttackState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter() { }

        public void Update()
        {
            //_enemyCombat.Attack(_player);
            if (!_enemyCombat.IsPlayerInSight())
            {
                _movement.MoveTo(_player.position);
                return;
            }

            _movement.Stop();

            if (!_enemyCombat.IsInAttackRange())
            {
                _enemy.ChangeState(_chaseState);
            }
        }

        public void Exit() { }
    }
}