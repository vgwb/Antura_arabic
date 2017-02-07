using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(WMG_Radar_Graph))]
public class WMG_Radar_Graph_E : WMG_Axis_Graph_E
{
	WMG_Radar_Graph radar;
	Dictionary<string, WMG_PropertyField> fields;

	void OnEnable()
	{
		radar = (WMG_Radar_Graph)target;
		fields = GetProperties(radar);

		Init();
	}

	public override void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		if( GUI.changed ) {
			EditorUtility.SetDirty( radar );
		}

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Radar Graph Parameters", EditorStyles.boldLabel);

		radar.randomData = EditorGUILayout.Toggle ("Random Data", radar.randomData);
		ExposeProperty(fields["numPoints"]);
		ExposeProperty(fields["offset"]);
		ExposeProperty(fields["degreeOffset"]);
		ExposeProperty(fields["radarMinVal"]);
		ExposeProperty(fields["radarMaxVal"]);
		ExposeProperty(fields["numGrids"]);
		ExposeProperty(fields["gridLineWidth"]);
		ExposeProperty(fields["gridColor"]);
		ExposeProperty(fields["numDataSeries"]);
		ExposeProperty(fields["dataSeriesLineWidth"]);
		ArrayGUIoc<Color> (radar.dataSeriesColors, "Data Series Colors", "_dataSeriesColors");
		ExposeProperty(fields["labelsColor"]);
		ExposeProperty(fields["labelsOffset"]);
		ExposeProperty(fields["fontSize"]);
		ArrayGUIoc<string> (radar.labelStrings, "Label Strings", "_labelStrings");
		ExposeProperty(fields["hideLabels"]);
		
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Axis Graph Parameters", EditorStyles.boldLabel);

		base.OnInspectorGUI();
	}
	

}