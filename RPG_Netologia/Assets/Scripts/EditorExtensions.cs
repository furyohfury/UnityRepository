using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace RPG
{
    public static class EditorExtensions
    {

    }
    public static class EditorConstants
    {
        public static readonly string FocusTargetPointName = "B-neck";
        public static readonly string AirColliderName = "AirCollider";
        public static Bounds AirColliderBounds = new Bounds(new Vector3(0f, 0.037f, 0f), new Vector3(0.5f, 0.08f, 0.5f));
        public static readonly string TriggersLayer = "Triggers";
    }
    [CustomPropertyDrawer(typeof(SQRFloatAttribute))]
    public class SQRFloatDrawer : PropertyDrawer
    {
        private float _labelWidthPercent = 0.4f;
        private float _space = 35f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position.x, position.y, position.width * _labelWidthPercent, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, property.displayName);

            var rectangle = new Rect(position.x + position.width * _labelWidthPercent , position.y, position.width * (1- _labelWidthPercent)/2f, EditorGUIUtility.singleLineHeight);
            
            var printRect = new Rect(rectangle.x + _space, rectangle.y, rectangle.width - _space, rectangle.height);
            GUI.enabled = false;
            var value = EditorGUI.FloatField(printRect, GUIContent.none, Mathf.Sqrt(property.floatValue));
            GUI.enabled = true;
            rectangle.x += rectangle.width;

            printRect = new Rect(rectangle.x, rectangle.y, rectangle.width - _space, rectangle.height);
            EditorGUI.LabelField(printRect, "Sqrt");

            printRect = new Rect(rectangle.x + _space, rectangle.y, rectangle.width - _space, rectangle.height);
            EditorGUI.PropertyField(printRect, property, GUIContent.none);
        }
    }
}





