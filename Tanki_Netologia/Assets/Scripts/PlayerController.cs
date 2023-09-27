using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEditor;

namespace Tanks
{
    public class PlayerController : Tank
    {
        private PlayerControls _controls;
        private bool _canShoot = true;
        public Vector2 StartPosition {get; private set;}
        public bool Invulnerable {get; private set;} = false;
        public bool testblink = false;
        [SerializeField]
        private Animation _blinkingAnimation; 
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
            StartPosition = transform.position;
            _blinkingAnimation = GetComponent<Animation>();
        }
        private void Update()
        {
            if (testblink)
            {
                StartCoroutine(Blinking(3));
                testblink = false;
            }
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
        #region Damaged
        public void PlayerGotDamaged(float invulTime)
        {
            if (Health > 0)
            {
                transform.position = StartPosition;
                StartCoroutine(Blinking(invulTime));
            }
            else 
            {
#if UNITY_EDITOR
                EditorApplication.isPaused = true;
#endif
            }
        }
        private IEnumerator Blinking(float invulTime)
        {
            Invulnerable = true;
            _blinkingAnimation.enabled = true;
            yield return new WaitForSeconds(invulTime);
            Invulnerable = false;
            _blinkingAnimation.enabled = false;
        }
        #endregion
    }
}