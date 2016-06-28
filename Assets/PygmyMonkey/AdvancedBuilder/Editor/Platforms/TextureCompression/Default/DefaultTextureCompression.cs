using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	public class DefaultTextureCompression : ITextureCompression
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
		 * Constructor
		 */
		public DefaultTextureCompression()
		{
			m_textureProperties = new TextureProperties("Default", true);
		}
	}
}