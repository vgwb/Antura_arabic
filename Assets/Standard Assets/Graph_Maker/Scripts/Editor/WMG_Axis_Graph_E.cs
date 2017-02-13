using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(WMG_Axis_Graph))]
public class WMG_Axis_Graph_E : WMG_E_Util
{
	WMG_Axis_Graph graph;
	Dictionary<string, WMG_PropertyField> fields;
	SerializedObject yAxisSO;
	SerializedObject yAxis2SO;
	SerializedObject xAxisSO;
	Dictionary<string, WMG_PropertyField> yAxisfields;
	Dictionary<string, WMG_PropertyField> yAxis2fields;
	Dictionary<string, WMG_PropertyField> xAxisfields;
	bool nullYaxis2;
	WMG_Axis_Graph.orientationTypes previousOrientationType;

	enum eTabType
	{
		Core,
		YAxis,
		YAxis2,
		XAxis,
		Tooltip,
		Anim,
		Misc
	}

	enum eTabTypeNullYaxis2
	{
		Core,
		YAxis,
		XAxis,
		Tooltip,
		Anim,
		Misc
	}

	private eTabType m_tabType = eTabType.Core;
	private eTabTypeNullYaxis2 m_tabTypeNullYaxis2 = eTabTypeNullYaxis2.Core;

	public void Init() {
		graph = (WMG_Axis_Graph)target;
		fields = GetProperties(graph);
		yAxisSO = new SerializedObject(serializedObject.FindProperty("yAxis").objectReferenceValue);
		nullYaxis2 = serializedObject.FindProperty("yAxis2").objectReferenceValue == null;
		if (!nullYaxis2) {
			yAxis2SO = new SerializedObject(serializedObject.FindProperty("yAxis2").objectReferenceValue);
		}
		xAxisSO = new SerializedObject(serializedObject.FindProperty("xAxis").objectReferenceValue);
		yAxisfields = GetProperties(graph.yAxis);
		if (!nullYaxis2) {
			yAxis2fields = GetProperties(graph.yAxis2);
		}
		xAxisfields = GetProperties(graph.xAxis);
	}

	void OnEnable()
	{
		Init();
	}

	public override void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		yAxisSO.Update();
		if (!nullYaxis2) {
			yAxis2SO.Update();
		}
		xAxisSO.Update();

		if (!nullYaxis2 && graph.axesType == WMG_Axis_Graph.axesTypes.DUAL_Y) {
			string[] toolBarButtonNames = System.Enum.GetNames(typeof(eTabType));
			
			m_tabType = (eTabType)GUILayout.Toolbar((int)m_tabType, toolBarButtonNames);

			switch (m_tabType)
			{
			case eTabType.Core: DrawCore(); break;
			case eTabType.YAxis: DrawYAxis(); break;
			case eTabType.YAxis2: DrawYAxis2(); break;
			case eTabType.XAxis: DrawXAxis(); break;
			case eTabType.Tooltip: DrawTooltip(); break;
			case eTabType.Anim: DrawAnim(); break;
			case eTabType.Misc: DrawMisc(); break;
			}
		}
		else {
			string[] toolBarButtonNames = System.Enum.GetNames(typeof(eTabTypeNullYaxis2));
			
			m_tabTypeNullYaxis2 = (eTabTypeNullYaxis2)GUILayout.Toolbar((int)m_tabTypeNullYaxis2, toolBarButtonNames);
			
			switch (m_tabTypeNullYaxis2)
			{
			case eTabTypeNullYaxis2.Core: DrawCore(); break;
			case eTabTypeNullYaxis2.YAxis: DrawYAxis(); break;
			case eTabTypeNullYaxis2.XAxis: DrawXAxis(); break;
			case eTabTypeNullYaxis2.Tooltip: DrawTooltip(); break;
			case eTabTypeNullYaxis2.Anim: DrawAnim(); break;
			case eTabTypeNullYaxis2.Misc: DrawMisc(); break;
			}
		}

		// In editor mode, update graphics based on graph width and height
		if (!Application.isPlaying) {
			UpdateSceneView();
		}

		if( GUI.changed ) {
			EditorUtility.SetDirty( graph );
			EditorUtility.SetDirty( yAxisSO.targetObject);
			if (!nullYaxis2) {
				EditorUtility.SetDirty( yAxis2SO.targetObject);
			}
			EditorUtility.SetDirty( xAxisSO.targetObject);

			if (!Application.isPlaying) {
				if (previousOrientationType != graph.orientationType) {
					graph.UpdateOrientation();
				}
			}
		}
		previousOrientationType = graph.orientationType;

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
		yAxisSO.ApplyModifiedProperties();
		if (!nullYaxis2) {
			yAxis2SO.ApplyModifiedProperties();
		}
		xAxisSO.ApplyModifiedProperties();
	}

	void UpdateSceneView() {
		graph.InEditorUpdate ();
	}
	
	void DrawCore() {
		ExposeProperty(fields["graphType"]);
		ExposeProperty(fields["orientationType"]);
		ExposeProperty(fields["axesType"]);
		ExposeProperty(fields["resizeEnabled"]);
		ExposeEnumMaskProperty(fields["resizeProperties"]);
		ExposeProperty(fields["useGroups"]);
		ArrayGUIoc<string> (graph.groups, "Groups", "_groups");
		ArrayGUI("Series", "lineSeries");
		ExposeProperty(fields["autoPaddingEnabled"]);
		ExposeEnumMaskProperty(fields["autoPaddingProperties"]);
		ExposeProperty(fields["autoPaddingAmount"]);
		if (!graph.autoPaddingEnabled) {
			ExposeProperty (fields ["paddingLeftRight"]);
			ExposeProperty (fields ["paddingTopBottom"]);
		}
		if (!ExposeAndReturnBool (fields ["autoUpdateOrigin"])) {
			ExposeProperty(fields["theOrigin"]);
		}
		if (!ExposeAndReturnBool (fields ["autoUpdateBarWidth"])) {
			ExposeProperty (fields ["barWidth"]);
		} else {
			ExposeProperty(fields["autoUpdateBarWidthSpacing"]);
		}
		if (!ExposeAndReturnBool (fields ["autoUpdateBarAxisValue"])) {
			ExposeProperty(fields["barAxisValue"]);
		}

		ExposeProperty(fields["autoUpdateSeriesAxisSpacing"]);
	}

	void DrawYAxis() {
		ExposeProperty(yAxisfields["AxisMinValue"]);
		ExposeProperty(yAxisfields["AxisMaxValue"]);
		ExposeProperty(yAxisfields["AxisNumTicks"]);
		ExposeProperty(yAxisfields["MinAutoGrow"]);
		ExposeProperty(yAxisfields["MaxAutoGrow"]);
		ExposeProperty(yAxisfields["MinAutoShrink"]);
		ExposeProperty(yAxisfields["MaxAutoShrink"]);
		ExposeProperty(yAxisfields["AxisLinePadding"]);
		ExposeProperty(yAxisfields["HideAxisArrowTopRight"], "" , "Hide Top Arrow");
		ExposeProperty(yAxisfields["HideAxisArrowBotLeft"], "" , "Hide Bot Arrow");
		ExposeProperty(yAxisfields["hideGrid"]);
		ExposeProperty(yAxisfields["hideTicks"]);
		ExposeProperty(yAxisfields["hideAxisLine"]);
		ExposeProperty(yAxisfields["AxisTitleString"]);
		ExposeProperty(yAxisfields["AxisTitleOffset"]);
		ExposeProperty(yAxisfields["AxisTitleFontSize"]);
		ExposeProperty(yAxisfields["AxisUseNonTickPercent"]);

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Label Parameters", EditorStyles.boldLabel);

		ExposeProperty(yAxisfields["hideLabels"]);
		ExposeProperty(yAxisfields["LabelType"]);
		ExposeProperty(yAxisfields["SetLabelsUsingMaxMin"], "Use the axis Min and Max values to automatically set the labels.");
		ArrayGUIoc<string> (((WMG_Axis)yAxisSO.targetObject).axisLabels, "Labels", "_axisLabels", yAxisSO);
		ExposeProperty(yAxisfields["AxisLabelSkipInterval"]);
		ExposeProperty(yAxisfields["AxisLabelSkipStart"]);
		ExposeProperty(yAxisfields["AxisLabelRotation"]);
		ExposeProperty(yAxisfields["AxisLabelSize"]);
		ExposeProperty(yAxisfields["AxisLabelColor"]);
		ExposeProperty(yAxisfields["AxisLabelFontStyle"]);
		graph.yAxis.AxisLabelFont = (Font)EditorGUILayout.ObjectField ("Axis Label Font", graph.yAxis.AxisLabelFont, typeof(Font), false);
		ExposeProperty(yAxisfields["numDecimalsAxisLabels"]);
		ExposeProperty(yAxisfields["AxisLabelSpaceOffset"]);

		if (graph.axesType == WMG_Axis_Graph.axesTypes.MANUAL) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Axes Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(yAxisfields["AxisNonTickPercent"]);
			ExposeProperty(yAxisfields["AxisTicksRightAbove"]);
			ExposeProperty(yAxisfields["AxisTick"]);
			ExposeProperty(yAxisfields["hideTick"]);
		}

		if (graph.yAxis.LabelType == WMG_Axis.labelTypes.manual) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Label Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(yAxisfields["AxisLabelSpacing"]);
			ExposeProperty(yAxisfields["AxisLabelDistBetween"]);
		}
	}

	void DrawYAxis2() {
		ExposeProperty(yAxis2fields["AxisMinValue"]);
		ExposeProperty(yAxis2fields["AxisMaxValue"]);
		ExposeProperty(yAxis2fields["AxisNumTicks"]);
		ExposeProperty(yAxis2fields["MinAutoGrow"]);
		ExposeProperty(yAxis2fields["MaxAutoGrow"]);
		ExposeProperty(yAxis2fields["MinAutoShrink"]);
		ExposeProperty(yAxis2fields["MaxAutoShrink"]);
		ExposeProperty(yAxis2fields["AxisLinePadding"]);
		ExposeProperty(yAxis2fields["HideAxisArrowTopRight"], "" , "Hide Top Arrow");
		ExposeProperty(yAxis2fields["HideAxisArrowBotLeft"], "" , "Hide Bot Arrow");
		ExposeProperty(yAxis2fields["hideGrid"]);
		ExposeProperty(yAxis2fields["hideTicks"]);
		ExposeProperty(yAxis2fields["hideAxisLine"]);
		ExposeProperty(yAxis2fields["AxisTitleString"]);
		ExposeProperty(yAxis2fields["AxisTitleOffset"]);
		ExposeProperty(yAxis2fields["AxisTitleFontSize"]);
		ExposeProperty(yAxis2fields["AxisUseNonTickPercent"]);
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Label Parameters", EditorStyles.boldLabel);

		ExposeProperty(yAxis2fields["hideLabels"]);
		ExposeProperty(yAxis2fields["LabelType"]);
		ExposeProperty(yAxis2fields["SetLabelsUsingMaxMin"], "Use the axis Min and Max values to automatically set the labels.");
		ArrayGUIoc<string> (((WMG_Axis)yAxis2SO.targetObject).axisLabels, "Labels", "_axisLabels", yAxis2SO);
		ExposeProperty(yAxis2fields["AxisLabelSkipInterval"]);
		ExposeProperty(yAxis2fields["AxisLabelSkipStart"]);
		ExposeProperty(yAxis2fields["AxisLabelRotation"]);
		ExposeProperty(yAxis2fields["AxisLabelSize"]);
		ExposeProperty(yAxis2fields["AxisLabelColor"]);
		ExposeProperty(yAxis2fields["AxisLabelFontStyle"]);
		graph.yAxis2.AxisLabelFont = (Font)EditorGUILayout.ObjectField ("Axis Label Font", graph.yAxis2.AxisLabelFont, typeof(Font), false);
		ExposeProperty(yAxis2fields["numDecimalsAxisLabels"]);
		ExposeProperty(yAxis2fields["AxisLabelSpaceOffset"]);
		
		if (graph.axesType == WMG_Axis_Graph.axesTypes.MANUAL) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Axes Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(yAxis2fields["AxisNonTickPercent"]);
			ExposeProperty(yAxis2fields["AxisTicksRightAbove"]);
			ExposeProperty(yAxis2fields["AxisTick"]);
			ExposeProperty(yAxis2fields["hideTick"]);
		}
		
		if (graph.yAxis2.LabelType == WMG_Axis.labelTypes.manual) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Label Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(yAxis2fields["AxisLabelSpacing"]);
			ExposeProperty(yAxis2fields["AxisLabelDistBetween"]);
		}
	}

	void DrawXAxis() {
		ExposeProperty(xAxisfields["AxisMinValue"]);
		ExposeProperty(xAxisfields["AxisMaxValue"]);
		ExposeProperty(xAxisfields["AxisNumTicks"]);
		ExposeProperty(xAxisfields["MinAutoGrow"]);
		ExposeProperty(xAxisfields["MaxAutoGrow"]);
		ExposeProperty(xAxisfields["MinAutoShrink"]);
		ExposeProperty(xAxisfields["MaxAutoShrink"]);
		ExposeProperty(xAxisfields["AxisLinePadding"]);
		ExposeProperty(xAxisfields["HideAxisArrowTopRight"], "" , "Hide Right Arrow");
		ExposeProperty(xAxisfields["HideAxisArrowBotLeft"], "" , "Hide Left Arrow");
		ExposeProperty(xAxisfields["hideGrid"]);
		ExposeProperty(xAxisfields["hideTicks"]);
		ExposeProperty(xAxisfields["hideAxisLine"]);
		ExposeProperty(xAxisfields["AxisTitleString"]);
		ExposeProperty(xAxisfields["AxisTitleOffset"]);
		ExposeProperty(xAxisfields["AxisTitleFontSize"]);
		ExposeProperty(xAxisfields["AxisUseNonTickPercent"]);
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Label Parameters", EditorStyles.boldLabel);

		ExposeProperty(xAxisfields["hideLabels"]);
		ExposeProperty(xAxisfields["LabelType"]);
		ExposeProperty(xAxisfields["SetLabelsUsingMaxMin"], "Use the axis Min and Max values to automatically set the labels.");
		ArrayGUIoc<string> (((WMG_Axis)xAxisSO.targetObject).axisLabels, "Labels", "_axisLabels", xAxisSO);
		ExposeProperty(xAxisfields["AxisLabelSkipInterval"]);
		ExposeProperty(xAxisfields["AxisLabelSkipStart"]);
		ExposeProperty(xAxisfields["AxisLabelRotation"]);
		ExposeProperty(xAxisfields["AxisLabelSize"]);
		ExposeProperty(xAxisfields["AxisLabelColor"]);
		ExposeProperty(xAxisfields["AxisLabelFontStyle"]);
		graph.xAxis.AxisLabelFont = (Font)EditorGUILayout.ObjectField ("Axis Label Font", graph.xAxis.AxisLabelFont, typeof(Font), false);
		ExposeProperty(xAxisfields["numDecimalsAxisLabels"]);
		ExposeProperty(xAxisfields["AxisLabelSpaceOffset"]);

		if (graph.axesType == WMG_Axis_Graph.axesTypes.MANUAL) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Axes Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(xAxisfields["AxisNonTickPercent"]);
			ExposeProperty(xAxisfields["AxisTicksRightAbove"]);
			ExposeProperty(xAxisfields["AxisTick"]);
			ExposeProperty(xAxisfields["hideTick"]);
		}

		if (graph.xAxis.LabelType == WMG_Axis.labelTypes.manual) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Manual Label Type Parameters", EditorStyles.boldLabel);
			ExposeProperty(xAxisfields["AxisLabelSpacing"]);
			ExposeProperty(xAxisfields["AxisLabelDistBetween"]);
		}
	}

	void DrawTooltip() {
		ExposeProperty(fields["tooltipEnabled"]);
		graph.tooltipOffset = EditorGUILayout.Vector2Field("Offset", graph.tooltipOffset);
		graph.tooltipNumberDecimals = EditorGUILayout.IntField("Number Decimals", graph.tooltipNumberDecimals);
		graph.tooltipDisplaySeriesName = EditorGUILayout.Toggle ("Display Series Name", graph.tooltipDisplaySeriesName);
	}

	void DrawAnim() {
		graph.tooltipAnimationsEnabled = EditorGUILayout.Toggle ("Tooltip Animations Enabled", graph.tooltipAnimationsEnabled);
		graph.tooltipAnimationsEasetype = (Ease)EditorGUILayout.EnumPopup("Tooltip Animations Easetype", graph.tooltipAnimationsEasetype);
		graph.tooltipAnimationsDuration = EditorGUILayout.FloatField("Tooltip Animations Duration", graph.tooltipAnimationsDuration);
		ExposeProperty(fields["autoAnimationsEnabled"]);
		graph.autoAnimationsEasetype = (Ease)EditorGUILayout.EnumPopup("Auto Animations Easetype", graph.autoAnimationsEasetype);
		graph.autoAnimationsDuration = EditorGUILayout.FloatField("Auto Animations Duration", graph.autoAnimationsDuration);
	}

	void DrawMisc() {
		ExposeProperty(fields["axisWidth"]);
		ExposeProperty(fields["autoShrinkAtPercent"]);
		ExposeProperty(fields["autoGrowAndShrinkByPercent"]);
		ArrayGUI("Point Prefabs", "pointPrefabs");
		ArrayGUI("Link Prefabs", "linkPrefabs");
		graph.barPrefab = EditorGUILayout.ObjectField("Bar Prefab", graph.barPrefab, typeof(Object), false);
		graph.seriesPrefab = EditorGUILayout.ObjectField("Series Prefab", graph.seriesPrefab, typeof(Object), false);
		ExposeProperty(fields["tickSize"]);
		graph.resizingChangesFontScaleInsteadOfFontSize = EditorGUILayout.Toggle 
			(new GUIContent("Resize - Font Scale", "For Resize functionality - If true, label objects resize using localScale, otherwise font size."), 
			 graph.resizingChangesFontScaleInsteadOfFontSize);
		ExposeProperty(fields["graphTitleString"]);
		ExposeProperty(fields["graphTitleOffset"]);
		ExposeProperty(fields["graphTitleSize"]);
		graph.yAxis = (WMG_Axis)EditorGUILayout.ObjectField("Y Axis", graph.yAxis, typeof(WMG_Axis), true);
		graph.yAxis2 = (WMG_Axis)EditorGUILayout.ObjectField("Y Axis 2", graph.yAxis2, typeof(WMG_Axis), true);
		graph.xAxis = (WMG_Axis)EditorGUILayout.ObjectField("X Axis", graph.xAxis, typeof(WMG_Axis), true);
		graph.legend = (WMG_Legend)EditorGUILayout.ObjectField("Legend", graph.legend, typeof(WMG_Legend), true);
		graph.anchoredParent = (GameObject)EditorGUILayout.ObjectField("Anchored Parent", graph.anchoredParent, typeof(GameObject), true);
		graph.graphAreaBoundsParent = (GameObject)EditorGUILayout.ObjectField("Graph Area Bounds", graph.graphAreaBoundsParent, typeof(GameObject), true);
		graph.graphBackground = (GameObject)EditorGUILayout.ObjectField("Graph Background", graph.graphBackground, typeof(GameObject), true);
		graph.seriesParent = (GameObject)EditorGUILayout.ObjectField("Series Parent", graph.seriesParent, typeof(GameObject), true);
	}
	

}