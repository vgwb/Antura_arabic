using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformiOS : IPlatform
	{
		/*
		 * Platform common properties
		 */
		[SerializeField] private PlatformProperties m_platformProperties;
		
		public PlatformProperties getPlatformProperties()
		{
			return m_platformProperties;
		}

		
		/*
		 * Constructor
		 */
		public PlatformiOS()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.iOS,
				"iOS",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(null, "", BuildTarget.iOS, true),
				},
				new List<ITextureCompression>() { new DefaultTextureCompression() }
			);
		}
		
		
		/*
		 * Set up additional parameters
		 */
		public void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration)
		{
			PlayerSettings.iPhoneBundleIdentifier = configuration.releaseType.bundleIdentifier;
			PlayerSettings.iOS.applicationDisplayName = configuration.releaseType.productName;
		}
		
		
		/*
		 * Format destination file path
		 */
		public string formatDestinationPath(string filePath)
		{
			return filePath;
		}
		
		
		/*
		 * Return specific platform errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
		}
	}
}