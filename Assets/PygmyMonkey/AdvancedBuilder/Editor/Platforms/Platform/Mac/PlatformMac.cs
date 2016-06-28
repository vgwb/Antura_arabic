using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformMac : IPlatform
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
		public PlatformMac()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.Mac,
				"Mac",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture("OSX x86", ".app", BuildTarget.StandaloneOSXIntel, true),
					new PlatformArchitecture("OSX x86_64", ".app", BuildTarget.StandaloneOSXIntel64),
					new PlatformArchitecture("OSX Universal", ".app", BuildTarget.StandaloneOSXUniversal),
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