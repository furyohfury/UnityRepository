using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
namespace RPG.Units
{
    public class UnitInputComponent : MonoBehaviour
    {
        private Dictionary<string, FieldInfo> _events = new();
        private Func<FieldInfo, Delegate[]> _expression;

        protected Vector3 _movement;

        public ref Vector3 MoveDirection => ref _movement;
        public SimpleHandle MainEventHandler;
        public SimpleHandle AdditionalEventHandler;
        public SimpleHandle TargetEventHandler;
        protected virtual void Awake()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(t => t.FieldType == typeof(SimpleHandle));
            foreach(var it in fields)
            {
                _events.Add(it.Name, it);
            }
            var strExpr = Expression.Parameter(typeof(string));
            var dicConstExpr = Expression.Constant(_events, typeof(Dictionary<string, FieldInfo>));

            
        }
        protected void CallSimpleHandle(string name)
        {
            var field = _events[name];
            var fieldExpr = Expression.Field(Expression.Constant(this), field);

            var delegates = Expression.Convert(fieldExpr, typeof(MulticastDelegate));

            var methodInfo = typeof(MulticastDelegate).GetMethod(nameof(MulticastDelegate.GetInvocationList));
            var getInvocExpr = Expression.Call(delegates, methodInfo);

            var arrayExpr = Expression.Convert(getInvocExpr, typeof(Delegate[]));
            _expression = Expression.Lambda<Func<FieldInfo, Delegate[]>>(arrayExpr, Expression.Parameter(typeof(FieldInfo))).Compile();
            foreach (var @event in _expression.Invoke(_events[name]))
            {
                @event.Method.Invoke(@event.Target, null);
            }
        }
    }
}