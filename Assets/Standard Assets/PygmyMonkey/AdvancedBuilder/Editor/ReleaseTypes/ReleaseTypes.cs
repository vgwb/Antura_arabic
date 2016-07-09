using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class ReleaseTypes
	{
		/*
		 * Release types
		 */
		[SerializeField]
		private List<ReleaseType> m_releaseTypeList = new List<ReleaseType>()
		{
			new ReleaseType("Dev", "com.mygame.dev", "My Game Dev"),
			new ReleaseType("Beta", "com.mygame.beta", "My Game Beta"),
			new ReleaseType("Release", "com.mygame", "My Game"),
		};
		
		public List<ReleaseType> getReleaseTypeList()
		{
			return m_releaseTypeList;
		}
	}
}