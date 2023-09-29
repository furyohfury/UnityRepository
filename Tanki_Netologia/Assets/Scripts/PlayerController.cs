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
        [SerializeField]
        private Sprite[] _blinkDirectionSprites = new Sprite[4];
        private Dictionary<Vector2, Sprite> _blinkDirectionDict;
        [SerializeField]
        private float _blinkInterval = 0.5f;
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
            _blinkDirectionDict = new() { { Vector2.up, _blinkDirectionSprites[0] }, { Vector2.down, _blinkDirectionSprites[1] }, { Vector2.left, _blinkDirectionSprites[2] }, { Vector2.right, _blinkDirectionSprites[3] } };
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
        public void SetInvulnerability(bool isInvulnerable)
        {
            Invulnerable = isInvulnerable;
        }
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
                //todo lose
            }
        }
        private IEnumerator Blinking(float invulTime)
        {
            float time = 0;
            float blinkTime = 0;
            Invulnerable = true;
            _directionDict = _blinkDirectionDict;
            _spriteRenderer.sprite = _directionDict[_direction];
            while (time < invulTime)
            {                                                                             
                time += Time.deltaTime;
                blinkTime += Time.deltaTime;
                if (blinkTime > _blinkInterval)
                {
                    _directionDict = _directionDict == _defaultDirectionDict ? _blinkDirectionDict : _defaultDirectionDict;
                    _spriteRenderer.sprite = _directionDict[_direction];
                    blinkTime = 0;                    
                }
                yield return null;
            }
            _directionDict = _defaultDirectionDict;
            _spriteRenderer.sprite = _directionDict[_direction];
            Invulnerable = false;
        }
        #endregion
    }
}