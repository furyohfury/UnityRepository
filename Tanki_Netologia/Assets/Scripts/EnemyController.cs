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
        private Vector2 _enemyDirectionMove = Vector2.up;
        private Vector2 EnemyDirectionMove
        {
            get
            {
                return _enemyDirectionMove;
            }
            set
            {
                _enemyDirectionMove = value;
                _checkingCollider.offset = _enemyDirectionMove;
                DirectionVisual = _enemyDirectionMove;
            }
        }
        private Coroutine _shootingCor;
        private Coroutine _moveForwardCoroutine;
        #region Unity_Methods
        private void Start()
        {
            _shootingCor = StartCoroutine(RepeatShooting());
            FindDirectionToPlayer();
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
                    yield return new WaitForSeconds(_movingForwardTime + 0.1f);
                }
                else
                {
                    ChangeDirection();
                    MoveForward();
                    yield return new WaitForSeconds(_movingForwardTime + 0.1f);
                }
            }
        }
        private void OnDestroy()
        {
            StopCoroutine(_shootingCor);
            if (_moveForwardCoroutine != null) StopCoroutine(_moveForwardCoroutine);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Bullet _) && !collision.gameObject.TryGetComponent(out EnemySpawn _))
            {
                _inFrontGO = collision.gameObject;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out Bullet _) && !collision.gameObject.TryGetComponent(out EnemySpawn _))
            {
                _inFrontGO = null;
            }            
        }
        #endregion
        #region CheckingAI
        private void FindDirectionToPlayer()
        {
            Vector2 playerPos = GameManager.Instance.GetPlayerPosition();
            Vector2 directionToPlayer = playerPos - (Vector2)transform.position;
            directionToPlayer = directionToPlayer.x > directionToPlayer.y ? new Vector2(0, directionToPlayer.y) : new Vector2(directionToPlayer.x, 0);
            directionToPlayer.Normalize();
            EnemyDirectionMove = directionToPlayer;
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
            if (_moveForwardCoroutine == null) _moveForwardCoroutine = StartCoroutine(MoveForwardForTime());
            // Debug.Log(gameObject.name + " moves forward");
        }
        private IEnumerator MoveForwardForTime()
        {
            RigidBody.velocity = EnemyDirectionMove * MoveSpeed;
            yield return new WaitForSeconds(_movingForwardTime);
            _moveForwardCoroutine = null;
        }

        private void ChangeDirection()
        {
            // Debug.Log(gameObject.name + " change direction");
            int randomAngle;
            float r = UnityEngine.Random.value;
            if (r <= 0.33f) randomAngle = -90;
            else if (r >= 0.66f) randomAngle = 90;
            else randomAngle = 180;
            EnemyDirectionMove = Rotate(EnemyDirectionMove, randomAngle);
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
            // ѕри получении урона стопаетс€ и находит игрока
            if (Health > 0)
            {
                StopCoroutine(_moveForwardCoroutine);
                FindDirectionToPlayer();
            }
        }
        #endregion
    }
}