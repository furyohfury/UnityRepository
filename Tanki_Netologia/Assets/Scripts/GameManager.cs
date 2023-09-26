using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public List<Bullet> _bullets = new();
        public List<EnemyController> _enemies = new();
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
            if (tank.Health > 0) return;
            // Подбит игрок
            if (tank.GetType() == typeof(PlayerController))
            {
                StartCoroutine(PlayerGotDamaged());
            }
            // Подбит враг
            else if (tank.GetType() == typeof(EnemyController))
            {
                _enemies.Remove(tank as EnemyController);
                Destroy(tank.gameObject);
            }
        }
        private IEnumerator PlayerGotDamaged()
        {
            yield return null;
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