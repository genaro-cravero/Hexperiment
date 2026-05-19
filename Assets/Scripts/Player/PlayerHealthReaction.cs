using Health;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

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

        [Header("Hurt Blink")]
        [SerializeField] private Material _blinkMat;
        [SerializeField, Range(0.01f, 1f)] private float _timeBlinking = 0.05f;
        private List<Renderer> _renderers = new List<Renderer>();
        private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
        private bool _isBlinkging;

        [SerializeField] private CinemachineImpulseSource _impulseSourceHurt;
        [SerializeField] private CinemachineImpulseSource _impulseSourceDeath;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _movement = GetComponent<PlayerMovement>();

            if (_brokenBodyGO)
                _brokenBodies = _brokenBodyGO.GetComponentsInChildren<Rigidbody>(true);

            if (_visualGO)
            {
                Renderer[] renderers = _visualGO.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    if (!renderer) continue;
                    _renderers.Add(renderer);
                    _originalMaterials[renderer] = renderer.materials;
                }
            }
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
            //Todo SFX 
            _impulseSourceHurt.GenerateImpulse(0.35f);

            if (_isBlinkging) return;
            StartCoroutine(BlinkEffect());
        }

        private void HandlePush(float damage, GameObject source)
        {
            Vector3 direction = transform.position - source.transform.position;
            _movement.Knockback(direction, damage);
        }

        private void HandleDie()
        {
            _impulseSourceDeath.GenerateImpulse(1);

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

        private IEnumerator BlinkEffect()
        {
            _isBlinkging = true;
            foreach (var renderer in _renderers)
            {
                if (!renderer) continue;
                Material[] mats = new Material[renderer.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = _blinkMat;
                }
                renderer.materials = mats;
            }
            yield return new WaitForSeconds(_timeBlinking);
            foreach (var renderer in _renderers)
            {
                if (!renderer) continue;
                renderer.materials = _originalMaterials[renderer];
            }
            _isBlinkging = false;
        }
    }
}
