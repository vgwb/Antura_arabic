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
using System;
using System.Collections;


namespace SA.Manifest {

	public class AddPermissionDialog : EditorWindow {

		public event Action<object> onClose = delegate { };
		public event Action<string> onAddClick = delegate { };

		private string permission = string.Empty;

		private bool _isStarted = false;
		
		void OnGUI() {
			EditorGUILayout.Space ();

			GUI.SetNextControlName ("PermissionField");
			permission = EditorGUILayout.TextField("Name", permission);

			if (!_isStarted) {
				_isStarted = true;
				EditorGUI.FocusTextInControl("PermissionField");
			}
			
			if (GUILayout.Button("Add")) {
				if (!permission.Equals(string.Empty)) {
					OnClickAddValue();
					GUIUtility.ExitGUI();
				}
			}

			EditorGUILayout.Space ();
		}
		
		void OnClickAddValue() {
			onAddClick (permission);
			Close();
		}

		void OnDestroy() {
			onClose (this);
		}
	}
}
#endif
