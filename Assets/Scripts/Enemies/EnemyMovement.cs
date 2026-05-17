using System.Collections;
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
        private ICharacterAnimator _cAnimator => _enemy.Context.cAnimator;
        private Coroutine _pushCoroutine;
        private bool _isbeingPushed;
        [SerializeField] private bool _canBePushed = true;
        private const float STOP_DISTANCE_DIFF = 0.5f;
        void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_agent.pathPending) return;
            if (_agent.isStopped) return;

            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                _cAnimator.SetBool("IsMoving", false);
            }
        }

        private void Start()
        {
            _agent.speed = _data.moveSpeed;
            _agent.angularSpeed = _data.rotationSpeed * 360;
            _agent.stoppingDistance = _data.attackDistance - STOP_DISTANCE_DIFF;
            _agent.acceleration = _agent.angularSpeed / 2;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            if(_isbeingPushed) return;

            if (_agent.isStopped)
                _agent.isStopped = false;

            if (!_enemyCombat.IsInAttackRange() || !_enemyCombat.IsPlayerInSight())
            {
                _cAnimator.SetBool("IsMoving", true);
                _agent.SetDestination(targetPosition);
            }
        }

        public void ForceMoveTo(Vector3 targetPosition)
        {
            if (_isbeingPushed) return;
            if (_agent.isStopped)
                _agent.isStopped = false;

            _cAnimator.SetBool("IsMoving", true);
            _agent.SetDestination(targetPosition);
        }

        public void Stop()
        {
            if(!_agent.isActiveAndEnabled) return;
            _agent.isStopped = true;
            _agent.ResetPath();
        }

        public void PushBack(Vector3 direction, float damage)
        {
            if (_pushCoroutine != null)
            {
                StopCoroutine(_pushCoroutine);
            }

            _pushCoroutine = StartCoroutine(PushBackCoroutine(direction, damage));
        }

        private const float PUSHBACK_DISTANCE = 0.3f;
        private const float PUSHBACK_DURATION = 0.1f;
        private IEnumerator PushBackCoroutine(Vector3 direction, float damage)
        {
            if (!_canBePushed) yield break;
            _isbeingPushed = true;
            _agent.isStopped = true;
            _agent.ResetPath();

            float duration = PUSHBACK_DURATION;
            float distance = PUSHBACK_DISTANCE * damage;

            if (direction.sqrMagnitude < 0.001f || duration <= 0f || distance <= 0f)
            {
                _agent.isStopped = false;
                _pushCoroutine = null;
                _isbeingPushed = false;
                yield break;
            }

            Vector3 pushDirection = direction.normalized;
            float elapsed = 0f;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + pushDirection * distance;

            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = 1f - Mathf.Pow(1f - t, 2f);
                Vector3 desiredPosition = Vector3.Lerp(startPosition, targetPosition, easedT);
                Vector3 delta = desiredPosition - transform.position;
                _agent.Move(delta);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _isbeingPushed = false;
            _agent.isStopped = false;
            _pushCoroutine = null;
        }

        public void SetStoppingDistance(float dist)
        {
            _agent.stoppingDistance = dist - STOP_DISTANCE_DIFF;
        }
    }
}
