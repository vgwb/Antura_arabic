using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformBlackBerry : IPlatform
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
		 * List of supported texture compression
		 */
		[SerializeField]
		private List<TextureCompressionBlackBerry> m_textureCompressionList = new List<TextureCompressionBlackBerry>()
		{
			new TextureCompressionBlackBerry(MobileTextureSubtarget.PVRTC),
			new TextureCompressionBlackBerry(MobileTextureSubtarget.ATC),
			new TextureCompressionBlackBerry(MobileTextureSubtarget.ETC),
			new TextureCompressionBlackBerry(MobileTextureSubtarget.Generic, true),
		};


		/*
		 * BlackBerry Build types
		 */
		private const string m_localBuildType = "Local";
		private const string m_signedBuildType = "Signed";
		
		
		/*
		 * Constructor
		 */
		public PlatformBlackBerry()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.BlackBerry,
				"BlackBerry",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(m_localBuildType, ".bar", BuildTarget.BlackBerry, true),
					new PlatformArchitecture(m_signedBuildType, ".bar", BuildTarget.BlackBerry),
				},
				m_textureCompressionList.Cast<ITextureCompression>().ToList()
			);
		}
		
		
		/*
		 * Set up additional parameters
		 */
		public void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration)
		{
			if (configuration.platformArchitecture.name.Equals(m_localBuildType))
			{
				EditorUserBuildSettings.blackberryBuildType = BlackBerryBuildType.Debug;
			}
			else if (configuration.platformArchitecture.name.Equals(m_signedBuildType))
			{
				EditorUserBuildSettings.blackberryBuildType = BlackBerryBuildType.Submission;
			}

			TextureCompressionBlackBerry textureCompressionBlackBerry = (TextureCompressionBlackBerry)configuration.textureCompression;
			EditorUserBuildSettings.blackberryBuildSubtarget = textureCompressionBlackBerry.subTarget;
		}


		/*
		 * Format destination file path
		 */
		public string formatDestinationPath(string filePath)
		{
			string folderPath = filePath.Substring(0, filePath.LastIndexOf("/") + 1);
			string fileName = filePath.Replace(folderPath, string.Empty);
			//fileName = fileName.Replace(" ", string.Empty);

			return folderPath + fileName;
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