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
        }
        public class BulletEventArgs : EventArgs
        {
            public Tank collidedTank;
            public BulletEventArgs(Tank tank)
            {
                collidedTank = tank;
            }
        }
        public event EventHandler<BulletEventArgs> OnBulletHit;
    }
}