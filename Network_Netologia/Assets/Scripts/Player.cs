using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Network
{
    public class Player : MonoBehaviourPunCallbacks
    {
        private PlayerControls _controls;
        private CharacterController _charController;
        [field: SerializeField]
        public Camera _camera { get; private set; }
        [SerializeField, Range(1, 10)]
        private float _cameraSpeed = 7;
        [field: SerializeField]
        public int Health { get; private set; } = 10;
        [SerializeField, Range(1, 100)]
        private float _moveSpeed = 5;
        [SerializeField, Range(0, 3)]
        private float _shootDelay = 0.5f;
        [SerializeField]
        private PhotonView _photonView;
        private bool _canShoot = true;
        private GameManager _gameManager;
        public static GameObject LocalPlayerInstance;
        #region Unity_Methods
        private void Awake()
        {
            if (_photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            DontDestroyOnLoad(gameObject);
            _gameManager = FindObjectOfType<GameManager>();
            _controls = new PlayerControls();
            _controls.Enable();
            _charController = GetComponent<CharacterController>();
            if (_photonView.IsMine) Instantiate(_camera, transform);
        }
        public override void OnEnable()
        {
            base.OnEnable();
            _controls.ActionMap.Fire.performed += OnFire;
            _controls.ActionMap.Quit.performed += _gameManager.OnQuit;
        }
        private void Update()
        {
            Movement();
            CameraMovement();
        }
        public override void OnDisable()
        {
            base.OnDisable();
            _controls.ActionMap.Fire.performed -= OnFire;
            _controls.ActionMap.Quit.performed -= _gameManager.OnQuit;
        }
        private void OnDestroy()
        {
            _controls.Disable();
        }
        #endregion
        #region Shooting
        private void OnFire(CallbackContext context)
        {
            if (!_photonView.IsMine || !_canShoot) return;
            // Debugger.Log("FIRE");
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", transform.position + transform.right + 2.5f * transform.forward, transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (bulletComp == null) Debug.Log("Bullet null");
            _gameManager.AddBullet(bulletComp);
            StartCoroutine(ShootDelaying());
        }
        private IEnumerator ShootDelaying()
        {
            _canShoot = false;
            float time = 0;
            while (time < _shootDelay)
            {
                time += Time.deltaTime;
                yield return null;
            }
            _canShoot = true;
        }
        #endregion
        #region Movement
        private void CameraMovement()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            Vector2 cameraMove = _controls.ActionMap.CameraMove.ReadValue<Vector2>();
            transform.rotation *= Quaternion.Euler(new Vector3(0, cameraMove.x * Time.deltaTime * _cameraSpeed, 0));
        }
        private void Movement()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;

            float RotationInRads = (float)(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            Vector2 movement = _moveSpeed * _controls.ActionMap.Movement.ReadValue<Vector2>();
            movement = new Vector2(movement.x * (float)System.Math.Cos(RotationInRads) + movement.y * (float)System.Math.Sin(RotationInRads), -movement.x * (float)System.Math.Sin(RotationInRads) + movement.y * (float)System.Math.Cos(RotationInRads));
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y));

        }
        #endregion
        #region Damaged
        private void OnTriggerEnter(Collider other)
        {
            Debugger.Log("Trigger");
            if (other.gameObject.TryGetComponent(out Bullet bullet) && _photonView.IsMine)
            {
                OnDamaged?.Invoke(this, bullet);
            }
            else if (other.gameObject.TryGetComponent<Killbox>(out _) && _photonView.IsMine)
            {
                OnDamaged?.Invoke(this, null, true);
            }
        }
        public void ChangeHP(int delta)
        {
            Health += delta;
        }
        #endregion
        public delegate void Damaged(Player player, Bullet bullet, bool isKillbox = false);
        public event Damaged OnDamaged;
    }
}