using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WMG_Pie_Graph))]
public class WMG_Pie_Graph_E : WMG_E_Util
{
	WMG_Pie_Graph graph;
	Dictionary<string, WMG_PropertyField> fields;

	enum eTabType
	{
		Core,
		OtherSlice,
		Anim,
		Labels,
		Misc
	}

	private eTabType m_tabType = eTabType.Core;

	void OnEnable()
	{
		graph = (WMG_Pie_Graph)target;
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
		case eTabType.OtherSlice: DrawOtherSlice(); break;
		case eTabType.Anim: DrawAnim(); break;
		case eTabType.Labels: DrawLabels(); break;
		case eTabType.Misc: DrawMisc(); break;
		}								

		// Update graphics based on graph width and height
		if (!Application.isPlaying) {
			UpdateSceneView();
		}

		if( GUI.changed ) {
			EditorUtility.SetDirty( graph );
		}
		
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	void UpdateSceneView() {
		graph.updateBG(Mathf.RoundToInt(graph.pieSize));
	}
	
	void DrawCore() {
		ExposeProperty(fields["resizeEnabled"]);
		ExposeEnumMaskProperty(fields["resizeProperties"]);
		ArrayGUIoc<float> (graph.sliceValues, "Values", "_sliceValues");
		ArrayGUIoc<string> (graph.sliceLabels, "Labels", "_sliceLabels");
		ArrayGUIoc<Color> (graph.sliceColors, "Colors", "_sliceColors");
		ExposeProperty(fields["leftRightPadding"]);
		ExposeProperty(fields["topBotPadding"]);
		ExposeProperty(fields["autoCenter"]);
		ExposeProperty(fields["autoCenterMinPadding"]);
		ExposeProperty(fields["bgCircleOffset"]);
		ExposeProperty(fields["sortBy"]);
		ExposeProperty(fields["swapColorsDuringSort"]);
		ExposeProperty(fields["sliceLabelType"]);
		ExposeProperty(fields["explodeLength"]);
		ExposeProperty(fields["explodeSymmetrical"], "Doesn't work with interactivity");
		if (ExposeAndReturnBool(fields["useDoughnut"])) {
			ExposeProperty(fields["doughnutPercentage"]);
		}
		ExposeProperty(fields["hideZeroValueLegendEntry"]);
		if (ExposeAndReturnBool(fields["interactivityEnabled"], "Replaces raycaster with custom raycaster. Doesn't work with Explode Symmetrical")) {
			graph.explodeSymmetrical = false;
		}
	}

	void DrawOtherSlice() {
		ExposeProperty(fields["limitNumberSlices"]);
		ExposeProperty(fields["includeOthers"]);
		ExposeProperty(fields["maxNumberSlices"]);
		ExposeProperty(fields["includeOthersLabel"]);
		ExposeProperty(fields["includeOthersColor"]);
	}

	void DrawAnim() {
		ExposeProperty(fields["animationDuration"]);
		ExposeProperty(fields["sortAnimationDuration"]);
	}

	void DrawLabels() {
		ExposeProperty(fields["sliceLabelExplodeLength"]);
		ExposeProperty(fields["sliceLabelFontSize"]);
		ExposeProperty(fields["numberDecimalsInPercents"]);
		ExposeProperty(fields["sliceLabelColor"]);
	}

	void DrawMisc() {
		graph.sliceValuesDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Values Data Source", graph.sliceValuesDataSource, typeof(WMG_Data_Source), true);
		graph.sliceLabelsDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Labels Data Source", graph.sliceLabelsDataSource, typeof(WMG_Data_Source), true);
		graph.sliceColorsDataSource = (WMG_Data_Source)EditorGUILayout.ObjectField("Colors Data Source", graph.sliceColorsDataSource, typeof(WMG_Data_Source), true);
		graph.background = (GameObject)EditorGUILayout.ObjectField("Background", graph.background, typeof(GameObject), true);
		graph.backgroundCircle = (GameObject)EditorGUILayout.ObjectField("Circle Background", graph.backgroundCircle, typeof(GameObject), true);
		graph.slicesParent = (GameObject)EditorGUILayout.ObjectField("Slices Parent", graph.slicesParent, typeof(GameObject), true);
		graph.legend = (WMG_Legend)EditorGUILayout.ObjectField("Legend", graph.legend, typeof(WMG_Legend), true);
		graph.legendEntryPrefab = EditorGUILayout.ObjectField("Legend Entry Prefab", graph.legendEntryPrefab, typeof(Object), false);
		graph.nodePrefab = EditorGUILayout.ObjectField("Slice Prefab", graph.nodePrefab, typeof(Object), false);
	}

}