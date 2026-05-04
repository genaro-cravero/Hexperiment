using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 targetPosition)
        {
            _agent.isStopped = false;
            _agent.SetDestination(targetPosition);
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
