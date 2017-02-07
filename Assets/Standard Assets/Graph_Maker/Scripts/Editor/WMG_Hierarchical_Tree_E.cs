using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[CustomEditor(typeof(WMG_Hierarchical_Tree))] 
public class WMG_Hierarchical_Tree_E : WMG_E_Util
{
	WMG_Hierarchical_Tree graph;
	Dictionary<string, WMG_PropertyField> fields;

	enum eTabType
	{
		Core,
		Misc
	}
	
	private eTabType m_tabType = eTabType.Core;
	
	public void OnEnable()
	{
		graph = (WMG_Hierarchical_Tree)target;
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
		ExposeProperty(fields["resizeEnabled"]);
		ExposeEnumMaskProperty(fields["resizeProperties"]);
		graph.numNodes = EditorGUILayout.IntField ("Num Nodes", graph.numNodes);
		graph.numLinks = EditorGUILayout.IntField ("Num Links", graph.numLinks);
		ArrayGUI("Node Prefabs", "nodePrefabs");
		ArrayGUI("Node Columns", "nodeColumns");
		ArrayGUI("Node Rows", "nodeRows");
		ArrayGUI("Link Node From IDs", "linkNodeFromIDs");
		ArrayGUI("Link Node To IDs", "linkNodeToIDs");
		graph.numInvisibleNodes = EditorGUILayout.IntField ("Num Invisible Nodes", graph.numInvisibleNodes);
		ArrayGUI("Invisible Node Columns", "invisibleNodeColumns");
		ArrayGUI("Invisible Node Rows", "invisibleNodeRows");
		ExposeProperty (fields ["nodeWidthHeight"]);
		ExposeProperty (fields ["nodeRadius"]);
		ExposeProperty (fields ["squareNodes"]);
	}

	void DrawMisc() {
		graph.nodeParent = (GameObject)EditorGUILayout.ObjectField("Node Parent", graph.nodeParent, typeof(GameObject), true);
		graph.linkParent = (GameObject)EditorGUILayout.ObjectField("Link Parent", graph.linkParent, typeof(GameObject), true);
		graph.defaultNodePrefab = EditorGUILayout.ObjectField("Default Node Prefab", graph.defaultNodePrefab, typeof(Object), false);
		graph.linkPrefab = EditorGUILayout.ObjectField("Link Prefab", graph.linkPrefab, typeof(Object), false);
		graph.invisibleNodePrefab = EditorGUILayout.ObjectField("Invisible Node Prefab", graph.invisibleNodePrefab, typeof(Object), false);
	}
}