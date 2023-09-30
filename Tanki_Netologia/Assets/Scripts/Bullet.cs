using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tanks
{
    public class Bullet : MonoBehaviour
    {
        public BulletData bulletData;
        /* private void OnTriggerEnter2D(Collider2D collision)
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
                OnBulletHit?.Invoke(this, new BulletEventArgs(dwall, collision));
            }
        } */
        private void OnCollisionEnter2D(Collision2D collision)
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
                OnBulletHit?.Invoke(this, new BulletEventArgs(dwall, collision.gameObject.GetComponent<Tilemap>(), collision.contacts[0].point, (Vector2) transform.right));
            }
        }
        public class BulletEventArgs : EventArgs
        {
            public Tank CollidedTank;
            public DestructibleWall DestructedWall;
            //public Collision2D DestructedWallCollision;
            public Tilemap Tilemap;
            public Vector2 HitPoint;
            public BulletEventArgs(Tank tank)
            {
                CollidedTank = tank;
            }
            public BulletEventArgs(Wall _) { }
            public BulletEventArgs(DestructibleWall desWall, Tilemap tilemap, Vector2 hitPoint, Vector2 delta)
            {                
                DestructedWall = desWall;
                Tilemap = tilemap;
                HitPoint = hitPoint + delta * 0.1f;
            }
        }
        public event EventHandler<BulletEventArgs> OnBulletHit;
    }
}