using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

namespace Network
{

    public class MenuManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject _console;
        public void CreateRoom_Editor()
        {
            PhotonNetwork.CreateRoom("Rooma", new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }
        public void JoinRoom_Editor()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        public void QuitGame_Editor()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
        Application.Quit();
#endif
        }
        public void ShowHideConsole_Editor()
        {
            if (_console.activeSelf) _console.SetActive(false);
            else _console.SetActive(true);
        }
        private void Start()
        {
            Cursor.visible = true;
            PhotonNetwork.AutomaticallySyncScene = true;  
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }            
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.NickName = "Player" + Random.Range(1,50);
            // Debugger.Log("PhotonNetwork.CountOfPlayers = " + PhotonNetwork.CountOfPlayers);
            // Debugger.Log("PhotonNetwork.CountOfPlayersOnMaster = " + PhotonNetwork.CountOfPlayersOnMaster);
            // Debugger.Log("PhotonNetwork.CountOfPlayersInRooms = " + PhotonNetwork.CountOfPlayersInRooms);
        }
        
        public override void OnJoinedRoom()
        {            
            Debugger.Log("OnJoinedRoom");
            if (PhotonNetwork.IsMasterClient) //todo mb && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel("InGame");
            }
        }
        #region MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            Debugger.Log("OnConnectedToMaster");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debugger.Log("OnDisconnected() was called by PUN with reason" + cause);
        }
        #endregion
    }
}