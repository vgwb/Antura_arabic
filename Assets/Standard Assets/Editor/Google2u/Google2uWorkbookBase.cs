// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System.Collections.Generic;
    using Google.GData.Client;
    using UnityEditor;
    using UnityEngine;

    #endregion

    public abstract class WorkbookBase
    {
        private Vector2 _CSVPreviewScrollPos = Vector2.zero;
        private string _CSVPreviewString = string.Empty;
        private Vector2 _JSONPreviewClassScrollPos = Vector2.zero;
        private string _JSONPreviewClassString = string.Empty;
        private Vector2 _JSONPreviewScrollPos = Vector2.zero;
        private string _JSONPreviewString = string.Empty;
        private Vector2 _NGUIPreviewScrollPos = Vector2.zero;
        private string _NGUIPreviewString = string.Empty;
        protected Service _Service;
        private Google2uWorksheet[] _WorksheetsDisplay = new Google2uWorksheet[0];
        private Vector2 _XMLPreviewScrollPos = Vector2.zero;
        private string _XMLPreviewString = string.Empty;
        public int ActiveWorksheetIndex = -1;
        public Google2uExportOptions ExportOptions;
        public bool ShowSpreadsheet;
        public bool ShowSpreadsheetExport;
        public bool ShowSpreadsheetOptions;
        public bool ShowSpreadsheetOptionsArrayDelimiters;
        public bool ShowSpreadsheetOptionsCodeGeneration;
        public bool ShowSpreadsheetOptionsCSV;
        public bool ShowSpreadsheetOptionsJSON;
        public bool ShowSpreadsheetOptionsLegacy;
        public bool ShowSpreadsheetOptionsNGUI;
        public bool ShowSpreadsheetOptionsObjectDB;
        public bool ShowSpreadsheetOptionsStaticDB;
        public bool ShowSpreadsheetOptionsWhitespace;
        public bool ShowSpreadsheetOptionsXML;
        public bool ShowSpreadsheetPreviewCSV;
        public bool ShowSpreadsheetPreviewJSON;
        public bool ShowSpreadsheetPreviewJSONClass;
        public bool ShowSpreadsheetPreviewNGUI;
        public bool ShowSpreadsheetPreviewXML;
        public bool SpreadsheetVisible = true;
        public string WorkbookName;
        public string WorkbookUrl;
        protected QueryStatus WorksheetQueryStatus = QueryStatus.Idle;
        public List<Google2uWorksheet> Worksheets;

        public Google2uWorksheet[] WorksheetsDisplay
        {
            get { return _WorksheetsDisplay; }
        }

        public abstract void Update();
        public abstract bool DrawGUIList(EditorGUILayoutEx in_layout, bool in_showAll);
        public abstract void DrawGUIFull(EditorGUILayoutEx in_layout);

        public void EndOfFrame()
        {
            _WorksheetsDisplay = Worksheets.ToArray();
            foreach (var ws in _WorksheetsDisplay)
            {
                ws.EndOfFrame();
            }
        }

        protected void DrawSpreadsheetOptions(EditorGUILayoutEx in_layout, ExportType in_exportType,
            Google2uWorksheet in_activeWorksheet)
        {
            ShowSpreadsheetOptions =
                Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsOpen",
                    ShowSpreadsheetOptions);
            var showWorkbookOptions = in_layout.BeginFadeArea(ShowSpreadsheetOptions,
                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_OPTIONS),
                "workbook" + WorkbookName.Replace(' ', '_') + "_Options", in_layout.InnerBox, in_layout.InnerBoxHeader);
            ShowSpreadsheetOptions = showWorkbookOptions.Open;
            Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsOpen", ShowSpreadsheetOptions);
            if (showWorkbookOptions.Show())
            {
                var prefix = "workbook" + WorkbookName.Replace(' ', '_') + "_Option_";

                ShowSpreadsheetOptionsLegacy =
                    Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsLegacyOpen",
                        ShowSpreadsheetOptionsLegacy);
                var showWorkbookOptionsLegacy = in_layout.BeginFadeArea(ShowSpreadsheetOptionsLegacy,
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_LEGACY_OPTIONS),
                    "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsLegacy", in_layout.OuterBox,
                    in_layout.OuterBoxHeader);
                ShowSpreadsheetOptionsLegacy = showWorkbookOptionsLegacy.Open;
                Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsLegacyOpen",
                    ShowSpreadsheetOptionsLegacy);
                if (showWorkbookOptionsLegacy.Show())
                {
                    EditorGUILayoutEx.ToggleInput(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_LOWERCASE_HEADER),
                        ref ExportOptions.LowercaseHeader, prefix + "LowercaseHeader");
                }
                in_layout.EndFadeArea();


                ShowSpreadsheetOptionsWhitespace =
                    Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsWhitespaceOpen",
                        ShowSpreadsheetOptionsWhitespace);
                var showWorkbookOptionsWhitespace = in_layout.BeginFadeArea(ShowSpreadsheetOptionsWhitespace,
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_WHITESPACE),
                    "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsWhitespace", in_layout.OuterBox,
                    in_layout.OuterBoxHeader);
                ShowSpreadsheetOptionsWhitespace = showWorkbookOptionsWhitespace.Open;
                Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsWhitespaceOpen",
                    ShowSpreadsheetOptionsWhitespace);
                if (showWorkbookOptionsWhitespace.Show())
                {
                    EditorGUILayoutEx.ToggleInput(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_TRIM_STRINGS),
                        ref ExportOptions.TrimStrings, prefix + "TrimStrings");
                    EditorGUILayoutEx.ToggleInput(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_TRIM_STRING_ARRAYS),
                        ref ExportOptions.TrimStringArrays, prefix + "TrimStringArrays");
                }
                in_layout.EndFadeArea();

                ShowSpreadsheetOptionsCodeGeneration =
                Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCodeGenerationOpen",
                    ShowSpreadsheetOptionsCodeGeneration);
                var showWorkbookOptionsCodeGeneration = in_layout.BeginFadeArea(ShowSpreadsheetOptionsCodeGeneration,
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CODE_GENERATION_OPTIONS),
                    "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCodeGeneration", in_layout.OuterBox,
                    in_layout.OuterBoxHeader);
                ShowSpreadsheetOptionsCodeGeneration = showWorkbookOptionsCodeGeneration.Open;
                Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCodeGenerationOpen",
                    ShowSpreadsheetOptionsCodeGeneration);
                if (showWorkbookOptionsCodeGeneration.Show())
                {
                    EditorGUILayoutEx.ToggleInput(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_PREPEND_UNDERSCORES),
                        ref ExportOptions.TrimStrings, prefix + "PrependUnderscoreToVariableNames");
                }
                in_layout.EndFadeArea();

                ShowSpreadsheetOptionsArrayDelimiters =
                    Google2uGUIUtil.GetBool(
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsArrayDelimitersOpen",
                        ShowSpreadsheetOptionsArrayDelimiters);
                var showWorkbookOptionsArrayDelimiters = in_layout.BeginFadeArea(ShowSpreadsheetOptionsArrayDelimiters,
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ARRAY_DELIMITERS),
                    "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsArrayDelimiters", in_layout.OuterBox,
                    in_layout.OuterBoxHeader);
                ShowSpreadsheetOptionsArrayDelimiters = showWorkbookOptionsArrayDelimiters.Open;
                Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsArrayDelimitersOpen",
                    ShowSpreadsheetOptionsArrayDelimiters);
                if (showWorkbookOptionsArrayDelimiters.Show())
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_NON_STRING));
                    var newArrayDelimiters = EditorGUILayout.Popup(ExportOptions.ArrayDelimiters,
                        ExportOptions.DelimiterOptionStrings, GUILayout.Width(100));
                    GUILayout.FlexibleSpace();

                    switch (newArrayDelimiters)
                    {
                        case 0: // , - Comma
                            GUILayout.Label("Example Int Array - 1,2,3,4");
                            break;
                        case 1: // | - Pipe
                            GUILayout.Label("Example Int Array - 1|2|3|4");
                            break;
                        case 2: //   - Space
                            GUILayout.Label("Example Int Array - 1 2 3 4");
                            break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (newArrayDelimiters != ExportOptions.ArrayDelimiters)
                    {
                        Google2uGUIUtil.SetInt(prefix + "ArrayDelimiters", newArrayDelimiters);
                        ExportOptions.ArrayDelimiters = newArrayDelimiters;
                        in_activeWorksheet.UpdateValidation = true;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_STRINGS));
                    var newStringArrayDelimiters = EditorGUILayout.Popup(ExportOptions.StringArrayDelimiters,
                        ExportOptions.DelimiterOptionStrings, GUILayout.Width(100));
                    GUILayout.FlexibleSpace();

                    switch (newStringArrayDelimiters)
                    {
                        case 0: // , - Comma
                            GUILayout.Label("Example String Array - Hello,Hola,Bonjour");
                            break;
                        case 1: // | - Pipe
                            GUILayout.Label("Example String Array - Hello|Hola|Bonjour");
                            break;
                        case 2: //   - Space
                            GUILayout.Label("Example String Array - Hello Hola Bonjour");
                            break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (newStringArrayDelimiters != ExportOptions.StringArrayDelimiters)
                    {
                        Google2uGUIUtil.SetInt(prefix + "StringArrayDelimiters", newStringArrayDelimiters);
                        ExportOptions.StringArrayDelimiters = newStringArrayDelimiters;
                        in_activeWorksheet.UpdateValidation = true;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_COMPLEX_TYPES));
                    var newComplexTypeDelimiters = EditorGUILayout.Popup(ExportOptions.ComplexTypeDelimiters,
                        ExportOptions.DelimiterOptionStrings, GUILayout.Width(100));
                    GUILayout.FlexibleSpace();

                    switch (newComplexTypeDelimiters)
                    {
                        case 0: // , - Comma
                            GUILayout.Label("Example Vector - 1,2,3");
                            break;
                        case 1: // | - Pipe
                            GUILayout.Label("Example Vector - 1|2|3");
                            break;
                        case 2: //   - Space
                            GUILayout.Label("Example Vector - 1 2 3");
                            break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (newComplexTypeDelimiters != ExportOptions.ComplexTypeDelimiters)
                    {
                        Google2uGUIUtil.SetInt(prefix + "ComplexTypeDelimiters", newComplexTypeDelimiters);
                        ExportOptions.ComplexTypeDelimiters = newComplexTypeDelimiters;
                        in_activeWorksheet.UpdateValidation = true;
                    }

                    var tmpDelimStrings = new List<string>();
                    var tmpDelimInts = new List<int>();
                    var curDelim = 0;
                    for (var i = 0; i < ExportOptions.DelimiterOptionStrings.Length; ++i)
                    {
                        if (i == ExportOptions.ComplexTypeDelimiters)
                            continue;
                        tmpDelimStrings.Add(ExportOptions.DelimiterOptionStrings[i]);
                        if (ExportOptions.ComplexArrayDelimiters == i)
                            curDelim = tmpDelimInts.Count;
                        tmpDelimInts.Add(i);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_COMPLEX_ARRAYS));
                    var newComplexArrayDelimiters = EditorGUILayout.Popup(curDelim,
                        tmpDelimStrings.ToArray(), GUILayout.Width(100));
                    newComplexArrayDelimiters = tmpDelimInts[newComplexArrayDelimiters];
                    GUILayout.FlexibleSpace();

                    switch (newComplexArrayDelimiters)
                    {
                        case 0: // , - Comma
                        {
                            switch (ExportOptions.ComplexTypeDelimiters)
                            {
                                case 0: // , - Comma
                                    GUILayout.Label("Cannot use Comma as both Complex Type and Complex Array delimiters");
                                    break;
                                case 1: // | - Pipe
                                    GUILayout.Label("Example Vector Array - 1,2,3|4,5,6|7,8,9");
                                    break;
                                case 2: //   - Space
                                    GUILayout.Label("Example Vector Array - 1,2,3 4,5,6 7,8,9");
                                    break;
                            }
                        }
                            break;
                        case 1: // | - Pipe
                        {
                            switch (ExportOptions.ComplexTypeDelimiters)
                            {
                                case 0: // , - Comma
                                    GUILayout.Label("Example Vector Array - 1|2|3,4|5|6,7|8|9");
                                    break;
                                case 1: // | - Pipe
                                    GUILayout.Label("Cannot use Pipe as both Complex Type and Complex Array delimiters");
                                    break;
                                case 2: //   - Space
                                    GUILayout.Label("Example Vector Array - 1|2|3 4|5|6 7|8|9");
                                    break;
                            }
                        }
                            break;
                        case 2: //   - Space
                        {
                            switch (ExportOptions.ComplexTypeDelimiters)
                            {
                                case 0: // , - Comma
                                    GUILayout.Label("Example Vector Array - 1 2 3,4 5 6,7 8 9");
                                    break;
                                case 1: // | - Pipe
                                    GUILayout.Label("Example Vector Array - 1 2 3|4 5 6|7 8 9");
                                    break;
                                case 2: //   - Space
                                    GUILayout.Label("Cannot use Space as both Complex Type and Complex Array delimiters");
                                    break;
                            }
                        }
                            break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (newComplexArrayDelimiters != ExportOptions.ComplexArrayDelimiters)
                    {
                        Google2uGUIUtil.SetInt(prefix + "ComplexArrayDelimiters", newComplexArrayDelimiters);
                        ExportOptions.ComplexArrayDelimiters = newComplexArrayDelimiters;
                    }
                }
                in_layout.EndFadeArea();

                if (in_exportType == ExportType.ObjectDatabase)
                {
                    ShowSpreadsheetOptionsObjectDB =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsObjectDBOpen",
                            ShowSpreadsheetOptionsObjectDB);
                    var showWorkbookOptionsObjectDB = in_layout.BeginFadeArea(ShowSpreadsheetOptionsObjectDB,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GAME_OBJECT_DATABASE) + " " +
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CREATION_OPTIONS),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsObjectDB", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);
                    ShowSpreadsheetOptionsObjectDB = showWorkbookOptionsObjectDB.Open;
                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsObjectDBOpen",
                        ShowSpreadsheetOptionsObjectDB);
                    if (showWorkbookOptionsObjectDB.Show())
                    {
                        EditorGUILayout.LabelField("Global Options");
                        EditorGUILayout.Separator();

                        ExportOptions.ExportDatabaseGameObject =
                            EditorGUILayout.ObjectField(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GAME_OBJECT_DATABASE) +
                                ": ", ExportOptions.ExportDatabaseGameObject, typeof (GameObject), true) as GameObject;

                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PLAYMAKER),
                            ref ExportOptions.GeneratePlaymakerActions, prefix + "GeneratePlaymakerActions");
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_PERSIST_SCENE_LOADING),
                            ref ExportOptions.UseDoNotDestroy, prefix + "UseDoNotDestroy");

                        var oldObjectDBCullColumns = ExportOptions.ObjectDBCullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.ObjectDBCullColumns, prefix + "ObjectDBCullColumns");

                        if (oldObjectDBCullColumns != ExportOptions.ObjectDBCullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldObjectDBCullRows = ExportOptions.ObjectDBCullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.ObjectDBCullRows, prefix + "ObjectDBCullRows");

                        if (oldObjectDBCullRows != ExportOptions.ObjectDBCullRows)
                            in_activeWorksheet.UpdateValidation = true;

                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Local Options - " + in_activeWorksheet.WorksheetName);
                        EditorGUILayout.Separator();

                        var overrideObject =
                            EditorGUILayout.ObjectField(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GAME_OBJECT_DATABASE) +
                                ": ",
                                ExportOptions.GetOverrideObjectDatabaseGameObject(in_activeWorksheet.WorksheetName),
                                typeof (GameObject), true) as GameObject;
                        if (overrideObject != null)
                            ExportOptions.SetOverrideObjectDatabaseGameObject(in_activeWorksheet.WorksheetName,
                                overrideObject);
                    }
                    in_layout.EndFadeArea();
                }

                if (in_exportType == ExportType.StaticDatabase)
                {
                    ShowSpreadsheetOptionsStaticDB =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsStaticDBOpen",
                            ShowSpreadsheetOptionsObjectDB);

                    var showWorkbookOptionsStaticDB = in_layout.BeginFadeArea(ShowSpreadsheetOptionsStaticDB,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_STATIC_DATABASE) + " " +
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CREATION_OPTIONS),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsStaticDB", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetOptionsStaticDB = showWorkbookOptionsStaticDB.Open;
                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsStaticDBOpen",
                        ShowSpreadsheetOptionsStaticDB);
                    if (showWorkbookOptionsStaticDB.Show())
                    {
                        var oldStaticDBCullColumns = ExportOptions.StaticDBCullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.StaticDBCullColumns, prefix + "StaticDBCullColumns");
                        if (oldStaticDBCullColumns != ExportOptions.StaticDBCullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldStaticDBCullRows = ExportOptions.StaticDBCullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.StaticDBCullRows, prefix + "StaticDBCullRows");
                        if (oldStaticDBCullRows != ExportOptions.StaticDBCullRows)
                            in_activeWorksheet.UpdateValidation = true;
                    }
                    in_layout.EndFadeArea();
                }


                if (in_exportType == ExportType.JSON)
                {
                    ShowSpreadsheetOptionsJSON =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONOpen",
                            ShowSpreadsheetOptionsJSON);

                    var showWorkbookOptionsJSON = in_layout.BeginFadeArea(ShowSpreadsheetOptionsJSON,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_JSON_FORMATTING),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSON", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetOptionsJSON = showWorkbookOptionsJSON.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONOpen",
                        ShowSpreadsheetOptionsJSON);

                    if (showWorkbookOptionsJSON.Show())
                    {
                        var oldEscapeUnicode = ExportOptions.EscapeUnicode;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ESCAPE_UNICODE),
                            ref ExportOptions.EscapeUnicode, prefix + "EscapeUnicode");
                        if (oldEscapeUnicode != ExportOptions.EscapeUnicode)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldJSONCellArrayToString = ExportOptions.JSONCellArrayToString;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CONVERT_CELL_ARRAYS),
                            ref ExportOptions.JSONCellArrayToString, prefix + "JSONCellArrayToString");
                        if (oldJSONCellArrayToString != ExportOptions.JSONCellArrayToString)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldJSONExportClass = ExportOptions.JSONExportClass;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_JSON_EXPORT_CLASS),
                            ref ExportOptions.JSONExportClass, prefix + "JSONExportClass");
                        if (oldJSONExportClass != ExportOptions.JSONExportClass)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldExportType = ExportOptions.JSONExportType;
                        ExportOptions.JSONExportType =
                            (Google2uExportOptions.ExportType) EditorGUILayout.EnumPopup(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_JSON_EXPORT_TYPE) + ":",
                                ExportOptions.JSONExportType);
                        if (oldExportType != ExportOptions.JSONExportType)
                        {
                            Google2uGUIUtil.SetEnum(prefix + "JSONExportType", ExportOptions.JSONExportType);
                            in_activeWorksheet.UpdateValidation = true;
                        }

                        var oldJSONCullColumns = ExportOptions.JSONCullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.JSONCullColumns, prefix + "JSONCullColumns");
                        if (oldJSONCullColumns != ExportOptions.JSONCullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldJSONCullRows = ExportOptions.JSONCullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.JSONCullRows, prefix + "JSONCullRows");
                        if (oldJSONCullRows != ExportOptions.JSONCullRows)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldJSONIgnoreIDColumn = ExportOptions.JSONIgnoreIDColumn;
                        EditorGUILayoutEx.ToggleInput(
                            "Ignore ID Column",
                            ref ExportOptions.JSONIgnoreIDColumn, prefix + "JSONIgnoreIDColumn");
                        if (oldJSONIgnoreIDColumn != ExportOptions.JSONIgnoreIDColumn)
                            in_activeWorksheet.UpdateValidation = true;

                        EditorGUILayoutEx.ToggleInput(
                            "Readable Export",
                            ref ExportOptions.JSONExportPretty, prefix + "JSONExportPretty");

                    }
                    in_layout.EndFadeArea();


                    ShowSpreadsheetPreviewJSON =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewOpen",
                            ShowSpreadsheetPreviewJSON);

                    var showWorkbookPreviewJSON = in_layout.BeginFadeArea(ShowSpreadsheetPreviewJSON,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_JSON_OBJECT_PREVIEW),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewOpen", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetPreviewJSON = showWorkbookPreviewJSON.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewOpen",
                        ShowSpreadsheetPreviewJSON);

                    if (showWorkbookPreviewJSON.Show())
                    {
                        var oldEnabled = GUI.enabled;
                        if (!in_activeWorksheet.IsDataValid)
                            GUI.enabled = false;

                        GUI.SetNextControlName("JSONPREVIEW");
                        if (
                            GUILayout.Button(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PREVIEW)))
                        {
                            _JSONPreviewString = Google2u.ExportJsonObjectString(in_activeWorksheet, ExportOptions, true);
                        }

                        var textSize = in_layout.CellHeader.CalcSize(new GUIContent(_JSONPreviewString));
                        _JSONPreviewScrollPos = EditorGUILayout.BeginScrollView(_JSONPreviewScrollPos, false, false,
                            GUILayout.MinHeight(250));

                        var newString = EditorGUILayout.TextArea(_JSONPreviewString, GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
                        if (newString != _JSONPreviewString)
                            GUI.FocusControl("JSONPREVIEW");

                        GUI.enabled = oldEnabled;
                        EditorGUILayout.EndScrollView();
                    }
                    in_layout.EndFadeArea();


                    ShowSpreadsheetPreviewJSONClass =
                        Google2uGUIUtil.GetBool(
                            "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewClassOpen",
                            ShowSpreadsheetPreviewJSONClass);

                    var showWorkbookPreviewJSONClass = in_layout.BeginFadeArea(ShowSpreadsheetPreviewJSONClass,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_JSON_CLASS_PREVIEW),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewClassOpen", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetPreviewJSONClass = showWorkbookPreviewJSONClass.Open;

                    Google2uGUIUtil.SetBool(
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsJSONPreviewClassOpen",
                        ShowSpreadsheetPreviewJSONClass);

                    if (showWorkbookPreviewJSONClass.Show())
                    {
                        var oldEnabled = GUI.enabled;
                        if (!in_activeWorksheet.IsDataValid)
                            GUI.enabled = false;

                        GUI.SetNextControlName("JSONPREVIEWCLASS");
                        if (
                            GUILayout.Button(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_CLASS)))
                        {
                            _JSONPreviewClassString = Google2u.ExportJsonObjectClassString(in_activeWorksheet,
                                ExportOptions);
                        }

                        var textSize = in_layout.CellHeader.CalcSize(new GUIContent(_JSONPreviewClassString));
                        _JSONPreviewClassScrollPos = EditorGUILayout.BeginScrollView(_JSONPreviewClassScrollPos, false,
                            false, GUILayout.MinHeight(250));

                        var newString = EditorGUILayout.TextArea(_JSONPreviewClassString, GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
                        if (newString != _JSONPreviewClassString)
                            GUI.FocusControl("JSONPREVIEWCLASS");

                        GUI.enabled = oldEnabled;
                        EditorGUILayout.EndScrollView();
                    }
                    in_layout.EndFadeArea();
                }


                if (in_exportType == ExportType.CSV)
                {
                    ShowSpreadsheetOptionsCSV =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSVOpen",
                            ShowSpreadsheetOptionsCSV);

                    var showWorkbookOptionsCSV = in_layout.BeginFadeArea(ShowSpreadsheetOptionsCSV,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CSV_FORMATTING),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSV", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetOptionsCSV = showWorkbookOptionsCSV.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSVOpen",
                        ShowSpreadsheetOptionsCSV);

                    if (showWorkbookOptionsCSV.Show())
                    {
                        var oldEscapeCSVStrings = ExportOptions.EscapeCSVStrings;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ESCAPE_STRINGS),
                            ref ExportOptions.EscapeCSVStrings, prefix + "EscapeCSVStrings");
                        if (oldEscapeCSVStrings != ExportOptions.EscapeCSVStrings)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldCSVCullColumns = ExportOptions.CSVCullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.CSVCullColumns, prefix + "CSVCullColumns");
                        if (oldCSVCullColumns != ExportOptions.CSVCullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldCSVCullRows = ExportOptions.CSVCullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.CSVCullRows, prefix + "CSVCullRows");
                        if (oldCSVCullRows != ExportOptions.CSVCullRows)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldCSVConvertLineBreaks = ExportOptions.CSVConvertLineBreaks;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ESCAPE_LINE_BREAKS),
                            ref ExportOptions.CSVConvertLineBreaks, prefix + "CSVConvertLineBreaks");
                        if (oldCSVConvertLineBreaks != ExportOptions.CSVConvertLineBreaks)
                            in_activeWorksheet.UpdateValidation = true;
                    }
                    in_layout.EndFadeArea();

                    ShowSpreadsheetPreviewCSV =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSVPreviewOpen",
                            ShowSpreadsheetPreviewCSV);

                    var showWorkbookPreviewCSV = in_layout.BeginFadeArea(ShowSpreadsheetPreviewCSV,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CSV_PREVIEW),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSVPreviewOpen", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetPreviewCSV = showWorkbookPreviewCSV.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsCSVPreviewOpen",
                        ShowSpreadsheetPreviewCSV);

                    if (showWorkbookPreviewCSV.Show())
                    {
                        var oldEnabled = GUI.enabled;
                        if (!in_activeWorksheet.IsDataValid)
                            GUI.enabled = false;

                        GUI.SetNextControlName("CSVPREVIEW");
                        if (
                            GUILayout.Button(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PREVIEW)))
                        {
                            _CSVPreviewString = Google2u.ExportCsvString(in_activeWorksheet, ExportOptions);
                        }

                        var textSize = in_layout.CellHeader.CalcSize(new GUIContent(_CSVPreviewString));
                        _CSVPreviewScrollPos = EditorGUILayout.BeginScrollView(_CSVPreviewScrollPos, false, false,
                            GUILayout.MinHeight(250));

                        var newString = EditorGUILayout.TextArea(_CSVPreviewString, GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
                        if (newString != _CSVPreviewString)
                            GUI.FocusControl("CSVPREVIEW");

                        GUI.enabled = oldEnabled;
                        EditorGUILayout.EndScrollView();
                    }
                    in_layout.EndFadeArea();
                }

                if (in_exportType == ExportType.NGUI)
                {
                    ShowSpreadsheetOptionsNGUI =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUIOpen",
                            ShowSpreadsheetOptionsNGUI);

                    var showWorkbookOptionsNGUI = in_layout.BeginFadeArea(ShowSpreadsheetOptionsNGUI,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_NGUI_FORMATTING),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUI", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetOptionsNGUI = showWorkbookOptionsNGUI.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUIOpen",
                        ShowSpreadsheetOptionsNGUI);

                    if (showWorkbookOptionsNGUI.Show())
                    {
                        var oldEscapeNGUIStrings = ExportOptions.EscapeNGUIStrings;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ESCAPE_STRINGS),
                            ref ExportOptions.EscapeNGUIStrings, prefix + "EscapeNGUIStrings");
                        if (oldEscapeNGUIStrings != ExportOptions.EscapeNGUIStrings)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldNGUICullColumns = ExportOptions.NGUICullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.NGUICullColumns, prefix + "NGUICullColumns");
                        if (oldNGUICullColumns != ExportOptions.NGUICullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldNGUICullRows = ExportOptions.NGUICullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.NGUICullRows, prefix + "NGUICullRows");
                        if (oldNGUICullRows != ExportOptions.NGUICullRows)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldNGUIConvertLineBreaks = ExportOptions.NGUIConvertLineBreaks;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ESCAPE_LINE_BREAKS),
                            ref ExportOptions.NGUIConvertLineBreaks, prefix + "NGUIConvertLineBreaks");
                        if (oldNGUIConvertLineBreaks != ExportOptions.NGUIConvertLineBreaks)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldNGUILegacyExport = ExportOptions.NGUILegacyExport;
                        // TODO: Localize
                        EditorGUILayoutEx.ToggleInput(
                            "Use NGUI Legacy Export",
                            ref ExportOptions.NGUILegacyExport, prefix + "NGUILegacyExport");
                        if (oldNGUILegacyExport != ExportOptions.NGUILegacyExport)
                            in_activeWorksheet.UpdateValidation = true;
                    }
                    in_layout.EndFadeArea();

                    ShowSpreadsheetPreviewNGUI =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUIPreviewOpen",
                            ShowSpreadsheetPreviewNGUI);

                    var showWorkbookPreviewNGUI = in_layout.BeginFadeArea(ShowSpreadsheetPreviewNGUI,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_NGUI_PREVIEW),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUIPreviewOpen", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetPreviewNGUI = showWorkbookPreviewNGUI.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsNGUIPreviewOpen",
                        ShowSpreadsheetPreviewNGUI);

                    if (showWorkbookPreviewNGUI.Show())
                    {
                        var oldEnabled = GUI.enabled;
                        if (!in_activeWorksheet.IsDataValid)
                            GUI.enabled = false;

                        GUI.SetNextControlName("NGUIPREVIEW");
                        if (
                            GUILayout.Button(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PREVIEW)))
                        {
                            if (ExportOptions.NGUILegacyExport)
                                _NGUIPreviewString = Google2u.ExportNGUILegacyString(in_activeWorksheet, ExportOptions);
                            else
                                _NGUIPreviewString = Google2u.ExportNGUIString(in_activeWorksheet, ExportOptions);
                        }

                        var textSize = in_layout.CellHeader.CalcSize(new GUIContent(_NGUIPreviewString));
                        _NGUIPreviewScrollPos = EditorGUILayout.BeginScrollView(_NGUIPreviewScrollPos, false, false,
                            GUILayout.MinHeight(250));

                        var newString = EditorGUILayout.TextArea(_NGUIPreviewString, GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
                        if (newString != _NGUIPreviewString)
                            GUI.FocusControl("NGUIPREVIEW");

                        GUI.enabled = oldEnabled;
                        EditorGUILayout.EndScrollView();
                    }
                    in_layout.EndFadeArea();
                }

                if (in_exportType == ExportType.XML)
                {
                    ShowSpreadsheetOptionsXML =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXMLOpen",
                            ShowSpreadsheetOptionsXML);

                    var showWorkbookOptionsXML = in_layout.BeginFadeArea(ShowSpreadsheetOptionsXML,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_XML_FORMATTING),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXML", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetOptionsXML = showWorkbookOptionsXML.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXMLOpen",
                        ShowSpreadsheetOptionsXML);

                    if (showWorkbookOptionsXML.Show())
                    {

                        

                        var oldXMLColsAsChildTags = ExportOptions.XMLColsAsChildTags;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_XML_COLUMN_AS_CHILD_TAGS),
                            ref ExportOptions.XMLColsAsChildTags, prefix + "XMLColsAsChildTags");
                        if (oldXMLColsAsChildTags != ExportOptions.XMLColsAsChildTags)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldXMLCellArrayToString = ExportOptions.XMLCellArrayToString;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CONVERT_CELL_ARRAYS),
                            ref ExportOptions.XMLCellArrayToString, prefix + "XMLCellArrayToString");
                        if (oldXMLCellArrayToString != ExportOptions.XMLCellArrayToString)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldXMLCullColumns = ExportOptions.XMLCullColumns;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_COLUMNS),
                            ref ExportOptions.XMLCullColumns, prefix + "XMLCullColumns");
                        if (oldXMLCullColumns != ExportOptions.XMLCullColumns)
                            in_activeWorksheet.UpdateValidation = true;

                        var oldXMLCullRows = ExportOptions.XMLCullRows;
                        EditorGUILayoutEx.ToggleInput(
                            Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CULL_ROWS),
                            ref ExportOptions.XMLCullRows, prefix + "XMLCullRows");
                        if (oldXMLCullRows != ExportOptions.XMLCullRows)
                            in_activeWorksheet.UpdateValidation = true;
                    }
                    in_layout.EndFadeArea();

                    ShowSpreadsheetPreviewXML =
                        Google2uGUIUtil.GetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXMLPreviewOpen",
                            ShowSpreadsheetPreviewXML);

                    var showWorkbookPreviewXML = in_layout.BeginFadeArea(ShowSpreadsheetPreviewXML,
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_XML_PREVIEW),
                        "workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXMLPreviewOpen", in_layout.OuterBox,
                        in_layout.OuterBoxHeader);

                    ShowSpreadsheetPreviewXML = showWorkbookPreviewXML.Open;

                    Google2uGUIUtil.SetBool("workbook" + WorkbookName.Replace(' ', '_') + "_OptionsXMLPreviewOpen",
                        ShowSpreadsheetPreviewXML);

                    if (showWorkbookPreviewXML.Show())
                    {
                        var oldEnabled = GUI.enabled;
                        if (!in_activeWorksheet.IsDataValid)
                            GUI.enabled = false;

                        GUI.SetNextControlName("XMLPREVIEW");
                        if (
                            GUILayout.Button(
                                Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PREVIEW)))
                        {
                            _XMLPreviewString = Google2u.ExportXMLString(in_activeWorksheet, ExportOptions);
                        }

                        var textSize = in_layout.CellHeader.CalcSize(new GUIContent(_XMLPreviewString));
                        _XMLPreviewScrollPos = EditorGUILayout.BeginScrollView(_XMLPreviewScrollPos, false, false,
                            GUILayout.MinHeight(250));

                        var newString = EditorGUILayout.TextArea(_XMLPreviewString, GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true), GUILayout.MinWidth(textSize.x), GUILayout.MinHeight(textSize.y));
                        if (newString != _XMLPreviewString)
                            GUI.FocusControl("XMLPREVIEW");

                        GUI.enabled = oldEnabled;
                        EditorGUILayout.EndScrollView();
                    }
                    in_layout.EndFadeArea();
                }
            }
            in_layout.EndFadeArea();
        }

        public void DoExport(List<Google2uWorksheet> in_ExportWorksheets)
        {
            // Kick off the export process
            foreach (var Google2uWorksheet in in_ExportWorksheets)
            {
                Google2uWorksheet.DoExport();
            }
        }
    }
}