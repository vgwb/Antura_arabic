// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using UnityEngine;
    using UnityEditor;

    #endregion

    [InitializeOnLoad]
    public class Google2uHierarchyIcon
    {
        private static readonly Texture Google2uIcon;

        static Google2uHierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGUI;

            Google2uIcon = (Texture2D) EditorGUIUtility.Load("g2u/Google2uSm.png");
        }

        private static void HierarchWindowOnGUI(int in_instanceID, Rect in_selectionRect)
        {
            var r = new Rect(in_selectionRect);
            r.x = r.width - 10;
            r.width = 18;

            var g = (GameObject) EditorUtility.InstanceIDToObject(in_instanceID);

            if (g != null && g.GetComponent<Google2uComponentBase>() != null)
            {
                GUI.Label(r, Google2uIcon);
            }
        }
    }
}