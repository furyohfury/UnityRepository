using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{

    [RequireComponent(typeof(Rigidbody2D))]
    public class Tank : MonoBehaviour
    {
        [field: SerializeField]
        public int Health { get; private set; } = 2;
        [field: SerializeField]
        protected float MoveSpeed { get; private set; } = 2f;
        [field: SerializeField]
        protected float AttackSpeed { get; private set; } = 0.5f;
        [field: SerializeField]
        public int Damage { get; private set; } = 1;
        [field: SerializeField]
        protected float BulletSpeed { get; private set; } = 3f;
        [SerializeField]
        protected Sprite[] _defaultDirectionSprites = new Sprite[4];
        [SerializeField]
        protected GameObject BulletPrefab;
        protected Rigidbody2D RigidBody;
        protected SpriteRenderer SpriteRenderer;
        protected Dictionary<Directions, Sprite> DirectionDict;
        protected Dictionary<Directions, Sprite> DefaultDirectionDict;
        protected Vector2 _direction = Vector2.zero;
        protected virtual Vector2 DirectionVisual
        {
            get
            {
                return _direction;
            }
            set
            {
                if (value != _direction && value != Vector2.zero)
                {
                    value.Normalize();
                    _direction = value;
                    if (_direction == Vector2.up) eDirection = Directions.Up;
                    if (_direction == Vector2.down) eDirection = Directions.Down;
                    if (_direction == Vector2.left) eDirection = Directions.Left;
                    if (_direction == Vector2.right) eDirection = Directions.Right;
                    if (DirectionDict.ContainsKey(eDirection))
                    {
                        SpriteRenderer.sprite = DirectionDict[eDirection];
                    }
                    else Debug.Log("No key " + _direction + " in dict");
                }
            }
        }
        protected Directions eDirection = Directions.Up;
        #region Unity_Methods
        protected virtual void Awake()
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            RigidBody = GetComponent<Rigidbody2D>();
            DefaultDirectionDict = new() { { Directions.Up, _defaultDirectionSprites[0] }, { Directions.Down, _defaultDirectionSprites[1] }, { Directions.Left, _defaultDirectionSprites[2] }, { Directions.Right, _defaultDirectionSprites[3] } };
            DirectionDict = DefaultDirectionDict;
        }
        protected virtual void FixedUpdate()
        {
            if (RigidBody.velocity.x != 0 && RigidBody.velocity.y != 0) RigidBody.velocity = RigidBody.velocity.x > RigidBody.velocity.y ? new Vector2(RigidBody.velocity.x, 0) : new Vector2(0, RigidBody.velocity.y);
            DirectionVisual = RigidBody.velocity;
        }
        #endregion
        protected void Shoot()
        {
            Bullet bulletComp = Instantiate(BulletPrefab, (Vector2)transform.position + 0.7f * transform.localScale.x * DirectionVisual, BulletPrefab.transform.rotation * Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, DirectionVisual)))).GetComponent<Bullet>();
            bulletComp.bulletData = new(Damage, BulletSpeed, this);
            GameManager.Instance.AddBullet(bulletComp);
        }
        public virtual void ChangeHealth(int delta)
        {
            Health += delta;
        }
    }
}