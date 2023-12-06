using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGMine
{
    public class PlayerUnit : Unit
    {
        private PlayerInput _playerInput;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _playerInput.OnLightAttack += LightAttack;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _playerInput.OnLightAttack -= LightAttack;
        }
        private override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected override Vector2 GetMovement()
        {
            return _playerInput.ReadMovement();
        }
        protected override void Dead()
        {
            _playerInput.enabled = false;
            base.Dead();
            //todo interface appearance save/load
        }
}