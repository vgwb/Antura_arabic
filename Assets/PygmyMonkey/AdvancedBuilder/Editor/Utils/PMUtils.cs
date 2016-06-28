using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace PygmyMonkey.AdvancedBuilder.Utils
{
	public static class PMUtils
	{
		public static void DrawAskToRate(string prefID, string name, string assetStoreID)
		{
			if (!EditorPrefs.GetBool(prefID, true))
			{
				return;
			}

			if (GUIUtils.DrawHeader("Rate " + name))
			{
				GUIUtils.BeginContents();
				{
					EditorGUILayout.LabelField("Hey friend!\nIf you could take a few seconds to rate and let a comment for " + name + " on the Asset Store, it would help a lot!\n\nThanks in advance and have a great day!", EditorStyles.wordWrappedLabel);

					EditorGUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("Do not show again"))
						{
							EditorPrefs.SetBool(prefID, false);
						}

						if (GUILayout.Button("Rate " + name))
						{
							if (EditorUtility.DisplayDialog("How to rate " + name + "?", "You just need to go inside the download manager of the Asset Store, and click on 5 stars ;)\n\nIf you also want to let a nice comment, you'll make our day!", "Got it!"))
							{
								Application.OpenURL("com.unity3d.kharma:content/" + assetStoreID);
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				GUIUtils.EndContents();
			}
		}

		public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
		{
			T scriptableObject = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
			
			if (scriptableObject != null)
			{
				return scriptableObject;
			}
			
			scriptableObject = ScriptableObject.CreateInstance<T>();
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
			
			string directoryPath = path.Substring(0, path.LastIndexOf("/"));
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
				return null;
			}
			
			AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			return scriptableObject;
		}
	}
}