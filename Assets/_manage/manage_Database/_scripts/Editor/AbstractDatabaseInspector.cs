using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Db.Management.Editor
{
    public class AbstractDatabaseInspector : UnityEditor.Editor
    {
        SerializedObject sobj;

        void OnEnable()
        {
            sobj = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            sobj.Update();

            var iterator = sobj.GetIterator();
            iterator.Next(true);
            do
            {
                var innerList = iterator.FindPropertyRelative("innerList");
                if (innerList != null)
                { 
                    EditorGUILayout.PropertyField(innerList, new GUIContent(iterator.displayName), true);
                }
            } while (iterator.Next(false));

            sobj.ApplyModifiedProperties();
        }

    }
}