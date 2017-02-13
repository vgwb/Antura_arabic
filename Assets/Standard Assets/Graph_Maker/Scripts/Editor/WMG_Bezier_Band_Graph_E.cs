using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[CustomEditor(typeof(WMG_Bezier_Band_Graph))] 
public class WMG_Bezier_Band_Graph_E : WMG_E_Util
{
	WMG_Bezier_Band_Graph graph;
	Dictionary<string, WMG_PropertyField> fields;

	enum eTabType
	{
		Core,
		Misc
	}
	
	private eTabType m_tabType = eTabType.Core;
	
	public void OnEnable()
	{
		graph = (WMG_Bezier_Band_Graph)target;
		fields = GetProperties(graph);
	}
	
	public override void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		
		string[] toolBarButtonNames = System.Enum.GetNames(typeof(eTabType));
		
		m_tabType = (eTabType)GUILayout.Toolbar((int)m_tabType, toolBarButtonNames);
		
		switch (m_tabType)
		{
		case eTabType.Core: DrawCore(); break;
		case eTabType.Misc: DrawMisc(); break;
		}								
		
		if( GUI.changed ) {
			EditorUtility.SetDirty( graph );
		}
		
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	void DrawCore() {
		ArrayGUIoc<float> (graph.values, "Values", "_values");
		ArrayGUIoc<string> (graph.labels, "Labels", "_labels");
		ArrayGUIoc<Color> (graph.fillColors, "Fill Colors", "_fillColors");
		ExposeProperty(fields["bandLineColor"]);
		ExposeProperty(fields["startHeightPercent"]);
		ExposeProperty(fields["bandSpacing"]);
		ExposeProperty(fields["bandLineWidth"]);
		ExposeProperty(fields["cubicBezierP1"]);
		ExposeProperty(fields["cubicBezierP2"]);
		ExposeProperty(fields["numDecimals"]);
		ExposeProperty(fields["fontSize"]);
	}

	void DrawMisc() {
		graph.bandsParent = (GameObject)EditorGUILayout.ObjectField("Bands Parent", graph.bandsParent, typeof(GameObject), true);
		graph.bandPrefab = EditorGUILayout.ObjectField("Band Prefab", graph.bandPrefab, typeof(Object), false);
		graph.textureResolution = EditorGUILayout.IntField ("Texture Resolution", graph.textureResolution);
	}
}