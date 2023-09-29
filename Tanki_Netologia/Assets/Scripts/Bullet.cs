using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{
    public class Bullet : MonoBehaviour
    {
        public BulletData bulletData;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Tank tank))
            {
                OnBulletHit?.Invoke(this, new BulletEventArgs(tank));
            }            
            else if (collision.gameObject.TryGetComponent(out Wall wall))
            {
                // Destroy(gameObject);
                OnBulletHit?.Invoke(this, new BulletEventArgs(wall));
            }
            else if (collision.gameObject.TryGetComponent(out DestructibleWall dwall))
            {
                // Destroy(collision.gameObject);
                // Destroy(gameObject);
                OnBulletHit?.Invoke(this, new BulletEventArgs(dwall));
            }
        }
        public class BulletEventArgs : EventArgs
        {
            public Tank CollidedTank;
            public DestructibleWall DestructedWall;
            public BulletEventArgs(Tank tank)
            {
                CollidedTank = tank;
            }
            public BulletEventArgs(Wall wall) { }
            public BulletEventArgs(DestructibleWall desWall)
            {
                DestructedWall = desWall;
            }
        }
        public event EventHandler<BulletEventArgs> OnBulletHit;
    }
}