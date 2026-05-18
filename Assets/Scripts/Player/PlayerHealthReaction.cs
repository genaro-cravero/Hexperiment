using Enemy;
using Health;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerHealthReaction : MonoBehaviour
    {
        [SerializeField] private GameObject _visualGO;
        [SerializeField] private GameObject _brokenBodyGO;

        [Header("Death Explosion")]
        [SerializeField] private float _explosionForce = 100f;
        [SerializeField] private float _explosionRadius = 10f;
        [SerializeField] private float _upwardsModifier = 0.5f;
        private Rigidbody[] _brokenBodies;
        private WaitForSeconds _waitAfterDeath = new WaitForSeconds(2.5f);

        private HealthComponent _health;
        private PlayerMovement _movement;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _movement = GetComponent<PlayerMovement>();

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
            //VFX and SFX for player damage
        }

        private void HandlePush(float damage, GameObject source)
        {
            Vector3 direction = transform.position - source.transform.position;
            _movement.Knockback(direction, damage);
        }

        private void HandleDie()
        {
            UnsuscribeEvents();
            GameManager.Instance.SetCurrentState(GameState.Pause);

            _visualGO.SetActive(false);
            ExplodeBrokenBody();

            StartCoroutine(DeathExplosion());
        }

        private IEnumerator DeathExplosion()
        {
            _brokenBodyGO.SetActive(true);
            ExplodeBrokenBody();
            yield return _waitAfterDeath;
            GameManager.Instance.LoseGame();
        }

        private void ExplodeBrokenBody()
        {
            if (_brokenBodies == null || _brokenBodies.Length == 0)
                return;

            foreach (var body in _brokenBodies)
            {
                if (!body) continue;
                body.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
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
