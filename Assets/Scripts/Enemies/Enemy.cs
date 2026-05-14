using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyCombat))]
    [RequireComponent(typeof(EnemyMovement))]
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

            var enemycombat = GetComponent<EnemyCombat>();
            var cAnimator = GetComponentInChildren<ICharacterAnimator>();

            _context = new EnemyContext
            {
                enemyData = _enemyData,
                player = _player,
                movement = _movement,
                cAnimator = cAnimator,
                enemyCombat = enemycombat
            };

            _attackState = new EnemyAttackState(this);
            _chaseState = new EnemyChaseState(this);

            _context.attackState = _attackState;
            _context.chaseState = _chaseState;

            ChangeState(_chaseState);

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