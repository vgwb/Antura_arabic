using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformWebGL : IPlatform
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
		public PlatformWebGL()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.WebGL,
				"WebGL",
				new List<DistributionPlatform>
				{
					new DistributionPlatform("My Website", true),
					new DistributionPlatform("Kongregate", true),
				},
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(null, "", BuildTarget.WebGL, true),
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