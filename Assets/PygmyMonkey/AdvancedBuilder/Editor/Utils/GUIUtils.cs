using UnityEngine;
using UnityEditor;
using System;

namespace PygmyMonkey.AdvancedBuilder.Utils
{
	public static class GUIUtils
	{
		public static void DrawTwoColumns(string column1, string column2)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(column1, GUILayout.Width(143f));
			EditorGUILayout.LabelField(column2);
			EditorGUILayout.EndHorizontal();
		}

		public static void DrawCategoryLabel(string label)
		{
			GUILayout.Space(5f);
		
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1f)});
			EditorGUILayout.LabelField(label, LegacyGUIStyle.LabelCategoryStyle);
			GUILayout.Space(2f);
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1f)});
			
			GUILayout.Space(5f);
		}


		public static bool DrawBigButton(string label) { return DrawBigButton(label, Color.white); }
		public static bool DrawBigButton(string label, Color color) { return DrawBigButton(label, color, LegacyGUIStyle.ButtonStyle); }
		public static bool DrawBigButton(string label, Color color, GUIStyle style)
		{
			bool result = false;
			
			GUI.backgroundColor = color;
			
			if (GUILayout.Button(label, style))
			{
				result = true;
			}
			
			GUI.backgroundColor = Color.white;
			
			return result;
		}

		public static bool DrawDeleteRedButton() { return DrawDeleteRedButton("Delete"); }
		public static bool DrawDeleteRedButton(string name)
		{
			return DrawBigButton(name, Color.red, LegacyGUIStyle.ButtonDeleteStyle);
		}


		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title)
		{
			bool isActive = false;
			return DrawHeader(title, false, ref isActive);
		}

		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title, bool isLocked)
		{
			return DrawHeader(title, title, isLocked);
		}

		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title, string key)
		{
			bool isActive = false;
			return DrawHeader(title, key, false, ref isActive, false);
		}

		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title, string key, bool isLocked)
		{
			bool isActive = false;
			return DrawHeader(title, key, false, ref isActive, isLocked);
		}

		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title, bool drawToggle, ref bool isEnabled)
		{
			return DrawHeader(title, title, drawToggle, ref isEnabled, false);
		}

		/// <summary>
		/// Draw a distinctly different looking header label
		/// </summary>
		public static bool DrawHeader(string title, string key, bool drawToggle, ref bool isEnabled, bool isLocked)
		{
			bool state = isLocked ? true : EditorPrefs.GetBool(key, true);

			GUILayout.Space(5f);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(3f);
				
				bool hasStateChanged = false;

				if (drawToggle)
				{
					isEnabled = EditorGUILayout.Toggle(string.Empty, isEnabled, GUILayout.Width(15f), GUILayout.Height(15f));
				}

				if (!state)
				{
					GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
				}

				if (!GUILayout.Toggle(true, "<b><size=11>" + title + "</size></b>", "dragtab"))
				{
					if (!isLocked)
					{
						state = !state;
						hasStateChanged = true;
					}
				}
				
				if (hasStateChanged)
				{
					if (!isLocked)
					{
						EditorPrefs.SetBool(key, state);
					}
				}
				
				GUILayout.Space(2f);
			}
			EditorGUILayout.EndHorizontal();
			
			GUI.backgroundColor = Color.white;
			
			if (!state)
			{
				GUILayout.Space(3f);
			}
			
			return state;
		}


		/// <summary>
		/// Begin drawing the content area.
		/// </summary>
		public static void BeginContents()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(4f);
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
			EditorGUILayout.BeginVertical();
			GUILayout.Space(4f);
		}


		/// <summary>
		/// End drawing the content area.
		/// </summary>
		public static void EndContents()
		{
			GUILayout.Space(5f);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(3f);
			EditorGUILayout.EndHorizontal();
		}


		public static void RefreshAssets()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public class GUIContents : IDisposable
		{
			public GUIContents()
			{
				BeginContents();
			}

			public void Dispose()
			{
				EndContents();
			}
		}

		public class GUIHorizontal : IDisposable
		{
			public GUIHorizontal(params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginHorizontal(options);
			}

			public GUIHorizontal(GUIStyle style, params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginHorizontal(style, options);
			}

			public void Dispose()
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		public class GUIVertical : IDisposable
		{
			public GUIVertical(params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginVertical(options);
			}

			public GUIVertical(GUIStyle style, params GUILayoutOption[] options)
			{
				EditorGUILayout.BeginVertical(style, options);
			}

			public void Dispose()
			{
				EditorGUILayout.EndVertical();
			}
		}

		public class GUIScrollView : IDisposable
		{
			public GUIScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options)
			{
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
			}

			public void Dispose()
			{
				EditorGUILayout.EndScrollView();
			}
		}

		public class GUIIndent : IDisposable
		{
			public GUIIndent()
	        {
	        	EditorGUI.indentLevel++;
	        }

	        public void Dispose()
	        {
				EditorGUI.indentLevel--;
	        }
		}

		public class GUIEnabled : IDisposable
		{
	        private readonly bool previousEnabled;

	        public GUIEnabled(bool enabled)
	        {
				previousEnabled = GUI.enabled;
	            GUI.enabled = enabled;
	        }

	        public void Dispose()
	        {
				GUI.enabled = previousEnabled;
	        }
	    }

		public class GUIColor : IDisposable
		{
	        private readonly Color previousColor;

	        public GUIColor(Color color)
	        {
	            previousColor = GUI.color;
	            GUI.color = color;
	        }

	        public void Dispose()
	        {
	            GUI.color = previousColor;
	        }
	    }
	}
}