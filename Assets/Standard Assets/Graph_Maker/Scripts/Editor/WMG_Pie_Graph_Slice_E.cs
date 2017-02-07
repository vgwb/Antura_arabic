using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WMG_Pie_Graph_Slice))]
public class WMG_Pie_Graph_Slice_E : Editor
{
	WMG_Pie_Graph_Slice slice;

	void OnEnable()
	{
		slice = (WMG_Pie_Graph_Slice)target;
	}

	public override void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		DrawCore();
		
		if( GUI.changed ) {
			EditorUtility.SetDirty( slice );
		}
		
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}	
	
	void DrawCore() {
		slice.id = EditorGUILayout.IntField("Id", slice.id);
		slice.objectToScale = (GameObject)EditorGUILayout.ObjectField("Object To Scale", slice.objectToScale, typeof(GameObject), true);
		slice.objectToColor = (GameObject)EditorGUILayout.ObjectField("Object To Color", slice.objectToColor, typeof(GameObject), true);
		slice.objectToLabel = (GameObject)EditorGUILayout.ObjectField("Object To Label", slice.objectToLabel, typeof(GameObject), true);
		slice.objectToMask = (GameObject)EditorGUILayout.ObjectField("Object To Mask", slice.objectToMask, typeof(GameObject), true);
	}
}