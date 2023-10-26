using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units.Player
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private Transform _cameraPivot;

        [SerializeField, Range(0f, 10f)]
        private float _rotateSpeed = 5f;

        /// <summary>
        /// Извлечено ли оружие
        /// </summary>
        private bool _isArms; //todo 
        [SerializeField]
        private WeaponType _weaponType = WeaponType.SwordAndShield;

        protected override void Start()
        {
            base.Start();
            Stats.Name = name;
        }
        /* protected override void OnRotate()
        {
            transform.rotation = Quaternion.Euler(0f, _cameraPivot.eulerAngles.y, 0f);
            // transform.rotation = _camera.PivotTransform.rotation;
        } */
        protected override void OnMove()
        {
            base.OnMove();
            ref var movement = ref _inputs.MoveDirection;
            if (Target != null)
            {
                transform.rotation = Quaternion.Euler(0f, _cameraPivot.eulerAngles.y, 0f);
            }
            else if (movement.z > 0 && movement.x == 0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, _cameraPivot.eulerAngles.y, 0f), _rotateSpeed * Time.deltaTime); 
            }
            else if (movement.z < 0f && movement.x == 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, _cameraPivot.eulerAngles.y, 0f), _rotateSpeed / 2f * Time.deltaTime);
            }
        }
        protected override void FindNewTarget()
        {
            var units = _unitManager.GetNPCs;

            var distance = _sqrFindTargetDistance;
            Target = null;

            foreach(var unit in units)
            {
                if (unit.GetStats.SideType == GetStats.SideType) continue;
                var currentDistance = (unit.transform.position - transform.position).sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    Target = unit;
                }
            }
        }
    }
}