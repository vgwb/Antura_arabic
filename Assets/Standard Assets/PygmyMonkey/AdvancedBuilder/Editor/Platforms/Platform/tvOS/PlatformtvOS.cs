using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformtvOS : IPlatform
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
		public PlatformtvOS()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.tvOS,
				"tvOS",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(null, "", BuildTarget.tvOS, true),
				},
				new List<ITextureCompression>() { new DefaultTextureCompression() }
			);
		}
		
		
		/*
		 * Set up additional parameters
		 */
		public void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration)
		{
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
#endif