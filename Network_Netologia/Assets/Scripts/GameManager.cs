using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Manager;
        private List<Bullet> _bullets = new();
        [SerializeField, Range(1, 10)]
        private float _bulletSpeed = 3;
        [SerializeField, Range(1, 10)]
        private float _bulletsDamage = 2;
        
        private void Awake()
        {
            if (Manager == null) Manager = this;
            else Destroy(this);
        }
        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }
        private void Update()
        {
            BulletsMovement();
        }
        private void BulletsMovement()
        {
            foreach (Bullet bullet in _bullets)
            {
                bullet.transform.position += _bulletSpeed * Time.deltaTime * bullet.transform.up;
            }
        }
    }
}