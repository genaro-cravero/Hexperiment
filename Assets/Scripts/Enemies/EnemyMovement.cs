using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour
    {
        private Enemy _enemy;
        private NavMeshAgent _agent;
        private CharacterData _data => _enemy.Context.enemyData;
        private EnemyCombat _enemyCombat => _enemy.Context.enemyCombat;
        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _agent = GetComponent<NavMeshAgent>();

            _agent.speed = _data.moveSpeed;
            _agent.angularSpeed = _data.rotationSpeed * 360;
            _agent.stoppingDistance = _data.attackDistance;
            _agent.acceleration = _agent.angularSpeed /2;

        }

        public void MoveTo(Vector3 targetPosition)
        {
            if (_agent.isStopped)
                _agent.isStopped = false;

            if (!_enemyCombat.IsInAttackRange())
            {
                _agent.SetDestination(targetPosition);
            }
        }

        public void Stop()
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public float GetRemainingDistance()
        {
            return _agent.remainingDistance;
        }

        public bool HasReachedDestination(float threshold = 0.5f)
        {
            if (_agent.pathPending) return false;
            return _agent.remainingDistance <= threshold;
        }
    }
}
