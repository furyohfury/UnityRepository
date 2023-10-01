using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEditor;
using TMPro;

namespace Tanks
{
    public class PlayerController : Tank
    {
        [SerializeField]
        private Sprite[] _blinkDirectionSprites = new Sprite[4];
        [SerializeField]
        private float _blinkInterval = 0.5f;
        [SerializeField]
        private TextMeshProUGUI _godmodeText;
        [SerializeField, Range(0, 5f)]
        private float _invulPlayerTime = 3f;
        [SerializeField]
        private TextMeshProUGUI _winLoseText;
        private PlayerControls _controls;
        private bool _canShoot = true;
        private Vector2 _startPosition;
        /// <summary>
        /// Словарь для спрайтов мигания
        /// </summary>
        private Dictionary<Directions, Sprite> _blinkDirectionDict;
        public bool Invulnerable { get; private set; } = false;
        private Coroutine _godmodeCoroutine;
        #region Unity_Methods
        protected override void Awake()
        {
            base.Awake();
            _controls = new();
            _controls.Enable();
            // Debug.Log(_controls.PlayerMap.Exit.interactions);
        }
        private void OnEnable()
        {
            _controls.PlayerMap.Shoot.performed += OnShoot;
            _controls.PlayerMap.Godmode.performed += OnStartGodmode;
        }
        private void Start()
        {
            _startPosition = transform.position;
            _blinkDirectionDict = new() { { Directions.Up, _blinkDirectionSprites[0] }, { Directions.Down, _blinkDirectionSprites[1] }, { Directions.Left, _blinkDirectionSprites[2] }, { Directions.Right, _blinkDirectionSprites[3] } };
        }
        protected override void FixedUpdate()
        {
            Movement();
            base.FixedUpdate();
        }
        private void OnDisable()
        {
            _controls.PlayerMap.Shoot.performed -= OnShoot;
            _controls.PlayerMap.Godmode.performed -= OnStartGodmode;
            _controls.Dispose();
        }
        #endregion
        #region Movement
        private void Movement()
        {
            Vector2 input = _controls.PlayerMap.Movement.ReadValue<Vector2>();
            if (input.x * input.y != 0) return;
            _rigidBody.velocity = input * MoveSpeed;
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
        public void PlayerGotDamaged()
        {
            if (Health > 0)
            {
                transform.position = _startPosition;
                StartCoroutine(Blinking());
            }
            else
            {
                OnLosing();
            }
        }
        private IEnumerator Blinking()
        {
            float time = 0;
            float blinkTime = 0;
            Invulnerable = true;
            _directionDict = _blinkDirectionDict;
            _spriteRenderer.sprite = _directionDict[eDirection];
            while (time < _invulPlayerTime)
            {
                time += Time.deltaTime;
                blinkTime += Time.deltaTime;
                if (blinkTime > _blinkInterval)
                {
                    _directionDict = _directionDict == _defaultDirectionDict ? _blinkDirectionDict : _defaultDirectionDict;
                    _spriteRenderer.sprite = _directionDict[eDirection];
                    blinkTime = 0;
                }
                yield return null;
            }
            _directionDict = _defaultDirectionDict;
            _spriteRenderer.sprite = _directionDict[eDirection];
            Invulnerable = false;
        }
        #endregion
        #region Godmode
        private void OnStartGodmode(CallbackContext context)
        {
            if (_godmodeCoroutine == null)
            {
                _godmodeText.text = "G for Godmode. Godmode: Enabled";
                _godmodeCoroutine = StartCoroutine(Godmode());
            }
            else
            {
                _godmodeText.text = "G for Godmode. Godmode: Disabled";
                OnStopGodmode();
            }
        }
        private IEnumerator Godmode()
        {
            float blinkTime = 0;
            Invulnerable = true;
            _directionDict = _blinkDirectionDict;
            _spriteRenderer.sprite = _directionDict[eDirection];
            while (true)
            {
                blinkTime += Time.deltaTime;
                if (blinkTime > _blinkInterval)
                {
                    _directionDict = _directionDict == _defaultDirectionDict ? _blinkDirectionDict : _defaultDirectionDict;
                    _spriteRenderer.sprite = _directionDict[eDirection];
                    blinkTime = 0;
                }
                yield return null;
            }
        }
        private void OnStopGodmode()
        {
            StopCoroutine(_godmodeCoroutine);
            _godmodeCoroutine = null;
            _directionDict = _defaultDirectionDict;
            _spriteRenderer.sprite = _directionDict[eDirection];
            Invulnerable = false;
            StartCoroutine(Blinking());
        }
        #endregion
        #region Win_Lose
        public void OnWinning()
        {
            StartCoroutine(Winning());
        }
        private IEnumerator Winning()
        {
            _controls.Disable();
            _winLoseText.text = "You win!";
            yield return new WaitForSeconds(3f);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
        private void OnLosing()
        {
            StartCoroutine(Losing());
        }
        private IEnumerator Losing()
        {
            _controls.Disable();
            _winLoseText.text = "You lost(";
            yield return new WaitForSeconds(3f);
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Application.Quit();
#endif
        }
        #endregion
    }
}