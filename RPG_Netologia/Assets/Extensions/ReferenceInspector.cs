using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
namespace CustomEditor
{
    public class ReferenceInspector : EditorWindow
    {
        FieldInfo[] _fieldsInfo;
        // MethodInfo[] _methodsInfo;
        //todo properties
        [MenuItem("Extensions/Window/References Inspector")]
        public static void ShowCustomEditor()
        {
            var window = GetWindow<ReferenceInspector>(false, "Reference Inspector", true);
            window.minSize = new Vector2(500f, 500f);
        }
        private void OnEnable()
        {

        }
        private void OnGUI()
        {
            GameObject selectedGO = Selection.activeGameObject;
            if (selectedGO == null) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Unit: ");
            EditorGUILayout.DelayedTextField(selectedGO.name);
            EditorGUILayout.EndHorizontal();
            MonoBehaviour[] monoBehaviours = selectedGO.GetComponents<MonoBehaviour>();
            // Debug.Log($"Selected GO has {_monoBehaviours.Length} on it");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Number of MBhs");
            EditorGUILayout.TextField(monoBehaviours.Length.ToString());
            EditorGUILayout.EndHorizontal();
            Draw(monoBehaviours);
            // GetAllFields(monoBehaviours[0]);
            Repaint();

        }
        private void Draw(MonoBehaviour[] monoBehaviours)
        {
            foreach (var mbh in monoBehaviours)
            {
                EditorGUILayout.LabelField($"MBhs name: {mbh.GetType().Name}");
                SerializedObject serObj = new(mbh);
                GetAllFields(mbh, serObj);
            }
        }
        private void GetAllFields(MonoBehaviour monobehaviour, SerializedObject serObj)
        {
            List<FieldInfo> fields = monobehaviour.GetType().GetFields().ToList();
            fields.AddRange(monobehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
            foreach (var field in fields)
            {
                var serializedField = field.GetCustomAttributes(typeof(SerializeField), false);
                if (field.IsPublic || serializedField.Length > 0)
                {
                    DrawField(field, serObj);
                }
                // EditorGUILayout.LabelField($"Field Name: {field.Name} Attributes: {field.Attributes.ToString()} Custom Attributes: {field.FieldType.GetCustomAttributes(typeof(SerializeField), false)}");                        
            }


        }
        private void DrawField(FieldInfo field, SerializedObject serObj)
        {
            var type = field.FieldType;
            if (type.IsValueType)
            {
                if (type.IsPrimitive) DrawPrimitive(field, serObj);
                //else if (type.IsEnum) 
                
            }
            else if (type.IsSubclassOf(typeof(UnityEngine.Object))) DrawUnityObject(field.Name, serObj);

        }
        private void DrawPrimitive(FieldInfo field, SerializedObject serObj)
        {
            var t = field.FieldType;
            if (t == typeof(float)) DrawFloat(field.Name, serObj);
            else if (t == typeof(int)) DrawInt(field.Name, serObj);
            else if (t == typeof(bool)) DrawBool(field.Name, serObj);
            else if (t == typeof(long)) DrawLong(field.Name, serObj);
            else if (t == typeof(double)) DrawDouble(field.Name, serObj);
        }
        private void DrawUnityObject(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawFloat(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedFloatField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawInt(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedIntField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawBool(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boolValue = EditorGUILayout.Toggle(fieldName, serProp.boolValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawLong(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.longValue = EditorGUILayout.LongField(fieldName, serProp.longValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawDouble(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.doubleValue = EditorGUILayout.DelayedDoubleField(fieldName, serProp.doubleValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawVector2(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector2Value = EditorGUILayout.Vector2Field(fieldName, serProp.vector2Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawVector2Int(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector2IntValue = EditorGUILayout.Vector2IntField(fieldName, serProp.vector2IntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawVector3(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector3Value = EditorGUILayout.Vector3Field(fieldName, serProp.vector3Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawVector3Int(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector3IntValue = EditorGUILayout.Vector3IntField(fieldName, serProp.vector3IntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawVector4(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector4Value = EditorGUILayout.Vector4Field(fieldName, serProp.vector4Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawQuaternion(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            Vector4 v = EditorGUILayout.Vector4Field(fieldName, new Vector4(serProp.quaternionValue.x, serProp.quaternionValue.y, serProp.quaternionValue.z, serProp.quaternionValue.w));
            serProp.quaternionValue = new Quaternion(v.x, v.y, v.z, v.w);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawColor(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.colorValue = EditorGUILayout.ColorField(fieldName, serProp.colorValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawBounds(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boundsValue = EditorGUILayout.BoundsField(fieldName, serProp.boundsValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawBoundsInt(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boundsIntValue = EditorGUILayout.BoundsIntField(fieldName, serProp.boundsIntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        private void DrawEnum(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            //serProp.enumValueFlag = EditorGUILayout.EnumPopup(fieldName, serProp.enumValueFlag);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }

    }
}