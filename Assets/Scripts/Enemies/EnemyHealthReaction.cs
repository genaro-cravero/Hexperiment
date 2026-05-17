using Health;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyHealthReaction : MonoBehaviour
    {
        [SerializeField] private GameObject _visualGO;
        [SerializeField] private GameObject _brokenBodyGO;

        [Header("Death Explosion")]
        [SerializeField] private float _explosionForce = 4f;
        [SerializeField] private float _explosionRadius = 2f;
        [SerializeField] private float _upwardsModifier = 0.5f;
        [SerializeField] private float _forceRandomRange = 1.5f;
        private Rigidbody[] _brokenBodies;

        private HealthComponent _health;
        private Enemy _enemy;
        private EnemyMovement _enemyMovement;
        private EnemyCombat _enemyCombat;
        private IEnemyAttack[] _enemyAttacks;
        private Collider _col;
        private NavMeshAgent _agent;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _enemy = GetComponent<Enemy>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyCombat = GetComponent<EnemyCombat>();
            _enemyAttacks = GetComponents<IEnemyAttack>();
            _col = GetComponent<Collider>();
            _agent = GetComponent<NavMeshAgent>();
            if (_brokenBodyGO)
                _brokenBodies = _brokenBodyGO.GetComponentsInChildren<Rigidbody>(true);
        }

        private void Start()
        {
            _health.OnDamage += HandleDamage;
            _health.OnDie += HandleDie;
            _health.OnPush += HandlePush;
        }

        private void OnDisable()
        {
            UnsuscribeEvents();
        }

        private void HandleDamage(float damage, GameObject source)
        {
            Vector3 pushDirection = (transform.position - source.transform.position).normalized;
        }

        private void HandlePush(float damage, GameObject source)
        {
            _enemyMovement.PushBack(-transform.forward, damage);

        }

        private void HandleDie()
        {
            UnsuscribeEvents();

            _visualGO.SetActive(false);
            _brokenBodyGO.SetActive(true);
            ExplodeBrokenBody();

            _col.enabled = false;

            if (_enemyMovement)
            {
                _enemyMovement.Stop();
                _enemyMovement.enabled = false;
            }
            if (_enemyCombat)
                _enemyCombat.enabled = false;

            if (_enemyAttacks != null)
            {
                foreach (var attack in _enemyAttacks)
                {
                    if (attack is Behaviour behaviour)
                        behaviour.enabled = false;
                }
            }

            if (_enemy)
                _enemy.enabled = false;

            _agent.enabled = false;
        }

        private void ExplodeBrokenBody()
        {
            if (_brokenBodies == null || _brokenBodies.Length == 0)
                return;

            foreach (var body in _brokenBodies)
            {
                if (!body) continue;
                float force = _explosionForce + Random.Range(-_forceRandomRange, _forceRandomRange);
                body.AddExplosionForce(Mathf.Max(0f, force), transform.position, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
            }
        }

        private void UnsuscribeEvents()
        {
            _health.OnDamage -= HandleDamage;
            _health.OnDie -= HandleDie;
            _health.OnPush -= HandlePush;
        }
    }
}
