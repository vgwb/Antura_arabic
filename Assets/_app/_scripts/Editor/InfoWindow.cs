using UnityEngine;
using UnityEditor;
using EA4S.Core;

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
            EditorGUILayout.LabelField("Version " + AppConstants.AppVersion);

            DrawFooterLayout(Screen.width);
        }


        public void DrawFooterLayout(float width)
        {
            EditorGUILayout.BeginVertical();

            var margin = (EditorStyles.miniButton.padding.left) / 2f;
            width = width - margin * 2;

            if (GUILayout.Button("GitHub project")) {
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