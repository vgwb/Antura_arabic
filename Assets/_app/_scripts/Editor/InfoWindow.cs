using Antura.Core;
using UnityEngine;
using UnityEditor;

namespace Antura.Editor
{

    public class InfoView : EditorWindow
    {
        [MenuItem("Tools/Antura/Info", false, 300)]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(InfoView));
        }

        void OnGUI()
        {
            this.titleContent.text = "Antura";
            EditorGUILayout.LabelField("Version " + AppConstants.AppVersion);

            DrawFooterLayout(Screen.width);
        }


        public void DrawFooterLayout(float width)
        {
            EditorGUILayout.BeginVertical();

            var margin = (EditorStyles.miniButton.padding.left) / 2f;
            width = width - margin * 2;

            if (GUILayout.Button("Developer docs")) {
                Application.OpenURL(AppConstants.UrlDeveloperDocs);
            }
            
            if (GUILayout.Button("Source Code (GitHub project)")) {
                Application.OpenURL(AppConstants.UrlGithubRepository);
            }

            if (GUILayout.Button("GitHub issues")) {
                Application.OpenURL(AppConstants.UrlGithubRepository + "/issues");
            }

            if (GUILayout.Button("antura.org")) {
                Application.OpenURL(AppConstants.UrlWebsite);
            }

            //if (GUILayout.Button("Trello", GUILayout.Width(width / 2f - margin))) {
            //    Application.OpenURL(AppConstants.UrlTrello);
            //}

            EditorGUILayout.EndVertical();
        }
    }
}