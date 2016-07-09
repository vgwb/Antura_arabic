using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ProjectConfigurationsRenderer : IDefaultRenderer
	{
		/*
		 * ProjectConfigurations data
		 */
		private AdvancedBuilder m_advancedBuilder;
		private ProjectConfigurations m_projectConfigurations;
		private ConfigurationSceneRenderer m_configurationSceneRenderer;
		
		
		/*
		 * Constructor
		 */
		public ProjectConfigurationsRenderer(AdvancedBuilder advancedBuilder, ProjectConfigurations projectConfigurations)
		{
			m_advancedBuilder = advancedBuilder;
			m_projectConfigurations = projectConfigurations;

			m_configurationSceneRenderer = new ConfigurationSceneRenderer(advancedBuilder.getAdvancedSettings());
		}
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector() { }
		
		public bool drawInspector(PlatformsRenderer platformsRenderer, ErrorReporter errorReporter)
		{
			List<Configuration> configurationList = m_projectConfigurations.getAvailableConfigurationList(m_advancedBuilder.getReleaseTypes(), m_advancedBuilder.getPlatforms());
			drawAvailableConfigurationList(configurationList);
			return drawSelectedConfigurationList(platformsRenderer, errorReporter);
		}

		private void drawAvailableConfigurationList(List<Configuration> configurationList)
		{
			GUILayout.BeginHorizontal();
			{
				if (m_projectConfigurations.configurationIndex >= configurationList.Count)
				{
					m_projectConfigurations.configurationIndex = 0;
				}

				GUI.enabled = configurationList.Count > 0;
				m_projectConfigurations.configurationIndex = EditorGUILayout.Popup(m_projectConfigurations.configurationIndex, configurationList.Select(x => x.getDescription()).ToArray());
				if (GUILayout.Button("Add", GUILayout.MaxWidth(75f)))
				{
					Configuration selectedConfiguration = configurationList[m_projectConfigurations.configurationIndex];

					selectedConfiguration.isEnabled = true;
					selectedConfiguration.initSceneList();
					m_projectConfigurations.configurationList.Add(selectedConfiguration);
				}
				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();
		}

		private bool drawSelectedConfigurationList(PlatformsRenderer platformsRenderer, ErrorReporter errorReporter)
		{
			EditorGUILayout.HelpBox("Here you can switch to a configuration, so you can directly test your game inside the Editor in the configuration you want. Of course if you decide to build, every build will have the configuration you specified. You can click on a configuration to see details and apply specific parameters.", MessageType.Info, true);
			GUILayout.BeginVertical();
			{
				for (int i = 0; i < m_projectConfigurations.configurationList.Count; i++)
				{
					Configuration configuration = m_projectConfigurations.configurationList[i];
					
					// If we could not find the texture compression, it's because we loaded AdvancedBuilder with an older version of Unity used to create the AdvancedBuilder data
					if (configuration.textureCompression == null)
					{
						m_projectConfigurations.configurationList.Remove(configuration);
						return false;
					}
					
					EditorGUILayout.BeginHorizontal();
					{
						configuration.isEnabled = EditorGUILayout.Toggle(configuration.isEnabled, GUILayout.Width(15f));

						GUI.backgroundColor = m_projectConfigurations.selectedConfigurationIndex == i ? Color.green : Color.white;
						if (GUILayout.Button(configuration.ToString()))
						{
							ConfigurationSceneRenderer.ComputeProjectSceneList();
							m_configurationSceneRenderer.updateAvailableSceneList(configuration);

							if (m_projectConfigurations.selectedConfigurationIndex == i)
							{
								m_projectConfigurations.selectedConfigurationIndex = -1;
							}
							else
							{
								m_projectConfigurations.selectedConfigurationIndex = i;
							}

							GUI.FocusControl(null);
						}
						GUI.backgroundColor = Color.white;

						GUI.backgroundColor = Color.red;
						if (GUILayout.Button("x", GUILayout.Width(25f)))
						{
							m_projectConfigurations.selectedConfigurationIndex = -1;
							m_projectConfigurations.configurationList.Remove(configuration);
							return false;
						}
						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndHorizontal();

					if (m_projectConfigurations.selectedConfigurationIndex == i)
					{
						GUILayout.Space(5);

						bool hasPressBuild = drawConfigurationDetail(platformsRenderer, errorReporter);
						if (hasPressBuild)
						{
							return true;
						}
						
						GUILayout.Space(10);
					}
				}
			}
			GUILayout.EndVertical();

			return false;
		}

		private bool drawConfigurationDetail(PlatformsRenderer platformsRenderer, ErrorReporter errorReporter)
		{
			if (m_projectConfigurations.selectedConfigurationIndex >= m_projectConfigurations.configurationList.Count)
			{
				m_projectConfigurations.selectedConfigurationIndex = -1;
				return false;
			}

			Configuration configuration = m_projectConfigurations.configurationList[m_projectConfigurations.selectedConfigurationIndex];

			if (GUIUtils.DrawHeader("Configuration information", true))
			{
				GUIUtils.BeginContents();
				
				GUILayout.BeginHorizontal();
				{
					GUILayout.BeginVertical();
					{
						configuration.name = EditorGUILayout.TextField("Name:", configuration.name);

						EditorGUILayout.LabelField("Default Defines:", configuration.getAdvancedBuilderDefines());
						configuration.customDefines = EditorGUILayout.TextField("Custom Defines:", configuration.customDefines);

						if (configuration.platformType == PlatformType.WindowsStore)
						{
							if (PlatformWindowsStore.IsUniversal(configuration.platformArchitecture.name))
							{
								configuration.wsaBuildAndRunDeployTarget = (WSABuildAndRunDeployTarget)EditorGUILayout.EnumPopup("Build and Deploy Target", configuration.wsaBuildAndRunDeployTarget);
							}
						}

						/*if (configuration.platformType == PlatformType.Android)
						{
							configuration.appendProject = EditorGUILayout.ToggleLeft(new GUIContent("Append project", "(Used when building Eclipse project) This setting will create a new Eclipse project. Existing Eclipse project setting changes will be discarded."), configuration.appendProject);
						}
						else */if (configuration.platformType == PlatformType.iOS)
						{
							configuration.appendProject = EditorGUILayout.ToggleLeft(new GUIContent("Append project", "This setting will append an existing Xcode project. Existing Xcode project setting changes will be preserved."), configuration.appendProject);
						}
						else if (configuration.platformType == PlatformType.WindowsStore)
						{
							configuration.appendProject = EditorGUILayout.ToggleLeft(new GUIContent("Append project", "This setting will append an existing project instead of replacing it."), configuration.appendProject);
						}

						configuration.openBuildFolder = EditorGUILayout.ToggleLeft("Open build folder after build", configuration.openBuildFolder);
						configuration.isDevelopmentBuild = EditorGUILayout.ToggleLeft("Development build", configuration.isDevelopmentBuild);
						configuration.shouldAutoconnectProfiler = EditorGUILayout.ToggleLeft("Autoconnect Profiler", configuration.shouldAutoconnectProfiler);
						configuration.allowDebugging = EditorGUILayout.ToggleLeft("Allow script debugging", configuration.allowDebugging);
						configuration.shouldAutoRunPlayer = EditorGUILayout.ToggleLeft("Autorun Build", configuration.shouldAutoRunPlayer);

						m_configurationSceneRenderer.drawInspector(configuration);

						if (GUIUtils.DrawHeader("Build information"))
						{
							GUIUtils.BeginContents();
							{
								PlatformRenderer platformRenderer = platformsRenderer.getPlatformRendererFromPlatform(configuration.platform.getPlatformProperties().platformType);
								platformRenderer.drawBuildSummary(m_advancedBuilder.getAdvancedSettings(), configuration, m_advancedBuilder.getProductParameters().bundleVersion);
							}
							GUIUtils.EndContents();
						}
						
						GUILayout.BeginHorizontal();
						{
							if (GUILayout.Button("Apply this config" + (m_projectConfigurations.isCurrentConfig(configuration) ? " (Current)" : "")))
							{
								configuration.applyConfiguration();
							}

							GUI.enabled = errorReporter.getErrorCount() == 0;
							if (GUILayout.Button("Build"))
							{
								AdvancedBuilder.PerformBuild(configuration);
								return true;
							}
							GUI.enabled = true;
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
				
				GUIUtils.EndContents();
			}

			return false;
		}
		
		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			if (m_advancedBuilder.getAdvancedSettings().overwriteScenes)
			{
				foreach (Configuration configuration in m_projectConfigurations.configurationList)
				{
					if (configuration.scenePathList == null || configuration.scenePathList.Count == 0)
					{
						errorReporter.addError("The configuration " + configuration.ToString() + " has no scene defined");
					}
				}
			}
			else
			{
				if (PlatformBuilder.GetActiveScenePathArray() == null || PlatformBuilder.GetActiveScenePathArray().Length == 0)
				{
					errorReporter.addError("You need at least one active scene for your project. Check File -> Build Settings");
				}
			}

			#if !UNITY_EDITOR_OSX
			if (m_projectConfigurations.configurationList.Count(x => x.platformType.Equals(PlatformType.iOS) && x.shouldAutoRunPlayer) != 0)
			{
				errorReporter.addError("You can't autorun an iOS build because you're not using a Mac. You can still build for iOS, but can't autorun. Please uncheck the 'Autorun Build' box in your build configuration");
			}
			#endif

			List<ReleaseType> activeReleaseTypeList = m_advancedBuilder.getReleaseTypes().getReleaseTypeList().Where(x => x.isActive).ToList();
			
			if (activeReleaseTypeList.Count == 0)
			{
				errorReporter.addWarning("You need to select at least one release type in the 'Release Type' window.");
			}

			if (m_projectConfigurations.configurationList.Count == 0)
			{
				errorReporter.addWarning("You need to add at least one configuration in order to build multiple configurations.");
			}
			else
			{
				if (m_projectConfigurations.configurationList.Count(x => x.isEnabled) == 0)
				{
					errorReporter.addWarning("You need to enable at least one configuration in order to build multiple configurations.");
				}

				if (m_projectConfigurations.configurationList.GroupBy(x => x.getBuildPath(m_advancedBuilder.getAdvancedSettings(), System.DateTime.Now, m_advancedBuilder.getProductParameters().bundleVersion)).SelectMany(group => group.Skip(1)).Count() != 0)
				{
					errorReporter.addError("Be careful. You have some build path that are exactly the same for some configuration. So one configuration build will erase the other! Please modify the build path in the 'Build folder path' section to avoid this!");
				}

				foreach (Configuration configuration in m_projectConfigurations.configurationList)
				{
					string finalBuildPath = Application.dataPath.Replace("/Assets", string.Empty) + configuration.getBuildPath(m_advancedBuilder.getAdvancedSettings(), System.DateTime.Now, m_advancedBuilder.getProductParameters().bundleVersion);
					if (File.Exists(finalBuildPath) || Directory.Exists(finalBuildPath))
					{
						errorReporter.addWarning("There is already a build at path " + finalBuildPath + " for the configuration " + configuration.getDescription() + ". If you build, it will overwrite it.");
					}
				}
			}

			foreach (Configuration configuration in m_projectConfigurations.configurationList)
			{
				if (m_advancedBuilder.getReleaseTypes().getReleaseTypeList().Where(x => !string.IsNullOrEmpty(x.name) && x.name.Equals(configuration.releaseType.name)).Count() == 0)
				{
					errorReporter.addWarning("Be careful, it seems you changed a release type name, and your configuration " + configuration.getDescription() + " still uses the old one (" + (configuration.releaseType == null ? "N/A" : configuration.releaseType.name) + "). To update, please delete the configuration and add it back again (remember to save your custom defines)");
				}

				if (m_advancedBuilder.getReleaseTypes().getReleaseTypeList().Where(x => !string.IsNullOrEmpty(x.bundleIdentifier) && x.bundleIdentifier.Equals(configuration.releaseType.bundleIdentifier)).Count() == 0)
				{
					errorReporter.addWarning("Be careful, it seems you changed a release type bundle identifier, and your configuration " + configuration.getDescription() + " still uses the old one (" + (configuration.releaseType == null ? "N/A" : configuration.releaseType.bundleIdentifier) + "). To update, please delete the configuration and add it back again (remember to save your custom defines)");
				}

				if (m_advancedBuilder.getReleaseTypes().getReleaseTypeList().Where(x => !string.IsNullOrEmpty(x.productName) && x.productName.Equals(configuration.releaseType.productName)).Count() == 0)
				{
					errorReporter.addWarning("Be careful, it seems you changed a release type product name, and your configuration " + configuration.getDescription() + " still uses the old one (" + (configuration.releaseType == null ? "N/A" : configuration.releaseType.productName) + "). To update, please delete the configuration and add it back again (remember to save your custom defines)");
				}

				if (!configuration.distributionPlatform.name.Equals("Default"))
				{
					if (configuration.platform.getPlatformProperties().getDistributionPlatformList().Where(x => x.name.Equals(configuration.distributionPlatform.name)).Count() == 0)
					{
						errorReporter.addWarning("Be careful, it seems you changed a distribution platform name, and your configuration " + configuration.getDescription() + " still uses the old one (" + (configuration.distributionPlatform == null ? "N/A" : configuration.distributionPlatform.name) + "). To update, please delete the configuration and add it back again (remember to save your custom defines)");
					}
				}
			}
		}
	}
}