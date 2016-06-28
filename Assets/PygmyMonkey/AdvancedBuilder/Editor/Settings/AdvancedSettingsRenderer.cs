using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class AdvancedSettingsRenderer : IDefaultRenderer
	{
		/*
		 * AdvancedSettings data
		 */
		private AdvancedSettings m_advancedSettings;
		
		
		/*
		 * Constructor
		 */
		public AdvancedSettingsRenderer(AdvancedSettings advancedSettings)
		{
			m_advancedSettings = advancedSettings;
			
			if (string.IsNullOrEmpty(advancedSettings.customPath))
			{
				setDefaultCustomPath();
			}
		}
		
		
		/*
		 * Reset custom path
		 */
		private void setDefaultCustomPath()
		{
			m_advancedSettings.customPath = "$BUILD_DATE/$RELEASE_TYPE/$PLATFORM/$DISTRIB_PLATFORM/$ARCHITECTURE/$TEXT_COMPRESSION/";
			m_advancedSettings.customFileName = "$PRODUCT_NAME";
		}
		
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			/*
			 * BUILD FOLDER SETTINGS
			 */

			GUIUtils.DrawCategoryLabel("Build folder path");
			
			/*
			 * Display settings for the final build folder
			 */

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$BUILD_DATE", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The build date (format: yy-MM-dd HHhmm)");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$RELEASE_TYPE", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The release type");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$PLATFORM", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The platform (example: iOS, Android, Mac...)");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$DISTRIB_PLATFORM", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The distribution platform (example: Google Play, Apple Store...)");
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$ARCHITECTURE", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The platform architecture (Universal, x86, x64...)");
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$TEXT_COMPRESSION", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The texture compression (Generic, ATC, ETC...)");
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$PRODUCT_NAME", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The product name of your release type");
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$BUNDLE_VERSION", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The bundle version");
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.SelectableLabel("$BUNDLE_IDENTIFIER", GUILayout.Width(145f), GUILayout.Height(15f));
				EditorGUILayout.LabelField("The bundle identifier of your release type");
			}
			EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = isCustomPathValid() ? Color.white : Color.red;
			m_advancedSettings.customPath = EditorGUILayout.TextField("Build path", m_advancedSettings.customPath);
			GUI.backgroundColor = isCustomFileNameValid() ? Color.white : Color.red;
			m_advancedSettings.customFileName = EditorGUILayout.TextField("Build file name", m_advancedSettings.customFileName);
			GUI.backgroundColor = Color.white;

			if (GUILayout.Button("Reset custom path to default"))
			{
				setDefaultCustomPath();
				
				// We release focus on the text field so unity can update it
				GUIUtility.keyboardControl = 0;
				GUIUtility.hotControl = 0;
			}
			
			
			
			/*
			 * CUSTOM BUILD SETTINGS
			 */
			GUILayout.Space(20f);
			GUIUtils.DrawCategoryLabel("Custom settings");
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Custom build script");
				m_advancedSettings.customBuildMonoScript = (MonoScript)EditorGUILayout.ObjectField(m_advancedSettings.customBuildMonoScript == null ? null : m_advancedSettings.customBuildMonoScript, typeof(MonoScript), false);
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Global custom defines");
				m_advancedSettings.globalCustomDefines = EditorGUILayout.TextField(m_advancedSettings.globalCustomDefines);
			}
			EditorGUILayout.EndHorizontal();
			
			
			
			/*
			 * BUILD OPTIONS
			 */
			GUILayout.Space(20f);
			GUIUtils.DrawCategoryLabel("Build options");

			m_advancedSettings.overwriteScenes = EditorGUILayout.ToggleLeft("Let AdvancedBuilder overwrite scenes", m_advancedSettings.overwriteScenes);
			m_advancedSettings.overwriteDefines = EditorGUILayout.ToggleLeft("Let AdvancedBuilder overwrite defines", m_advancedSettings.overwriteDefines);
			m_advancedSettings.useSymlinkLibraries = EditorGUILayout.ToggleLeft("Use Symlink Libraries (iOS)", m_advancedSettings.useSymlinkLibraries);
			m_advancedSettings.checkAndroidKeystorePasswords = EditorGUILayout.ToggleLeft("Check to show error if android keystore passwords are not set (Android)", m_advancedSettings.checkAndroidKeystorePasswords);
		}


		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			if (!isCustomPathValid())
			{
				errorReporter.addError("Custom Path in AdvancedSettings has bad format. It must not contain $ (except for the AdvancedBuilder constants).");
			}

			if (!isCustomFileNameValid())
			{
				errorReporter.addError("Custom File Name in AdvancedSettings has bad format. It must not contain $ (except for the AdvancedBuilder constants), / or \\.");
			}

			if (!isCustomBuildMonoScriptValid())
			{
				errorReporter.addError("Your custom build script must implements the interface IAdvancedCustomBuild.");
			}
		}

		private bool isCustomPathValid()
		{
			return isCustomValid(m_advancedSettings.customPath);
		}

		private bool isCustomFileNameValid()
		{
			if (m_advancedSettings.customFileName.Equals("$PRODUCT_NAME"))
			{
				return true;
			}

			if (m_advancedSettings.customFileName.Contains("/") || m_advancedSettings.customFileName.Contains(@"\"))
			{
				return false;
			}

			return isCustomValid(m_advancedSettings.customFileName);
		}
		
		private bool isCustomValid(string thePath)
		{
			string path = thePath
				.Replace("$BUILD_DATE", string.Empty)
				.Replace("$RELEASE_TYPE", string.Empty)
				.Replace("$PLATFORM", string.Empty)
				.Replace("$DISTRIB_PLATFORM", string.Empty)
				.Replace("$ARCHITECTURE", string.Empty)
				.Replace("$TEXT_COMPRESSION", string.Empty)
				.Replace("$PRODUCT_NAME", string.Empty)
				.Replace("$BUNDLE_VERSION", string.Empty)
				.Replace("$BUNDLE_IDENTIFIER", string.Empty);
			
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			
			if (path.Contains("$"))
			{
				return false;
			}
			
			return true;
		}

		public bool isCustomBuildMonoScriptValid()
		{
			if (m_advancedSettings.customBuildMonoScript == null)
			{
				return true;
			}
			else if (m_advancedSettings.customBuildMonoScript.GetClass().GetInterface("IAdvancedCustomBuild") != null)
			{
				return true;
			}

			return false;
		}
	}
}