using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Db.Management.Editor
{
    [CustomEditor(typeof(DatabaseLoader))]
    public class DatabaseLoaderInspector : UnityEditor.Editor
    {
        DatabaseLoader src;

        void OnEnable()
        {
            src = target as DatabaseLoader;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Load All Data"))
            {
                if (AllDataIsSet())
                {
                    src.LoadDatabase();
                }
                else
                {
                    Debug.LogError("Error: not all references to data files were correctly added to the DatabaseLoader inspector.");
                }
            }
        }

        private bool AllDataIsSet()
        {
            if (!src.database) return false;
            return src.inputData.AllDataIsSet();
        }

    }
}