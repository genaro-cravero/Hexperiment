using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterData _playerData;
        private CharacterController _controller;
        private PlayerInputController _inputController;

        [Header("Movement Values")]
        private float _moveSpeed;
        private float _gravitySpeed;
        private Vector3 _velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputController = GetComponent<PlayerInputController>();

            _moveSpeed = _playerData.moveSpeed;
            _gravitySpeed = _playerData.gravity;
        }

        private void Update()
        {
            float x = _inputController.moveInput.x;
            float z = _inputController.moveInput.y;

            Vector3 move = transform.right * x + transform.forward * z;
            _controller.Move(move * _moveSpeed * Time.deltaTime);

            CalculateGravity();
        }
        private void CalculateGravity()
        {
            // Gravity
            _velocity.y += _gravitySpeed * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }

}