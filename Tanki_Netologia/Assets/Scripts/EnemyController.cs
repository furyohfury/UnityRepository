using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{
    public class EnemyController : Tank
    {
        Coroutine _shootingCor;
        [SerializeField]
        private float _movingForwardTime;
        private Vector2 _enemyDirection;
        #region Unity_Methods
        private void Start()
        {
            _shootingCor = StartCoroutine(RepeatShooting());
            _enemyDirection = FindDirectionToPlayer();
        }
        private IEnumerator RepeatShooting()
        {
            while (true)
            {
                yield return new WaitForSeconds(AttackSpeed);
                Shoot();
            }            
        }
        private void OnDestroy()
        {
            StopCoroutine(_shootingCor);
        }
        #endregion
        #region CheckingAI
        private Vector2 FindDirectionToPlayer()
        {
            Vector2 playerPos = GameManager.Instance.GetPlayerPosition();
            Vector2 directionToPlayer = playerPos - transform.position;
            directionToPlayer = directionToPlayer.x > directionToPlayer.y ? new Vector2(0, directionToPlayer.y) : new Vector2(directionToPlayer.x, 0);
            return Vector2.Normalize(directionToPlayer);
        }
        #endregion
        #region ActionsAI
        private void MoveForward()
        {
            StartCoroutine(MoveForwardForTime());
        }
        private IEnumerator MoveForwardForTime()
        {
            _rigidbody.velocity = _enemyDirection * _moveSpeed;
            yield return new WaitForSeconds(_movingForwardTime);
        }
        
        private void ChangeDirection()
        {
            int randomAngle = UnityEngine.Random.Value >= 0.5f ? -90 : 90;
            _enemyDirection = Rotate(_enemyDirection, randomAngle);
        }
        private Vector2 Rotate(Vector2 v, float delta) {
            delta = delta * Mathf.Deg2Rad;
    return new Vector2(
        v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
        v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
    );
    }
        #endregion
    }
}