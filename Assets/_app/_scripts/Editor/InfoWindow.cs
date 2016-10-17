using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EA4S
{

    public class InfoView : EditorWindow
    {
        [MenuItem("Tools/EA4S Antura/Info")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EA4S.InfoView));
        }

        void OnGUI()
        {
            this.titleContent.text = "EA4S Antura";
            EditorGUILayout.LabelField("Hi there! I'm Antura and i'm version " + AppManager.AppVersion);
        }

    }
}