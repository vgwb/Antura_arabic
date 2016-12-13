using UnityEditor;

public class WorldPrefabsEditorWindow : EditorWindow
{
    EA4S.WorldID world = EA4S.WorldID.Default;
    EA4S.WorldID lastWorld = EA4S.WorldID.Default;

    [MenuItem("Tools/EA4S Antura/World Prefabs")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        WorldPrefabsEditorWindow window = (WorldPrefabsEditorWindow)EditorWindow.GetWindow(typeof(WorldPrefabsEditorWindow));
        window.Show();
    }

    void OnGUI()
    {
        world = (EA4S.WorldID)EditorGUILayout.EnumPopup(world);

        if (world != lastWorld) {
            var prefabs = FindObjectsOfType<EA4S.AutoWorldPrefab>();
            var cameras = FindObjectsOfType<EA4S.AutoWorldCameraColor>();

            foreach (var p in prefabs) {
                p.testWorld = world;
                p.SendMessage("Update");
            }

            foreach (var c in cameras) {
                c.testWorld = world;
                c.SendMessage("Update");
            }
        }
        lastWorld = world;
    }
}