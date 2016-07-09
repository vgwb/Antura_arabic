using UnityEditor;
using System;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class PlatformArchitecture
	{
		/*
		 * Determine if the architecture will be used
		 */
		public bool isActive;
		
		
		/*
		 * The architecture name
		 */
		public string name;


		/*
		 * The build target
		 */
		public BuildTarget buildTarget;


		/*
		 * The binary suffix
		 */
		public string binarySuffix;

		
		public PlatformArchitecture(string name, string binarySuffix, BuildTarget buildTarget, bool isActive = false)
		{
			this.name = name;
			this.binarySuffix = binarySuffix;
			this.buildTarget = buildTarget;
			this.isActive = isActive;
		}

		public string getFormattedName()
		{
			if (name == null)
			{
				return string.Empty;
			}

			return name;
		}

		public string getDefine()
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}

			return "PA_" + name.ToUpper().Replace(" ", "_");
		}
	}
}