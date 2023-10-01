using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tanks
{
    public class Bullet : MonoBehaviour
    {
        public BulletData bulletData;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Bullet _))
            {
                OnBulletHit?.Invoke(this, new BulletEventArgs(BulletHits.Bullet));
            }
            if (collision.gameObject.TryGetComponent(out Tank tank))
            {
                OnBulletHit?.Invoke(this, new BulletEventArgs(BulletHits.Tank, tank)); //todo не дамажу энеми
            }
            else if (collision.gameObject.TryGetComponent(out Wall wall))
            {
                OnBulletHit?.Invoke(this, new BulletEventArgs(BulletHits.Wall));
            }
            else if (collision.gameObject.TryGetComponent(out DestructibleWall dwall))
            {
                OnBulletHit?.Invoke(this, new BulletEventArgs(BulletHits.DestructibleWall, null, collision.gameObject.GetComponent<Tilemap>(), collision.contacts[0].point, (Vector2)transform.right));
            }
        }
        public class BulletEventArgs : EventArgs
        {
            public BulletHits hitObj;
            public Tank CollidedTank;
            public Tilemap Tilemap;
            public Vector2 HitPoint;
            public BulletEventArgs(BulletHits hit, Tank tank = null, Tilemap tilemap = null, Vector2 hitPoint = default, Vector2 bulletForwardVector = default)
            {
                switch (hit)
                {
                    case BulletHits.Bullet:
                        {
                            hitObj = BulletHits.Bullet;
                        }
                        break;
                    case BulletHits.Tank:
                        {
                            hitObj = BulletHits.Tank;
                            CollidedTank = tank;
                        }
                        break;
                    case BulletHits.Wall:
                        {
                            hitObj = BulletHits.Wall;
                        }
                        break;
                    case BulletHits.DestructibleWall:
                        {
                            hitObj = BulletHits.DestructibleWall;
                            Tilemap = tilemap;
                            HitPoint = hitPoint + bulletForwardVector * 0.1f;
                        }
                        break;
                }
            }
        }
        public event EventHandler<BulletEventArgs> OnBulletHit;
    }
}
