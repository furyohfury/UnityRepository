using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tanks
{
    public class EnemyController : Tank
    {
        Coroutine _shootingCor;
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
    }
}