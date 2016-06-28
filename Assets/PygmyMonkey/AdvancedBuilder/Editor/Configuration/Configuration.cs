using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class Configuration : IEquatable<Configuration>
	{
		public ReleaseType releaseType;
		public PlatformType platformType;
		public DistributionPlatform distributionPlatform;
		public PlatformArchitecture platformArchitecture;
		public TextureProperties textureProperties;

		public string name;
		public string customDefines;
		public List<string> scenePathList = new List<string>();
		public bool isEnabled;

		// Open build folder at the end of every build
		public bool openBuildFolder = true;

		// Append (rather than replace) the build of an iOS Xcode project
		public bool appendProject = false;

		// Is development build
		public bool isDevelopmentBuild = false;
		
		// Should autoconnect to profiler
		public bool shouldAutoconnectProfiler = false;
		
		// Allow debugging
		public bool allowDebugging = false;

		// Should autorun player
		public bool shouldAutoRunPlayer = false;

		// Windows Store build and deploy target
		public WSABuildAndRunDeployTarget wsaBuildAndRunDeployTarget = WSABuildAndRunDeployTarget.LocalMachine;

		private IPlatform m_platform;
		public IPlatform platform
		{
			get
			{
				if (m_platform == null)
				{
					AdvancedBuilder advancedBuilder = (AdvancedBuilder)AssetDatabase.LoadAssetAtPath("Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset", typeof(AdvancedBuilder));
					m_platform = advancedBuilder.getPlatforms().getPlatformFromType(platformType);
				}

				return m_platform;
			}
		}

		private ITextureCompression m_textureCompression;
		public ITextureCompression textureCompression
		{
			get
			{
				if (m_textureCompression == null)
				{
					AdvancedBuilder advancedBuilder = (AdvancedBuilder)AssetDatabase.LoadAssetAtPath("Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset", typeof(AdvancedBuilder));
					m_textureCompression = advancedBuilder.getPlatforms().getTextureCompressionFromNameAndPlatformType(textureProperties.name, platformType);
				}
				
				return m_textureCompression;
			}
		}

		public Configuration(ReleaseType releaseType, IPlatform platform, DistributionPlatform distributionPlatform, PlatformArchitecture platformArchitecture, ITextureCompression textureCompression)
		{
			this.releaseType = releaseType;
			this.platformType = platform.getPlatformProperties().platformType;
			this.distributionPlatform = distributionPlatform;
			this.platformArchitecture = platformArchitecture;
			this.textureProperties = textureCompression.getTextureProperties();

			m_platform = platform;
			m_textureCompression = textureCompression;
		}
		
		public void applyConfiguration()
		{
			AdvancedBuilder advancedBuilder = (AdvancedBuilder)AssetDatabase.LoadAssetAtPath("Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset", typeof(AdvancedBuilder));

			AppParameters.Get.updateParameters(releaseType.getFormattedName(), platform.getPlatformProperties().getFormattedPlatformType(), distributionPlatform.getFormattedName(), platformArchitecture.getFormattedName(), textureProperties.name, releaseType.productName, releaseType.bundleIdentifier, advancedBuilder.getProductParameters().bundleVersion);

			PlayerSettings.bundleIdentifier = releaseType.bundleIdentifier;
			PlayerSettings.bundleVersion = advancedBuilder.getProductParameters().bundleVersion;
			PlayerSettings.productName = releaseType.productName;

			foreach (BuildTargetGroup buildTargetGroup in Enum.GetValues(typeof(BuildTargetGroup)))
			{
				if (buildTargetGroup != BuildTargetGroup.Unknown)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, getFinalDefines(buildTargetGroup, advancedBuilder.getAdvancedSettings().globalCustomDefines, advancedBuilder.getAdvancedSettings().overwriteDefines));
				}
			}

			if (advancedBuilder.getAdvancedSettings().overwriteScenes)
			{
				EditorBuildSettings.scenes = scenePathList.Select(x => new EditorBuildSettingsScene(x, true)).ToArray();
			}
			
			platform.setupAdditionalParameters(advancedBuilder.getProductParameters(), this);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private string getFinalDefines(BuildTargetGroup buildTargetGroup, string globalCustomDefines, bool overwriteDefines)
		{
			List<string> defineList = new List<string>();
			if (!overwriteDefines)
			{
				defineList = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
			}

			if (!string.IsNullOrEmpty(customDefines))
			{
				string[] customDefineArray = customDefines.Split(';');
				for (int i = 0; i < customDefineArray.Length; i++)
				{
					if (!defineList.Contains(customDefineArray[i]))
					{
						defineList.Add(customDefineArray[i]);
					}
				}
			}

			if (!string.IsNullOrEmpty(globalCustomDefines))
			{
				string[] globalCustomDefineArray = globalCustomDefines.Split(';');
				for (int i = 0; i < globalCustomDefineArray.Length; i++)
				{
					if (!defineList.Contains(globalCustomDefineArray[i]))
					{
						defineList.Add(globalCustomDefineArray[i]);
					}
				}
			}			
			
			if (!string.IsNullOrEmpty(getAdvancedBuilderDefines()))
			{
				string[] advancedBuilderDefineArray = getAdvancedBuilderDefines().Split(';');
				for (int i = 0; i < advancedBuilderDefineArray.Length; i++)
				{
					if (!defineList.Contains(advancedBuilderDefineArray[i]))
					{
						defineList.Add(advancedBuilderDefineArray[i]);
					}
				}
			}

			return string.Join(",", defineList.ToArray());
		}

		public string getDescription()
		{
			string result = "(" + releaseType.name;

			if (!string.IsNullOrEmpty(platform.getPlatformProperties().name))
			{
				result += " \\ " + platform.getPlatformProperties().name;
			}

			if (!string.IsNullOrEmpty(distributionPlatform.name) && !distributionPlatform.name.Equals("Default"))
			{
				result += " \\ " + distributionPlatform.name;
			}
			
			if (!string.IsNullOrEmpty(platformArchitecture.name))
			{
				result += " \\ " + platformArchitecture.name;
			}
			
			if (!string.IsNullOrEmpty(textureProperties.name) && !textureProperties.name.Equals("Default"))
			{
				result += " \\ " + textureProperties.name;
			}
			
			result += ")";
			
			return result;
		}
		
		
		/*
		 * Return the final build path (used only for GUI)
		 */
		public string getBuildPath(AdvancedSettings advancedSettings, DateTime buildDate, string bundleVersion)
		{
			return "/Builds/" + getBuildDestinationPath(advancedSettings, buildDate, bundleVersion) + "" + platformArchitecture.binarySuffix;
		}

		
		/*
		 * Return the build destination
		 */
		public string getBuildDestinationPath(AdvancedSettings advancedSettings, DateTime buildDate, string bundleVersion)
		{
			string destinationName = advancedSettings.customPath + advancedSettings.customFileName;
			
			destinationName = destinationName.Replace("$BUILD_DATE", buildDate.ToString("yy-MM-dd HH\\hmm"));
			destinationName = destinationName.Replace("$RELEASE_TYPE", releaseType.name);
			destinationName = destinationName.Replace("$PLATFORM", platform.getPlatformProperties().name);
			
			if (!distributionPlatform.name.Equals("Default"))
			{
				destinationName = destinationName.Replace("$DISTRIB_PLATFORM", distributionPlatform.name);
			}
			else
			{
				destinationName = destinationName.Replace("$DISTRIB_PLATFORM/", string.Empty).Replace("$DISTRIB_PLATFORM", string.Empty);
			}
			
			if (!string.IsNullOrEmpty(platformArchitecture.name))
			{
				destinationName = destinationName.Replace("$ARCHITECTURE", platformArchitecture.name);
			}
			else
			{
				destinationName = destinationName.Replace("$ARCHITECTURE/", string.Empty).Replace("$ARCHITECTURE", string.Empty);
			}
			
			if (!textureProperties.name.Equals("Default"))
			{
				destinationName = destinationName.Replace("$TEXT_COMPRESSION", textureProperties.name);
			}
			else
			{
				destinationName = destinationName.Replace("$TEXT_COMPRESSION/", string.Empty).Replace("$TEXT_COMPRESSION", string.Empty);
			}
			
			destinationName = destinationName.Replace("$PRODUCT_NAME", releaseType.productName);
			destinationName = destinationName.Replace("$BUNDLE_VERSION", bundleVersion);
			destinationName = destinationName.Replace("$BUNDLE_IDENTIFIER", releaseType.bundleIdentifier);

			destinationName = platform.formatDestinationPath(destinationName);

			return destinationName;
		}

		public string getAdvancedBuilderDefines()
		{
			List<string> defineList = new List<string>();

			if (!string.IsNullOrEmpty(releaseType.getDefine()))
			{
				defineList.Add(releaseType.getDefine());
			}

			if (!string.IsNullOrEmpty(platform.getPlatformProperties().getDefine()))
			{
				defineList.Add(platform.getPlatformProperties().getDefine());
			}

			if (!string.IsNullOrEmpty(distributionPlatform.getDefine()))
			{
				defineList.Add(distributionPlatform.getDefine());
			}

			if (!string.IsNullOrEmpty(platformArchitecture.getDefine()))
			{
				defineList.Add(platformArchitecture.getDefine());
			}

			if (!string.IsNullOrEmpty(textureProperties.getDefine()))
			{
				defineList.Add(textureProperties.getDefine());
			}

			return string.Join(";", defineList.ToArray());
		}

		public void initSceneList()
		{
			scenePathList = EditorBuildSettings.scenes.Select(x => x.path.Replace("/", @"\")).ToList();
		}

		public override string ToString()
		{
			string result = "";

			if (!string.IsNullOrEmpty(name))
			{
				result += name + " \n";
			}

			result += getDescription();

			return result;
		}

		public bool Equals(Configuration other) 
		{
			return this.releaseType.name == other.releaseType.name
				&& platform.getPlatformProperties().name == other.platform.getPlatformProperties().name
				&& distributionPlatform.name == other.distributionPlatform.name
				&& platformArchitecture.name == other.platformArchitecture.name
				&& textureProperties.name == other.textureProperties.name;
		}
	}
}