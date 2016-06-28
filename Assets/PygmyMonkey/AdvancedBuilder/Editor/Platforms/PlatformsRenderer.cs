using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class PlatformsRenderer : IDefaultRenderer
	{
		/*
		 * Platforms data
		 */
		private Platforms m_platforms;
		
		
		/*
		 * Constructor
		 */
		public PlatformsRenderer(Platforms platforms)
		{
			m_platforms = platforms;
			
			m_platformRendererList = new List<PlatformRenderer>()
			{
				#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
					new PlatformRenderer(m_platforms.getPlatformAndroid(), new PlatformAndroidAdditionalRenderer(m_platforms.getPlatformAndroid()), updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformiOS(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindows(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformMac(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformLinux(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindowsPhone8(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindowsStore(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWebGL(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformtvOS(), null, updateSupportedPlatformList),
				#else
					new PlatformRenderer(m_platforms.getPlatformAndroid(), new PlatformAndroidAdditionalRenderer(m_platforms.getPlatformAndroid()), updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformiOS(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWebPlayer(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindows(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformMac(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformLinux(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindowsPhone8(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWindowsStore(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformWebGL(), null, updateSupportedPlatformList),
					new PlatformRenderer(m_platforms.getPlatformBlackBerry(), null, updateSupportedPlatformList),
					#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
					new PlatformRenderer(m_platforms.getPlatformtvOS(), null, updateSupportedPlatformList),
					#endif
				#endif
			};
			
			updateSupportedPlatformList();
		}
		
		
		/*
		 * Some private parameters
		 */
		private static int m_platformListIndex;
		private List<IPlatform> m_platformSupportedList = new List<IPlatform>();
		private List<PlatformRenderer> m_platformRendererList;
		
		
		/*
		 * Get platform renderer from platform type
		 */
		public PlatformRenderer getPlatformRendererFromPlatform(PlatformType platformType)
		{
			switch(platformType)
			{

			#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
				case PlatformType.Android:
					return m_platformRendererList[0];

				case PlatformType.iOS:
					return m_platformRendererList[1];

				case PlatformType.Windows:
					return m_platformRendererList[2];

				case PlatformType.Mac:
					return m_platformRendererList[3];

				case PlatformType.Linux:
					return m_platformRendererList[4];

				case PlatformType.WindowsPhone8:
					return m_platformRendererList[5];

				case PlatformType.WindowsStore:
					return m_platformRendererList[6];

				case PlatformType.WebGL:
					return m_platformRendererList[7];

				case PlatformType.tvOS:
					return m_platformRendererList[8];
			#else
				case PlatformType.Android:
					return m_platformRendererList[0];

				case PlatformType.iOS:
					return m_platformRendererList[1];

				case PlatformType.WebPlayer:
					return m_platformRendererList[2];

				case PlatformType.Windows:
					return m_platformRendererList[3];

				case PlatformType.Mac:
					return m_platformRendererList[4];

				case PlatformType.Linux:
					return m_platformRendererList[5];

				case PlatformType.WindowsPhone8:
					return m_platformRendererList[6];

				case PlatformType.WindowsStore:
					return m_platformRendererList[7];

				case PlatformType.WebGL:
					return m_platformRendererList[8];

				case PlatformType.BlackBerry:
					return m_platformRendererList[9];

				#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
				case PlatformType.tvOS:
					return m_platformRendererList[10];
				#endif
			#endif

			default:
				throw new Exception("The platform " + platformType + " does not have a platform renderer");
			}
		}


		private IEnumerable<KeyValuePair<IPlatform, PlatformRenderer>> getSupportedPlatformRenderers()
		{
			IEnumerable<IPlatform> platforms = m_platforms.platformDictionary.Values;
			IEnumerable<IPlatform> supportedPlatforms = platforms.Where(x => x.getPlatformProperties().isSupported());
			IEnumerable<KeyValuePair<IPlatform, PlatformRenderer>> platformsPair = supportedPlatforms.Select(x => new KeyValuePair<IPlatform, PlatformRenderer>(x, getPlatformRendererFromPlatform(x.getPlatformProperties().platformType)));

			return platformsPair;
		}


		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			IEnumerable<KeyValuePair<IPlatform, PlatformRenderer>> supportedPlatformList = getSupportedPlatformRenderers();

			foreach (KeyValuePair<IPlatform, PlatformRenderer> supportedPlatform in supportedPlatformList)
			{
				IPlatform platform = supportedPlatform.Key;
				PlatformRenderer platformRenderer = supportedPlatform.Value;

				if (GUIUtils.DrawHeader(platform.getPlatformProperties().name))
				{
					GUIUtils.BeginContents();
					platformRenderer.drawInspector();
					GUIUtils.EndContents();
				}
			}

			if (supportedPlatformList.Count() > 0)
			{
				GUILayout.Space(10f);
			}
			
			drawSupportedPlatformList();
		}

		
		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			IEnumerable<KeyValuePair<IPlatform, PlatformRenderer>> supportedPlatformList = getSupportedPlatformRenderers();

			foreach (KeyValuePair<IPlatform, PlatformRenderer> supportedPlatform in supportedPlatformList)
			{
				PlatformRenderer platformRenderer = supportedPlatform.Value;
				platformRenderer.checkWarningsAndErrors(errorReporter);
			}

			/*
			 * If no platform is selected at all
			 */
			if (supportedPlatformList.Count() == 0)
			{
				errorReporter.addError("You need to add at least on platform in the 'Platforms' section.");
			}
			else
			{
				/*
				 * If no distributionplatform is activated at all
				 */
				int totalDistributionPlatformUsedCount = m_platforms.platformDictionary.Values.Where(x => x.getPlatformProperties().isSupported()).Sum(x => x.getPlatformProperties().getActiveDistributionPlatformList().Count);
				if (totalDistributionPlatformUsedCount == 0)
				{
					errorReporter.addError("No batch build will be performed\nYou must have at least one distribution platform selected.");
				}


				/*
				 * If no platform architecture is activated at all
				 */
				int totalPlatformArchitectureUsedCount = m_platforms.platformDictionary.Values.Where(x => x.getPlatformProperties().isSupported()).Sum(x => x.getPlatformProperties().getActivePlatformArchitectureList().Count);
				
				if (totalPlatformArchitectureUsedCount == 0)
				{
					errorReporter.addError("No batch build will be performed\nYou must have at least one platform architecture selected, see warnings.");
				}


				/*
				 * If no textureCompression is activated at all
				 */
				int totalTextureCompressionUsedCount = m_platforms.platformDictionary.Values.Where(x => x.getPlatformProperties().isSupported()).Sum(x => x.getPlatformProperties().getActiveTextureCompressionList().Count);

				if (totalTextureCompressionUsedCount == 0)
				{
					errorReporter.addError("No batch build will be performed\nYou must have at least one texture compression selected, see warnings.");
				}
			}
		}
		
		
		/*
		 * Draw the inspector popup with available platforms
		 */
		private void drawSupportedPlatformList()
		{
			bool isAtLeastOnePlatformSupported = (m_platforms.platformDictionary.Values.Where(x => !x.getPlatformProperties().isSupported()).Count() != 0);

			GUI.enabled = isAtLeastOnePlatformSupported;
			EditorGUILayout.BeginVertical(LegacyGUIStyle.CenterMarginStyle);
			{
				string[] platformSupportedStringArray = m_platformSupportedList.Select(p => p.getPlatformProperties().name).ToArray();

				m_platformListIndex = EditorGUILayout.Popup(m_platformListIndex, platformSupportedStringArray);
				if (GUILayout.Button("Add Platform"))
				{
					PlatformType paltformType = m_platformSupportedList[m_platformListIndex].getPlatformProperties().platformType;
					m_platforms.platformDictionary[paltformType].getPlatformProperties().setSupported(true);
					updateSupportedPlatformList();
				}
			}
			EditorGUILayout.EndVertical();
			GUI.enabled = true;
		}
		
		
		/*
		 * Update the available platform list
		 */
		private void updateSupportedPlatformList()
		{
			m_platformSupportedList = m_platforms.platformDictionary.Values.Where(x => !x.getPlatformProperties().isSupported()).ToList();

			if (m_platformListIndex >= m_platformSupportedList.Count)
			{
				m_platformListIndex = m_platformSupportedList.Count - 1;
			}
			
			if (m_platformListIndex < 0)
			{
				m_platformListIndex = 0;
			}
		}
	}
}