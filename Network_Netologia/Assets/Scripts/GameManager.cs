using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Network
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField, Range(1, 10)]
        private float _bulletSpeed = 3;
        [SerializeField, Range(1, 10)]
        private int _bulletsDamage = 2;
        private Dictionary<Bullet, Coroutine> _bullets = new();
        private float _bulletsLifeTime = 5f;
        private Player _localPlayer;
        // UI
        [SerializeField]
        private GameObject _loadingYvi;
        [SerializeField]
        private GameObject _loadingText;
        [SerializeField]
        private GameObject _losingText;
        [SerializeField]
        private GameObject _winningText;
        #region Unity_Methods
        private void Start()
        {
            Debugger.Onstart();
            StartCoroutine(SpawnPlayer());
        }
        public override void OnDisable()
        {
            base.OnDisable();
            if (_localPlayer != null) _localPlayer.OnDamaged -= PlayerDamaged;
        }
        private void Update()
        {
            BulletsMovement();
        }
        #endregion

        #region Bullets
        public void AddBullet(Bullet bullet)
        {
            if (!bullet.photonView.IsMine) return;
            Coroutine bulletCor = StartCoroutine(BulletDying(bullet));
            _bullets.Add(bullet, bulletCor);
        }
        private IEnumerator BulletDying(Bullet bullet)
        {
            yield return new WaitForSeconds(_bulletsLifeTime);
            if (bullet.gameObject != null || bullet.photonView.IsMine)
            {
                DestroyBullet(bullet);
            }
        }
        private void BulletsMovement()
        {
            foreach (Bullet bullet in _bullets.Keys)
            {
                if (bullet == null || !bullet.photonView.IsMine) continue;
                bullet.transform.position += _bulletSpeed * Time.deltaTime * bullet.transform.up;
            }
        }
        private void DestroyBullet(Bullet bullet)
        {
            //todo пули то в чужих словарях
            StopCoroutine(_bullets[bullet]);
            _bullets.Remove(bullet);
            PhotonNetwork.Destroy(bullet.gameObject);
        }
        #endregion
        #region Player
        private IEnumerator SpawnPlayer()
        {
            yield return new WaitForSeconds(3f);
            _loadingYvi.SetActive(false);
            _loadingText.SetActive(false);
            Vector3 spawnPos = new Vector3(Random.Range(-50f, 50f), 2, Random.Range(-50f, 50f));
            GameObject _playerGO = PhotonNetwork.Instantiate("Player1", spawnPos, Quaternion.identity, 0);
            _localPlayer = _playerGO.GetComponent<Player>();
            _localPlayer.OnDamaged += PlayerDamaged;
            Cursor.visible = false;
        }
        private void PlayerDamaged(Player player, Bullet bullet, bool isKillbox)
        {

            if (isKillbox)
            {
                player.ChangeHP(-player.Health);
                Debugger.Log(player.gameObject.name + " DED");
            }
            else
            {
                DestroyBullet(bullet);
                player.ChangeHP(-_bulletsDamage);
            }
            Debugger.Log(player.gameObject.name + " have been damaged, his health now is " + player.Health);
            if (player.Health <= 0)
            {
                StartCoroutine(LosingWinning(player));
            }
        }
        private IEnumerator LosingWinning(Player player)
        {
            if (player == _localPlayer)
            {
                if (_localPlayer != null) _localPlayer.OnDamaged -= PlayerDamaged;
                _localPlayer.enabled = false;
                _losingText.SetActive(true);
                _loadingYvi.SetActive(true);
                yield return new WaitForSeconds(4f);
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (_localPlayer != null) _localPlayer.OnDamaged -= PlayerDamaged;
                _localPlayer.enabled = false;
                _winningText.SetActive(true);
                yield return new WaitForSeconds(4f);
                PhotonNetwork.LeaveRoom();
            }
        }
        #endregion        
        public void OnQuit(CallbackContext context)
        {
            Debug.Log("Quit");
#if UNITY_EDITOR
            PhotonNetwork.LeaveRoom();
            // PhotonNetwork.LoadLevel(0);
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN
            PhotonNetwork.LeaveRoom();
            // PhotonNetwork.LoadLevel(0);
#endif
        }
        #region Photon_Shit

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            // Transform playerTransform = _player.transform;
            // PhotonNetwork.Destroy(_player.gameObject);
            // SpawnPlayer();
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debugger.Log(otherPlayer + " left room");
        }
        #endregion
    }
}