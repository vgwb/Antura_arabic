using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class ReleaseType
	{
		/*
		 * Define if this release type is used
		 */
		public bool isActive;
		
		
		/*
		 * The release type name
		 */
		public string name;
		
		
		/*
		 * The bundle identifier
		 */
		public string bundleIdentifier;
		
		
		/*
		 * The product name
		 */
		public string productName;
		
		
		/*
		 * Constructors
		 */
		public ReleaseType(int index) : this("Release Type #" + index)
		{
		}
		
		public ReleaseType(string name, string bundleIdentifier, string productName) : this(name)
		{
			this.bundleIdentifier = bundleIdentifier;
			this.productName = productName;
		}
		
		public ReleaseType(string name)
		{
			this.name = name;
			isActive = true;
		}

		public string getFormattedName()
		{
			if (name == null)
			{
				return string.Empty;
			}

			return name;
		}
		
		
		/*
		 * Script compilation defines
		 */
		public string getDefine()
		{
			return "RT_" + name.ToUpper().Replace(" ", "_");
		}
	}
}