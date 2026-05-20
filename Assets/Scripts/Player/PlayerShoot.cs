using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private GameObject _playerVisual;
        [SerializeField, Range(0.1f, 20f)] private float _smoothRotationVelocity = 5f;

        [Header("Shooting Settings")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private LayerMask _shootLayer;
        [SerializeField] private VisualEffect _muzzleVFX;
        private IObjectPool<Bullet> _bulletPool;

        [Header("Audio")]
        [SerializeField] private AudioClip[] _shootsClip;

        private DetectCollision _detectCollision;
        private PlayerInputController _inputController;
        private PlayerStats _playerStats;
        private ICharacterAnimator _cAnimator;

        private bool _isShooting;
        private bool _justEndedShooting;
        private bool _smoothRotating;
        private float _nextFireTime;

        private const float MIN_ROTATION_FACTOR = 0.01f;
        private Camera _camera;

        private void Awake()
        {
            _inputController = GetComponent<PlayerInputController>();
            _detectCollision = GetComponent<DetectCollision>();
            _playerStats = GetComponent<PlayerStats>();
            _cAnimator = GetComponentInChildren<ICharacterAnimator>();

            _camera = Camera.main;

            _bulletPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(_bulletPrefab),
                actionOnGet: bullet =>
                {
                    bullet.SetParameters(_shootLayer, _playerStats.damage, true);
                    bullet.gameObject.SetActive(true);
                },
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                maxSize: 20
            );

        }

        private void OnEnable()
        {
            _inputController.OnFirePressed += StartShooting;
            _inputController.OnFireReleased += StopShooting;
        }
        private void OnDisable()
        {
            _inputController.OnFirePressed -= StartShooting;
            _inputController.OnFireReleased -= StopShooting;
        }

        void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            if (!_isShooting && _justEndedShooting)
            {
                if (_smoothRotating) return;
                if (_inputController.IsUsingGamepad)
                    StartCoroutine(SmoothRotateStick());
                else
                    StartCoroutine(SmoothRotateMouse());
                return;
            }

            if (_inputController.IsUsingGamepad)
                LookToStick();
            else
                LookToMouse();
        }

        private void LookToMouse()
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _detectCollision.groundLayer))
            {
                Vector3 lookDirection = hit.point - _playerVisual.transform.position;
                lookDirection.y = 0f;

                _playerVisual.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
        private void LookToStick()
        {
            var input = _inputController.lookInput;
            if (input.sqrMagnitude < MIN_ROTATION_FACTOR) return;

            Vector3 direction = new Vector3(input.x, 0f, input.y);
            _playerVisual.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void StartShooting()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            if (_isShooting || Time.time < _nextFireTime) return;
            StartCoroutine(ShootCoroutine());
        }
        private void StopShooting()
        {
            _isShooting = false;
        }

        private IEnumerator ShootCoroutine()
        {
            _isShooting = true;
            while (_isShooting)
            {
                if (_smoothRotating)
                    yield return new WaitUntil(() => !_smoothRotating);

                var waitTime = _nextFireTime - Time.time;
                if (waitTime > 0f)
                    yield return new WaitForSeconds(waitTime);

                var bullet = _bulletPool.Get();
                bullet.transform.SetPositionAndRotation(_shootPoint.position, _shootPoint.rotation);
                bullet.Init(_bulletPool);

                SoundFXManager.Instance.PlaySound(_shootsClip, _shootPoint.position);
                _muzzleVFX.Play();
                _cAnimator.Play("Attack");

                _nextFireTime = Time.time + _playerStats.fireCooldown;

                yield return null;
                yield return new WaitForSeconds(_playerStats.fireCooldown);

            }

            _justEndedShooting = true;
            _isShooting = false;
        }

        private IEnumerator SmoothRotateStick()
        {
            _smoothRotating = true;

            var input = _inputController.lookInput;
            Vector3 targetDirection = new Vector3(input.x, 0f, input.y);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (_playerVisual.transform.rotation != targetRotation)
            {
                input = _inputController.lookInput;
                if (input.sqrMagnitude < MIN_ROTATION_FACTOR) { yield return null; continue; }

                targetDirection = new Vector3(input.x, 0f, input.y);
                targetRotation = Quaternion.LookRotation(targetDirection);

                _playerVisual.transform.rotation = Quaternion.RotateTowards(_playerVisual.transform.rotation,
                        targetRotation, _smoothRotationVelocity);
                yield return null;
            }

            _smoothRotating = false;
            _justEndedShooting = false;
        }

        private IEnumerator SmoothRotateMouse()
        {
            _smoothRotating = true;
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 targetDirection = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit1, 100f, _detectCollision.groundLayer))
            {
                targetDirection = hit1.point - _playerVisual.transform.position;
                targetDirection.y = 0f;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            while (_playerVisual.transform.rotation != targetRotation)
            {
                ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _detectCollision.groundLayer))
                {
                    targetDirection = hit.point - _playerVisual.transform.position;
                    targetDirection.y = 0f;
                    targetRotation = Quaternion.LookRotation(targetDirection);

                    _playerVisual.transform.rotation = Quaternion.RotateTowards(_playerVisual.transform.rotation,
                        targetRotation, _smoothRotationVelocity);
                }
                yield return null;
            }

            _smoothRotating = false;
            _justEndedShooting = false;
        }
    }
}
