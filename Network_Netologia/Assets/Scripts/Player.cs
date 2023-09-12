using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using static Network.GameManager;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Network {
    public class Player : MonoBehaviourPunCallbacks
    {
        private PlayerControls _controls;
        private CharacterController _charController;
        [field:SerializeField]
        public Camera _camera { get; private set; }        
        [SerializeField, Range(1, 10)]
        private float _cameraSpeed = 7;
        [field: SerializeField]
        public int Health { get; private set; } = 10;
        [SerializeField, Range(1, 100)]
        private float _moveSpeed = 5;
        [SerializeField, Range(0, 3)]
        private float _shootDelay = 0.5f;
        private GameObject _bulletPrefab;
        private GameObject _playerPrefab;
        private bool _canShoot = true;
        private PhotonView _photonView;
        private void Awake()
        {
            _camera.GetComponent<AudioListener>().enabled = true;
            _photonView = GetComponent<PhotonView>();
            _controls = new PlayerControls();
            _controls.Enable();
            _charController = GetComponent<CharacterController>();
            // _bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
            // _playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        }
        private void Start()
        {            
            
        }
        public override void OnEnable()
        {
            base.OnEnable();
            _controls.ActionMap.Fire.performed += OnFire;
            _controls.ActionMap.Quit.performed += OnQuit;
        }
        #region Shooting
        private void OnFire(CallbackContext context)
        {
            Debug.Log("FIRE");
            if (!_canShoot) return;
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", transform.position + transform.right + 2.5f * transform.forward, transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (bulletComp == null) Debug.Log("null");

            Manager.AddBullet(bulletComp);
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
        private void OnQuit(CallbackContext context)
        {
            Debug.Log("Quit");
#if UNITY_EDITOR
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            PhotonNetwork.LeaveRoom();
            // PhotonNetwork.LoadLevel(0);
#endif
        }
        private void Update()
        {
            Movement();
            CameraMovement();
            // Debug.Log(movement);
        }
        private void OnDisable()
        {
            _controls.ActionMap.Fire.performed -= OnFire;
            _controls.ActionMap.Quit.performed -= OnQuit;
        }
        private void OnDestroy()
        {
            _controls.Disable();
        }
        private void CameraMovement()
        {
            if (_photonView.IsMine)
            {
                Vector2 cameraMove = _controls.ActionMap.CameraMove.ReadValue<Vector2>();
                transform.rotation *= Quaternion.Euler(new Vector3(0, cameraMove.x * Time.deltaTime * _cameraSpeed, 0));
            }   
            
        }
        private void Movement()
        {
            if (_photonView.IsMine)
            {
                float RotationInRads = (float)(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
                Vector2 movement = _moveSpeed * _controls.ActionMap.Movement.ReadValue<Vector2>();
                movement = new Vector2(movement.x * (float)System.Math.Cos(RotationInRads) + movement.y * (float)System.Math.Sin(RotationInRads), -movement.x * (float)System.Math.Sin(RotationInRads) + movement.y * (float)System.Math.Cos(RotationInRads));
                _charController.SimpleMove(new Vector3(movement.x, 0, movement.y));
            }            
        }
        #region Damaged
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Bullet bullet))
            {
                OnDamaged?.Invoke(this, bullet);
            }
            else if (other.gameObject.TryGetComponent<Killbox>(out _))
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