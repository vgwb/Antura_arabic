using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformWebPlayer : IPlatform
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
		public PlatformWebPlayer()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.WebPlayer,
				"WebPlayer",
				new List<DistributionPlatform>
				{
					new DistributionPlatform("My Website", true),
					new DistributionPlatform("Kongregate", true),
				},
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture("WebPlayer", "", BuildTarget.WebPlayer, true),
					new PlatformArchitecture("WebPlayer streamed", "", BuildTarget.WebPlayerStreamed),
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