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

        [Header("Movement Values")]
        private Vector3 _velocity;
        private float _gravitySpeed;
        private float _gravityAcceleration;
        private float _currentGravitySpeed = 1;

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
            float x = _inputController.moveInput.x;
            float z = _inputController.moveInput.y;

            Vector3 move = transform.right * x + transform.forward * z;
            _controller.Move(move * _playerStats.moveSpeed * Time.deltaTime);

            CalculateGravity();
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
    }

}