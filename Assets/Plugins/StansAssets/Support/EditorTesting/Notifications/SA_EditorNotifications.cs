using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SA_EditorNotifications {

	private static SA_Notifications_EditorUIController _EditorUI = null;


	public static void ShowNotification(string title, string message, SA_EditorNotificationType type) {
	
		if(SA_EditorTesting.IsInsideEditor) {
			EditorUI.ShowNotification(title, message, type);
		}
	}



	private static SA_Notifications_EditorUIController EditorUI {
		get {
			#if UNITY_EDITOR
			if (_EditorUI == null) {
				GameObject o = AssetDatabase.LoadAssetAtPath("Assets/Plugins/StansAssets/Support/EditorTesting/UI/Prefabs/NotificationsEditorTestingUI.prefab", typeof(GameObject)) as GameObject;
				GameObject go = GameObject.Instantiate(o) as GameObject;
				_EditorUI = go.GetComponent<SA_Notifications_EditorUIController>();
			}
			#endif
			return _EditorUI;
		}
	}
}

