using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class ProjectConfigurations
	{
		/*
		 * ProjectConfigurations data
		 */
		public List<Configuration> configurationList = new List<Configuration>();
		public int configurationIndex;
		public int selectedConfigurationIndex;

		/*
		 * We compute the total number of build
		 */
		public int getTotalBuildCount()
		{
			return configurationList.Count(x => x.isEnabled);
		}

		public int getTotalAutoRunBuildCount()
		{
			return configurationList.Count(x => x.shouldAutoRunPlayer);
		}

		/*
		 * Get list of available configurations
		 */
		public List<Configuration> getAvailableConfigurationList(ReleaseTypes releasesTypes, Platforms platforms)
		{
			List<ReleaseType> activeReleaseTypeList = releasesTypes.getReleaseTypeList().Where(x => x.isActive).ToList();
			List<IPlatform> activePlatformList = platforms.platformDictionary.Values.Where(x => x.getPlatformProperties().isSupported()).ToList();
			
			List<Configuration> configurationListResult = new List<Configuration>();

			foreach (ReleaseType releaseType in activeReleaseTypeList)
			{
				foreach (IPlatform platform in activePlatformList)
				{
					PlatformProperties platformProperties = platform.getPlatformProperties();
					
					foreach (DistributionPlatform distributionPlatform in platformProperties.getActiveDistributionPlatformList())
					{
						foreach (PlatformArchitecture platformArchitecture in platformProperties.getActivePlatformArchitectureList())
						{
							foreach (ITextureCompression textureCompression in platformProperties.getActiveTextureCompressionList())
							{
								Configuration configuration = new Configuration(releaseType, platform, distributionPlatform, platformArchitecture, textureCompression);
								
								if (!configurationList.Contains(configuration))
								{
									configurationListResult.Add(configuration); 
								}
							}
						}
					}
				}
			}
			
			return configurationListResult;
		}

		/*
		 * Return if the configuration is the one in AppParameters
		 */
		public bool isCurrentConfig(Configuration configuration)
		{
			return configuration.releaseType.getFormattedName().Equals(AppParameters.Get.releaseType)
				&& configuration.platform.getPlatformProperties().getFormattedPlatformType().Equals(AppParameters.Get.platformType)
				&& configuration.distributionPlatform.getFormattedName().Equals(AppParameters.Get.distributionPlatform)
				&& configuration.platformArchitecture.getFormattedName().Equals(AppParameters.Get.platformArchitecture)
				&& configuration.textureProperties.name.Equals(AppParameters.Get.textureCompression)
				&& configuration.releaseType.productName.Equals(AppParameters.Get.productName)
				&& configuration.releaseType.bundleIdentifier.Equals(AppParameters.Get.bundleIdentifier);
		}
	}
}