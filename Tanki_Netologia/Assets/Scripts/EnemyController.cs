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
        #endregion
    }
}