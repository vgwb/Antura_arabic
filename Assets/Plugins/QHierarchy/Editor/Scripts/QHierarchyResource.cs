using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace qtools.qhierarchy
{
	public enum QHierarchyTexture
	{
		BackgroundTreeMap,
		ButtonDown,
		ButtonUp,
		ButtonLockOff,
		ButtonLockOn,
		ButtonVisibilityOff,
		ButtonVisibilityOffEdit,		
		ButtonVisibilityOffParent,
		ButtonVisibilityOffParentEdit,
		ButtonVisibilityOn,
		ButtonVisibilityOnEdit,
		IconError,
		IconErrorChild,
        ButtonStaticOn,
        ButtonStaticOff,
        ButtonStaticHalf,
        ButtonMeshOn,
        ButtonMeshOff,
        ButtonMeshWireframe
	}

	public class QHierarchyResource
	{
		private static string[] darkSkin = 
		{ 
			"DarkBackgroundTreeMap",
			"DarkButtonDown",
			"DarkButtonUp",
			"DarkButtonLockOff",
			"DarkButtonLockOn",
			"DarkButtonVisibilityOff",
			"DarkButtonVisibilityOffEdit",		
			"DarkButtonVisibilityOffParent",
			"DarkButtonVisibilityOffParentEdit",
			"DarkButtonVisibilityOn",
			"DarkButtonVisibilityOnEdit",
			"DarkIconError",
			"DarkIconErrorChild",
            "DarkButtonStaticOn", 
            "DarkButtonStaticOff",
            "DarkButtonStaticHalf",
            "DarkButtonMeshOn",
            "DarkButtonMeshOff",
            "DarkButtonMeshWireframe"
		}; 

		private static string[] lightSkin = 
		{ 
			"LightBackgroundTreeMap",
			"LightButtonDown",
			"LightButtonUp",
			"LightButtonLockOff",
			"LightButtonLockOn",
			"LightButtonVisibilityOff",
			"LightButtonVisibilityOffEdit",		
			"LightButtonVisibilityOffParent",
			"LightButtonVisibilityOffParentEdit",
			"LightButtonVisibilityOn",
			"LightButtonVisibilityOnEdit", 
			"LightIconError",
			"LightIconErrorChild",
            "LightButtonStaticOn",
            "LightButtonStaticOff",
            "LightButtonStaticHalf",
            "LightButtonMeshOn",
            "LightButtonMeshOff",
            "LightButtonMeshWireframe"
		};

		private static Texture2D[] textures;

		static QHierarchyResource()
		{
			loadAssets();
		} 

		private static void loadAssets() 
		{
			string[] skinTextures = EditorGUIUtility.isProSkin ? darkSkin : lightSkin;
            textures = new Texture2D[skinTextures.Length];
			
			for (int i = 0; i < skinTextures.Length; i++) 
			{ 
				textures[i] = getAsset<Texture2D>(skinTextures[i]);
			} 
		} 
         
		public static Texture2D getTexture(QHierarchyTexture textureName) 
		{
            if (textures == null)
            {
                loadAssets();
            }
            else if (textures[(int)textureName] == null)
            {
                string[] skinTextures = EditorGUIUtility.isProSkin ? darkSkin : lightSkin;
                textures[(int)textureName] = getAsset<Texture2D>(skinTextures[(int)textureName], "QHierarchy");
                if (textures[(int)textureName]  == null)
                {
                    textures[(int)textureName] = getAsset<Texture2D>(skinTextures[(int)textureName]);
                    if (textures[(int)textureName]  == null)
                    {
                        throw new Exception("QHierarchy: Texture with name \"" + skinTextures[(int)textureName] + ".psd\" not found. Please try to re-import plugin.");
                    }
                }
            }
			return textures[(int)textureName];
		} 

		public static T getAsset<T>(string fileNameFilter, params string[] pathFilter) where T: UnityEngine.Object
		{
			string[] guids = AssetDatabase.FindAssets(fileNameFilter);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				int i = 0;
				for (; i < pathFilter.Length; i++) 
					if (!path.Contains(pathFilter[i])) break;
				if (i == pathFilter.Length)
					return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
			}
			return null; 
		}
	}
}
