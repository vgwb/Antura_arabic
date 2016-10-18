using UnityEngine;
using UnityEditor;

namespace EA4S.Db.Loader.Editor
{
    public class DatabaseManagerWindow : EditorWindow
    {
        public DatabaseLoader dbLoader;
        public DatabaseManager dbTester;

        [MenuItem("Tools/EA4S Antura/Database Manager")]
        public static void ShowWindow()
        {
            var window = (DatabaseManagerWindow)EditorWindow.GetWindow(typeof(DatabaseManagerWindow), false, "DB Manager");
            if (window.dbLoader == null) window.dbLoader = FindObjectOfType<DatabaseLoader>();
            if (window.dbTester == null) window.dbTester = FindObjectOfType<DatabaseManager>();
        }

        void OnGUI()
        {
            dbLoader = (DatabaseLoader)EditorGUILayout.ObjectField("Loader", dbLoader, typeof(DatabaseLoader), true);
            if (GUILayout.Button("Load All Data")) {
                dbLoader.LoadDatabase();
            }

            dbTester = (DatabaseManager)EditorGUILayout.ObjectField("Tester", dbTester, typeof(DatabaseManager), true);
            if (GUILayout.Button("Log: Counts")) {
                dbTester.LogDataCounts();
            }
            if (GUILayout.Button("Log: MiniGames")) {
                dbTester.LogMiniGames();
            }
            if (GUILayout.Button("Log: Test Access")) {
                dbTester.TestAccess();
            }
        }
    }
}
