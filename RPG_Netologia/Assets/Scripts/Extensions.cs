using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public delegate void SimpleHandle();
    public delegate void SimpleHandle<T>(T arg);
    public delegate void SimpleHandle<T1, T2>(T1 arg1, T2 arg2);
    public class SQRFloatAttribute : PropertyAttribute { }

    public static class Constants
    {
        public static readonly string FloorTag = "Floor";
        public static readonly string ObstaclesTagName = "Obstacles";
        public static int ObstacleLayerInt;

        public static void Construct()
        {
            ObstacleLayerInt = LayerMask.NameToLayer(ObstaclesTagName);
        }
    }
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