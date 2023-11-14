using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;

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
            GetAllFields(mbh);
        }
    }
    private void GetAllFields(MonoBehaviour monobehaviour)
    {
        EditorGUILayout.LabelField($"MBhs name: {monobehaviour.GetType().Name}");
        List<FieldInfo> fields = monobehaviour.GetType().GetFields().ToList();
        fields.AddRange(monobehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        // fields = fields.Where(f => f.IsPublic || f.a).ToArray();
        foreach (var field in fields)
        {
            var privs = field.GetCustomAttributes(typeof(SerializeField), false);
            if (field.IsPublic || privs.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                DrawField(field, monobehaviour);
                EditorGUILayout.EndHorizontal();
            }
            // EditorGUILayout.LabelField($"Field Name: {field.Name} Attributes: {field.Attributes.ToString()} Custom Attributes: {field.FieldType.GetCustomAttributes(typeof(SerializeField), false)}");                        
        }


    }
    private void DrawField(FieldInfo field, MonoBehaviour mbh)
    {
        if (field.FieldType.IsPrimitive) DrawPrimitive(field, mbh);
    }
    private void DrawPrimitive(FieldInfo field, MonoBehaviour mbh)
    {
        var t = field.FieldType;
        if (t == typeof(float)) DrawFloat(field, mbh);
        else if (t == typeof(int)) DrawInt(field, mbh);
    }
    private void DrawObject(FieldInfo field, MonoBehaviour mbh)
    {

    }
    private void DrawFloat(FieldInfo field, MonoBehaviour mbh)
    {
        SerializedObject serObj = new(mbh);
        SerializedProperty serProp = serObj.FindProperty(field.Name);
        EditorGUILayout.BeginHorizontal();
        // EditorGUILayout.LabelField(field.Name);        
        EditorGUILayout.DelayedFloatField(serProp, new GUIContent(field.Name));
        // EditorGUILayout.DelayedFloatField((float)field.GetValue(new object[] { }));
        // SerializedProperty
        EditorGUILayout.EndHorizontal();
        serObj.ApplyModifiedProperties();
    }
    private void DrawInt(FieldInfo field, MonoBehaviour mbh)
    {
        SerializedObject serObj = new(mbh);
        SerializedProperty serProp = serObj.FindProperty(field.Name);
        EditorGUILayout.BeginHorizontal();
        // EditorGUILayout.LabelField(field.Name);        
        EditorGUILayout.DelayedIntField(serProp);
        // EditorGUILayout.DelayedFloatField((float)field.GetValue(new object[] { }));
        // SerializedProperty
        EditorGUILayout.EndHorizontal();
        serObj.ApplyModifiedProperties();
    }
}
