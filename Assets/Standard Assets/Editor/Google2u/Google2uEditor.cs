// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using UnityEditor;

    #endregion

    public class Google2uEditor : EditorWindow
    {
        public WorkbookBase Workbook { get; set; }
        public EditorGUILayoutEx Layout { get; set; }

        public string WorkbookName
        {
            get { return Workbook == null ? string.Empty : Workbook.WorkbookName; }
        }

        public void OnGUI()
        {
            if (Workbook != null)
            {
                Workbook.DrawGUIFull(Layout);
                Repaint();
            }
        }
    }
}