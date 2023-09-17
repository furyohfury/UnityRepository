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
        [SerializeField]
        private GameObject _loadingYvi;
        [SerializeField]
        private GameObject _loadingText;
        [SerializeField]
        private GameObject _losingText;
        [SerializeField]
        private GameObject _winningText;
        public static GameManager Instance;
        #region Unity_Methods
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }
        private void Start()
        {
            Debugger.Onstart();
            StartCoroutine(SpawnPlayer());
        }
        public override void OnDisable()
        {
            base.OnDisable();
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
            GameObject _playerGO = PhotonNetwork.Instantiate("Player1", spawnPos, Quaternion.identity, 0);
            _localPlayer = _playerGO.GetComponent<Player>();
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Cursor.visible = false;
#endif
        }
        #endregion
        public void OnQuit(CallbackContext context)
        {
            Debug.Log("Quit");
#if UNITY_EDITOR
            PhotonNetwork.LeaveRoom();
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            PhotonNetwork.LeaveRoom();
#endif
        }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #region Photon_Shit
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            Debugger.Log(newPlayer + " has entered the room");
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debugger.Log(otherPlayer + " left room");
        }
        #endregion
    }
}