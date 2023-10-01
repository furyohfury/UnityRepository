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
        protected GameObject _bulletPrefab;
        protected Rigidbody2D _rigidBody;
        protected SpriteRenderer _spriteRenderer;
        protected Dictionary<Directions, Sprite> _directionDict;
        protected Dictionary<Directions, Sprite> _defaultDirectionDict;
        protected Vector2 _direction = Vector2.zero;
        protected virtual Vector2 Direction
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
                    if (_directionDict.ContainsKey(eDirection))
                    {
                        _spriteRenderer.sprite = _directionDict[eDirection];
                    }
                    else Debug.Log("No key " + _direction + " in dict");
                }
            }
        }
        protected Directions eDirection = Directions.Up;
        #region Unity_Methods
        protected virtual void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _defaultDirectionDict = new() { { Directions.Up, _defaultDirectionSprites[0] }, { Directions.Down, _defaultDirectionSprites[1] }, { Directions.Left, _defaultDirectionSprites[2] }, { Directions.Right, _defaultDirectionSprites[3] } };
            _directionDict = _defaultDirectionDict;
        }
        protected virtual void FixedUpdate()
        {
            Direction = _rigidBody.velocity;
        }
        #endregion
        protected void Shoot()
        {
            Bullet bulletComp = Instantiate(_bulletPrefab, (Vector2)transform.position + 0.7f * transform.localScale.x * _direction, _bulletPrefab.transform.rotation * Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, _direction)))).GetComponent<Bullet>();
            bulletComp.bulletData = new(Damage, BulletSpeed, this);
            GameManager.Instance.AddBullet(bulletComp);
        }
        public virtual void ChangeHealth(int delta)
        {
            Health += delta;
        }
    }
}