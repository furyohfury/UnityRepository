using System.Collections;
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
        protected float BulletSpeed { get; private set; } = 2f;
        protected Rigidbody2D _rigidBody;
        protected SpriteRenderer _spriteRenderer;
        [SerializeField]
        protected Sprite[] _defaultDirectionSprites = new Sprite[4];
        protected Sprite _activeSprite;
        [SerializeField]
        protected GameObject _bullet;
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
            /* if (_rigidBody.velocity.x > 0) _activeSprite = _rightView;
            else if (_rigidBody.velocity.x < 0) _activeSprite = _leftView;
            else if (_rigidBody.velocity.y > 0) _activeSprite = _upView;
            else if (_rigidBody.velocity.y < 0) _activeSprite = _downView;
            _spriteRenderer.sprite = _activeSprite;
            if (_rigidBody.velocity != Vector2.zero)
            {
                _direction = _rigidBody.velocity;
            } */
            Direction = _rigidBody.velocity;
        }
        #endregion
        protected void Shoot()
        {
            Bullet bulletComp = Instantiate(_bullet, (Vector2)transform.position + 0.7f * transform.localScale.x * _direction, _bullet.transform.rotation * Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, _direction)))).GetComponent<Bullet>();
            bulletComp.bulletData = new(Damage, BulletSpeed, this);
            GameManager.Instance.AddBullet(bulletComp);
        }
        public virtual void ChangeHealth(int delta)
        {
            Health += delta;
        }
    }
}