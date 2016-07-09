using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class Platforms
	{
		[SerializeField] private PlatformAndroid m_plateformAndroid = new PlatformAndroid();
		public PlatformAndroid getPlatformAndroid()
		{
			return m_plateformAndroid;
		}
		
		[SerializeField] private PlatformiOS m_plateformiOS = new PlatformiOS();
		public PlatformiOS getPlatformiOS()
		{
			return m_plateformiOS;
		}

		#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		[SerializeField] private PlatformWebPlayer m_plateformWebPlayer = new PlatformWebPlayer();
		public PlatformWebPlayer getPlatformWebPlayer()
		{
			return m_plateformWebPlayer;
		}
		#endif
		
		[SerializeField] private PlatformWindows m_plateformWindows = new PlatformWindows();
		public PlatformWindows getPlatformWindows()
		{
			return m_plateformWindows;
		}
		
		[SerializeField] private PlatformMac m_plateformMac = new PlatformMac();
		public PlatformMac getPlatformMac()
		{
			return m_plateformMac;
		}
		
		[SerializeField] private PlatformLinux m_plateformLinux = new PlatformLinux();
		public PlatformLinux getPlatformLinux()
		{
			return m_plateformLinux;
		}
		
		[SerializeField] private PlatformWindowsPhone8 m_plateformWindowsPhone8 = new PlatformWindowsPhone8();
		public PlatformWindowsPhone8 getPlatformWindowsPhone8()
		{
			return m_plateformWindowsPhone8;
		}

		[SerializeField] private PlatformWindowsStore m_plateformWindowsStore = new PlatformWindowsStore();
		public PlatformWindowsStore getPlatformWindowsStore()
		{
			return m_plateformWindowsStore;
		}
		
		[SerializeField] private PlatformWebGL m_plateformWebGL = new PlatformWebGL();
		public PlatformWebGL getPlatformWebGL()
		{
			return m_plateformWebGL;
		}

		#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		[SerializeField] private PlatformBlackBerry m_plateformBlackBerry = new PlatformBlackBerry();
		public PlatformBlackBerry getPlatformBlackBerry()
		{
			return m_plateformBlackBerry;
		}
		#endif

		#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
		[SerializeField] private PlatformtvOS m_plateformtvOS = new PlatformtvOS();
		public PlatformtvOS getPlatformtvOS()
		{
			return m_plateformtvOS;
		}
		#endif
		
		
		/*
		 * The dictionary containing all the supported platforms
		 */
		public Dictionary<PlatformType, IPlatform> platformDictionary { get; private set; }
		
		
		public Platforms()
		{
			platformDictionary = new Dictionary<PlatformType, IPlatform>();

			#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
				platformDictionary.Add(m_plateformAndroid.getPlatformProperties().platformType, m_plateformAndroid);
				platformDictionary.Add(m_plateformiOS.getPlatformProperties().platformType, m_plateformiOS);
				platformDictionary.Add(m_plateformWindows.getPlatformProperties().platformType, m_plateformWindows);
				platformDictionary.Add(m_plateformMac.getPlatformProperties().platformType, m_plateformMac);
				platformDictionary.Add(m_plateformLinux.getPlatformProperties().platformType, m_plateformLinux);
				platformDictionary.Add(m_plateformWindowsPhone8.getPlatformProperties().platformType, m_plateformWindowsPhone8);
				platformDictionary.Add(m_plateformWindowsStore.getPlatformProperties().platformType, m_plateformWindowsStore);
				platformDictionary.Add(m_plateformWebGL.getPlatformProperties().platformType, m_plateformWebGL);
				platformDictionary.Add(m_plateformtvOS.getPlatformProperties().platformType, m_plateformtvOS);
			#else
				platformDictionary.Add(m_plateformAndroid.getPlatformProperties().platformType, m_plateformAndroid);
				platformDictionary.Add(m_plateformiOS.getPlatformProperties().platformType, m_plateformiOS);
				platformDictionary.Add(m_plateformWebPlayer.getPlatformProperties().platformType, m_plateformWebPlayer);
				platformDictionary.Add(m_plateformWindows.getPlatformProperties().platformType, m_plateformWindows);
				platformDictionary.Add(m_plateformMac.getPlatformProperties().platformType, m_plateformMac);
				platformDictionary.Add(m_plateformLinux.getPlatformProperties().platformType, m_plateformLinux);
				platformDictionary.Add(m_plateformWindowsPhone8.getPlatformProperties().platformType, m_plateformWindowsPhone8);
				platformDictionary.Add(m_plateformWindowsStore.getPlatformProperties().platformType, m_plateformWindowsStore);
				platformDictionary.Add(m_plateformWebGL.getPlatformProperties().platformType, m_plateformWebGL);
				platformDictionary.Add(m_plateformBlackBerry.getPlatformProperties().platformType, m_plateformBlackBerry);
				#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0
				platformDictionary.Add(m_plateformtvOS.getPlatformProperties().platformType, m_plateformtvOS);
				#endif
			#endif
		}

		public IPlatform getPlatformFromType(PlatformType platformType)
		{
			if (!platformDictionary.ContainsKey(platformType))
			{
				throw new Exception("PlatformType : " + platformType.ToString() + " is not defined");
			}

			return platformDictionary[platformType];
		}


		public ITextureCompression getTextureCompressionFromNameAndPlatformType(string name, PlatformType platformType)
		{
			IPlatform platform = getPlatformFromType(platformType);
			ITextureCompression textureCompression = platform.getPlatformProperties().getTextureCompressionList().Where(x => x.getTextureProperties().name.Equals(name)).FirstOrDefault();
			if (textureCompression == null)
			{
				Debug.LogWarning("Could not find the texture compression with name: " + name);
			}

			return textureCompression;
		}
	}
}