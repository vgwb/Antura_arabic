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
		

	public class AddValueDialog : EditorWindow {

		public event Action<object> onClose = delegate { };
		public event Action<string, string> onAddClick = delegate { };

		private string key = string.Empty;
		private string value = string.Empty;

		private bool _isStarted = false;

		
		void OnGUI() {
			

			EditorGUILayout.Space ();

			GUI.SetNextControlName ("KeyField");
			key = EditorGUILayout.TextField("Key", key);
			value = EditorGUILayout.TextField ("Value", value);

			if (!_isStarted) {
				_isStarted = true;
				EditorGUI.FocusTextInControl("KeyField");
			}
			
			if (GUILayout.Button("Add")) {
				if (!key.Equals(string.Empty)) {
					OnClickAddValue();
					GUIUtility.ExitGUI();
				}
			}

			EditorGUILayout.Space ();
		}
		
		void OnClickAddValue() {
			onAddClick (key, value);
			Close();
		}

		void OnDestroy() {
			onClose (this);
		}
	}
}
#endif
