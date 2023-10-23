using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public delegate void SimpleHandle();
    public delegate void SimpleHandle<T>(T arg);

    public static class MonoBehaviourExtensions
    {
        public static T FindComponent<T>(this MonoBehaviour source ) where T : Component
        {
            T component = source.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning("Theres no " + typeof(T) + "  component on " + source.gameObject.name);
                
            }
            return component;
        }
    }
}