using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField]
        private GameObject _enemyPrefab;
        private PlayerController _player;
        private List<Bullet> _bullets = new();
        private List<EnemyController> _enemies = new();
        private EnemySpawn[] _enemySpawns;
        [SerializeField, Range (0, 5f)]
        private float _invulPlayerTime = 3f;
        [SerializeField, Range(0, 10)]
        private int _numberOfEnemies = 3;
        [SerializeField, Range(0, 30f)]
        private float _enemySpawnInterval = 5f;

        #region Unity_Methods
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
            /* Debug.Log(typeof(PlayerController) == typeof(Tank));
            // Debug.Log(typeof(PlayerController) == typeof(EnemyController));
            PlayerController player = FindObjectOfType<PlayerController>();
            Tank playerTank = player;
            Debug.Log(playerTank as PlayerController != null);
            Debug.Log(playerTank as EnemyController != null);
            Debug.Log(playerTank.GetType()); */

        }
        private void Start()
        {
            _player = FindObjectOfType<PlayerController>();
            _enemySpawns = FindObjectsOfType<EnemySpawn>();
            StartCoroutine(SpawnEnemies());            
        }
        private IEnumerator SpawnEnemies()
        {
            for (int i = 0; i < _numberOfEnemies; i++)
            {
                // Выбор рандомной точки спауна
                EnemySpawn randomSpawnPoint = _enemySpawns[UnityEngine.Random.Range(0, _enemySpawns.Length)];
                while (!randomSpawnPoint.isBusy)
                {
                    randomSpawnPoint = _enemySpawns[UnityEngine.Random.Range(0, _enemySpawns.Length)];
                }
                // Спаун противника
                GameObject newEnemy = Instantiate(_enemyPrefab, randomSpawnPoint.transform.position, Quaternion.identity);   
                _enemies.Add(newEnemy.GetComponent<EnemyController>());
                yield return new WaitForSeconds(_enemySpawnInterval);
            }
        }
        private void Update()
        {
            BulletMovement();
        }
        #endregion
        #region Public_Methods
        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
            bullet.OnBulletHit += BulletHit;
        }
        public Vector2 GetPlayerPosition()
        {
            return _player.transform.position;
        }
        #endregion
        #region Private_Methods
        private void BulletHit(object sender, Bullet.BulletEventArgs e)
        {
            Bullet bullet = (Bullet)sender;
            Tank collidedTank = e.collidedTank;
            if (bullet.bulletData.Owner.GetType() != collidedTank.GetType())
            {
                ChangeTankHP(collidedTank, bullet);
                _bullets.Remove(bullet);
                Destroy(bullet.gameObject);
            }

        }
        private void ChangeTankHP(Tank tank, Bullet bullet)
        {
            tank.ChangeHealth(-bullet.bulletData.Damage);
            // Player hit
            if (tank as PlayerController != null &&  !((PlayerController)tank).Invulnerable)
            {
                _player.PlayerGotDamaged(_invulPlayerTime);
            }
            // Enemy hit
            else
            {
                _enemies.Remove(tank as EnemyController);
                Destroy(tank.gameObject);
            }
        }
        private void BulletMovement()
        {
            foreach(Bullet bullet in _bullets)
            {
                bullet.transform.position += bullet.bulletData.Speed * Time.deltaTime * bullet.transform.right;
            }
        }
        #endregion
    }
}