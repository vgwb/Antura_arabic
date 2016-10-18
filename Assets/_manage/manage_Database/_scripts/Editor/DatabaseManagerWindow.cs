using UnityEngine;
using UnityEditor;

namespace EA4S.Db.Management.Editor
{
    // @deprecated: this is now handled in a specific manager scene
    //public class DatabaseManagerWindow : EditorWindow
    //{
    //    public DatabaseLoader dbLoader;
    //    public DatabaseTester dbTester;

    //    [MenuItem("Tools/EA4S Antura/Database Manager")]
    //    public static void ShowWindow()
    //    {
    //        var window = (DatabaseManagerWindow)EditorWindow.GetWindow(typeof(DatabaseManagerWindow), false, "DB Manager");
    //        if (window.dbLoader == null) window.dbLoader = FindObjectOfType<DatabaseLoader>();
    //        if (window.dbTester == null) window.dbTester = FindObjectOfType<DatabaseTester>();
    //    }

    //    void OnGUI()
    //    {
    //        dbLoader = (DatabaseLoader)EditorGUILayout.ObjectField("Loader", dbLoader, typeof(DatabaseLoader), true);
    //        if (GUILayout.Button("Load All Data")) {
    //            dbLoader.LoadDatabase();
    //        }

    //        dbTester = (DatabaseTester)EditorGUILayout.ObjectField("Tester", dbTester, typeof(DatabaseTester), true);
    //        if (GUILayout.Button("Log: Counts")) {
    //            dbTester.DumpAllDataCounts();
    //        }
    //        if (GUILayout.Button("Log: MiniGames")) {
    //            dbTester.DumpAllMiniGameData();
    //        }
    //    }
    //}
}
