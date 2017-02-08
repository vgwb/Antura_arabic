using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

[Flags]
public enum EditorListOption {
	None = 0,
	ListSize = 1,
	ListLabel = 2,
	Default = ListSize | ListLabel
}

public class WMG_E_Util : Editor {
	// Function to display List<T> in inspector
	public void ArrayGUI(string label, string name, EditorListOption options = EditorListOption.Default) {
		bool showListLabel = (options & EditorListOption.ListLabel) != 0;
		bool showListSize = (options & EditorListOption.ListSize) != 0;
		SerializedProperty list = serializedObject.FindProperty(name);
		
		if (showListLabel) {
			EditorGUILayout.PropertyField(list, new GUIContent(label));
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			if (showListSize) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			}
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			}
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}

	// Function to display observable collection WMG_List<T> in inspector
	public void ArrayGUIoc<T>(WMG_List<T> observableCollection, string label, string name, EditorListOption options = EditorListOption.Default) {
		ArrayGUIoc(observableCollection, label, name, serializedObject, options);
	}

	public void ArrayGUIoc<T>(WMG_List<T> observableCollection, string label, string name, SerializedObject serObj, EditorListOption options = EditorListOption.Default) {
		bool showListLabel = (options & EditorListOption.ListLabel) != 0;
		bool showListSize = (options & EditorListOption.ListSize) != 0;
		SerializedProperty list = serObj.FindProperty(name);
		
		if (showListLabel) {
			EditorGUILayout.PropertyField(list, new GUIContent(label));
			EditorGUI.indentLevel += 1;
		}
		if (!showListLabel || list.isExpanded) {
			int prevSize = list.arraySize;
			if (showListSize) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			}
			// The size changed notify observableCollection of this change
			if (prevSize != list.arraySize) {
				if (Application.isPlaying) {
					list.serializedObject.ApplyModifiedProperties ();
					observableCollection.SizeChangedViaEditor();
				}
			}

			for (int i = 0; i < list.arraySize; i++) {
				SerializedProperty prop = list.GetArrayElementAtIndex(i);
				if (prop.propertyType == SerializedPropertyType.String) {
					string prev = prop.stringValue;
					EditorGUILayout.PropertyField(prop);
					if (prev != prop.stringValue) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor(i);
					}
				}
				else if (prop.propertyType == SerializedPropertyType.Float) {
					float prev = prop.floatValue;
					EditorGUILayout.PropertyField(prop);
					if (prev != prop.floatValue) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor(i);
					}
				}
				else if (prop.propertyType == SerializedPropertyType.Color) {
					Color prev = prop.colorValue;
					EditorGUILayout.PropertyField(prop);
					if (prev != prop.colorValue) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor(i);
					}
				}
				else if (prop.propertyType == SerializedPropertyType.Vector2) {
					Vector2 prev = prop.vector2Value;
					EditorGUILayout.PropertyField(prop);
					if (prev != prop.vector2Value) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor(i);
					}
				}
				else if (prop.propertyType == SerializedPropertyType.ObjectReference) {
					UnityEngine.Object prev = prop.objectReferenceValue;
					EditorGUILayout.PropertyField(prop);
					if (prev != prop.objectReferenceValue) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor(i);
					}
				}
				else if (prop.propertyType == SerializedPropertyType.Boolean) {
					bool prev = prop.boolValue;
					EditorGUILayout.PropertyField (prop);
					if (prev != prop.boolValue) {
						list.serializedObject.ApplyModifiedProperties ();
						observableCollection.ValueChangedViaEditor (i);
					}
				}
			}
		}
		if (showListLabel) {
			EditorGUI.indentLevel -= 1;
		}
	}

//	public void ArrayGUIoc2<T>(WMG_List<T> observableCollection, string label, string name, SerializedObject serObj = serializedObject , EditorListOption options = EditorListOption.Default) {
//		bool showListLabel = (options & EditorListOption.ListLabel) != 0;
//		bool showListSize = (options & EditorListOption.ListSize) != 0;
//		SerializedProperty list = serObj.FindProperty(name);
//		
//		if (showListLabel) {
//			EditorGUILayout.PropertyField(list, new GUIContent(label));
//			EditorGUI.indentLevel += 1;
//		}
//		if (!showListLabel || list.isExpanded) {
//			int prevSize = list.arraySize;
//			if (showListSize) {
//				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
//			}
//			// The size changed notify observableCollection of this change
//			if (prevSize != list.arraySize) {
//				if (Application.isPlaying) {
//					list.serializedObject.ApplyModifiedProperties ();
//					observableCollection.SizeChangedViaEditor();
//				}
//			}
//			
//			for (int i = 0; i < list.arraySize; i++) {
//				SerializedProperty prop = list.GetArrayElementAtIndex(i);
//				if (prop.propertyType == SerializedPropertyType.String) {
//					string prev = prop.stringValue;
//					EditorGUILayout.PropertyField(prop);
//					if (prev != prop.stringValue) {
//						list.serializedObject.ApplyModifiedProperties ();
//						observableCollection.ValueChangedViaEditor(i);
//					}
//				}
//				else if (prop.propertyType == SerializedPropertyType.Float) {
//					float prev = prop.floatValue;
//					EditorGUILayout.PropertyField(prop);
//					if (prev != prop.floatValue) {
//						list.serializedObject.ApplyModifiedProperties ();
//						observableCollection.ValueChangedViaEditor(i);
//					}
//				}
//				else if (prop.propertyType == SerializedPropertyType.Color) {
//					Color prev = prop.colorValue;
//					EditorGUILayout.PropertyField(prop);
//					if (prev != prop.colorValue) {
//						list.serializedObject.ApplyModifiedProperties ();
//						observableCollection.ValueChangedViaEditor(i);
//					}
//				}
//				else if (prop.propertyType == SerializedPropertyType.Vector2) {
//					Vector2 prev = prop.vector2Value;
//					EditorGUILayout.PropertyField(prop);
//					if (prev != prop.vector2Value) {
//						list.serializedObject.ApplyModifiedProperties ();
//						observableCollection.ValueChangedViaEditor(i);
//					}
//				}
//				else if (prop.propertyType == SerializedPropertyType.ObjectReference) {
//					UnityEngine.Object prev = prop.objectReferenceValue;
//					EditorGUILayout.PropertyField(prop);
//					if (prev != prop.objectReferenceValue) {
//						list.serializedObject.ApplyModifiedProperties ();
//						observableCollection.ValueChangedViaEditor(i);
//					}
//				}
//			}
//		}
//		if (showListLabel) {
//			EditorGUI.indentLevel -= 1;
//		}
//	}

	public bool ExposeAndReturnBool(WMG_PropertyField field, string tooltip = "") {
		var emptyOptions = new GUILayoutOption[0];
		if (field.Type == SerializedPropertyType.Boolean)
		{
			bool oldValue = (bool)field.GetValue();
			bool newValue = EditorGUILayout.Toggle(new GUIContent(field.Name, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
			return newValue;
		}
		return false;
	}

	// Function to display properties in inspector, invokes setter for observable properties
	public void ExposeProperty(WMG_PropertyField field, string tooltip = "", string nameOverride = "")
	{
		string propDisplayName = nameOverride == "" ? field.Name : nameOverride;
		var emptyOptions = new GUILayoutOption[0];
		if (field.Type == SerializedPropertyType.Integer)
		{
			var oldValue = (int)field.GetValue();
			var newValue = EditorGUILayout.IntField(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Float)
		{
			var oldValue = (float)field.GetValue();
			var newValue = EditorGUILayout.FloatField(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Boolean)
		{
			var oldValue = (bool)field.GetValue();
			var newValue = EditorGUILayout.Toggle(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.String)
		{
			var oldValue = (string)field.GetValue();
			var newValue = EditorGUILayout.TextField(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Vector2)
		{
			var oldValue = (Vector2)field.GetValue();
			var newValue = EditorGUILayout.Vector2Field(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Vector3)
		{
			var oldValue = (Vector3)field.GetValue();
			var newValue = EditorGUILayout.Vector3Field(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Enum)
		{
			var oldValue = (Enum)field.GetValue();
			var newValue = EditorGUILayout.EnumPopup(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
		else if (field.Type == SerializedPropertyType.Color)
		{
			var oldValue = (Color)field.GetValue();
			var newValue = EditorGUILayout.ColorField(new GUIContent(propDisplayName, tooltip), oldValue, emptyOptions);
			if (oldValue != newValue)
				field.SetValue(newValue);
		}
	}
	
	public void ExposeEnumMaskProperty(WMG_PropertyField field) {
		var emptyOptions = new GUILayoutOption[0];
		var oldValue = (Enum)field.GetValue();
		var newValue = EditorGUILayout.EnumMaskField(field.Name, oldValue, emptyOptions);
		if (oldValue != newValue)
			field.SetValue(newValue);
	}


	
	public Dictionary<string, WMG_PropertyField> GetProperties(object obj)
	{
		Dictionary<string, WMG_PropertyField> fields = new Dictionary<string, WMG_PropertyField>();
		
		PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		
		foreach (PropertyInfo info in infos)
		{
			if (!(info.CanRead && info.CanWrite))
				continue;
			
			var type = SerializedPropertyType.Integer;
			if (WMG_PropertyField.GetPropertyType(info, out type))
			{
				var field = new WMG_PropertyField(obj, info, type);
				fields.Add(info.Name, field);
			}
		}

		return fields;
	}
}

public class WMG_PropertyField
{
	object obj;
	PropertyInfo info;
	SerializedPropertyType type;
	
	MethodInfo getter;
	MethodInfo setter;

	public SerializedPropertyType Type
	{
		get { return type; }
	}
	
	public String Name
	{
		get { return ObjectNames.NicifyVariableName(info.Name); }
	}
	
	public WMG_PropertyField(object obj, PropertyInfo info, SerializedPropertyType type)
	{
		this.obj = obj;
		this.info = info;
		this.type = type;
		
		getter = this.info.GetGetMethod();
		setter = this.info.GetSetMethod();
	}
	
	public object GetValue() { return getter.Invoke(obj, null); }
	public void SetValue(object value) { setter.Invoke(obj, new[] {value}); }
	
	public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType)
	{
		Type type = info.PropertyType;
		propertyType = SerializedPropertyType.Generic;
		if (type == typeof(int))
			propertyType = SerializedPropertyType.Integer;
		else if (type == typeof(float))
			propertyType = SerializedPropertyType.Float;
		else if (type == typeof(bool))
			propertyType = SerializedPropertyType.Boolean;
		else if (type == typeof(string))
			propertyType = SerializedPropertyType.String;
		else if (type == typeof(Vector2))
			propertyType = SerializedPropertyType.Vector2;
		else if (type == typeof(Vector3))
			propertyType = SerializedPropertyType.Vector3;
		else if (type.IsEnum)
			propertyType = SerializedPropertyType.Enum;
		else if (type == typeof(Color))
			propertyType = SerializedPropertyType.Color;
		return propertyType != SerializedPropertyType.Generic;
	}
}
