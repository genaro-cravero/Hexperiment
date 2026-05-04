using UnityEngine;

namespace Enemy
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private float _attackRange = 10f;
        private const float RANGE_THRESHOLD = 0.5f;
        private bool _isInAttackRange;

        private Enemy _enemy;
        private Transform _player => _enemy.Context.player;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();    
        }

        public bool IsInAttackRange()
        {
            var addedThreshold = _isInAttackRange ? RANGE_THRESHOLD : 0;
            float distance = Vector3.Distance(transform.position, _player.position);
            return distance <= _attackRange + addedThreshold;
        }
    }
}
