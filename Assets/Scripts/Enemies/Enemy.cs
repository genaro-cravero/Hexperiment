using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private CharacterData _enemyData;
        private EnemyStateMachine _stateMachine;
        private Transform _player;
        private EnemyMovement _movement;

        private IEnemyState _attackState;
        private IEnemyState _chaseState;

        private EnemyContext _context;
        public EnemyContext Context => _context;
        void Awake()
        {
            _stateMachine = new EnemyStateMachine();

            _player = FindAnyObjectByType<Player.PlayerMovement>().transform;
            _movement = GetComponent<EnemyMovement>();

            _attackState = new EnemyAttackState(this);
            _chaseState = new EnemyChaseState(this);

            var enemycombat = GetComponent<EnemyCombat>();

            _context = new EnemyContext
            {
                enemyData = _enemyData,
                player = _player,
                movement = _movement,
                chaseState = _chaseState,
                attackState = _attackState,
                enemyCombat = enemycombat
            };

        }

        void Update()
        {
            _stateMachine.Update();
        }

        public void ChangeState(IEnemyState newState)
        {
            _stateMachine.ChangeState(newState);
        }

    }
}