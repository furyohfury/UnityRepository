using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{
    public class EnemyController : Tank
    {
        Coroutine _shootingCor;
        [SerializeField, Range(0, 10f)]
        private float _movingForwardTime = 3f;
        private Vector2 _enemyDirection;
        [SerializeField]
        private CapsuleCollider2D _checkingCollider;
        [SerializeField]
        private GameObject _inFrontGO;
        #region Unity_Methods
        private void Start()
        {
            OnMoveForward += MoveForward;
            OnChangeDirection += ChangeDirection;
            OnCheckForward += CheckForward;
            _shootingCor = StartCoroutine(RepeatShooting());
            _enemyDirection = FindDirectionToPlayer();
            CheckForward();
        }
        private IEnumerator RepeatShooting()
        {
            while (true)
            {
                yield return new WaitForSeconds(AttackSpeed);
                Shoot();
            }
        }
        private void OnDisable()
        {
            OnMoveForward -= MoveForward;
            OnChangeDirection -= ChangeDirection;
            OnCheckForward -= CheckForward;
        }
        private void OnDestroy()
        {
            StopCoroutine(_shootingCor);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            _inFrontGO = collision.gameObject;
        }
        #endregion
        #region CheckingAI
        private Vector2 FindDirectionToPlayer()
        {
            Vector2 playerPos = GameManager.Instance.GetPlayerPosition();
            Vector2 directionToPlayer = playerPos - (Vector2)transform.position;
            directionToPlayer = directionToPlayer.x > directionToPlayer.y ? new Vector2(0, directionToPlayer.y) : new Vector2(directionToPlayer.x, 0);
            directionToPlayer.Normalize();
            return directionToPlayer;
        }
        private void CheckForward()
        {
            Debug.Log(gameObject.name + " checks forward");
            if (_inFrontGO == null || _inFrontGO.TryGetComponent(out DestructibleWall _) || _inFrontGO.TryGetComponent(out Bullet _))
            {
                OnMoveForward?.Invoke();
                // Debug.Log("DestructibleWall ahead");
            }
            else if (_inFrontGO.TryGetComponent(out Wall _) || _inFrontGO.TryGetComponent(out Water _) || _inFrontGO.TryGetComponent(out EnemyController _))
            {
                // Debug.Log("wall/water/enemy ahead");
                OnChangeDirection?.Invoke();
                // Debug.Log(gameObject.name + " change direction");
            }
        }
        #endregion
        #region ActionsAI
        private void MoveForward()
        {
            StartCoroutine(MoveForwardForTime());
            Debug.Log(gameObject.name + " moves forward");
        }
        private IEnumerator MoveForwardForTime()
        {
            _rigidBody.velocity = _enemyDirection * MoveSpeed;
            yield return new WaitForSeconds(_movingForwardTime);
            OnCheckForward?.Invoke();
        }

        private void ChangeDirection()
        {
            Debug.Log(gameObject.name + " change direction");
            int randomAngle;
            float r = UnityEngine.Random.value;
            if (r <= 0.33f) randomAngle = -90;
            else if (r >= 0.66f) randomAngle = 90;
            else randomAngle = 180;
            _enemyDirection = Rotate(_enemyDirection, randomAngle);
            OnCheckForward?.Invoke();

        }
        private Vector2 Rotate(Vector2 v, float delta)
        {
            delta *= Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }
        public override void ChangeHealth(int delta)
        {
            base.ChangeHealth(delta);
            if (Health > 0)
            {
                StopCoroutine(MoveForwardForTime());
                FindDirectionToPlayer();
                MoveForward();
            }
        }
        #endregion
        public delegate void EnemyAction();
        public event EnemyAction OnCheckForward;
        public event EnemyAction OnMoveForward;
        public event EnemyAction OnChangeDirection;
    }    
}