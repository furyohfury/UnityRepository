using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Tanks
{
    public class PlayerController : Tank
    {
        private PlayerControls _controls;
        private bool _canShoot = true;
        public Vector2 StartPosition {get; private set;}
        public bool Invulnerable {get; private set;} = false;
        #region Unity_Methods
        protected override void Awake()
        {
            base.Awake();
            _controls = new();
            _controls.Enable();
        }
        private void OnEnable()
        {
            _controls.PlayerMap.Shoot.performed += OnShoot;
        }
        private void Start()
        {
            StartPosition = (Vector2) transform.position;
        }
        private void Update()
        {

        }
        protected override void FixedUpdate()
        {
            Movement();
            base.FixedUpdate();
        }
        private void OnDisable()
        {
            _controls.PlayerMap.Shoot.performed -= OnShoot;
            _controls.Dispose();
        }
        #endregion
        #region 
        public void SetInvulnerability(bool isInvulnerable)
        {
            Invulnerable = isInvulnerable;
        }
        #endregion
        #region Movement
        private void Movement()
        {
            Vector2 input = _controls.PlayerMap.Movement.ReadValue<Vector2>();
            if (input.x * input.y != 0) return;
            _rigidBody.velocity = input;
        }
        #endregion
        #region Shooting
        private void OnShoot(CallbackContext context)
        {
            if (_canShoot)
            {
                Shoot();
                StartCoroutine(CanShootCount());
            }
        }
        private IEnumerator CanShootCount()
        {
            _canShoot = false;
            yield return new WaitForSeconds(AttackSpeed);
            _canShoot = true;
        }
        #endregion
        public void PlayerGotDamaged()
        {
            if (Health > 0)
            {
                //todo blink and move to center
            }
            else 
            {
                //todo losing
            }
        }
    }
}