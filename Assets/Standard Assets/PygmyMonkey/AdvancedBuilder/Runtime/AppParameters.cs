using System;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class AppParameters : ScriptableObject
	{
		private static AppParameters mInstance;
		
		public static AppParameters Get
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = (AppParameters)Resources.Load("AppParameters", typeof(AppParameters));
					if (mInstance == null)
					{
						throw new Exception("We could not find the ScriptableObject AppParameters.asset inside the folder 'PygmyMonkey/AdvancedBuilder/Resources/'");
					}
				}
				
				return mInstance;
			}
		}

		public void updateParameters(string releaseType, string platformType, string distributionPlatform, string platformArchitecture, string textureCompression, string productName, string bundleIdentifier, string bundleVersion)
		{
			m_releaseType = releaseType;
			m_platformType = platformType;
			m_distributionPlatform = distributionPlatform;
			m_platformArchitecture = platformArchitecture;
			m_textureCompression = textureCompression;
			m_productName = productName;
			m_bundleIdentifier = bundleIdentifier;
			m_bundleVersion = bundleVersion;

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			#endif
		}

		[SerializeField] private string m_releaseType = "";
		public string releaseType { get { return m_releaseType; } }

		[SerializeField] private string m_platformType = "";
		public string platformType { get { return m_platformType; } }

		[SerializeField] private string m_distributionPlatform = "";
		public string distributionPlatform { get { return m_distributionPlatform; } }

		[SerializeField] private string m_platformArchitecture = "";
		public string platformArchitecture { get { return m_platformArchitecture; } }

		[SerializeField] private string m_textureCompression = "";
		public string textureCompression { get { return m_textureCompression; } }
		
		[SerializeField] private string m_productName = "";
		public string productName { get { return m_productName; } }

		[SerializeField] private string m_bundleIdentifier = "";
		public string bundleIdentifier { get { return m_bundleIdentifier; } }

		[SerializeField] private string m_bundleVersion = "";
		public string bundleVersion { get { return m_bundleVersion; } }
	}
}
