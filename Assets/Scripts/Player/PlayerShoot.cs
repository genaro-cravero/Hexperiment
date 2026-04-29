using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Player
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private GameObject _playerVisual;
        [SerializeField, Range(0.1f, 20f)] private float _smoothRotationVelocity = 5f;

        private DetectCollision _detectCollision;
        private PlayerInputController _inputController;

        private bool _isShooting;
        private bool _justShot;
        private bool _smoothRotating;
        private Camera _camera;
        private const float MIN_ROTATION_FACTOR = 0.01f;

        private void Awake()
        {
            _inputController = GetComponent<PlayerInputController>();
            _detectCollision = GetComponent<DetectCollision>();
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputController.OnFire += Shoot;
        }
        private void OnDisable()
        {
            _inputController.OnFire -= Shoot;
        }

        void Update()
        {
            if (_isShooting) return;
            if (_justShot)
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

        private void Shoot()
        {
            if (_isShooting) return;
            StartCoroutine(ShootCoroutine());
        }

        private IEnumerator ShootCoroutine()
        {
            _isShooting = true;
            yield return null;
            yield return new WaitForSeconds(0.5f);
            _isShooting = false;
            _justShot = true;
        }

        private IEnumerator SmoothRotateStick()
        {
            _smoothRotating = true;
            Quaternion targetRotation = new Quaternion();

            while (_playerVisual.transform.rotation != targetRotation)
            {
                var input = _inputController.lookInput;
                if (input.sqrMagnitude < MIN_ROTATION_FACTOR) { yield return null; continue; }

                Vector3 targetDirection = new Vector3(input.x, 0f, input.y);
                targetRotation = Quaternion.LookRotation(targetDirection);

                _playerVisual.transform.rotation = Quaternion.RotateTowards(_playerVisual.transform.rotation,
                        targetRotation, _smoothRotationVelocity);
                yield return null;
            }

            _smoothRotating = false;
            _justShot = false;
        }

        private IEnumerator SmoothRotateMouse()
        {
            _smoothRotating = true;
            Quaternion targetRotation = new Quaternion();

            while (_playerVisual.transform.rotation != targetRotation)
            {
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _detectCollision.groundLayer))
                {
                    Vector3 targetDirection = hit.point - _playerVisual.transform.position;
                    targetDirection.y = 0f;
                    targetRotation = Quaternion.LookRotation(targetDirection);
                    
                    _playerVisual.transform.rotation = Quaternion.RotateTowards(_playerVisual.transform.rotation,
                        targetRotation, _smoothRotationVelocity);
                }
                yield return null;
            }

            _smoothRotating = false;
            _justShot = false;
        }
    }
}
