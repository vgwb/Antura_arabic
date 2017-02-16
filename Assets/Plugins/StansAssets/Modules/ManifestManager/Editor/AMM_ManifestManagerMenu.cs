////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;


namespace SA.Manifest {

	static public class ManifestManagerMenu {

		[MenuItem("Window/Stan's Assets/Manifest Manager/Edit" , false, 401)]
		public static void LoadManagerWindow() {
			EditorWindow.GetWindow<ManifestManagerWindow>();
		}

		//--------------------------------------
		//  GENERAL
		//--------------------------------------

		[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Overview", false, 401)]
		public static void AMMOverview() {
			Application.OpenURL("https://unionassets.com/android-manifest-manager/overview-222");
		}

		[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Using with Unity Editor", false, 401)]
		public static void AMMUsingWithUnityEditor() {
			Application.OpenURL("https://unionassets.com/android-manifest-manager/using-with-unity-editor-223");
		}

		[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Scripting API", false, 401)]
		public static void AMMScriptingAPI() {
			Application.OpenURL("https://unionassets.com/android-manifest-manager/scripting-api-244");
		}

	}
}
#endif
