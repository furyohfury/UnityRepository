using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEditor.ShortcutManagement;

namespace CustomEditor
{
    public class ReferenceInspector : EditorWindow
    {
        FieldInfo[] _fieldsInfo;
        [MenuItem("Extensions/Window/References Inspector"), Shortcut("ReferencesInspector", KeyCode.X, ShortcutModifiers.Shift)]
        public static void ShowCustomEditor()
        {
            var window = GetWindow<ReferenceInspector>(false, "Reference Inspector", true);
            window.minSize = new Vector2(500f, 500f);
        }
        public void OnGUI()
        {           
            GameObject selectedGO = Selection.activeGameObject;
            if (selectedGO == null) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Unit: " + selectedGO.name);
            EditorGUILayout.EndHorizontal();
            MonoBehaviour[] monoBehaviours = selectedGO.GetComponents<MonoBehaviour>();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Number of MonoBehaviour components : " + monoBehaviours.Length.ToString());
            EditorGUILayout.EndHorizontal();
            Draw(monoBehaviours);
            Repaint();
        }
        public void Draw(MonoBehaviour[] monoBehaviours)
        {
            foreach (var mbh in monoBehaviours)
            {
                EditorGUILayout.LabelField($"MBhs name: {mbh.GetType().Name}");
                SerializedObject serObj = new(mbh);
                GetAllFields(mbh, serObj);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }
        public void GetAllFields(MonoBehaviour monobehaviour, SerializedObject serObj)
        {
            List<FieldInfo> fields = monobehaviour.GetType().GetFields().ToList();
            fields.AddRange(monobehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
            foreach (var field in fields)
            {
                var serializedField = field.GetCustomAttributes(typeof(SerializeField), false);
                if (field.IsPublic || serializedField.Length > 0)
                {
                    DrawField(field, serObj, monobehaviour);
                }                       
            }
        }
        public void DrawField(FieldInfo field, SerializedObject serObj, MonoBehaviour mbh)
        {
            var type = field.FieldType;
            if (type.IsValueType)
            {
                if (type.IsPrimitive) DrawPrimitive(field, serObj);
                else if (type.IsEnum) DrawEnum(field, serObj, mbh);
                else DrawStruct(field, serObj);                
            }
            else if (type == typeof(string)) DrawString(field.Name, serObj);
            else if (type.IsSubclassOf(typeof(UnityEngine.Object))) DrawUnityObject(field.Name, serObj);
            else if (type.IsArray || type.GetInterfaces().Contains(typeof(IEnumerable))) DrawUnsupported(type.ToString(), null);
        }
        public void DrawPrimitive(FieldInfo field, SerializedObject serObj)
        {
            string t = field.FieldType.ToString();
            string tString = t[t.IndexOf('.')..].TrimStart('.');
            tString = string.Concat("Draw", tString[0].ToString().ToUpper(), tString[1..]);
            try
            {
                // Invoking method of this class
                this.GetType().GetMethod(tString).Invoke(null, new object[] { field.Name, serObj });
            }
            catch (Exception e)
            {
                DrawUnsupported(t.ToString(), e);
            }
        }
        public void DrawStruct(FieldInfo field, SerializedObject serObj)
        {
            string t = field.FieldType.ToString();
            string tString = t.Substring(t.IndexOf('.'), t.Length - t.IndexOf('.')).TrimStart('.');
            tString = string.Concat("Draw", tString);
            try 
            {                
                // Invoking method of this class
                this.GetType().GetMethod(tString).Invoke(null, new System.Object[] { field.Name, serObj });
            }
            catch (Exception e)
            {
                DrawUnsupported(t.ToString(), e);
            }
        }
        public static void DrawSingle(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedFloatField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }  
        public static void DrawUnityObject(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawInt32(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DelayedIntField(serProp, new GUIContent(fieldName));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawBoolean(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boolValue = EditorGUILayout.Toggle(fieldName, serProp.boolValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawInt64(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.longValue = EditorGUILayout.LongField(fieldName, serProp.longValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawDouble(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.doubleValue = EditorGUILayout.DelayedDoubleField(fieldName, serProp.doubleValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawString(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.stringValue = EditorGUILayout.DelayedTextField(fieldName, serProp.stringValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawVector2(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector2Value = EditorGUILayout.Vector2Field(fieldName, serProp.vector2Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawVector2Int(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector2IntValue = EditorGUILayout.Vector2IntField(fieldName, serProp.vector2IntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawVector3(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector3Value = EditorGUILayout.Vector3Field(fieldName, serProp.vector3Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawVector3Int(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector3IntValue = EditorGUILayout.Vector3IntField(fieldName, serProp.vector3IntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawVector4(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.vector4Value = EditorGUILayout.Vector4Field(fieldName, serProp.vector4Value);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawQuaternion(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            Vector4 v = EditorGUILayout.Vector4Field(fieldName, new Vector4(serProp.quaternionValue.x, serProp.quaternionValue.y, serProp.quaternionValue.z, serProp.quaternionValue.w));
            serProp.quaternionValue = new Quaternion(v.x, v.y, v.z, v.w);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawColor(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.colorValue = EditorGUILayout.ColorField(fieldName, serProp.colorValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawBounds(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boundsValue = EditorGUILayout.BoundsField(fieldName, serProp.boundsValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawBoundsInt(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.boundsIntValue = EditorGUILayout.BoundsIntField(fieldName, serProp.boundsIntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawRect(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.rectValue = EditorGUILayout.RectField(fieldName, serProp.rectValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawRectInt(string fieldName, SerializedObject serObj)
        {
            SerializedProperty serProp = serObj.FindProperty(fieldName);
            EditorGUILayout.BeginHorizontal();
            serProp.rectIntValue = EditorGUILayout.RectIntField(fieldName, serProp.rectIntValue);
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawEnum(FieldInfo field, SerializedObject serObj, MonoBehaviour mbh)
        {
            EditorGUILayout.BeginHorizontal();
            var enumValue = field.GetValue(mbh);
            field.SetValue(mbh, EditorGUILayout.EnumPopup(field.Name, (Enum) enumValue));
            EditorGUILayout.EndHorizontal();
            serObj.ApplyModifiedProperties();
        }
        public static void DrawUnsupported(string type, Exception e)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Type {type} isn't supported");
            EditorGUILayout.EndHorizontal();
            if (e != null) EditorGUILayout.HelpBox($"Exception message: {e.ToString()}", MessageType.Error, true);
        }        
    }
}