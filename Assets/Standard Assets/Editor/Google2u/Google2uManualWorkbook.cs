// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Google.GData.Client;
    using Google.GData.Spreadsheets;
    using UnityEditor;
    using UnityEngine;

    #endregion

    public class Google2uManualWorkbook : WorkbookBase
    {
        public WorksheetFeed Workbook;

        public Google2uManualWorkbook(WorksheetFeed in_feed, string in_url, string in_name, Service in_service)
        {
            _Service = in_service;
            WorkbookUrl = in_url;
            Workbook = in_feed;
            WorkbookName = in_name;
            Worksheets = new List<Google2uWorksheet>();
        }

        public override void Update()
        {
            if (WorksheetQueryStatus == QueryStatus.QueryComplete)
            {
                foreach (var google2UWorksheet in Worksheets)
                {
                    if (ActiveWorksheetIndex > -1 && Worksheets[ActiveWorksheetIndex] != null &&
                        google2UWorksheet == Worksheets[ActiveWorksheetIndex])
                        google2UWorksheet.Update(true, ExportOptions);
                    else google2UWorksheet.Update(false, ExportOptions);
                }
            }

            if (WorksheetQueryStatus != QueryStatus.Uninitialized) return;
            WorksheetQueryStatus = QueryStatus.Querying;
            var t = new Thread(QueryWorksheets);
            t.Start(this);
        }

        public void QueryWorksheets(object in_workbook)
        {
            var myWorkbook = in_workbook as Google2uManualWorkbook;
            if (myWorkbook == null) return;

            myWorkbook.Worksheets.Clear();
            foreach (var entry in myWorkbook.Workbook.Entries)
            {
                var ws = new Google2uWorksheet(entry as WorksheetEntry, _Service, this);
                myWorkbook.Worksheets.Add(ws);
                if (myWorkbook.ActiveWorksheetIndex == -1)
                    myWorkbook.ActiveWorksheetIndex = 0;
            }
            myWorkbook.WorksheetQueryStatus = QueryStatus.QueryComplete;
        }

        public override void DrawGUIFull(EditorGUILayoutEx in_layout)
        {
            switch (WorksheetQueryStatus)
            {
                case QueryStatus.Idle:
                {
                    WorksheetQueryStatus = QueryStatus.Uninitialized;
                }
                    break;

                case QueryStatus.Querying:
                    EditorGUILayout.LabelField(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_QUERYING_WORKSHEETS) +
                        Google2u.Ellipses);
                    break;

                case QueryStatus.QueryComplete:
                    if (WorksheetsDisplay.Length > 0)
                    {
                        EditorGUILayout.BeginHorizontal();


                        var worksheetNames = new string[WorksheetsDisplay.Length];
                        for (var i = 0; i < WorksheetsDisplay.Length; ++i)
                            worksheetNames[i] = WorksheetsDisplay[i].WorksheetName;

                        ActiveWorksheetIndex =
                            EditorGUILayout.Popup(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ACTIVE_WORKSHEET) + ": ",
                                ActiveWorksheetIndex,
                                worksheetNames);


                        var activeWorksheet = WorksheetsDisplay[ActiveWorksheetIndex];
                        if (activeWorksheet == null) throw new ArgumentNullException("ActiveWorksheet");


                        var newExportType =
                            (ExportType)
                                EditorGUILayout.EnumPopup(
                                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_AS) + ": ",
                                    activeWorksheet.WorksheetExportType, GUILayout.Width(300));
                        if (newExportType != activeWorksheet.WorksheetExportType)
                        {
                            activeWorksheet.WorksheetExportType =
                                Google2uGUIUtil.SetEnum(activeWorksheet.Prefix + "ExportType", newExportType);
                        }

                        var content = new GUIContent(in_layout.RefreshButton,
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_REFRESH_WORKBOOK));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            Worksheets.Clear();
                            WorksheetQueryStatus = QueryStatus.Uninitialized;
                        }

                        {
                            var guiEnabled = GUI.enabled;
                            if (activeWorksheet.WorksheetQueryStatus != QueryStatus.Idle)
                                GUI.enabled = false;
                            content = activeWorksheet.IsDataValid
                                ? new GUIContent(in_layout.ValidateButtonGreen,
                                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_VALIDATE_WORKSHEET))
                                : new GUIContent(in_layout.ValidateButtonRed,
                                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_VALIDATE_WORKSHEET));

                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                // Do Validation for the active worksheet
                                activeWorksheet.UpdateValidation = true;
                            }
                            GUI.enabled = guiEnabled;
                        }

                        content = new GUIContent(in_layout.GoogleButton,
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_OPEN_IN_GOOGLE));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            Application.OpenURL(WorkbookUrl);
                        }

                        GUILayout.FlexibleSpace();

                        if (activeWorksheet.WorksheetExportType != ExportType.DoNotExport)
                        {
                            var oldEnabled = GUI.enabled;
                            if (activeWorksheet.IsDataValid == false)
                                GUI.enabled = false;
                            content = new GUIContent(in_layout.SaveButton,
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                DoExport(new List<Google2uWorksheet> {activeWorksheet});
                            }
                            GUI.enabled = oldEnabled;
                        }

                        EditorGUILayout.EndHorizontal();

                        DrawSpreadsheetOptions(in_layout, activeWorksheet.WorksheetExportType, activeWorksheet);

                        EditorGUILayout.Separator();

                        activeWorksheet.DrawGUIFull(in_layout);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No Worksheets Found");
                    }
                    break;
            }
        }

        public override bool DrawGUIList(EditorGUILayoutEx in_layout, bool in_showAll)
        {
            var ret = true;
            ShowSpreadsheet = Google2uGUIUtil.GetBool("mworkbook" + WorkbookName.Replace(' ', '_') + "_Open",
                ShowSpreadsheet);
            var mainFadeArea = in_layout.BeginFadeArea(ShowSpreadsheet, WorkbookName,
                "mworkbook" + WorkbookName.Replace(' ', '_'), in_layout.OuterBox, in_layout.OuterBoxHeader);
            ShowSpreadsheet = mainFadeArea.Open;
            Google2uGUIUtil.SetBool("mworkbook" + WorkbookName.Replace(' ', '_') + "_Open", ShowSpreadsheet);

            if (mainFadeArea.Show())
            {
                // We have to do this here. Otherwise there is a threading issue (Can't initialize from EditorPreferences outside the main thread)
                if (ExportOptions == null)
                {
                    ExportOptions = new Google2uExportOptions("mworkbook" + WorkbookName.Replace(' ', '_') + "_Option_");
                }

                var showExport = false;
                var exportsheets = new List<Google2uWorksheet>();

                switch (WorksheetQueryStatus)
                {
                    case QueryStatus.Idle:
                    {
                        WorksheetQueryStatus = QueryStatus.Uninitialized;
                    }
                        break;

                    case QueryStatus.Querying:
                        EditorGUILayout.LabelField(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_QUERYING_WORKSHEETS) +
                            Google2u.Ellipses);
                        break;

                    case QueryStatus.QueryComplete:
                        if (Worksheets.Count > 0)
                        {
                            foreach (var google2UWorksheet in Worksheets)
                            {
                                if (google2UWorksheet.DrawGUIList(in_layout))
                                {
                                    exportsheets.Add(google2UWorksheet);
                                    showExport = true;
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            var content = new GUIContent(in_layout.RefreshButton,
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_REFRESH_WORKBOOK));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                Worksheets.Clear();
                                WorksheetQueryStatus = QueryStatus.Uninitialized;
                            }

                            var querying = false;
                            var bAllWorksheetsValid = true;
                            foreach (var google2UWorksheet in exportsheets)
                            {
                                if (google2UWorksheet.IsDataValid == false)
                                    bAllWorksheetsValid = false;
                                if (google2UWorksheet.WorksheetQueryStatus != QueryStatus.Idle)
                                    querying = true;
                            }

                            {
                                var guiEnabled = GUI.enabled;
                                if (querying)
                                    GUI.enabled = false;
                                content = bAllWorksheetsValid
                                    ? new GUIContent(in_layout.ValidateButtonGreen,
                                        Google2u.LocalizationInfo.Localize(
                                            Localization.rowIds.ID_LABEL_VALIDATE_WORKBOOK))
                                    : new GUIContent(in_layout.ValidateButtonRed,
                                        Google2u.LocalizationInfo.Localize(
                                            Localization.rowIds.ID_LABEL_VALIDATE_WORKBOOK));

                                if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                    GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                                {
                                    // Do Validation for the worksheets we will be exporting
                                    foreach (var google2UWorksheet in exportsheets)
                                    {
                                        google2UWorksheet.UpdateValidation = true;
                                    }
                                }
                                GUI.enabled = guiEnabled;
                            }

                            content = new GUIContent(in_layout.EditButton,
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_VIEW_WORKBOOK));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                var ed = EditorWindow.GetWindow<Google2uEditor>();
                                ed.Workbook = this;
                                ed.Layout = in_layout;
#if(UNITY_4)
                                ed.title = WorkbookName;
#elif(UNITY_5_0)
                                ed.title = WorkbookName;
#else
                                ed.titleContent.text = WorkbookName;
#endif
                                ed.wantsMouseMove = true;
                            }

                            content = new GUIContent(in_layout.DeleteButton,
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_REMOVE_WORKBOOK));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                if (
                                    EditorUtility.DisplayDialog(
                                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_REMOVE_WORKBOOK),
                                        Google2u.LocalizationInfo.Localize(
                                            Localization.rowIds.ID_MESSAGE_REMOVE_WORKBOOK),
                                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_DELETE),
                                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CANCEL)))
                                    ret = false;
                            }

                            GUILayout.FlexibleSpace();

                            if (showExport)
                            {
                                var oldEnabled = GUI.enabled;
                                if (bAllWorksheetsValid == false)
                                    GUI.enabled = false;
                                content = new GUIContent(in_layout.SaveButton,
                                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT));
                                if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                    GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                                {
                                    DoExport(exportsheets);
                                }
                                GUI.enabled = oldEnabled;
                            }


                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            EditorGUILayout.LabelField(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_NO_WORKSHEETS));
                        }
                        break;
                }
            }
            in_layout.EndFadeArea();
            return ret;
        }
    }
}