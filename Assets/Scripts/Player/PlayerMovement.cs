using System.Collections;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(DetectCollision))]
    [RequireComponent(typeof(PlayerInputController))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        private PlayerInputController _inputController;
        private CharacterController _controller;
        private DetectCollision _detectCollision;
        private PlayerStats _playerStats;
        private Coroutine _knockbackCoroutine;
        private bool _isKnockedBack;

        [Header("Movement Values")]
        private Vector3 _velocity;
        private float _gravitySpeed;
        private float _gravityAcceleration;
        private float _currentGravitySpeed = 1;

        [Header("Knockback")]
        [SerializeField] private float _knockbackBaseForce = 6f;
        [SerializeField] private float _knockbackDuration = 0.3f;

        private void Awake()
        {
            _inputController = GetComponent<PlayerInputController>();
            _detectCollision = GetComponent<DetectCollision>();
            _controller = GetComponent<CharacterController>();
            _playerStats = GetComponent<PlayerStats>();
        }

        private void Start()
        {
            _gravitySpeed = _playerStats.gravity;
            _gravityAcceleration = _playerStats.gravityAcceleration;
        }

        private void Update()
        {
            if(GameManager.Instance.CurrentState != GameState.Playing)
                return;

            if (_isKnockedBack)
            {
                CalculateGravity();
                return;
            }

            float x = _inputController.moveInput.x;
            float z = _inputController.moveInput.y;

            Vector3 move = transform.right * x + transform.forward * z;
            _controller.Move(move * _playerStats.moveSpeed * Time.deltaTime);

            CalculateGravity();
        }

        public void Knockback(Vector3 direction, float forceMultiplier)
        {
            if (_knockbackCoroutine != null)
                StopCoroutine(_knockbackCoroutine);

            _knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction, forceMultiplier));
        }
        private void CalculateGravity()
        {
            if (_detectCollision.IsGrounded)
            {
                _currentGravitySpeed = 1;
                _velocity.y = 0;
                return;
            }
            _currentGravitySpeed += _gravityAcceleration * Time.deltaTime;
            _velocity.y += _gravitySpeed * _currentGravitySpeed * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private IEnumerator KnockbackCoroutine(Vector3 direction, float forceMultiplier)
        {
            _isKnockedBack = true;

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f)
            {
                _isKnockedBack = false;
                yield break;
            }

            Vector3 pushDirection = direction.normalized;
            float elapsed = 0f;
            var force = _knockbackBaseForce * forceMultiplier;
            while (elapsed < _knockbackDuration)
            {
                _controller.Move(pushDirection * force * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _isKnockedBack = false;
        }
    }

}