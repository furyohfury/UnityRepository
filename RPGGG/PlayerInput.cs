using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace RPGMine
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerControls _playerControls;
        
        private void Awake()
        {            
            _playerControls = new();
        }
        private void OnEnable()
        {
            _playerControls.Enable();
            _playerControls.PlayerMap.ReadLightAttack.performed += ReadLightAttack;
            _playerControls.PlayerMap.Interact.performed += OnInteract;
        }
        private void OnDisable()
        {
            _playerControls.PlayerMap.ReadLightAttack.performed -= ReadLightAttack;
            _playerControls.PlayerMap.Interact.performed -= OnInteract;
            _playerControls.Disable();
        }
        private void OnDestroy()
        {
            _playerControls.Dispose();
        }
        private void ReadLightAttack(CallbackContext context)
        {            
            OnLightAttack?.Invoke();
        }
        private void OnInteract(CallbackContext context)
        {

        }
        private Vector2 ReadMovement()
        {
            return _playerControls.PlayerMap.Movement.ReadValue<Vector2>();
        }
        public event SimpleDelegate OnLightAttack;
    }
}