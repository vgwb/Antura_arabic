using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder.Utils
{
	public abstract class PMEditorWindow : EditorWindow
	{
		protected static EditorWindow createWindow<T>(string productName) where T : EditorWindow
		{
			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
			T window = (T)EditorWindow.GetWindow(typeof(T), false, " " + productName);
			assignEditorWindowIcon(window, getIconPath(productName), productName);
			return window;
			#else
			T window = EditorWindow.GetWindow<T>(false);
			window.titleContent = new GUIContent(" " + getShortProductName(productName), (Texture2D)AssetDatabase.LoadAssetAtPath(getIconPath(productName), typeof(Texture2D)));
			return window;
			#endif
		}

		public abstract string getProductName();
		public abstract string getVersionName();
		public abstract string getAssetStoreID();

		private static string getIconPath(string productName)
		{
			return "Assets/PygmyMonkey/" + getFormattedProductName(productName) + "/Editor/Textures/Icon.png";
		}

		protected static string getFormattedProductName(string productName)
		{
			return productName.Replace(" ", string.Empty).Trim();
		}

		private static string getShortProductName(string productName)
		{
			if (productName.Length <= 13)
			{
				return productName;
			}

			return productName.Substring(0, 13) + ".";
		}

		public abstract void init();

		private void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;

			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
			assignEditorWindowIcon(this, getIconPath(getProductName()), getProductName());
			#endif

			init();
		}
		
		private void OnFocus()
		{
			Repaint();
		}
		
		private void OnSelectionChange()
		{
			Repaint();
		}

		#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
		private static void assignEditorWindowIcon(EditorWindow editorWindow, string iconPath, string title)
		{
			System.Reflection.PropertyInfo propertyInfo = typeof(EditorWindow).GetProperty("cachedTitleContent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			if (propertyInfo == null) return;

			GUIContent windowTitle = propertyInfo.GetValue(editorWindow, null) as GUIContent;
			if (windowTitle != null)
			{
				windowTitle.image = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture2D));
			}
		}
		#endif

		private Vector2 scrollPosition;
		public virtual void drawBegin() {}
		public abstract void drawContent();
		public virtual void drawEnd() {}

		protected bool useCustomGUI = false;
		public virtual void drawCustomGUI() {}

		private void OnGUI()
		{
			if (useCustomGUI)
			{
				drawCustomGUI();
				return;
			}

			drawBegin();

			using (new GUIUtils.GUIScrollView(ref scrollPosition))
			{
				using (new GUIUtils.GUIHorizontal())
				{
					GUILayout.Space(5f);
					using (new GUIUtils.GUIVertical())
					{
						GUILayout.Space(5f);
						
						EditorGUILayout.LabelField(getProductName() + " - Version " + getVersionName(), LegacyGUIStyle.LabelVersionStyle);

						drawContent();

						GUILayout.Space(5f);
						PMUtils.DrawAskToRate(getFormattedProductName(getProductName()).ToLower() + "_show_rate_popup", getProductName(), getAssetStoreID());
						GUILayout.Space(10f);
					}
					GUILayout.Space(5f);
				}
			}

			drawEnd();
		}
	}
}