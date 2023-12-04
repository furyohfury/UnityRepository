using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace RPGMine
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerControls _playerInput;
        private CharacterController _charController;
        private Animator _playerAnimator;
        [SerializeField, Range(0, 10)]
        private float _movespeed;
        public bool CanAttack = true;
        public bool CanMove = true;
        public float MoveSpeed
        {
            get
            {
                return _movespeed;
            }
            set
            {
                if (value > 0) _movespeed = value;
                else Debug.LogWarning("Movespeed < 0");
            }
        }
        private void Awake()
        {
            _charController = GetComponent<CharacterController>();
            _playerAnimator = GetComponent<Animator>();
            _playerInput = new();
        }
        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.PlayerMap.LightAttack.performed += OnLightAttack;
            _playerInput.PlayerMap.Interact.performed += OnInteract;
        }
        private void OnDisable()
        {
            _playerInput.PlayerMap.LightAttack.performed -= OnLightAttack;
            _playerInput.PlayerMap.Interact.performed -= OnInteract;
            _playerInput.Disable();
        }
        private void OnDestroy()
        {
            _playerInput.Dispose();
        }
        private void OnLightAttack(CallbackContext context)
        {
            _playerAnimator.SetTrigger("LightAttack");
        }
        private void OnInteract(CallbackContext context)
        {

        }
        private void FixedUpdate()
        {
            Movement();
        }
        private void Movement()
        {
            var movement = _playerInput.PlayerMap.Movement.ReadValue<Vector2>();
            if (movement == Vector2.zero || !CanMove)
            {
                _playerAnimator.SetBool("Moving", false);
                return;
            }
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y) * MoveSpeed);
            _playerAnimator.SetBool("Moving", true);
            _playerAnimator.SetFloat("ForwardMove", movement.y);
            _playerAnimator.SetFloat("SideMove", movement.x);
        }
    }
}