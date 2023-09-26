using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks {

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
        protected Vector2 _direction;


        protected Rigidbody2D _rigidBody;
        protected SpriteRenderer _spriteRenderer;
        [SerializeField]
        protected Sprite _upView;
        [SerializeField]
        protected Sprite _downView;
        [SerializeField]
        protected Sprite _leftView;
        [SerializeField]
        protected Sprite _rightView;
        [SerializeField]
        protected GameObject _bullet;
        #region Unity_Methods
        protected virtual void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }
        protected virtual void FixedUpdate()
        {
            if (_rigidBody.velocity.x > 0 && _spriteRenderer.sprite != _rightView) _spriteRenderer.sprite = _rightView;
            else if (_rigidBody.velocity.x < 0 && _spriteRenderer.sprite != _leftView) _spriteRenderer.sprite = _leftView;
            else if (_rigidBody.velocity.y > 0 && _spriteRenderer.sprite != _upView) _spriteRenderer.sprite = _upView;
            else if (_rigidBody.velocity.y < 0 && _spriteRenderer.sprite != _downView) _spriteRenderer.sprite = _downView;
            if (_rigidBody.velocity != Vector2.zero)
            {
                _direction = _rigidBody.velocity;
            }
        }
        #endregion
        protected void Shoot()
        {
            Bullet bulletComp = Instantiate(_bullet, (Vector2)transform.position + 0.7f * transform.localScale.x * _direction, _bullet.transform.rotation * Quaternion.Euler(new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, _direction)))).GetComponent<Bullet>();
            bulletComp.bulletData = new(Damage, BulletSpeed, this);
            GameManager.Instance.AddBullet(bulletComp);
        }
        public void ChangeHealth(int delta)
        {
            Health += delta;
        }
    }
}