using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Units
{
    public class UnitInputComponent : MonoBehaviour
    {
        protected Vector3 _movement;

        public ref Vector3 MoveDirection => ref _movement;
        public SimpleHandle OnAttackEvent;
        public SimpleHandle OnShieldEvent;
        public SimpleHandle OnTargetEvent;

        protected void CallOnAttackEvent() => OnAttackEvent?.Invoke();
        protected void CallOnShieldEvent() => OnShieldEvent?.Invoke();
        protected void CallOnTargetEvent() => OnTargetEvent?.Invoke();
        
        protected void CallSimpleHandler(string name)
        {
            
        }
    }
}