using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGMine
{
    public class PlayerUnit : Unit
    {        
        private PlayerInput _playerInput;

        protected override void Awake()
        {
            base.Awake();
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
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        protected override Vector2 GetMovement()
        {
            return _playerInput.ReadMovement();
        }
        protected override void Dead()
        {
            base.Dead();
            StartCoroutine(PlayerDead());
        }
        private IEnumerator PlayerDead()
        {
            _playerInput.enabled = false;
            //todo 
            yield return null;
        }
    }
}