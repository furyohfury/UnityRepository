using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Linq;
using static UnityEngine.InputSystem.InputAction;
namespace Network
{
    public class Player : MonoBehaviourPunCallbacks, IPunObservable
    {
        public PlayerControls _controls;
        private CharacterController _charController;
        [SerializeField, Range(1, 10)]
        private float _cameraSpeed = 7;
        public int Health = 10;
        [SerializeField, Range(1, 100)]
        private float _moveSpeed = 5;
        [SerializeField, Range(0, 3)]
        private float _shootDelay = 0.5f;
        private bool _canShoot = true;
        [SerializeField]
        private TextMeshProUGUI _winningText;
        [SerializeField]
        private TextMeshProUGUI _losingText;
        private GameObject LocalPlayerInstance;
        private int _enemyHealth = 10;
        public int EnemyHealth
        {
            get { return _enemyHealth; }
            set
            {
                if (value > 0) _enemyHealth = value;
                else GameManager.Instance.PlayerHaveWon(this);
            }
        }
        #region Unity_Methods
        private void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            DontDestroyOnLoad(gameObject);
            _controls = new PlayerControls();
            _controls.Enable();
            _charController = GetComponent<CharacterController>();
            // if (_photonView.IsMine) Instantiate(_camera, transform);
            // _winningText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "WinningText");
            // _losingText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "LosingText");
        }        
        public override void OnEnable()
        {
            base.OnEnable();
            _controls.ActionMap.Fire.performed += OnFire;
            _controls.ActionMap.Quit.performed += GameManager.Instance.OnQuit;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            _controls.Disable();
            _controls.ActionMap.Fire.performed -= OnFire;
            _controls.ActionMap.Quit.performed -= GameManager.Instance.OnQuit;
        }
        private void Update()
        {
            Movement();
            CameraMovement();
        }
        #endregion
        #region Shooting
        private void OnFire(CallbackContext context)
        {
            if (!photonView.IsMine || !_canShoot) return;
            PhotonNetwork.Instantiate("Bullet", transform.position + transform.right + 2.5f * transform.forward, transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
            StartCoroutine(ShootDelaying());
        }
        private IEnumerator ShootDelaying()
        {
            _canShoot = false;
            yield return new WaitForSeconds(_shootDelay);
            _canShoot = true;
        }
        #endregion
        #region Movement
        private void CameraMovement()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
            Vector2 cameraMove = _controls.ActionMap.CameraMove.ReadValue<Vector2>();
            transform.rotation *= Quaternion.Euler(new Vector3(0, cameraMove.x * Time.deltaTime * _cameraSpeed, 0));
        }
        private void Movement()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
            float RotationInRads = (float)(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            Vector2 movement = _moveSpeed * _controls.ActionMap.Movement.ReadValue<Vector2>();
            movement = new Vector2(movement.x * (float)System.Math.Cos(RotationInRads) + movement.y * (float)System.Math.Sin(RotationInRads), -movement.x * (float)System.Math.Sin(RotationInRads) + movement.y * (float)System.Math.Cos(RotationInRads));
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y));
        }
        #endregion
        #region Damaged
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return; //todo разобраться с пулями
            if (other.gameObject.TryGetComponent(out Bullet bullet))
            {
                // Debugger.Log(other.gameObject.name);

                GameManager.Instance.PlayerIsDamaged(this, bullet);
            }
            else if (other.gameObject.TryGetComponent<Killbox>(out _))
            {
                GameManager.Instance.PlayerIsDamaged(this, null, true);
            }
        }
        public void ChangeHP(int delta)
        {
            Health += delta;
        }
        #endregion
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Health);
            }
            else
            {
                EnemyHealth = (int)stream.ReceiveNext();
            }            
        }
    }
}