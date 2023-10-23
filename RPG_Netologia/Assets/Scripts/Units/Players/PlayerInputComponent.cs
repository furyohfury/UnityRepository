using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Units.Player
{
    public class PlayerInputComponent : UnitInputComponent
    {
        private PlayerControls _controls;
        

        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Unit.SwordAttack.performed += OnSwordAttack;
            _controls.Unit.LockTarget.performed += OnTargetLock;
            _controls.Unit.ShieldAtatck.performed += OnShieldAttack;
        }        

        private void OnSwordAttack(InputAction.CallbackContext obj)
        {
            CallOnAttackEvent();
        }
        private void OnShieldAttack(InputAction.CallbackContext obj)
        {
            CallOnShieldEvent();
        }
        private void OnTargetLock(InputAction.CallbackContext obj)
        {
            CallOnTargetEvent();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }
        private void OnDisable()
        {
            _controls.Unit.SwordAttack.performed -= OnSwordAttack;
            _controls.Unit.LockTarget.performed -= OnTargetLock;
            _controls.Disable();
        }        
        void Start()
        {
           
        }

        void Update()
        {
            var direction = _controls.Unit.Move.ReadValue<Vector2>();
            _movement = new Vector3(direction.x, 0f, direction.y);


        }
        private void OnDestroy()
        {
            _controls.Dispose();
        }
    }
}