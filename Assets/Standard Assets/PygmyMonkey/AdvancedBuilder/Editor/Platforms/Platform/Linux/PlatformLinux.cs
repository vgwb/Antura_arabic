using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformLinux : IPlatform
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
		public PlatformLinux()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.Linux,
				"Linux",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture("Linux Universal", ".x86_64", BuildTarget.StandaloneLinuxUniversal, true),
					new PlatformArchitecture("Linux x86", ".x86", BuildTarget.StandaloneLinux),
					new PlatformArchitecture("Linux x86_64", ".x86_64", BuildTarget.StandaloneLinux64),
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