using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Network
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private float _spawnBorders = 40f;
        private Player _localPlayer;
        private GameObject _loadingYvi;
        private GameObject _loadingText;
        private GameObject _losingText;
        private GameObject _winningText;
        public static GameManager Instance;
        private Coroutine _losingCoroutine;
        private Coroutine _winningCoroutine;
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
            _winningText = GameObject.Find("WinningText");
            _losingText = GameObject.Find("LosingText");
            Debugger.Onstart();
            StartCoroutine(SpawnPlayer());
        }
        #endregion

        #region Bullets       

        #endregion
        #region Player
        private IEnumerator SpawnPlayer()
        {
            yield return new WaitForSeconds(3f);
            _loadingYvi.SetActive(false);
            _loadingText.SetActive(false);
            Vector3 spawnPos = new Vector3(Random.Range(-_spawnBorders, _spawnBorders), 2, Random.Range(-_spawnBorders, _spawnBorders));
            GameObject _playerGO = PhotonNetwork.Instantiate("Player1", spawnPos, Quaternion.identity);
            _localPlayer = _playerGO.GetComponent<Player>();
            Instantiate(Resources.Load("Camera"), _playerGO.transform);
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Cursor.visible = false;
#endif
        }
        public void PlayerIsDamaged(Player player, Bullet bullet, bool isKillbox = false)
        {
            if (isKillbox) player.ChangeHP(-player.Health);
            else
            {
                player.ChangeHP(-bullet.BulletDamage);
            }
            if (player.Health <= 0 && player == _localPlayer && _losingCoroutine == null)
            {
               _losingCoroutine = StartCoroutine(Losing(player));
            }
        }
        private IEnumerator Losing(Player player)
        {
            Debugger.Log("Entered losing coroutine");
            player._controls.Disable();
            _losingText.SetActive(true);
            yield return new WaitForSeconds(3f);
            LeaveRoom();
        }
        public void PlayerHaveWon(Player player)
        {
            if (_winningCoroutine == null && player == _localPlayer)
            {                
                _winningCoroutine = StartCoroutine(Won(player));
            }
        }
        private IEnumerator Won(Player player)
        {
            Debugger.Log("Entered winning coroutine");
            player._controls.Disable();
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
        #region Photon_Shit
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