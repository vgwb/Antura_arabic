using UnityEditor;
using UnityEngine;
using System;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class TextureCompressionBlackBerry : ITextureCompression
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
		 * Return the BlackBerry Build Sub Target
		 */
		public readonly MobileTextureSubtarget subTarget;
		

		/*
		 * Constructor
		 */
		public TextureCompressionBlackBerry(MobileTextureSubtarget subTarget, bool isActive = false)
		{
			this.subTarget = subTarget;

			m_textureProperties = new TextureProperties(subTarget.ToString(), isActive);
		}
	}
}
#endif