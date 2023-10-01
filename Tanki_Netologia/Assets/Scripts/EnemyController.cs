using System;
using System.Collections;
using UnityEngine;
namespace Tanks
{
    public class EnemyController : Tank
    {        
        [SerializeField, Range(0, 10f)]
        private float _movingForwardTime = 3f;        
        [SerializeField]
        private CapsuleCollider2D _checkingCollider;
        [SerializeField]
        private GameObject _inFrontGO;
        private Vector2 _enemyDirection;
        private Coroutine _shootingCor;
        private Coroutine _moveForwardCoroutine;
        #region Unity_Methods
        private void Start()
        {            
            _shootingCor = StartCoroutine(RepeatShooting());
            _enemyDirection = FindDirectionToPlayer();   
            StartCoroutine(AICycle());
        }
        private IEnumerator RepeatShooting()
        {
            while (true)
            {
                yield return new WaitForSeconds(AttackSpeed);
                Shoot();
            }
        }
        private IEnumerator AICycle()
        {
            while (true)
            {
                if (CheckMoveOrChangeDirection())
                {
                    MoveForward();
                    yield return new WaitForSeconds(_movingForwardTime);
                }
                else
                {
                    ChangeDirection();
                    MoveForward();
                    yield return new WaitForSeconds(_movingForwardTime);
                }
            }
        }
        private void OnDestroy()
        {
            StopCoroutine(_shootingCor);
            StopCoroutine(_moveForwardCoroutine);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Bullet _)) _inFrontGO = collision.gameObject;
        }
        #endregion
        #region CheckingAI
        private Vector2 FindDirectionToPlayer()
        {
            Vector2 playerPos = GameManager.Instance.GetPlayerPosition();
            Vector2 directionToPlayer = playerPos - (Vector2)transform.position;
            directionToPlayer = directionToPlayer.x > directionToPlayer.y ? new Vector2(0, directionToPlayer.y) : new Vector2(directionToPlayer.x, 0);
            directionToPlayer.Normalize();
            Direction = directionToPlayer;
            return directionToPlayer;
        }
        private bool CheckMoveOrChangeDirection()
        {
            // Debug.Log(gameObject.name + " checks forward");
            if (_inFrontGO == null || _inFrontGO.TryGetComponent(out DestructibleWall _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region ActionsAI
        private void MoveForward()
        {
            _moveForwardCoroutine = StartCoroutine(MoveForwardForTime());
            // Debug.Log(gameObject.name + " moves forward");
        }
        private IEnumerator MoveForwardForTime()
        {
            _rigidBody.velocity = Direction * MoveSpeed;
            yield return new WaitForSeconds(_movingForwardTime);
        }

        private void ChangeDirection()
        {
            // Debug.Log(gameObject.name + " change direction");
            int randomAngle;
            float r = UnityEngine.Random.value;
            if (r <= 0.33f) randomAngle = -90;
            else if (r >= 0.66f) randomAngle = 90;
            else randomAngle = 180;
            _enemyDirection = Rotate(_enemyDirection, randomAngle);
            Direction = _enemyDirection;
            _checkingCollider.offset = _enemyDirection;

        }
        private Vector2 Rotate(Vector2 v, float delta)
        {
            delta *= Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        #endregion
        #region Public_Methods
        public override void ChangeHealth(int delta)
        {
            base.ChangeHealth(delta);
            if (Health > 0)
            {
                StopCoroutine(_moveForwardCoroutine);
                _enemyDirection = FindDirectionToPlayer();
                Direction = _enemyDirection;
            }
        }
        #endregion
    }
}