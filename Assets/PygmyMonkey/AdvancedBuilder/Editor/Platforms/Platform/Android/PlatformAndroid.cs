using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformAndroid : IPlatform
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
		private List<TextureCompressionAndroid> m_textureCompressionList = new List<TextureCompressionAndroid>()
		{
			new TextureCompressionAndroid(MobileTextureSubtarget.ASTC,		16),
			new TextureCompressionAndroid(MobileTextureSubtarget.ETC2,		15),
			new TextureCompressionAndroid(MobileTextureSubtarget.PVRTC,		14),
			new TextureCompressionAndroid(MobileTextureSubtarget.DXT,		13),
			new TextureCompressionAndroid(MobileTextureSubtarget.ATC,		12),
			new TextureCompressionAndroid(MobileTextureSubtarget.ETC,		11),
			new TextureCompressionAndroid(MobileTextureSubtarget.Generic,	10, true),
		};
		
		
		/*
		 * Android device filters
		 */
		private const string m_fatDeviceFilter = "FAT";
		private const string m_armDeviceFilter = "ARMv7";
		private const string m_x86DeviceFilter = "x86";
		
		
		/*
		 * Constructor
		 */
		public PlatformAndroid()
		{
			m_platformProperties = new PlatformProperties(
				PlatformType.Android,
				"Android",
				new List<DistributionPlatform>
				{
					new DistributionPlatform("Google Play", true),
					new DistributionPlatform("Amazon Store"),
					new DistributionPlatform("Samsung AppStore")
				},
				new List<PlatformArchitecture>
				{
					new PlatformArchitecture(m_fatDeviceFilter, ".apk", BuildTarget.Android, true),
					new PlatformArchitecture(m_armDeviceFilter, ".apk", BuildTarget.Android, false),
					new PlatformArchitecture(m_x86DeviceFilter, ".apk", BuildTarget.Android, false),
				},
				m_textureCompressionList.Cast<ITextureCompression>().ToList()
			);
		}
		
		
		/*
		 * Return Android BundleVersion based on TextureCompression and BundleVersion
		 * Format: xxyyzzz
		 * xx: Device filter number
		 * yy: Texture compression number
		 * zzz: BundleVersion number (0.3.0 -> 030)
		 */
		public int getAndroidBundleVersionCode(PlatformArchitecture platformArchitecture, TextureCompressionAndroid textureCompressionAndroid, string bundleVersion)
		{
			int deviceFilterNumber = 0;
			if (platformArchitecture.name.Equals(m_x86DeviceFilter))
			{
				deviceFilterNumber = 12;
			}
			else if (platformArchitecture.name.Equals(m_armDeviceFilter))
			{
				deviceFilterNumber = 11;
			}
			else if (platformArchitecture.name.Equals(m_fatDeviceFilter))
			{
				deviceFilterNumber = 10;
			}

			return int.Parse(deviceFilterNumber.ToString() + textureCompressionAndroid.versionCodePrefix + Regex.Replace(bundleVersion, "[^0-9]", ""));
		}


		/*
		 * Set up additional parameters
		 */
		public void setupAdditionalParameters(ProductParameters productParameters, Configuration configuration)
		{
			//EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

			if (configuration.platformArchitecture.name.Equals(m_fatDeviceFilter))
			{
				PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
			}
			else if (configuration.platformArchitecture.name.Equals(m_armDeviceFilter))
			{
				PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
			}
			else if (configuration.platformArchitecture.name.Equals(m_x86DeviceFilter))
			{
				PlayerSettings.Android.targetDevice = AndroidTargetDevice.x86;
			}

			TextureCompressionAndroid textureCompressionAndroid = (TextureCompressionAndroid)configuration.textureCompression;
			PlayerSettings.Android.bundleVersionCode = getAndroidBundleVersionCode(configuration.platformArchitecture, textureCompressionAndroid, productParameters.bundleVersion);
			EditorUserBuildSettings.androidBuildSubtarget = textureCompressionAndroid.subTarget;
		}
		
		
		/*
		 * Format destination file path
		 */
		public string formatDestinationPath(string filePath)
		{
			return filePath;
		}
	}
}