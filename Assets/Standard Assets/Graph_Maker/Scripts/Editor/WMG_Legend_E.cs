using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WMG_Legend))] 
public class WMG_Legend_E : WMG_E_Util
{
	WMG_Legend legend;
	Dictionary<string, WMG_PropertyField> fields;

	enum eTabType
	{
		Core,
		Labels,
		Misc
	}
	
	private eTabType m_tabType = eTabType.Core;
	
	public void OnEnable()
	{
		legend = (WMG_Legend)target;
		fields = GetProperties(legend);
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
		case eTabType.Labels: DrawLabels(); break;
		case eTabType.Misc: DrawMisc(); break;
		}	

		if( GUI.changed ) {
			EditorUtility.SetDirty( legend );
		}

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	void DrawCore() {
		ExposeProperty(fields["hideLegend"]);
		ExposeProperty(fields["legendType"]);
		ExposeProperty(fields["showBackground"]);
		ExposeProperty(fields["oppositeSideLegend"]);
		ExposeProperty(fields["offset"]);
		ExposeProperty(fields["setWidthFromLabels"]);
		ExposeProperty(fields["legendEntryWidth"]);
		ExposeProperty(fields["legendEntryHeight"]);
		ExposeProperty(fields["numRowsOrColumns"]);
		ExposeProperty(fields["legendEntryLinkSpacing"]);
		ExposeProperty(fields["legendEntrySpacing"]);
		ExposeProperty(fields["pieSwatchSize"]);
		ExposeProperty(fields["backgroundPadding"]);
		ExposeProperty(fields["autofitEnabled"], "Changes the number rows / columns to best fit the graph width / height");
	}

	void DrawLabels() {
		ExposeProperty(fields["labelType"]);
		ExposeProperty(fields["numDecimals"]);
		ExposeProperty(fields["legendEntryFontSize"]);
		ExposeProperty(fields["labelColor"]);
		ExposeProperty(fields["legendEntryFontStyle"]);
		legend.legendEntryFont = (Font)EditorGUILayout.ObjectField ("Legend Entry Font", legend.legendEntryFont, typeof(Font), false);
	}

	void DrawMisc() {
		legend.theGraph = (WMG_Graph_Manager)EditorGUILayout.ObjectField ("The Graph", legend.theGraph, typeof(WMG_Graph_Manager), true);
		legend.background = (GameObject)EditorGUILayout.ObjectField("Background", legend.background, typeof(GameObject), true);
		legend.entriesParent = (GameObject)EditorGUILayout.ObjectField("Entries Parent", legend.entriesParent, typeof(GameObject), true);
		legend.emptyPrefab = EditorGUILayout.ObjectField("Empty Prefab", legend.emptyPrefab, typeof(Object), false);
		ArrayGUI("Legend Entries", "legendEntries");
	}
}