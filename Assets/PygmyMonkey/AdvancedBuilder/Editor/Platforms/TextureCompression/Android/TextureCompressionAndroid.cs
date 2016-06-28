using UnityEditor;
using UnityEngine;
using System;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class TextureCompressionAndroid : ITextureCompression
	{
		/*
		 * Platform common properties
		 */
		[SerializeField] private TextureProperties m_textureProperties;
		
		public TextureProperties getTextureProperties()
		{
			return m_textureProperties;
		}
		
		
		/*
		 * Return the Android Build Sub Target
		 */
		public readonly MobileTextureSubtarget subTarget;
		
		
		/*
		 * The version code prefix
		 */
		public readonly int versionCodePrefix;
		

		/*
		 * Constructor
		 */
		public TextureCompressionAndroid(MobileTextureSubtarget subTarget, int versionCodePrefix, bool isActive = false)
		{
			this.subTarget = subTarget;
			this.versionCodePrefix = versionCodePrefix;

			m_textureProperties = new TextureProperties(subTarget.ToString(), isActive);
		}
	}
}