using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder.Utils
{
	public static class PMPackagingUtils
	{
		public static void CreateUnityPackage(string productName, string versionName, Action onBeforeCreatingPackage)
		{
			string outputDirectory = Application.dataPath.Replace("Assets", string.Empty) + "Package/";
			Directory.CreateDirectory(outputDirectory);
			
			string packageFileName = productName.Replace(" ", string.Empty).Trim() + "_" + versionName.Replace(" ", string.Empty) + ".unitypackage";
			if (onBeforeCreatingPackage != null)
			{
				onBeforeCreatingPackage();
			}

			AssetDatabase.ExportPackage("Assets/PygmyMonkey", outputDirectory + packageFileName, ExportPackageOptions.Recurse);
			
			Debug.Log(".unitypackage " + packageFileName + " created!");
		}
	}
}
