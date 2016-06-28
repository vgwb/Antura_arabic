using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class PlatformRenderer : IDefaultRenderer
	{
		/*
		 * PlatformDefault data
		 */
		private IPlatform m_platform;
		private IPlatformAdditionalRenderer m_platformAdditionalRenderer;
		private Action m_updateAvailablePlatformListAction;

		
		/*
		 * Constructor
		 */
		public PlatformRenderer(IPlatform platform, IPlatformAdditionalRenderer platformAdditionalRenderer, Action updateAvailablePlatformList)
		{
			m_platform = platform;
			m_platformAdditionalRenderer = platformAdditionalRenderer;
			m_updateAvailablePlatformListAction = updateAvailablePlatformList;
		}

		public IPlatform getPlatform()
		{
			return m_platform;
		}
		
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			PlatformProperties platformProperties = m_platform.getPlatformProperties();

			DistributionsPlatformRenderer distributionsPlatformRenderer = new DistributionsPlatformRenderer(platformProperties.getDistributionPlatformList());
			distributionsPlatformRenderer.drawInspector();

			GUILayout.Space(20f);

			if (platformProperties.isUsingPlatformArchitecture())
			{
				GUIUtils.DrawCategoryLabel("Architectures");
				foreach (PlatformArchitecture platformArchitecture in platformProperties.getPlatformArchitectureList())
				{
					platformArchitecture.isActive = EditorGUILayout.ToggleLeft(platformArchitecture.name, platformArchitecture.isActive);
				}
				
				GUILayout.Space(20f);
			}

			if (platformProperties.isUsingTextureCompression())
			{
				GUIUtils.DrawCategoryLabel("Textures Compression");
				foreach (ITextureCompression textureCompression in platformProperties.getTextureCompressionList())
				{
					textureCompression.getTextureProperties().isActive = EditorGUILayout.ToggleLeft(textureCompression.getTextureProperties().name, textureCompression.getTextureProperties().isActive);
				}
			}
			
			
			GUILayout.Space(10f);
			
				
			if (GUIUtils.DrawDeleteRedButton("Delete platform"))
			{
				platformProperties.setSupported(false);
				m_updateAvailablePlatformListAction();
			}
		}
		
		
		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			PlatformProperties platformProperties = m_platform.getPlatformProperties();

			DistributionsPlatformRenderer distributionsPlatformRenderer = new DistributionsPlatformRenderer(platformProperties.getDistributionPlatformList());
			distributionsPlatformRenderer.checkWarningsAndErrors(errorReporter);

			if (platformProperties.getActivePlatformArchitectureList().Count == 0)
			{
				errorReporter.addWarning("No build will be performed for the platform '" + platformProperties.name + "'\nYou must select at least one platform architecture for this platform.");
			}
			else if (platformProperties.getActiveTextureCompressionList().Count == 0)
			{
				errorReporter.addWarning("No build will be performed for the platform '" + platformProperties.name + "'\nYou must select at least one texture compression for this platform.");
			}

			if (m_platformAdditionalRenderer != null)
			{
				m_platformAdditionalRenderer.checkWarningsAndErrors(errorReporter);
			}
		}
		
		
		/*
		 * Draw the summary of things that are going to be build for this platform
		 */
		public void drawBuildSummary(AdvancedSettings advancedSettings, Configuration configuration, string bundleVersion)
		{
			PlatformProperties platformProperties = configuration.platform.getPlatformProperties();
			
			if (!platformProperties.isSupported())
			{
				return;
			}

			GUIUtils.DrawTwoColumns("Release type", configuration.releaseType.name);
			GUIUtils.DrawTwoColumns("Product name", configuration.releaseType.productName);
			GUIUtils.DrawTwoColumns("Bundle identifier", configuration.releaseType.bundleIdentifier);
			GUIUtils.DrawTwoColumns("Bundle version", bundleVersion);
			GUIUtils.DrawTwoColumns("Platform", platformProperties.name);

			if (!configuration.distributionPlatform.name.Equals("Default"))
			{
				GUIUtils.DrawTwoColumns("Distribution Platform", configuration.distributionPlatform.name);
			}

			if (platformProperties.isUsingPlatformArchitecture())
			{
				GUIUtils.DrawTwoColumns("Architecture", configuration.platformArchitecture.name);
			}

			if (platformProperties.isUsingTextureCompression())
			{
				GUIUtils.DrawTwoColumns("Texture Compression", configuration.textureProperties.name);
			}

			if (m_platformAdditionalRenderer != null)
			{
				m_platformAdditionalRenderer.drawAdditionalBuildSummary(configuration.platformArchitecture, configuration.textureCompression, bundleVersion);
			}

			EditorGUILayout.LabelField(configuration.getBuildPath(advancedSettings, DateTime.Now, bundleVersion));
		}
	}
}