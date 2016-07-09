using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public static class AppParametersHelper
	{
		private static int m_androidBundleVersionCode;
		private static MobileTextureSubtarget m_androidTextureSubTarget;
		private static MobileTextureSubtarget m_blackberryTextureSubTarget;
		private static string m_iPhoneBundleIdentifier;
		private static string m_iOSApplicationDisplayName;
		private static string m_bundleIdentifier;
		private static string m_productName;
		private static string m_bundleVersion;
		private static string[] m_customScriptingDefineSymbolArray;

		private static BuildTarget m_buildTarget;

		private static string m_releaseType = "";
		private static string m_platformType = "";
		private static string m_distributionPlatform = "";
		private static string m_platformArchitecture = "";
		private static string m_textureCompression = "";
		private static string m_appParamBundleIdentifier;
		private static string m_appParamProductName;
		private static string m_appParamBundleVersion;

		public static void SaveParameters()
		{
			m_androidBundleVersionCode = PlayerSettings.Android.bundleVersionCode;

			m_androidTextureSubTarget = EditorUserBuildSettings.androidBuildSubtarget;
			#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
			m_blackberryTextureSubTarget = EditorUserBuildSettings.blackberryBuildSubtarget;
			#endif

			m_iPhoneBundleIdentifier = PlayerSettings.iPhoneBundleIdentifier;
			m_iOSApplicationDisplayName = PlayerSettings.iOS.applicationDisplayName;

			m_bundleIdentifier = PlayerSettings.bundleIdentifier;
			m_productName = PlayerSettings.productName;
			m_bundleVersion = PlayerSettings.bundleVersion;

			Array buildTargetGroupArray = Enum.GetValues(typeof(BuildTargetGroup));
            m_customScriptingDefineSymbolArray = new string[buildTargetGroupArray.Length];
			for (int i = 0; i < buildTargetGroupArray.Length; i++)
			{
				m_customScriptingDefineSymbolArray[i] = PlayerSettings.GetScriptingDefineSymbolsForGroup((BuildTargetGroup)buildTargetGroupArray.GetValue(i));
			}

			m_releaseType = AppParameters.Get.releaseType;
			m_platformType = AppParameters.Get.platformType;
			m_distributionPlatform = AppParameters.Get.distributionPlatform;
			m_platformArchitecture = AppParameters.Get.platformArchitecture;
			m_textureCompression = AppParameters.Get.textureCompression;
			m_appParamBundleIdentifier = AppParameters.Get.bundleIdentifier;
			m_appParamProductName = AppParameters.Get.productName;
			m_appParamBundleVersion = AppParameters.Get.bundleVersion;
		}

		public static void RestoreParameters()
		{
			PlayerSettings.Android.bundleVersionCode = m_androidBundleVersionCode;

			EditorUserBuildSettings.androidBuildSubtarget = m_androidTextureSubTarget;
			#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
			EditorUserBuildSettings.blackberryBuildSubtarget = m_blackberryTextureSubTarget;
			#endif
			
			PlayerSettings.iPhoneBundleIdentifier = m_iPhoneBundleIdentifier;
			PlayerSettings.iOS.applicationDisplayName = m_iOSApplicationDisplayName;
			
			PlayerSettings.bundleIdentifier = m_bundleIdentifier;
			PlayerSettings.productName = m_productName;
			PlayerSettings.bundleVersion = m_bundleVersion;

			Array buildTargetGroupArray = Enum.GetValues(typeof(BuildTargetGroup));
			for (int i = 0; i < m_customScriptingDefineSymbolArray.Length; i++)
			{
				BuildTargetGroup buildTargetGroup = (BuildTargetGroup)buildTargetGroupArray.GetValue(i);

				if (buildTargetGroup != BuildTargetGroup.Unknown)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, m_customScriptingDefineSymbolArray[i]);
				}
			}

			AppParameters.Get.updateParameters(m_releaseType, m_platformType, m_distributionPlatform, m_platformArchitecture, m_textureCompression, m_appParamBundleIdentifier, m_appParamProductName, m_appParamBundleVersion);

			GUIUtils.RefreshAssets();
		}

		public static void SaveBuildTarget()
		{
			m_buildTarget = EditorUserBuildSettings.activeBuildTarget;
		}

		public static void RestoreBuildTarget()
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(m_buildTarget);
		}
	}
}
