using UnityEngine;

namespace Enemy
{
    public class EnemyChaseState : IEnemyState
    {
        private Enemy _enemy;
        private Transform _player => _enemy.Context.player;
        private EnemyMovement _movement => _enemy.Context.movement;
        private IEnemyState _attackState => _enemy.Context.attackState;
        private EnemyCombat _enemyCombat => _enemy.Context.enemyCombat;


        public EnemyChaseState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public void Enter() { }

        public void Update()
        {
            _movement.MoveTo(_player.position);

            if (_enemyCombat.IsInAttackRange())
            {
                _enemy.ChangeState(_attackState);
            }
        }

        public void Exit() 
        {
            _movement.Stop();
        }
    }
}