using UnityEditor;
using UnityEngine;
using System;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class PlatformAndroidAdditionalRenderer : IPlatformAdditionalRenderer
	{
		private PlatformAndroid m_platformAndroid;

		public PlatformAndroidAdditionalRenderer(PlatformAndroid platformAndroid)
		{
			m_platformAndroid = platformAndroid;
		}

		/*
		 * Draw the summary of things that are going to be build for this platform
		 */
		public void drawAdditionalBuildSummary(PlatformArchitecture platformArchitecture, ITextureCompression textureCompression, string bundleVersion)
		{
			TextureCompressionAndroid textureCompressionAndroid = (TextureCompressionAndroid)textureCompression;
			GUIUtils.DrawTwoColumns("Bundle Version Code", m_platformAndroid.getAndroidBundleVersionCode(platformArchitecture, textureCompressionAndroid, bundleVersion).ToString());
		}
		
		
		/*
		 * Return specific platform errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			AdvancedBuilder advancedBuilder = (AdvancedBuilder)AssetDatabase.LoadAssetAtPath("Assets/Standard Assets/PygmyMonkey/AdvancedBuilder/Editor/AdvancedBuilder.asset", typeof(AdvancedBuilder));
			
			if (advancedBuilder.getAdvancedSettings().checkAndroidKeystorePasswords)
			{
				if (PlayerSettings.keystorePass.Length == 0)
				{
					errorReporter.addError("You need to define your Android Keystore password in Edit -> Project Settings -> Player -> Android -> Publishing Settings.");
				}
				else if (PlayerSettings.keyaliasPass.Length == 0)
				{
					errorReporter.addError("You need to define your Android Alias password in Edit -> Project Settings -> Player -> Android -> Publishing Settings.");
				}
			}
		}
	}
}