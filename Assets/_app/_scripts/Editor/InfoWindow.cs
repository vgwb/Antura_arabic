using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EA4S.Editor
{

    public class InfoView : EditorWindow
    {
        [MenuItem("Tools/EA4S Antura/Info")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EA4S.Editor.InfoView));
        }

        void OnGUI()
        {
            this.titleContent.text = "EA4S Antura";
            EditorGUILayout.LabelField("Hi there! I'm Antura and i'm version " + AppConstants.AppVersion);

            DrawFooterLayout(Screen.width - 15);
        }


        public void DrawFooterLayout(float width)
        {
            EditorGUILayout.BeginHorizontal();

            var margin = (EditorStyles.miniButton.padding.left) / 2f;
            width = width - margin * 2;

            if (GUILayout.Button("GitHub", GUILayout.Width(width / 2f - margin))) {
                Application.OpenURL(AppConstants.UrlGithubRepository);
        }

            if (GUILayout.Button("Trello", GUILayout.Width(width / 2f - margin))) {
                Application.OpenURL(AppConstants.UrlTrello);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}