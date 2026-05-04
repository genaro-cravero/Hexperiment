using UnityEngine;

namespace Enemy
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        private const float RANGE_THRESHOLD = 0.5f;
        private bool _isInAttackRange;

        private Enemy _enemy;
        private Transform _player => _enemy.Context.player;
        private float _attackRange => _enemy.Context.enemyData.attackDistance;


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

        public bool IsPlayerInSight()
        {
            Vector3 directionToPlayer = _player.position - transform.position;
            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            Ray ray = new Ray(transform.position, directionToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, distanceToPlayer, _layerMask))
            {
                return hit.transform == _player;
            }
            return false;
        }
    }
}
