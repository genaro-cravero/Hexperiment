using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputAction;
        private InputActionMap _gameplayActionMap;
        private InputActionMap _UIActionMap;
        public Vector2 moveInput { get; private set; }
        public Vector2 lookInput { get; private set; }

        public bool IsUsingGamepad => Gamepad.current != null;
        public Action OnFire;

        private void OnEnable()
        {
            _gameplayActionMap = _inputAction.FindActionMap("Gameplay");
            _UIActionMap = _inputAction.FindActionMap("UI");
            _gameplayActionMap.Enable();
            _UIActionMap.Disable();

            //Suscribe to input events
            _gameplayActionMap["Move"].performed += OnMove;
            _gameplayActionMap["Move"].canceled += OnStopMovement;

            _gameplayActionMap["Look"].performed += OnLook;
            _gameplayActionMap["Fire"].performed += OnFirePressed;
        }

        private void OnDisable()
        {
            //Unsuscribe to input events
            _gameplayActionMap["Move"].performed -= OnMove;
            _gameplayActionMap["Move"].canceled -= OnStopMovement;
            _gameplayActionMap["Look"].performed -= OnLook;
            _gameplayActionMap["Fire"].performed -= OnFirePressed;

            _gameplayActionMap.Disable();
            _UIActionMap.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        private void OnStopMovement(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }
        private void OnFirePressed(InputAction.CallbackContext context)
        {
            OnFire?.Invoke();
        }

    }
}
