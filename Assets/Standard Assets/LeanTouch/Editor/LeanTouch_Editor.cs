using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Lean
{
	[CustomEditor(typeof(LeanTouch))]
	public class LeanTouch_Editor : Editor
	{
		private static List<LeanFinger> allFingers = new List<LeanFinger>();
		
		private static GUIStyle fadingLabel;
		
		public static GUIStyle GetFadingLabel(bool active, float progress)
		{
			if (fadingLabel == null)
			{
				fadingLabel = new GUIStyle(EditorStyles.label);
			}
			
			var a = EditorStyles.label.normal.textColor;
			var b = a; b.a = active == true ? 0.5f : 0.0f;
			
			fadingLabel.normal.textColor = Color.Lerp(a, b, progress);
			
			return fadingLabel;
		}
		
		[MenuItem("GameObject/Lean/Touch", false, 1)]
		public static void CreateLocalization()
		{
			var gameObject = new GameObject(typeof(LeanTouch).Name);
			
			UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create Touch");
			
			gameObject.AddComponent<LeanTouch>();
			
			Selection.activeGameObject = gameObject;
		}
		
		// Draw the whole inspector
		public override void OnInspectorGUI()
		{
			var touch = (LeanTouch)target;
			
			EditorGUILayout.Separator();
			
			DrawSettings(touch);
			
			EditorGUILayout.Separator();
			
			DrawGestures();
			
			EditorGUILayout.Separator();
			
			DrawFingers(touch);
			
			EditorGUILayout.Separator();
			
			Repaint();
		}
		
		private static void DrawSettings(LeanTouch touch)
		{
			EditorGUI.LabelField(Reserve(), "Settings", EditorStyles.boldLabel);
			
			touch.TapThreshold = EditorGUILayout.FloatField("Tap Threshold", touch.TapThreshold);
			
			touch.SwipeThreshold = EditorGUILayout.FloatField("Swipe Threshold", touch.SwipeThreshold);
			
			touch.HeldThreshold = EditorGUILayout.FloatField("Held Threshold", touch.HeldThreshold);
			
			touch.ReferenceDpi = EditorGUILayout.IntField("Reference DPI", touch.ReferenceDpi);
			
			EditorGUILayout.Separator();
			
			touch.RecordFingers = EditorGUILayout.Toggle("Record Fingers", touch.RecordFingers);
			
			if (touch.RecordFingers == true)
			{
				EditorGUI.indentLevel += 1;
				{
					touch.RecordThreshold = EditorGUILayout.FloatField("Record Threshold", touch.RecordThreshold);
					
					touch.RecordLimit = EditorGUILayout.FloatField("Record Limit", touch.RecordLimit);
				}
				EditorGUI.indentLevel -= 1;
			}
			
			EditorGUILayout.Separator();
			
			touch.SimulateMultiFingers = EditorGUILayout.Toggle("Simulate Multi Fingers", touch.SimulateMultiFingers);
			
			if (touch.SimulateMultiFingers == true)
			{
				EditorGUI.indentLevel += 1;
				{
					touch.PinchTwistKey = (KeyCode)EditorGUILayout.EnumPopup("Pinch Twist Key", touch.PinchTwistKey);
					
					touch.MultiDragKey = (KeyCode)EditorGUILayout.EnumPopup("Multi Drag Key", touch.MultiDragKey);
					
					touch.FingerTexture = (Texture2D)EditorGUI.ObjectField(Reserve(), "Touch Texture", touch.FingerTexture, typeof(Texture2D), true);
				}
				EditorGUI.indentLevel -= 1;
			}
		}
		
		private static void DrawFingers(LeanTouch touch)
		{
			EditorGUI.LabelField(Reserve(), "Fingers", EditorStyles.boldLabel);
			
			allFingers.Clear();
			allFingers.AddRange(LeanTouch.Fingers);
			allFingers.AddRange(LeanTouch.InactiveFingers);
			allFingers.Sort((a, b) => a.Index.CompareTo(b.Index));
			
			for (var i = 0; i < allFingers.Count; i++)
			{
				var finger   = allFingers[i];
				var progress = touch.TapThreshold > 0.0f ? finger.Age / touch.TapThreshold : 0.0f;
				var style    = GetFadingLabel(finger.Set, progress);
				
				if (style.normal.textColor.a > 0.0f)
				{
					var screenPosition = finger.ScreenPosition;
					
					EditorGUILayout.LabelField("#" + finger.Index + " x " + finger.TapCount + " (" + Mathf.FloorToInt(screenPosition.x) + ", " + Mathf.FloorToInt(screenPosition.y) + ") - " + finger.Age.ToString("0.0"), style);
				}
			}
		}
		
		private static void DrawGestures()
		{
			EditorGUI.LabelField(Reserve(), "Gestures", EditorStyles.boldLabel);
			
			EditorGUI.BeginDisabledGroup(true);
			{
				DrawVector2("Drag Delta", LeanTouch.DragDelta);
				
				DrawVector2("Solo Drag Delta", LeanTouch.SoloDragDelta);
				
				DrawVector2("Multi Drag Delta", LeanTouch.MultiDragDelta);
				
				EditorGUILayout.FloatField("Twist Degrees", LeanTouch.TwistDegrees);
				
				EditorGUILayout.FloatField("Twist Radians", LeanTouch.TwistRadians);
				
				EditorGUILayout.FloatField("Pinch Scale", LeanTouch.PinchScale);
			}
			EditorGUI.EndDisabledGroup();
		}
		
		private static void DrawVector2(string name, Vector2 xy)
		{
			var left   = Reserve();
			var middle = Reserve(ref left);
			var right  = Reserve(ref middle, middle.width / 2);
			
			EditorGUI.LabelField(left, name);
			EditorGUI.FloatField(middle, xy.x);
			EditorGUI.FloatField(right, xy.y);
		}
		
		private static Rect Reserve(ref Rect rect, float rightWidth = 0.0f, float padding = 2.0f)
		{
			if (rightWidth == 0.0f)
			{
				rightWidth = rect.width - EditorGUIUtility.labelWidth;
			}
			
			var left  = rect; left.xMax -= rightWidth;
			var right = rect; right.xMin = left.xMax;
			
			left.xMax -= padding;
			
			rect = left;
			
			return right;
		}
		
		private static Rect Reserve(float height = 16.0f)
		{
			var rect = EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField("", GUILayout.Height(height));
			}
			EditorGUILayout.EndVertical();
			
			return rect;
		}
	}
}