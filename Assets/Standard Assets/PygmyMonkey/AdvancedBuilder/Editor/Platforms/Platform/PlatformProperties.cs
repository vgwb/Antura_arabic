using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformProperties
	{
		/*
		 * Determine if this platform is added to the list of supported platforms
		 */
		[SerializeField]
		private bool m_isSupported;
		
		public void setSupported(bool isSupported)
		{
			m_isSupported = isSupported;
		}
		
		public bool isSupported()
		{
			return m_isSupported;
		}


		/*
		 * List of distributionplatform for the platform
		 */
		[SerializeField]
		private List<DistributionPlatform> m_distributionPlatformList = new List<DistributionPlatform>();

		public List<DistributionPlatform> getDistributionPlatformList()
		{
			return m_distributionPlatformList;
		}

		public List<DistributionPlatform> getActiveDistributionPlatformList()
		{
			List<DistributionPlatform> distributionPlatformList = m_distributionPlatformList.ToList();
			if (distributionPlatformList.Count == 0)
			{
				distributionPlatformList.Add(new DistributionPlatform("Default", true));
			}

			return distributionPlatformList.Where(x => x.isActive).ToList();
		}
		
		
		/*
		 * List of platform architecture
		 */
		[SerializeField]
		private List<PlatformArchitecture> m_platformArchitectureList = new List<PlatformArchitecture>();
		
		public List<PlatformArchitecture> getPlatformArchitectureList()
		{
			return m_platformArchitectureList;
		}
		
		public List<PlatformArchitecture> getActivePlatformArchitectureList()
		{
			return m_platformArchitectureList.Where(x => x.isActive).ToList();
		}

		public bool isUsingPlatformArchitecture()
		{
			return m_platformArchitectureList.Count(x => !string.IsNullOrEmpty(x.name)) != 0;
		}
		
		
		/*
		 * List of texture compressions
		 */
		private List<ITextureCompression> m_textureCompressionList;

		public List<ITextureCompression> getTextureCompressionList()
		{
			return m_textureCompressionList;
		}

		public List<ITextureCompression> getActiveTextureCompressionList()
		{
			return m_textureCompressionList.Where(x => x.getTextureProperties().isActive).ToList();
		}

		public bool isUsingTextureCompression()
		{
			return m_textureCompressionList.Count(x => x.GetType() != typeof(DefaultTextureCompression)) != 0;
		}


		/*
		 * Platform type
		 */
		public readonly PlatformType platformType;
		
		
		/*
		 * Platform name
		 */
		public string name;


		public string getFormattedPlatformType()
		{
			return platformType.ToString();
		}


		public string getDefine()
		{
			return "PT_" + platformType.ToString().ToUpper().Replace(" ", "_");
		}

		/*
		 * Constructor
		 */
		public PlatformProperties(PlatformType platformType, string name, List<DistributionPlatform> distributionPlatformList, List<PlatformArchitecture> platformArchitectureList, List<ITextureCompression> textureCompressionList)
		{
			this.platformType = platformType;
			this.name = name;
			m_distributionPlatformList = distributionPlatformList;
			m_platformArchitectureList = platformArchitectureList;
			m_textureCompressionList = textureCompressionList;
		}
	}
}
