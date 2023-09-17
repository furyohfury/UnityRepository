/* using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

namespace Network
{
    public class Trash : MonoBehaviourPunCallbacks, IPunObservable
    {
        private PlayerControls _controls;
        private CharacterController _charController;
        [field: SerializeField]
        public Camera _camera { get; private set; }
        [SerializeField, Range(1, 10)]
        private float _cameraSpeed = 7;
        public int Health = 10;
        [SerializeField, Range(1, 100)]
        private float _moveSpeed = 5;
        [SerializeField, Range(0, 3)]
        private float _shootDelay = 0.5f;
        [SerializeField]
        private PhotonView _photonView;
        private bool _canShoot = true;
        public static GameObject LocalPlayerInstance;
        private bool _lost = false;
        [SerializeField]
        private bool _enemyLost = false;
        [SerializeField]
        private TextMeshProUGUI _winningText;
        [SerializeField]
        private TextMeshProUGUI _losingText;
        private Coroutine _losingCoroutine;
        private Coroutine _winningCoroutine;
        private int _enemyHealth = 10;
        #region Unity_Methods
        private void Awake()
        {
            if (_photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            DontDestroyOnLoad(gameObject);
            _controls = new PlayerControls();
            _controls.Enable();
            _charController = GetComponent<CharacterController>();
            if (_photonView.IsMine) Instantiate(_camera, transform);
            _winningText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "WinningText");
            _losingText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "LosingText");
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
            _controls.ActionMap.Fire.performed -= OnFire;
            _controls.ActionMap.Quit.performed -= GameManager.Instance.OnQuit;
        }
        private void Update()
        {
            Movement();
            CameraMovement();
            if (_enemyLost)
            {
            }
            Debugger.Log("ismine = " + photonView.IsMine);
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
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", transform.position + transform.right + 2.5f * transform.forward, transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (bulletComp == null) Debugger.Log("Bullet null");
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
            if (!_photonView.IsMine && PhotonNetwork.IsConnected || _enemyHealth <= 0) return;
            Vector2 cameraMove = _controls.ActionMap.CameraMove.ReadValue<Vector2>();
            transform.rotation *= Quaternion.Euler(new Vector3(0, cameraMove.x * Time.deltaTime * _cameraSpeed, 0));
        }
        private void Movement()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected || _enemyHealth <= 0) return;

            float RotationInRads = (float)(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            Vector2 movement = _moveSpeed * _controls.ActionMap.Movement.ReadValue<Vector2>();
            movement = new Vector2(movement.x * (float)System.Math.Cos(RotationInRads) + movement.y * (float)System.Math.Sin(RotationInRads), -movement.x * (float)System.Math.Sin(RotationInRads) + movement.y * (float)System.Math.Cos(RotationInRads));
            _charController.SimpleMove(new Vector3(movement.x, 0, movement.y));

        }
        #endregion
        #region Damaged
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Bullet bullet) && _photonView.IsMine)
            {
                PlayerDamaged(bullet);
            }
            else if (other.gameObject.TryGetComponent<Killbox>(out _) && _photonView.IsMine)
            {
                PlayerDamaged(null, true);
            }
        }
        public void ChangeHP(int delta)
        {
            Health += delta;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Health);
            }
            else
            {
                this._enemyHealth = (int)stream.ReceiveNext();
                if (this._enemyHealth <= 0 && _winningCoroutine == null) StartCoroutine(Winning());

            }
        }
        private void PlayerDamaged(Bullet bullet, bool isKillbox = false)
        {

            if (isKillbox)
            {
                ChangeHP(-Health);
            }
            else
            {
                ChangeHP(-bullet.BulletDamage);
            }
            Debugger.Log(gameObject.name + " health  = " + Health);
            if (Health <= 0 && photonView.IsMine && _losingCoroutine == null)
            {
                _losingCoroutine = StartCoroutine(Losing());
            }
        }
        private IEnumerator Losing()
        {
            Debugger.Log("Losing");
            _controls.Disable();
            _losingText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
            GameManager.Instance.LeaveRoom();


        }
        private IEnumerator Winning()
        {
            Debugger.Log("Winning");
            Debugger.Log("Photonview.isMine = " + this.photonView.IsMine);
            _controls.Disable();
            _winningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
            PhotonNetwork.LeaveRoom();
        }
        #endregion
        public delegate void Damaged(Player player, Bullet bullet, bool isKillbox = false);
        public event Damaged OnDamaged;
        public delegate void Won(Player player);
        public event Won OnWon;
    }
} */