using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	[Serializable]
	public class AdvancedSettings
	{
		/*
		 * Custom path for the build
		 */
		public string customPath = "$BUILD_DATE/$RELEASE_TYPE/$PLATFORM/$DISTRIB_PLATFORM/$ARCHITECTURE/$TEXT_COMPRESSION/";


		/*
		 * Custom build file name
		 */
		public string customFileName = "$PRODUCT_NAME";
		
		
		/*
		 * Custom build script
		 */
		public MonoScript customBuildMonoScript;
		
		
		/*
		 * Custom global scripting define symbols
		 */
		public string globalCustomDefines;
		
		
		/*
		 * Allow overwritting defines
		 */
		public bool overwriteDefines = true;
		
		
		/*
		 * Allow overwritting scenes
		 */
		public bool overwriteScenes = true;
		
		
		/*
		 * Use symslink libraries
		 */
		public bool useSymlinkLibraries = false;
		
		
		/*
		 * Check to show error if android keystore passwords are not set
		 */
		public bool checkAndroidKeystorePasswords = false;
	}
}