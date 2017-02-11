using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WMG_Series))]
public class WMG_Series_E : WMG_E_Util
{
	WMG_Series series;
	Dictionary<string, WMG_PropertyField> fields;

	enum eTabType
	{
		Core,
		Labels,
		Shading,
		Misc
	}

	private eTabType m_tabType = eTabType.Core;

	void OnEnable()
	{
		series = (WMG_Series)target;
		fields = GetProperties(series);
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
		case eTabType.Shading: DrawShading(); break;
		case eTabType.Misc: DrawMisc(); break;
		}								
		
		if( GUI.changed ) {
			EditorUtility.SetDirty( series );
		}
		
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	void DrawCore() {
		ArrayGUIoc<Vector2> (series.pointValues, "Point Values", "_pointValues");
		ExposeProperty(fields["comboType"]);
		ExposeProperty(fields["useSecondYaxis"]);
		ExposeProperty(fields["seriesName"]);
		ExposeProperty(fields["pointWidthHeight"]);
		ExposeProperty(fields["lineScale"]);
		ExposeProperty(fields["pointColor"]);
		ExposeProperty(fields["usePointColors"]);
		ArrayGUIoc<Color> (series.pointColors, "Point Colors", "_pointColors");
		ExposeProperty(fields["lineColor"]);
		if (ExposeAndReturnBool(fields["UseXDistBetweenToSpace"])) {
			ExposeProperty(fields["ManuallySetXDistBetween"]);
			ExposeProperty(fields["xDistBetweenPoints"]);
		}
		ExposeProperty(fields["ManuallySetExtraXSpace"]);
		ExposeProperty(fields["extraXSpace"]);
		ExposeProperty(fields["hidePoints"]);
		ExposeProperty(fields["hideLines"]);
		ExposeProperty(fields["connectFirstToLast"]);
		ExposeProperty(fields["linePadding"]);
	}

	void DrawLabels() {
		ExposeProperty(fields["dataLabelsEnabled"]);
		series.dataLabelPrefab = EditorGUILayout.ObjectField("Data Label Prefab", series.dataLabelPrefab, typeof(Object), false);
		ExposeProperty(fields["dataLabelsNumDecimals"]);
		ExposeProperty(fields["dataLabelsFontSize"]);
		ExposeProperty(fields["dataLabelsColor"]);
		ExposeProperty(fields["dataLabelsFontStyle"]);
		series.dataLabelsFont = (Font)EditorGUILayout.ObjectField ("Data Label Font", series.dataLabelsFont, typeof(Font), false);
		ExposeProperty(fields["dataLabelsAnchoredLeftBot"]);
		ExposeProperty(fields["dataLabelsOffset"]);
		series.dataLabelsParent = (GameObject)EditorGUILayout.ObjectField("Data Labels Parent", series.dataLabelsParent, typeof(GameObject), true);
	}

	void DrawShading() {
		ExposeProperty(fields["areaShadingType"]);
		ExposeProperty(fields["areaShadingUsesComputeShader"]);
		series.areaShadingMatSolid = (Material)EditorGUILayout.ObjectField ("Area Shading Mat Solid", series.areaShadingMatSolid, typeof(Material), false);
		series.areaShadingMatGradient = (Material)EditorGUILayout.ObjectField ("Area Shading Mat Gradient", series.areaShadingMatGradient, typeof(Material), false);
		series.areaShadingParent = (GameObject)EditorGUILayout.ObjectField("Area Shading Parent", series.areaShadingParent, typeof(GameObject), true);
		series.areaShadingPrefab = EditorGUILayout.ObjectField("Area Shading Prefab", series.areaShadingPrefab, typeof(Object), false);
		series.areaShadingCSPrefab = EditorGUILayout.ObjectField("Area Shading CS Prefab", series.areaShadingCSPrefab, typeof(Object), false);
		series.areaShadingTextureResolution = EditorGUILayout.IntField ("Area Shading Tex Res", series.areaShadingTextureResolution);
		ExposeProperty(fields["areaShadingColor"]);
		ExposeProperty(fields["areaShadingAxisValue"]);
	}

	void DrawMisc() {
		series.theGraph = (WMG_Axis_Graph)EditorGUILayout.ObjectField ("The Graph", series.theGraph, typeof(WMG_Axis_Graph), true);
		series.realTimeDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField ("Real Time Data Source", series.realTimeDataSource, typeof(WMG_Data_Source), true);
		series.pointValuesDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField ("Point Values Data Source", series.pointValuesDataSource, typeof(WMG_Data_Source), true);
		ExposeProperty(fields["pointPrefab"]);
		ExposeProperty(fields["linkPrefab"]);
		series.legendEntryPrefab = EditorGUILayout.ObjectField("Legend Entry Prefab", series.legendEntryPrefab, typeof(Object), false);
		series.linkParent = (GameObject)EditorGUILayout.ObjectField("Link Parent", series.linkParent, typeof(GameObject), true);
		series.nodeParent = (GameObject)EditorGUILayout.ObjectField("Node Parent", series.nodeParent, typeof(GameObject), true);
	}
}
