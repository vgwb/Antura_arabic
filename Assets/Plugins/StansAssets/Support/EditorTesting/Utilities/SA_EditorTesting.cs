using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SA_EditorTesting  {
	
	
	public const int DEFAULT_SORT_ORDER = 10000;
	
	
	public static bool IsInsideEditor {
		get {
			bool IsInsideEditor = false;
			if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
				IsInsideEditor = true;
			}
			
			return IsInsideEditor;
		}
	}
	
	
	public static bool HasFill(float fillRate) {
		int probability = UnityEngine.Random.Range(1, 100);
		if(probability <= fillRate) {
			return true;
		} else {
			return false;
		}
		
	}
	
	public static void CheckForEventSystem() {
		
		EventSystem	es = (EventSystem) UnityEngine.GameObject.FindObjectOfType(typeof(EventSystem));

		if(es == null) {
			#if UNITY_EDITOR
			GameObject o = AssetDatabase.LoadAssetAtPath("Assets/Plugins/StansAssets/Support/EditorTesting/UI/Prefabs/EventSystem.prefab", typeof(GameObject)) as GameObject;
			GameObject.Instantiate(o);
			#endif
		}


	}
	
	
}
