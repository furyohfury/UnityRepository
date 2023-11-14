using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEditor;
/* using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace Edit.Editor
{
	public class ReferencesInspectorEditor : EditorWindow
	{
		private static Type _referencesInspector;
		private static Type _inspector;

		private string _separator;
		private MonoBehaviour _currentComponent;
		private SerializedObject _currentComponentSer;
		private Vector2 _scroll;
		private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		[MenuItem("Extensions/Windows/References Inspector #c", priority = 1)]
		public static ReferencesInspectorEditor ShowReferencesInspectorEditor()
		{
			var window = GetWindow<ReferencesInspectorEditor>(false, "References Inspector", true);
			window.position = new Rect(1648f, 73.6f, 399f, 998f);
			return window;
		}

		[Shortcut("ReferencesInspector", KeyCode.X, ShortcutModifiers.Shift)]
		public static void FocusReferencesInspector()
		{
			if (_referencesInspector == null) _referencesInspector = typeof(ReferencesInspectorEditor);
			FocusWindowIfItsOpen(_referencesInspector);
		}

		[Shortcut("Inspector", KeyCode.Z, ShortcutModifiers.Shift)]
		public static void FocusInspector()
		{
			if (_inspector == null) _inspector = AppDomain.CurrentDomain.Load("UnityEditor").GetType("UnityEditor.InspectorWindow");
			FocusWindowIfItsOpen(_inspector);
		}

		public void OnEnable()
		{
			Selection.selectionChanged += Repaint;
		}

		public void OnDisable()
		{
			Selection.selectionChanged -= Repaint;
		}

		private void OnGUI()
		{
			_scroll = EditorGUILayout.BeginScrollView(_scroll);

			foreach (var obj in Selection.gameObjects)
			{
				var components = obj.GetComponents<MonoBehaviour>();
				if (components.Length == 0) continue;

				PrintHeader(obj.name, true);
				foreach (var comp in components)
				{
					_currentComponentSer = new SerializedObject(comp);
					_currentComponentSer.Update();

					EditorGUILayout.Space(10f);
					PrintComponent(comp);

					if (GUI.changed) EditorUtility.SetDirty(comp);
					_currentComponentSer.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.EndScrollView();
		}

		private void PrintComponent(MonoBehaviour component)
		{
			PrintHeader(component.GetType().Name, false);

			var fields = component.GetType().GetFields(_flags);

			EditorGUILayout.BeginVertical("box");

			foreach (var field in fields)
			{
				if (field.IsPrivate && field.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;
				_currentComponent = component;
				PrintField(field);
			}

			EditorGUILayout.EndVertical();
		}

		private void PrintField(FieldInfo field)
		{
			EditorGUILayout.BeginHorizontal();

			var property = _currentComponentSer.FindProperty(field.Name);
			EditorGUILayout.PropertyField(property);
			
			EditorGUILayout.LabelField(field.Name, GUILayout.Width(position.width * 0.3f));
			EditorGUILayout.Space(10f);

			var type = field.FieldType;

			if (type.IsPrimitive && type.IsValueType) PrintPrimitive(field);
			else if (type.IsEnum) PrintEnum(field);
			else if (!type.IsPrimitive && type.IsValueType) PrintValue(field);
			else if (type.IsClass) PrintObject(field);
			else if (type.IsArray || type.GetInterface(nameof(IEnumerable)) != null) PrintArray(field);
			
			EditorGUILayout.EndHorizontal();
		}

		private void PrintHeader(string text, bool isBig)
		{
			if (string.IsNullOrEmpty(_separator))
			{
				var strs = new StringBuilder();
				for (int i = 0; i < 40; i++) strs.Append("_____");
				_separator = strs.ToString();
			}

			EditorGUILayout.LabelField(_separator);

			var style = isBig ? new GUIStyle("label")
			{
				fontSize = 20,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter
			} : new GUIStyle("label")
			{
				fontSize = 15,
				fontStyle = FontStyle.Italic,
				alignment = TextAnchor.MiddleCenter
			};
			EditorGUILayout.LabelField(text, style, GUILayout.ExpandWidth(true), GUILayout.Height(30f));
		}

		private void PrintArray(FieldInfo field)
		{
			EditorGUILayout.BeginHorizontal("box");
			GUI.color = Color.red;
			EditorGUILayout.LabelField("������� �� ��������������");
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
			
			dynamic array = field.GetValue(_currentComponent);

			EditorGUILayout.LabelField(array.Length.ToString());

			var length = array.Length;
			length = EditorGUILayout.TextField(length);

			if(length > array.Length)
			{
				//ArrayUtility.
			}
			
		}

		private void PrintPrimitive(FieldInfo field)
		{
			var value = field.GetValue(_currentComponent);
			if (field.FieldType == typeof(int))
			{
				var attr = field.GetCustomAttribute(typeof(RangeAttribute)) as RangeAttribute;

				if (attr == null)
				{
					field.SetValue(_currentComponent, EditorGUILayout.IntField((int)value));
				}
				else
				{
					field.SetValue(_currentComponent, EditorGUILayout.IntSlider((int)value, (int)attr.min, (int)attr.max));
				}
			}
			else if (field.FieldType == typeof(float))
			{
				var attr = field.GetCustomAttribute(typeof(RangeAttribute)) as RangeAttribute;

				if (attr == null)
				{
					field.SetValue(_currentComponent, EditorGUILayout.FloatField((float)value));
				}
				else
				{
					field.SetValue(_currentComponent, EditorGUILayout.Slider((float)value, attr.min, attr.max));
				}
			}
			else if (field.FieldType == typeof(bool))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Toggle((bool)value));
			}
			else if (field.FieldType == typeof(long))
			{
				field.SetValue(_currentComponent, EditorGUILayout.LongField((long)value));
			}
			else if (field.FieldType == typeof(double))
			{
				field.SetValue(_currentComponent, EditorGUILayout.DoubleField((double)value));
			}
		}

		private void PrintValue(FieldInfo field)
		{
			var value = field.GetValue(_currentComponent);
			if (field.FieldType == typeof(Vector2))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Vector2Field((string)null, (Vector2)value));
			}
			else if (field.FieldType == typeof(Vector3))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Vector3Field((string)null, (Vector3)value));
			}
			else if (field.FieldType == typeof(Vector2Int))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Vector2IntField((string)null, (Vector2Int)value));
			}
			else if (field.FieldType == typeof(Vector3Int))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Vector3IntField((string)null, (Vector3Int)value));
			}
			else if (field.FieldType == typeof(Vector4))
			{
				field.SetValue(_currentComponent, EditorGUILayout.Vector4Field((string)null, (Vector4)value));
			}
			else if (field.FieldType == typeof(Quaternion))
			{
				var value1 = (Quaternion)field.GetValue(_currentComponent);
				var vector = EditorGUILayout.Vector4Field((string)null, new Vector4(value1.x, value1.y, value1.z, value1.w));
				field.SetValue(_currentComponent, new Quaternion(vector.x, vector.y, vector.z, vector.w));
			}
			else if (field.FieldType == typeof(Color))
			{
				field.SetValue(_currentComponent, EditorGUILayout.ColorField((Color)value));
			}
			else if (field.FieldType == typeof(BoundsInt))
			{
				field.SetValue(_currentComponent, EditorGUILayout.BoundsIntField((BoundsInt)value));
			}
			else if (field.FieldType == typeof(Bounds))
			{
				field.SetValue(_currentComponent, EditorGUILayout.BoundsField((Bounds)value));
			}
			else if (field.FieldType == typeof(RectInt))
			{
				field.SetValue(_currentComponent, EditorGUILayout.RectIntField((RectInt)value));
			}
			else if (field.FieldType == typeof(Rect))
			{
				field.SetValue(_currentComponent, EditorGUILayout.RectField((Rect)value));
			}
			else //if((typeof(System.SerializableAttribute)) != null)
			{

				var r = field.FieldType.CustomAttributes;
				var property = _currentComponentSer.FindProperty(field.Name);
				EditorGUILayout.PropertyField(property);
			}

		}

		private void PrintEnum(FieldInfo field)
		{
			field.SetValue(_currentComponent, EditorGUILayout.EnumPopup(field.GetValue(_currentComponent) as Enum));
		}

		private void PrintObject(FieldInfo field)
		{
			if (field.FieldType.IsSubclassOf(typeof(Object)))
			{
				field.SetValue(_currentComponent, EditorGUILayout.ObjectField(field.GetValue(_currentComponent) as Object, field.FieldType, true));
			}
			else if (field.FieldType == typeof(AnimationCurve))
			{
				field.SetValue(_currentComponent, EditorGUILayout.CurveField(field.GetValue(_currentComponent) as AnimationCurve));
			}
			else if (field.FieldType == typeof(string))
			{
				field.SetValue(_currentComponent, EditorGUILayout.TextField(field.GetValue(_currentComponent) as string));
			}
			else EditorGUILayout.LabelField("����� �� ����������e���");
		} 
	} 
} */