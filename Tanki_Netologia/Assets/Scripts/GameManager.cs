using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Tanks
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField]
        private GameObject _enemyPrefab;
        [SerializeField, Range(0, 10)]
        private int _numberOfEnemies = 3;
        [SerializeField, Range(0, 30f)]
        private float _enemySpawnInterval = 5f;
        private PlayerController _player;
        private List<Bullet> _bullets = new();
        private List<EnemyController> _enemies = new();
        private EnemySpawn[] _enemySpawns;
        private bool _isSpawningCompleted = false;

        #region Unity_Methods
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
            _player = FindObjectOfType<PlayerController>();
        }
        private void Start()
        {
            _enemySpawns = FindObjectsOfType<EnemySpawn>();
            // Начало спауна противников
            StartCoroutine(SpawnEnemies());
        }
        private IEnumerator SpawnEnemies()
        {
            for (int i = 0; i < _numberOfEnemies; i++)
            {
                // Выбор рандомной точки спауна
                EnemySpawn randomSpawnPoint = _enemySpawns[UnityEngine.Random.Range(0, _enemySpawns.Length)];
                if (randomSpawnPoint.isBusy) randomSpawnPoint = _enemySpawns.First(sp => !sp.isBusy);
                // Спаун противника
                GameObject newEnemy = Instantiate(_enemyPrefab, randomSpawnPoint.transform.position, Quaternion.identity);
                _enemies.Add(newEnemy.GetComponent<EnemyController>());
                yield return new WaitForSeconds(_enemySpawnInterval);
            }
            _isSpawningCompleted = true;
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
            return (Vector2)_player.gameObject.transform.position;
        }
        #endregion
        #region Private_Methods
        private void BulletHit(object sender, Bullet.BulletEventArgs e)
        {
            Bullet bullet = (Bullet)sender;
            if (e.hitObj == BulletHits.Bullet || e.hitObj == BulletHits.Wall)
            {
                DestroyBullet(bullet);
            }
            else if (e.hitObj == BulletHits.Tank)
            {
                Tank collidedTank = e.CollidedTank;
                if (bullet.bulletData.Owner.GetType() != collidedTank.GetType())
                {
                    ChangeTankHP(collidedTank, bullet);
                }
                DestroyBullet(bullet);
            }
            else if (e.hitObj == BulletHits.DestructibleWall)
            {
                e.Tilemap.SetTile(e.Tilemap.WorldToCell(e.HitPoint), null);
                DestroyBullet(bullet);
            }
        }
        private void ChangeTankHP(Tank tank, Bullet bullet)
        {
            // Player hit
            if (tank as PlayerController != null && !((PlayerController)tank).Invulnerable)
            {
                tank.ChangeHealth(-bullet.bulletData.Damage);
                _player.PlayerGotDamaged();
            }
            // Enemy hit
            else
            {
                if (tank.Health <= 0)
                {
                    _enemies.Remove(tank as EnemyController);
                    Destroy(tank.gameObject);
                    if (_enemies.Count <= 0 && _isSpawningCompleted) _player.OnWinning();
                }
            }
        }
        private void DestroyBullet(Bullet bullet)
        {
            bullet.OnBulletHit -= BulletHit;
            _bullets.Remove(bullet);
            Destroy(bullet.gameObject);
        }
        private void BulletMovement()
        {
            foreach (Bullet bullet in _bullets.ToList())
            {
                bullet.transform.position += bullet.bulletData.Speed * Time.deltaTime * bullet.transform.right;
            }
        }
        #endregion
    }
}