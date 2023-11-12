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

        public SimpleHandle MeleeSetEventHandler;
        public SimpleHandle RangeSetEventHandler;        
        protected override void Awake()
        {
            base.Awake();
            _controls = new PlayerControls();
            _controls.Unit.MainAction.performed += OnMainAction;
            _controls.Unit.LockTarget.performed += OnTargetLock;
            _controls.Unit.AdditionalAction.performed += OnAdditionalAction;
            _controls.Unit.MeleeSet.performed += OnMeleeSet;
            _controls.Unit.MeleeSet.performed += OnRangeSet;
        }

        private void OnRangeSet(InputAction.CallbackContext obj)
        {

        }

        private void OnMeleeSet(InputAction.CallbackContext obj)
        {

        }

        private void OnMainAction(InputAction.CallbackContext obj)
        {
            CallSimpleHandle(nameof(MainEventHandler));
        }
        private void OnAdditionalAction(InputAction.CallbackContext obj)
        {
            CallSimpleHandle(nameof(AdditionalEventHandler));
        }
        private void OnTargetLock(InputAction.CallbackContext obj)
        {
            CallSimpleHandle(nameof(TargetEventHandler));
        }

        private void OnEnable()
        {
            _controls.Enable();
        }
        private void OnDisable()
        {
            _controls.Unit.MainAction.performed -= OnMainAction;
            _controls.Unit.LockTarget.performed -= OnTargetLock;
            _controls.Unit.AdditionalAction.performed -= OnAdditionalAction;
            _controls.Unit.MeleeSet.performed -= OnMeleeSet;
            _controls.Unit.MeleeSet.performed -= OnRangeSet;
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