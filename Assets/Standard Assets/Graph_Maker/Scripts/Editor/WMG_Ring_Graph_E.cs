using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[CustomEditor(typeof(WMG_Ring_Graph))] 
public class WMG_Ring_Graph_E : WMG_E_Util
{
	WMG_Ring_Graph graph;
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
		graph = (WMG_Ring_Graph)target;
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
		case eTabType.Labels: DrawLabels(); break;
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
		ArrayGUIoc<bool> (graph.hideRings, "HideRings", "_hideRings");
		ExposeProperty (fields ["bandMode"]);
		ExposeProperty (fields ["pieMode"]);
		ExposeProperty (fields ["pieModePaddingDegrees"]);
		ExposeProperty (fields ["pieModeDegreeOffset"]);
		ExposeProperty (fields ["innerRadiusPercentage"]);
		ExposeProperty (fields ["degrees"]);
		ExposeProperty (fields ["minValue"]);
		ExposeProperty (fields ["maxValue"]);
		ExposeProperty (fields ["bandColor"]);
		ArrayGUIoc<Color> (graph.bandColors, "Band Colors", "_bandColors");
		ExposeProperty (fields ["autoUpdateBandAlpha"]);
		ExposeProperty (fields ["autoUpdateBandAlphaReverse"]);
		ExposeProperty (fields ["ringColor"]);
		ExposeProperty (fields ["ringWidth"]);
		ExposeProperty (fields ["bandPadding"]);
		ExposeProperty (fields ["leftRightPadding"]);
		ExposeProperty (fields ["topBotPadding"]);
		ExposeProperty (fields ["antiAliasing"]);
		ExposeProperty (fields ["antiAliasingStrength"]);
	}

	void DrawLabels() {
		ExposeProperty (fields ["hideZeroLabelLine"]);
		ExposeProperty (fields ["labelStartCenteredOnBand"]);
		ExposeProperty (fields ["labelPointSize"]);
		ExposeProperty (fields ["labelLinePadding"]);
		ExposeProperty (fields ["labelTextOffset"]);
		ExposeProperty (fields ["labelLineColor"]);
		ExposeProperty (fields ["labelPointColor"]);
		ExposeProperty (fields ["ringPointWidthFactor"]);
		ExposeProperty(fields["labelsFontSize"]);
		ExposeProperty(fields["labelsColor"]);
		ExposeProperty(fields["labelsFontStyle"]);
		graph.labelsFont = (Font)EditorGUILayout.ObjectField ("Label Font", graph.labelsFont, typeof(Font), false);
		ExposeProperty (fields ["showDataLabels"]);
		ExposeProperty(fields["dataLabelsFontSize"]);
		ExposeProperty(fields["dataLabelsColor"]);
		ExposeProperty(fields["dataLabelsFontStyle"]);
		graph.dataLabelsFont = (Font)EditorGUILayout.ObjectField ("Data Label Font", graph.dataLabelsFont, typeof(Font), false);
	}

	void DrawMisc() {
		graph.textureResolution = EditorGUILayout.IntField ("Texture Resolution", graph.textureResolution);
		graph.animateData = EditorGUILayout.Toggle ("Animate Data", graph.animateData);
		graph.animEaseType = (Ease)EditorGUILayout.EnumPopup("Anim Ease Type", graph.animEaseType);
		graph.animDuration = EditorGUILayout.FloatField("Anim Duration", graph.animDuration);
		ExposeProperty (fields ["useComputeShader"]);
		graph.computeShader = (ComputeShader)EditorGUILayout.ObjectField ("Compute Shader", graph.computeShader, typeof(ComputeShader), false);
		ArrayGUIoc<string> (graph.ringIDs, "Ring IDs", "_ringIDs");
		graph.ringPrefab = EditorGUILayout.ObjectField("Ring Prefab", graph.ringPrefab, typeof(Object), false);
		graph.contentParent = (GameObject)EditorGUILayout.ObjectField("Content Parent", graph.contentParent, typeof(GameObject), true);
		graph.valuesDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Values Data Source", graph.valuesDataSource, typeof(WMG_Data_Source), true);
		graph.labelsDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Labels Data Source", graph.labelsDataSource, typeof(WMG_Data_Source), true);
		graph.ringIDsDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Ring IDs Data Source", graph.ringIDsDataSource, typeof(WMG_Data_Source), true);
		graph.extraRing = (GameObject)EditorGUILayout.ObjectField("Extra Ring", graph.extraRing, typeof(GameObject), true);
		graph.ringsParent = (GameObject)EditorGUILayout.ObjectField("Rings Parent", graph.ringsParent, typeof(GameObject), true);
		graph.ringLabelsParent = (GameObject)EditorGUILayout.ObjectField("Ring Labels Parent", graph.ringLabelsParent, typeof(GameObject), true);
		graph.zeroLine = (GameObject)EditorGUILayout.ObjectField("Zero Line", graph.zeroLine, typeof(GameObject), true);
		graph.zeroLineText = (GameObject)EditorGUILayout.ObjectField("Zero Line Text", graph.zeroLineText, typeof(GameObject), true);
		graph.labelLineSprite = (Sprite)EditorGUILayout.ObjectField("Label Line Sprite", graph.labelLineSprite, typeof(Sprite), false);
		graph.botLeftCorners = (Sprite)EditorGUILayout.ObjectField("Bot Left Sprite", graph.botLeftCorners, typeof(Sprite), false);
		graph.botRightCorners = (Sprite)EditorGUILayout.ObjectField("Bot Right Sprite", graph.botRightCorners, typeof(Sprite), false);
	}
}