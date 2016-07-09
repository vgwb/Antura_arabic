using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class DistributionPlatform
	{
		/*
		 * Determine if the distribution platform will be used
		 */
		public bool isActive;
		
		
		/*
		 * The distributionplatform name
		 */
		public string name;
		
		
		public DistributionPlatform(string name, bool isActive = false)
		{
			this.name = name;
			this.isActive = isActive;
		}

		public bool isNameValid()
		{
			return !string.IsNullOrEmpty(name) && Regex.IsMatch(name, @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 _]*$") && !name.Equals("Default");
		}
		
		public string getFormattedName()
		{
			if (name == null)
			{
				return string.Empty;
			}

			return name.Replace(" ", string.Empty);
		}

		public string getDefine()
		{
			if (!isNameValid())
			{
				return null;
			}

			return "DP_" + name.ToUpper().Replace(" ", "_");
		}
	}
}