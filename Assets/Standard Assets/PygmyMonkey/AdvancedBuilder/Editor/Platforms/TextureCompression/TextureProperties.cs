using System;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class TextureProperties
	{
		/*
		 * Define if the texture compression is used
		 */
		public bool isActive;


		/*
		 * Texture Compression name
		 */
		public string name;


		/*
		 * Constructor
		 */
		public TextureProperties(string name, bool isActive)
		{
			this.name = name;
			this.isActive = isActive;
		}

		public string getDefine()
		{
			if (name.Equals("Default"))
			{
				return null;
			}

			return "TC_" + name.ToUpper().Replace(" ", "_");
		}
	}
}
