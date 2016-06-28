using UnityEditor;
using UnityEngine;
using System.IO;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class AdvancedBuilderWindow : PMEditorWindow
	{
		public static string ProductName = "Advanced Builder";
		public static string VersionName = "1.4.8";

		[MenuItem("Window/PygmyMonkey/Advanced Builder")] 
		private static void ShowWindow()
		{
			EditorWindow window = createWindow<AdvancedBuilderWindow>(ProductName);
			window.minSize = new Vector2(440, 500);
		}

		public override string getProductName()
		{
			return ProductName;
		}

		public override string getVersionName()
		{
			return VersionName;
		}

		public override string getAssetStoreID()
		{
			return "13624";
		}

		/*
		* Data
		*/
		private AdvancedBuilder m_advancedBuilder;
		private AppParameters m_appParameters;

		/*
		* Init
		*/
		public override void init()
		{
			loadScriptableObject();
			useCustomGUI = true;
		}

		void Update()
		{
			if (m_advancedBuilder == null || m_appParameters == null)
			{
				loadScriptableObject();
				return;
			}
		}

		public override void drawContent() { }
		private Vector2 m_scrollPosition;

		public override void drawCustomGUI()
		{
			bool wasBuildProcessLaunched = false;
			m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
			{
				GUILayout.Space(5f);
				
				EditorGUILayout.LabelField(getProductName() + " - Version " + getVersionName(), LegacyGUIStyle.LabelVersionStyle);

				wasBuildProcessLaunched = new AdvancedBuilderRenderer(m_advancedBuilder).drawInspector();
		
				if (GUI.changed)
				{
					EditorUtility.SetDirty(m_advancedBuilder);
				}

				GUILayout.Space(5f);
				PMUtils.DrawAskToRate(getFormattedProductName(getProductName()).ToLower() + "_show_rate_popup", getProductName(), getAssetStoreID());
				GUILayout.Space(10f);
			}
			if (!wasBuildProcessLaunched)
			{
				EditorGUILayout.EndScrollView();
			}
	    }
		
		
		
		// ===================================================================================
	    // ADVANCED BUILDER METHODS -----------------------------------------------------------------------
		
		private void loadScriptableObject()
		{
			m_advancedBuilder = PMUtils.CreateScriptableObject<AdvancedBuilder>("Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset");
			m_appParameters = PMUtils.CreateScriptableObject<AppParameters>("Assets/PygmyMonkey/AdvancedBuilder/Resources/AppParameters.asset");
		}
	}
}