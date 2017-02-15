using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SA_EditorInApps  {

	private static SA_InApps_EditorUIController _EditorUI = null;
	
	
	public static void ShowInAppPopup(string title, string describtion, string price, Action<bool> OnComplete = null) {

		EditorUI.ShowInAppPopup(title, describtion, price, OnComplete);
	}
	
	
	
	private static SA_InApps_EditorUIController EditorUI {
		get {
			#if UNITY_EDITOR
			if (_EditorUI == null) {
				GameObject o = AssetDatabase.LoadAssetAtPath("Assets/Plugins/StansAssets/Support/EditorTesting/UI/Prefabs/InAppsEditorTestingUI.prefab", typeof(GameObject)) as GameObject;
				GameObject go = GameObject.Instantiate(o) as GameObject;
				_EditorUI = go.GetComponent<SA_InApps_EditorUIController>();
			}
			#endif
			return _EditorUI;
		}
	}
}
