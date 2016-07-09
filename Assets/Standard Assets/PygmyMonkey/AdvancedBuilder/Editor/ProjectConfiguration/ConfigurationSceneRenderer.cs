using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ConfigurationSceneRenderer
	{
		private static List<string> m_sceneList;

		private static int m_sceneIndex;
		private static List<string> m_availableSceneList;

		private AdvancedSettings m_advancedSettings;

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnReloadScripts()
		{
			ComputeProjectSceneList();
		}

		public ConfigurationSceneRenderer(AdvancedSettings advancedSettings)
		{
			m_advancedSettings = advancedSettings;
		}

		public static void ComputeProjectSceneList()
		{
			string[] sceneAssetArray = AssetDatabase.FindAssets("t:Scene");
			if (sceneAssetArray == null)
			{
				return;
			}

			foreach (string sceneAsset in sceneAssetArray)
			{
				string scenePath = AssetDatabase.GUIDToAssetPath(sceneAsset);
				
				if (File.Exists(scenePath))
				{
					if (m_sceneList == null)
					{
						m_sceneList = new List<string>();
					}

					m_sceneList.Add(scenePath.Replace("/", @"\"));
				}
			}
		}

		public void updateAvailableSceneList(Configuration configuration)
		{
			m_availableSceneList = new List<string>();

			if (m_sceneList != null)
			{
				foreach (string scene in m_sceneList)
				{
					if (!configuration.scenePathList.Contains(scene))
					{
						m_availableSceneList.Add(scene);
					}
				}
			}

			if (m_sceneIndex >= m_availableSceneList.Count)
			{
				m_sceneIndex = 0;
			}
		}

		/*
		 * Draw in inspector
		 */
		public void drawInspector(Configuration configuration)
		{
			GUI.enabled = m_advancedSettings.overwriteScenes;

			if (m_availableSceneList == null)
			{
				updateAvailableSceneList(configuration);
			}

			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Scenes:", GUILayout.Width(146));

				GUI.enabled = m_advancedSettings.overwriteScenes && m_availableSceneList.Count > 0;
				m_sceneIndex = EditorGUILayout.Popup(m_sceneIndex, m_availableSceneList.ToArray());
				if (GUILayout.Button("Add scene", GUILayout.Width(80)))
				{
					configuration.scenePathList.Add(m_availableSceneList[m_sceneIndex]);
					updateAvailableSceneList(configuration);
				}
				GUI.enabled = m_advancedSettings.overwriteScenes && true;
			}
			GUILayout.EndHorizontal();

			for (int i = 0; i < configuration.scenePathList.Count; i++)
			{
				GUILayout.Space(3f);
				GUILayout.BeginHorizontal();
				{
					string sceneName = configuration.scenePathList[i].Remove(0, configuration.scenePathList[i].LastIndexOf(@"\") + 1);
					EditorGUILayout.LabelField(new GUIContent("\t\t\t  " + sceneName, configuration.scenePathList[i]));

					GUI.enabled = m_advancedSettings.overwriteScenes && i > 0;
					if (GUILayout.Button(new GUIContent("\u2191", "move up"), EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
					{
						string tmpScene = configuration.scenePathList[i];
						configuration.scenePathList.RemoveAt(i);
						configuration.scenePathList.Insert(i - 1, tmpScene);
						GUI.FocusControl(null);
					}
					GUI.enabled = m_advancedSettings.overwriteScenes && true;
					
					GUI.enabled = i < configuration.scenePathList.Count - 1;
					if (GUILayout.Button(new GUIContent("\u2193", "move down"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
					{
						string tmpScene = configuration.scenePathList[i];
						configuration.scenePathList.RemoveAt(i);
						configuration.scenePathList.Insert(i + 1, tmpScene);
						GUI.FocusControl(null);
					}
					GUI.enabled = m_advancedSettings.overwriteScenes && true;
					
					if (GUILayout.Button(new GUIContent("-", "delete"), EditorStyles.miniButtonRight, GUILayout.Width(20f)))
					{
						configuration.scenePathList.RemoveAt(i);
						GUI.FocusControl(null);
						updateAvailableSceneList(configuration);
						return;
					}
				}
				GUILayout.EndHorizontal();
			}

			GUI.enabled = true;
		}
	}
}