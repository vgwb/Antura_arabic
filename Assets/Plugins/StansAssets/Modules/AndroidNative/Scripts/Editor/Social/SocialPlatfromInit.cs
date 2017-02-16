////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class SocialPlatfromInit  {

	static SocialPlatfromInit () {

		if(SA.Common.Util.Files.IsFolderExists(SA.Common.Config.BUNDLES_PATH + "UltimateMobile/")) {
			return;
		}

		if(SA.Common.Util.Files.IsFolderExists(SA.Common.Config.MODULS_PATH + "AndroidNative/")) {
			return;
		}


		if(!SocialPlatfromSettingsEditor.IsInstalled) {
			EditorApplication.update += OnEditorLoaded;
		} else {
			if(!SocialPlatfromSettingsEditor.IsUpToDate) {
				EditorApplication.update += OnEditorLoaded;
			}
		}
		
	}
	
	private static void OnEditorLoaded() {
		
		EditorApplication.update -= OnEditorLoaded;
		Debug.LogWarning("Mobile Social Plugin Install Required. Opening Plugin settings...");
		Selection.activeObject = SocialPlatfromSettings.Instance;
	}
}
#endif
