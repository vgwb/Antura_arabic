using UnityEditor;
using UnityEngine;

namespace EA4S.Editor
{
    /// <summary>
    /// Various menu items
    /// </summary>
    public class AnturaMenuItems : MonoBehaviour
    {
        // Creates an editor-only tester for Daniele.
        // If left in the project it's not a problem since it's editor-only, and won't be included in builds
        [MenuItem("GameObject/Antura Debug/DTester", false, 10)]
        static void CreateDTester(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("[DTester - Editor-only]");
            go.tag = "EditorOnly";
            go.AddComponent<DTester>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}