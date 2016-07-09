using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Take a look at PropertyDrawer Here's how to abuse PropertyDrawer for annotated #unity3d inspectors
// http://mwegner.com/misc/InspectorNoteExample.zip http://pbs.twimg.com/media/BO3sc7SCYAAhXU3.png
// --
// See https://code.google.com/p/hounitylibs/source/browse/trunk/HOEditorGUIFramework/HOGUIStyle.cs
namespace PygmyMonkey.AdvancedBuilder.Utils
{
	public static class LegacyGUIStyle
	{
	    public static GUIStyle LabelTitleStyle;
		public static GUIStyle LabelBigStyle;
		public static GUIStyle LabelVersionStyle;
		public static GUIStyle LabelCategoryStyle;
		
		public static GUIStyle ButtonStyle;
		public static GUIStyle ButtonDeleteStyle;

		public static GUIStyle CenterMarginStyle;

		public static GUIStyle DropItemBoxStyle;
		
		static LegacyGUIStyle()
	    {
			LabelTitleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) };
			LabelBigStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, padding = new RectOffset(0, 0, 0, 0) };
			LabelVersionStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) };
			LabelCategoryStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, 0, 0) };
			
			ButtonStyle = new GUIStyle(GUI.skin.button) { fixedHeight = 30, margin = new RectOffset(50, 50, 10, 0) };
			ButtonDeleteStyle = new GUIStyle(GUI.skin.button) { fixedHeight = 20, margin = new RectOffset(150, 150, 0, 5) };

			CenterMarginStyle = new GUIStyle() { margin = new RectOffset(100, 100, 0, 0) };

			DropItemBoxStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
	    }
	}
}