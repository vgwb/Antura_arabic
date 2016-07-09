using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class AdvancedBuilderRenderer
	{
		/*
		 * AdvancedBuilder data
		 */
		private AdvancedBuilder m_advancedBuilder;
		
		
		/*
		 * Constructor
		 */
		public AdvancedBuilderRenderer(AdvancedBuilder advancedBuilder)
		{
			m_advancedBuilder = advancedBuilder;
		}


		public bool drawInspector()
		{
			ErrorReporter errorReporter = new ErrorReporter();
			
			// -------------------
			// Product parameters
			// -------------------
			ProductParametersRenderer productParametersRenderer = new ProductParametersRenderer(m_advancedBuilder.getProductParameters());
			productParametersRenderer.checkWarningsAndErrors(errorReporter);

			if (GUIUtils.DrawHeader("Product Parameters", "ab_product_parameters"))
			{
				GUIUtils.BeginContents();
				productParametersRenderer.drawInspector();
				GUIUtils.EndContents();
			}
			
			
			
			// --------------
			// Release Types
			// --------------
			ReleaseTypesRenderer releaseTypesRenderer = new ReleaseTypesRenderer(m_advancedBuilder.getReleaseTypes());
			releaseTypesRenderer.checkWarningsAndErrors(errorReporter);

			if (GUIUtils.DrawHeader("Release Types", "ab_release_types"))
			{
				GUIUtils.BeginContents();
				releaseTypesRenderer.drawInspector();
				GUIUtils.EndContents();
			}
			
			
			
			// ----------
			// Platforms
			// ----------
			PlatformsRenderer platformsRenderer = new PlatformsRenderer(m_advancedBuilder.getPlatforms());
			platformsRenderer.checkWarningsAndErrors(errorReporter);

			if (GUIUtils.DrawHeader("Platforms", "ab_platforms"))
			{
				GUIUtils.BeginContents();
				platformsRenderer.drawInspector();
				GUIUtils.EndContents();
			}



			// ------------------
			// Advanced Settings
			// ------------------
			AdvancedSettingsRenderer advancedSettingsRenderer = new AdvancedSettingsRenderer(m_advancedBuilder.getAdvancedSettings());
			advancedSettingsRenderer.checkWarningsAndErrors(errorReporter);

			if (GUIUtils.DrawHeader("Advanced Settings", "ab_advanced_settings"))
			{
				GUIUtils.BeginContents();
				advancedSettingsRenderer.drawInspector();
				GUIUtils.EndContents();
			}



			// ------------------
			// App Configuration
			// ------------------
			ProjectConfigurationsRenderer projectConfigurationsRenderer = new ProjectConfigurationsRenderer(m_advancedBuilder, m_advancedBuilder.getProjectConfigurations());
			projectConfigurationsRenderer.checkWarningsAndErrors(errorReporter);

			if (GUIUtils.DrawHeader("Project Configurations", "ab_project_configurations"))
			{
				GUIUtils.BeginContents();
				bool wasBuildProcessLaunched = projectConfigurationsRenderer.drawInspector(platformsRenderer, errorReporter);
				if (wasBuildProcessLaunched)
				{
					return true;
				}
				GUIUtils.EndContents();
			}



			// ------------------
			// Perform Build
			// ------------------
			if (GUIUtils.DrawHeader("Perform Build", "ab_perform_build", true))
			{
				GUIUtils.BeginContents();
				{
					if (errorReporter.getWarningCount() > 0 || errorReporter.getErrorCount() > 0)
					{
						foreach (string iMessage in errorReporter.getErrorList())
						{
							EditorGUILayout.HelpBox(iMessage, MessageType.Error, true);
						}

						foreach (string iMessage in errorReporter.getWarningList())
						{
							EditorGUILayout.HelpBox(iMessage, MessageType.Warning, true);
						}
					}


					bool isBuildAllowed = m_advancedBuilder.getProjectConfigurations().getTotalBuildCount() != 0 && errorReporter.getErrorCount() <= 0;
					
					
					/*
					 * Perform the build
					 */
					GUI.enabled = isBuildAllowed;
					
					if (GUIUtils.DrawBigButton("Perform a total of " + m_advancedBuilder.getProjectConfigurations().getTotalBuildCount() + " builds", Color.green))
					{
						if (m_advancedBuilder.getProjectConfigurations().getTotalAutoRunBuildCount() > 3)
						{
							if (EditorUtility.DisplayDialog("Launch build?", "You have selected the 'Autorun Build' option and you are making multiple builds, are you sure you want to continue?", "Yes", "Cancel"))
							{
								AdvancedBuilder.PerformBuild();
							}
						}
						else
						{
							AdvancedBuilder.PerformBuild();
						}

						return true;
					}
					
					GUI.enabled = true;
						
						
					/*
					 * Button to open build folder
					 */
					if (GUIUtils.DrawBigButton("Open Build folder"))
					{
						string buildFolderPath = Application.dataPath.Replace("Assets", "Builds");
						
						if (!Directory.Exists(buildFolderPath))
						{
							Directory.CreateDirectory(buildFolderPath);
						}
						
						EditorUtility.OpenWithDefaultApp(buildFolderPath);
					}
					
					GUILayout.Space(5f);
				}
				GUIUtils.EndContents();
			}

			return false;
		}
	}
}
