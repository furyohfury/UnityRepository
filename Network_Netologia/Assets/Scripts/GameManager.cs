using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;
using TMPro;

namespace Network
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private float _spawnBorders = 40f;
        private Player _localPlayer;
        public GameObject _loadingYvi;
        public GameObject _loadingText;
        public GameObject _losingText;
        public GameObject _winningText;
        public static GameManager Instance;
        public Coroutine _losingCoroutine;
        public Coroutine _winningCoroutine;
        #region Unity_Methods
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
        private void Start()
        {
            _loadingYvi = GameObject.Find("LoadingYvi");
            _loadingText = GameObject.Find("LoadingText");
            _winningText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "WinningText").gameObject;
            _losingText = FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "LosingText").gameObject;
            Debugger.Onstart();
            StartCoroutine(SpawnPlayer());
        }
        #endregion
        #region Player
        private IEnumerator SpawnPlayer()
        {
            yield return new WaitForSeconds(3f);
            _loadingYvi.SetActive(false);
            _loadingText.SetActive(false);
            Vector3 spawnPos = new Vector3(Random.Range(-_spawnBorders, _spawnBorders), 2, Random.Range(-_spawnBorders, _spawnBorders));
            GameObject playerGO = PhotonNetwork.Instantiate("Player1", spawnPos, Quaternion.identity);

            _localPlayer = playerGO.GetComponent<Player>();
            playerGO.name = PhotonNetwork.NickName;
            Instantiate(Resources.Load("Camera"), playerGO.transform);
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Cursor.visible = false;
#endif
        }
        public void PlayerIsDamaged(Bullet bullet, bool isKillbox = false)
        {
            if (isKillbox) _localPlayer.ChangeHP(-_localPlayer.Health);
            else if (!bullet.Hit)
            {
                _localPlayer.ChangeHP(-bullet.BulletDamage);
                bullet.Hit = true;
                Debugger.Log(_localPlayer + " got damaged and has " + _localPlayer.Health);
            }
            if (_localPlayer.Health <= 0 && _losingCoroutine == null)
            {
                _losingCoroutine = StartCoroutine(Losing());
            }
        }
        private IEnumerator Losing()
        {
            _localPlayer._controls.Disable();
            _losingText.SetActive(true);
            yield return new WaitForSeconds(3f);
            LeaveRoom();
        }
        public void PlayerHaveWon()
        {
            if (_winningCoroutine == null)
            {
                _winningCoroutine = StartCoroutine(Won());
            }
        }
        private IEnumerator Won()
        {
            _localPlayer._controls.Disable();
            _winningText.SetActive(true);
            yield return new WaitForSeconds(3f);
            LeaveRoom();
        }
        #endregion
        #region Public_Methods
        public void OnQuit(CallbackContext context)
        {
            Debug.Log("Quit");
            LeaveRoom();
        }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion
        #region Photon_Stuff
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debugger.Log(newPlayer.NickName + " has entered the room");
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debugger.Log(otherPlayer.NickName + " left room");
        }
        #endregion
    }
}