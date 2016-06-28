using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformWindowsStore : IPlatform
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
		 * Windows Store architectures
		 */
		private const string m_sdk80 = "8.0";
		private const string m_sdk81 = "8.1";
		private const string m_sdkPhone81 = "Phone 8.1";
		private const string m_sdkUniversal81 = "Universal 8.1";


		public static bool IsUniversal(string architectureName)
		{
			return architectureName.Equals(m_sdkUniversal81);
		}
		
		
		/*
		 * Constructor
		 */
		public PlatformWindowsStore()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.WindowsStore,
				"Windows Store",
				new List<DistributionPlatform> { },
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(m_sdk80, "/", BuildTarget.WSAPlayer),
					new PlatformArchitecture(m_sdk81, "/", BuildTarget.WSAPlayer),
					new PlatformArchitecture(m_sdkPhone81, "/", BuildTarget.WSAPlayer),
					new PlatformArchitecture(m_sdkUniversal81, "/", BuildTarget.WSAPlayer, true),
				},
				new List<ITextureCompression>() { new DefaultTextureCompression() }
			);
		}
		
		
		/*
		 * Set up additional parameters
		 */
		public void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration)
		{
			//EditorUserBuildSettings.wsaGenerateReferenceProjects = true;

			if (configuration.platformArchitecture.name.Equals(m_sdk80))
			{
				EditorUserBuildSettings.wsaSDK = WSASDK.SDK80;
			}
			else if (configuration.platformArchitecture.name.Equals(m_sdk81))
			{
				EditorUserBuildSettings.wsaSDK = WSASDK.SDK81;
			}
			else if (configuration.platformArchitecture.name.Equals(m_sdkPhone81))
			{
				EditorUserBuildSettings.wsaSDK = WSASDK.PhoneSDK81;
			}
			else if (configuration.platformArchitecture.name.Equals(m_sdkUniversal81))
			{
				EditorUserBuildSettings.wsaSDK = WSASDK.UniversalSDK81;
			}

			EditorUserBuildSettings.wsaBuildAndRunDeployTarget = configuration.wsaBuildAndRunDeployTarget;
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
