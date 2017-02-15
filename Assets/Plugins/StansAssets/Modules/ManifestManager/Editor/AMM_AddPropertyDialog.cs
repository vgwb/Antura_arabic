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

	public class AddPropertyDialog : EditorWindow {

		public event Action<object> onClose = delegate { };
		public event Action<string> onAddClick = delegate { };
		
		private string tag = string.Empty;

		private bool _isStarted = false;

		void OnGUI() {
			EditorGUILayout.Space ();

			GUI.SetNextControlName ("TagField");
			tag = EditorGUILayout.TextField("Tag", tag);
			
			if (GUILayout.Button("Add")) {
				if (!tag.Equals(string.Empty)) {
					OnClickAddProperty();
					GUIUtility.ExitGUI();
				}
			}

			if (!_isStarted) {
				_isStarted = true;
				EditorGUI.FocusTextInControl("TagField");
			}

			EditorGUILayout.Space ();
		}
		
		void OnClickAddProperty() {
			onAddClick (tag);
			Close();
		}
		
		void OnDestroy() {
			onClose (this);
		}
	}
}
#endif
