using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units.Player
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private Transform _cameraPivot;
        

        protected override void Start()
        {
            base.Start();
            Stats.Name = name;
        }
        protected override void OnRotate()
        {
            transform.rotation = Quaternion.Euler(0f, _cameraPivot.eulerAngles.y, 0f);
            // transform.rotation = _camera.PivotTransform.rotation;
        }
    }
}