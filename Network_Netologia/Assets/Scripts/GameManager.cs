using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        // public static GameManager Manager;
        private List<Bullet> _bullets = new();
        [SerializeField, Range(1, 10)]
        private float _bulletSpeed = 3;
        [SerializeField, Range(1, 10)]
        private int _bulletsDamage = 2;
        private float _bulletsLifeTime = 5f;

        private void Awake()
        {

        }
        private void Start()
        {
            Vector3 spawnPos = new Vector3(Random.Range(-50f, 50f), 2, Random.Range(-50f, 50f));
            // string playerName = Resources.Load<GameObject>("Prefabs/Player").name;
            GameObject player = PhotonNetwork.Instantiate("Player1", spawnPos, Quaternion.identity,0);
            // Player playerComp = player.GetComponent<Player>();
            // playerComp._camera.targetDisplay = 1;
        }
        public override void OnEnable()
        {
            base.OnEnable();
            //todo подписка на ondamaged
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }
        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
            StartCoroutine(BulletDying(bullet));
        }
        private IEnumerator BulletDying(Bullet bullet)
        {
            yield return new WaitForSeconds(_bulletsLifeTime);
            if (bullet.gameObject != null) PhotonNetwork.Destroy(bullet.gameObject);
        }
        private void Update()
        {
            BulletsMovement();
            // Debugger.Log("PhotonNetwork.CountOfPlayers = " + PhotonNetwork.CountOfPlayers);
            // Debugger.Log("PhotonNetwork.CountOfPlayersOnMaster = " + PhotonNetwork.CountOfPlayersOnMaster);
            // Debugger.Log("PhotonNetwork.CountOfPlayersInRooms = " + PhotonNetwork.CountOfPlayersInRooms);
        }
        private void BulletsMovement()
        {
            foreach (Bullet bullet in _bullets)
            {
                if (bullet == null || !bullet.photonView.IsMine) continue;
                bullet.transform.position += _bulletSpeed * Time.deltaTime * bullet.transform.up;
            }
        }
        private void PlayerDamaged(Player player, Bullet bullet, bool isKillbox)
        {
            if (isKillbox) player.ChangeHP(-player.Health);
            else
            {
                _bullets.Remove(bullet);
                Destroy(bullet.gameObject);
                player.ChangeHP(-_bulletsDamage);
            }
            if (player.Health <= 0)
            {
                Winning(player);
            }
        }
        private void Winning(Player player)
        {
            //todo
        }
        #region Photon_Shit
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            
        }
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            Debugger.Log(otherPlayer + " left room");
        }
        #endregion
    }
}