// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Google.GData.Client;
    using Google.GData.Spreadsheets;
    using UnityEditor;
    using UnityEngine;

    #endregion

    [Serializable]
    public class Google2uRow
    {
        public List<Google2uCell> Cells = new List<Google2uCell>();

        private Google2uRow(List<Google2uCell> in_data)
        {
            Cells = in_data;
        }

        public Google2uCell this[int in_i]
        {
            get { return Cells.Count > in_i ? Cells[in_i] : null; }
            set { if (Cells.Count > in_i) Cells[in_i] = value; }
        }

        public int Count
        {
            get { return Cells.Count; }
        }

        public static implicit operator Google2uRow(List<Google2uCell> in_data)
        {
            return new Google2uRow(in_data);
        }

        public static implicit operator List<Google2uCell>(Google2uRow in_instance)
        {
            return in_instance.Cells;
        }
    }

    [Serializable]
    public class Google2uWorksheet
    {
        private readonly Dictionary<int, Google2uGUIUtil.ComboBox> _ComboBoxes =
            new Dictionary<int, Google2uGUIUtil.ComboBox>();

        private readonly GUIContent[] _ComboBoxList;
        private string _CSVPath;
        private Thread _ExportThread;

        [SerializeField] private bool _Initialized;

        private string _JSONPath;
        private Rect _LastRect = new Rect(0f, 0f, 1f, 1f);

        [SerializeField] private Vector2 _MyScrollPos = Vector2.zero;

        private Thread _NewQueryThread;
        private string _NGUIPath;
        private string _ObjdbEditorPath;
        private string _ObjdbResourcesPath;
        private string _PlaymakerPath;
        private Thread _QueryThread;
        private Google2uRow[] _RowsDisplay = new Google2uRow[0];
        private bool _SetInvalidCellActive;
        private string _StaticDbPath;
        private Thread _UpdateCellTypesThread;
        private Thread _UpdateQueryThread;
        private Thread _ValidationThread;
        private string _XMLPath;
        public Google2uCell ActiveCell;
        public List<ColumnOption> ColOptions;
        public bool DoInitialSizeCheck = true;
        public bool DoUpdateQuery;
        public int FirstBlankCol;
        public bool IsDataValid;
        public int LastUpdateCount;
        public DateTime LastUpdateTime;
        public CellFeed MyCellFeed;
        public CellQuery MyCellQuery;
        public CellQuery MyCleanupQuery;
        public Service MyService;
        public WorkbookBase MyWorkbook;
        public string Prefix;
        public List<RowOption> RowOptions;
        public List<Google2uRow> Rows = new List<Google2uRow>();
        public bool UpdateCellTypes;
        public bool UpdateValidation;
        public bool UseTypeRow;
        public bool Validating;
        public WorksheetEntry WorksheetEntry;
        public ExportType WorksheetExportType = ExportType.DoNotExport;
        public string WorksheetName;
        public QueryStatus WorksheetQueryStatus = QueryStatus.Uninitialized;

        public Google2uWorksheet(WorksheetEntry in_entry, Service in_service, WorkbookBase in_base)
        {
            WorksheetEntry = in_entry;
            MyWorkbook = in_base;
            WorksheetName = WorksheetEntry.Title.Text;
            Prefix = "worksheet" + WorksheetName.Replace(' ', '_');
            MyService = in_service;

            MyCleanupQuery = new CellQuery(in_entry.CellFeedLink);
            MyCellQuery = new CellQuery(in_entry.CellFeedLink) {ReturnEmpty = ReturnEmptyCells.yes};

            _ComboBoxList = new GUIContent[Convert.ToInt32(SupportedType.Unrecognized)];
            var iter = 0;
            foreach (var enumval in Enum.GetValues(typeof (SupportedType)))
            {
                if ((SupportedType) enumval == SupportedType.Unrecognized)
                    break;
                _ComboBoxList[iter] = new GUIContent(Convert.ToString(enumval));
                iter++;
            }
        }

        public Google2uRow[] RowsDisplay
        {
            get { return _RowsDisplay; }
        }

        public void EndOfFrame()
        {
            _RowsDisplay = Rows.ToArray();
        }

        public static void UpdateQuery(object in_worksheet)
        {
            var worksheet = in_worksheet as Google2uWorksheet;
            if (worksheet == null)
                return;

            uint lastRow = 0;
            uint lastCol = 0;

            var cleanupFeed = worksheet.MyService.Query(worksheet.MyCleanupQuery) as CellFeed;
            if (cleanupFeed == null)
                return;

            // for loops are faster in Mono, do not convert to foreach
            {
                CellEntry c;
                for (var i = 0; i < cleanupFeed.Entries.Count; ++i)
                {
                    c = cleanupFeed.Entries[i] as CellEntry;
                    if (c.Row > lastRow)
                        lastRow = c.Row;
                    if (c.Column > lastCol)
                        lastCol = c.Column;
                }
            }

            worksheet.MyCellQuery.MaximumColumn = lastCol;
            worksheet.MyCellQuery.MaximumRow = lastRow;

            CellFeed updateFeed = null;

            try
            {
                updateFeed = worksheet.MyService.Query(worksheet.MyCellQuery) as CellFeed;
            }
            catch (Exception)
            {
                Google2u.PushNotification("Unable to Update " + worksheet.WorksheetName +
                                          ". If this problem persists, there is probably something wrong with the spreadsheet.");
            }

            if (updateFeed == null)
                return;

            CellEntry updatedEntry = null;
            // compare the update feed with the cells
            var bDoFullUpdate = updateFeed.Entries.Count != worksheet.LastUpdateCount;
            worksheet.LastUpdateCount = updateFeed.Entries.Count;

            if (!bDoFullUpdate)
            {
                // for loops are faster in Mono, do not convert to foreach
                CellEntry wsEntry;
                CellEntry fEntry;
                for (var i = 0; i < worksheet.MyCellFeed.Entries.Count; ++i)
                {
                    wsEntry = worksheet.MyCellFeed.Entries[i] as CellEntry;
                    if (wsEntry == null)
                        continue;

                    for (var j = 0; j < updateFeed.Entries.Count; ++j)
                    {
                        fEntry = updateFeed.Entries[j] as CellEntry;
                        if (fEntry == null)
                            continue;

                        if (fEntry.Row != wsEntry.Row || fEntry.Column != wsEntry.Column ||
                            fEntry.Value == wsEntry.Value)
                            continue;

                        if (updatedEntry == null)
                            updatedEntry = fEntry;
                        else
                            bDoFullUpdate = true;
                        break;
                    }
                    if (bDoFullUpdate)
                        break;
                }
            }


            if (bDoFullUpdate)
            {
                // Do a full re-query
                worksheet.WorksheetQueryStatus = QueryStatus.Uninitialized;
                return;
            }

            if (updatedEntry != null)
            {
                var myCell = worksheet.RowsDisplay[(int) updatedEntry.Row - 1][(int) updatedEntry.Column - 1];
                myCell.MyCell = updatedEntry;
                worksheet.UpdateCellTypes = true;
            }

            worksheet.WorksheetQueryStatus = QueryStatus.Idle;
            worksheet.UpdateCellTypes = true;
            worksheet.UpdateValidation = true;
        }

        public static void QueryCells(object in_worksheet)
        {
            var worksheet = in_worksheet as Google2uWorksheet;
            if (worksheet == null)
                return;

            uint lastRow = 0;
            uint lastCol = 0;
            var cleanupFeed = worksheet.MyService.Query(worksheet.MyCleanupQuery) as CellFeed;
            if (cleanupFeed == null)
                return;

            // for loops are faster in Mono, do not convert to foreach
            {
                CellEntry c;
                for (var i = 0; i < cleanupFeed.Entries.Count; ++i)
                {
                    c = cleanupFeed.Entries[i] as CellEntry;
                    if (c.Row > lastRow)
                        lastRow = c.Row;
                    if (c.Column > lastCol)
                        lastCol = c.Column;
                }
            }

            worksheet.MyCellQuery.MaximumColumn = lastCol;
            worksheet.MyCellQuery.MaximumRow = lastRow;

            if (lastCol < 2 || lastRow < 2)
            {
                Google2u.PushNotification("There are not enough cells in the worksheet " + worksheet.WorksheetName +
                                          " it will not be processed");
                worksheet.WorksheetQueryStatus = QueryStatus.Idle;
                worksheet.UpdateValidation = true;
                return;
            }

            try
            {
                worksheet.MyCellFeed = worksheet.MyService.Query(worksheet.MyCellQuery) as CellFeed;
            }
            catch (Exception ex)
            {
                Google2u.PushNotification("Unable to Query " + worksheet.WorksheetName +
                                          ". If this problem persists, there is probably something wrong with the spreadsheet." +
                                          ex.Message);
            }

            if (worksheet.MyCellFeed != null)
            {
                worksheet.LastUpdateCount = worksheet.MyCellFeed.Entries.Count;

                if (worksheet.MyCellFeed == null)
                {
                    return;
                }

                var tmpCells = new List<Google2uRow>();

                // for loops are faster in Mono, do not convert to foreach
                CellEntry c;
                for (var i = 0; i < worksheet.MyCellFeed.Entries.Count; ++i)
                {
                    c = worksheet.MyCellFeed.Entries[i] as CellEntry;
                    if (c == null)
                        continue;

                    while (tmpCells.Count < c.Row)
                    {
                        tmpCells.Add(new List<Google2uCell>());
                    }
                    var r = tmpCells[tmpCells.Count - 1];
                    r.Cells.Add(new Google2uCell(c));
                }

                worksheet.UseTypeRow = true;
                if (tmpCells.Count > 1)
                {
                    var row = tmpCells[1];
                    for (var i = 1; i < row.Count; i++)
                    {
                        if (row[i].GetTypeFromValue() == SupportedType.Unrecognized)
                            worksheet.UseTypeRow = false;
                    }
                }

                if (worksheet.ColOptions == null)
                    worksheet.ColOptions = new List<ColumnOption>();

                for (var i = 0; i < lastCol; ++i)
                    worksheet.ColOptions.Add(new ColumnOption(60));


                if (worksheet.RowOptions == null)
                    worksheet.RowOptions = new List<RowOption>();

                for (var i = 0; i < lastRow; ++i)
                    worksheet.RowOptions.Add(new RowOption(24));

                worksheet.Rows = tmpCells;
            }
            worksheet.WorksheetQueryStatus = QueryStatus.Idle;
            worksheet.UpdateCellTypes = true;
            worksheet.UpdateValidation = true;
        }

        public void Update(bool in_isActive, Google2uExportOptions in_options)
        {
            if (!_Initialized)
                return;

            if ((DateTime.Now - LastUpdateTime).TotalSeconds > 15)
            {
                LastUpdateTime = DateTime.Now;
                _QueryThread = null;

                if (WorksheetQueryStatus == QueryStatus.Uninitialized)
                {
                    if (_NewQueryThread == null || _NewQueryThread.IsAlive == false)
                    {
                        _NewQueryThread = new Thread(QueryCells);
                        _QueryThread = _NewQueryThread;
                    }
                }
                else if (IsDataValid && in_isActive && DoUpdateQuery && WorksheetQueryStatus == QueryStatus.Idle)
                {
                    if (_UpdateQueryThread == null || _UpdateQueryThread.IsAlive == false)
                    {
                        DoUpdateQuery = false;
                        _UpdateQueryThread = new Thread(UpdateQuery);
                        _QueryThread = _UpdateQueryThread;
                    }
                }


                if (_QueryThread == null)
                    return;

                WorksheetQueryStatus = QueryStatus.Querying;
                _QueryThread.Start(this);
            }
            else if (Rows != null && WorksheetQueryStatus == QueryStatus.Idle && UpdateCellTypes)
            {
                if (_UpdateCellTypesThread == null || _UpdateCellTypesThread.IsAlive == false)
                {
                    UpdateCellTypes = false;
                    _UpdateCellTypesThread = new Thread(DoUpdateCellTypes);
                    _UpdateCellTypesThread.Start(this);
                }
            }

            if (Rows != null && UpdateValidation)
            {
                if (_ValidationThread == null || _ValidationThread.IsAlive == false)
                {
                    UpdateValidation = false;
                    _ValidationThread = new Thread(DoDataValidation);
                    _ValidationThread.Start(new DataValidationParams(this, in_options)); 
                }
            }
        }

        private void DoUpdateCellTypes(object in_worksheet)
        {
            var worksheet = in_worksheet as Google2uWorksheet;
            if (worksheet == null)
                return;

            if (worksheet.Rows == null || worksheet.Rows.Count <= 1)
                return;

            if (worksheet.UseTypeRow == false)
            {
                

                for (var i = 0; i < worksheet.Rows.Count; ++i)
                {
                    if(i == 0)
                        FirstBlankCol = worksheet.Rows[0].Cells.Count;

                    for (var j = 0; j < worksheet.Rows[i].Cells.Count; ++j)
                    {
                        worksheet.Rows[i].Cells[j].MyType = SupportedType.String;

                        if (i == 0 && FirstBlankCol > j &&
                            (string.IsNullOrEmpty(worksheet.Rows[i].Cells[j].CellValueString) ||
                            worksheet.Rows[i].Cells[j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) || 
                            worksheet.Rows[i].Cells[j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)
                            )
                            )
                        {
                            FirstBlankCol = j;
                            worksheet.Rows[i].Cells[j].MyType = SupportedType.Void;
                        }

                    }
                }
            }
            else
            {
                // First, get the second row
                var typeRow = worksheet.Rows[1];
                var nameRow = worksheet.Rows[0];
                var typeList = new List<SupportedType>();
                FirstBlankCol = nameRow.Count;
                for (var i = 0; i < typeRow.Count; ++i)
                {
                    if (nameRow[i].GetTypeFromValue() == SupportedType.Void)
                    {
                        if (FirstBlankCol > i && string.IsNullOrEmpty(nameRow[i].CellValueString))
                            FirstBlankCol = i;
                        typeRow[i].MyType = SupportedType.Void;
                        typeList.Add(SupportedType.Void);
                    }
                    else
                    {
                        typeRow[i].SetTypeFromValue();
                        typeList.Add(typeRow[i].GetTypeFromValue());
                    }
                }

                // now that we have that, set the remaining cells in the spreadsheet to be equal to the type row
                Google2uRow r;
                for (var i = 0; i < worksheet.Rows.Count; ++i)
                {
                    r = worksheet.Rows[i];
                    for (var j = 1; j < typeList.Count; ++j)
                    {
                        r[j].MyType = typeList[j];
                    }
                }
            }
        }

        public void DoDataValidation(object in_dataValidationParams)
        {
            var dataValidationParams = in_dataValidationParams as DataValidationParams;

            if (dataValidationParams == null)
                return;

            var worksheet = dataValidationParams.Worksheet;
            var exportOptions = dataValidationParams.Options;

            for (var i = 1; i < worksheet.Rows[0].Count; ++i)
            {
                if (string.IsNullOrEmpty(worksheet.Rows[0][i].CellValueString) ||
                    worksheet.Rows[0][i].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                    worksheet.Rows[0][i].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase))
                {
                    FirstBlankCol = i;
                    break;
                }
            }


            if (worksheet.UseTypeRow == false)
            {
                worksheet.IsDataValid = true;
                
            }
            else
            {
                var lastCheckedRow = worksheet.Rows.Count;
                var lastCheckedCol = worksheet.Rows[0].Count;
                worksheet.IsDataValid = true;

                for (var i = 0; i <= worksheet.Rows.Count - 1; ++i)
                {
                    if (string.IsNullOrEmpty(worksheet.Rows[i][0].CellValueString))
                    {
                        var doBreak = false;
                        switch (WorksheetExportType)
                        {
                            case ExportType.CSV:
                                if (exportOptions.CSVCullRows)
                                    doBreak = true;
                                break;
                            case ExportType.JSON:
                                if (exportOptions.JSONCullRows)
                                    doBreak = true;
                                break;
                            case ExportType.ObjectDatabase:
                                if (exportOptions.ObjectDBCullRows)
                                    doBreak = true;
                                break;
                            case ExportType.StaticDatabase:
                                if (exportOptions.StaticDBCullRows)
                                    doBreak = true;
                                break;
                        }
                        if (doBreak)
                        {
                            lastCheckedRow = i;
                        }
                        continue;
                    }

                    worksheet.Rows[i][0].SetTypeFromValue();
                    if (worksheet.Rows[i][0].MyType == SupportedType.Void)
                        continue;

                    for (var j = 0; j <= worksheet.Rows[i].Count - 1; ++j)
                    {
                        var cell = worksheet.Rows[i][j];


                        cell.SkipValidation = false;

                        if (j > lastCheckedCol)
                        {
                            cell.SkipValidation = true;
                            continue;
                        }
                        
                        CellType t;
                        if (i == 0 && j == 0)
                            t = CellType.Null;
                        else if (i == 1 && j == 0)
                            t = CellType.Null;
                        else if (i == 0)
                            t = CellType.ColumnHeader;
                        else if (i == 1 && j > 1)
                            t = CellType.Type;
                        else if (j == 0)
                            t = CellType.RowHeader;
                        else
                            t = CellType.Value;

                        if (i >= lastCheckedRow)
                        {
                            worksheet.Validating = false;
                            worksheet.IsDataValid = true;
                            cell.SkipValidation = true;
                            continue;
                        }


                        switch (WorksheetExportType)
                        {
                            case ExportType.ObjectDatabase:
                            {
                                if (exportOptions.ObjectDBCullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }
                            }
                            break;

                            case ExportType.StaticDatabase:
                            {
                                if (exportOptions.StaticDBCullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }
                            }
                            break;

                            case ExportType.CSV:
                            {
                                if (exportOptions.CSVCullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }
                            }
                            break;

                            case ExportType.JSON:
                            {
                                if (exportOptions.JSONCullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }

                                if (exportOptions.JSONIgnoreIDColumn && j == 0)
                                {
                                    cell.SkipValidation = true;
                                    continue;
                                }
                            }
                            break;

                            case ExportType.NGUI:
                            {
                                if (exportOptions.NGUICullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }
                            }
                            break;

                            case ExportType.XML:
                            {
                                if (exportOptions.XMLCullColumns &&
                                    (string.IsNullOrEmpty(worksheet.Rows[0][j].CellValueString) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                    worksheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    cell.SkipValidation = true;
                                    lastCheckedCol = j;
                                    continue;
                                }
                            }
                            break;

                            default:
                                cell.SkipValidation = true;
                                continue;
                        }


                        try
                        {
                            if (!cell.DoDataValidation(t, worksheet.Rows, worksheet.MyWorkbook))
                            {
                                if (_SetInvalidCellActive)
                                {
                                    worksheet.ActiveCell = cell;
                                    _SetInvalidCellActive = false;
                                }
                                worksheet.IsDataValid = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.Message);
                            worksheet.Validating = false;
                        }
                    }
                }
            }

            if (!validWorksheetName(WorksheetName))
            {
                IsDataValid = false;
                Google2u.PushNotification("Worksheet name is invalid: " + WorksheetName);
            }
            worksheet.Validating = false;
        }

        private bool validWorksheetName(string val)
        {
            if (!char.IsLetter(val[0]))
            {
                return false;
            }

            if (!Google2uCell.ValidStart(val))
            {
                return false;
            }

            if (!Google2uCell.ContainsInvalidChars(val))
            {
                return false;
            }

            if (Google2uCell.ContainsKeyword(val))
            {
                return false;
            }
            return true;
        }

        private static string GetColumnName(int in_columnNumber)
        {
            var dividend = in_columnNumber;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1)%26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = ((dividend - modulo)/26);
            }

            return columnName;
        }

        public bool DrawGUIList(EditorGUILayoutEx in_layout)
        {
            if (!_Initialized)
            {
                _Initialized = true;
                UseTypeRow = Google2uGUIUtil.GetBool(Prefix + "UseTypeRow", UseTypeRow);
                WorksheetExportType = Google2uGUIUtil.GetEnum(Prefix + "ExportType", WorksheetExportType);
            }

            var old = GUI.enabled;
            if (WorksheetQueryStatus != QueryStatus.Idle && RowsDisplay.Length == 0)
                GUI.enabled = false;


            var newExportType = WorksheetExportType;

            if (IsDataValid)
                newExportType = (ExportType) EditorGUILayout.EnumPopup(WorksheetName, WorksheetExportType);
            else if (WorksheetQueryStatus != QueryStatus.Idle)
                EditorGUILayout.LabelField(WorksheetName,
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_QUERYING_CELLS) +
                    Google2u.Ellipses);
            else
            {
                var oldColor = GUI.color;
                if (GUI.GetNameOfFocusedControl() != WorksheetName + "Invalid")
                    GUI.color = Color.red;
                GUI.SetNextControlName(WorksheetName + "Invalid");
                newExportType = (ExportType) EditorGUILayout.EnumPopup(WorksheetName, WorksheetExportType);
                GUI.color = oldColor;
            }

            if (newExportType != WorksheetExportType)
            {
                WorksheetExportType = Google2uGUIUtil.SetEnum(Prefix + "ExportType", newExportType);
            }

            GUI.enabled = old;

            return WorksheetExportType != ExportType.DoNotExport;
        }

        public void DrawGUIFull(EditorGUILayoutEx in_layout)
        {
            if (!_Initialized)
            {
                _Initialized = true;
                UseTypeRow = Google2uGUIUtil.GetBool(Prefix + "UseTypeRow", UseTypeRow);
                WorksheetExportType = Google2uGUIUtil.GetEnum(Prefix + "ExportType", WorksheetExportType);
            }

            if (WorksheetQueryStatus != QueryStatus.QueryComplete && WorksheetQueryStatus != QueryStatus.Idle &&
                RowsDisplay.Length == 0)
            {
                EditorGUILayout.LabelField(
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_QUERYING_CELLS) +
                    Google2u.Ellipses);
            }
            else if (RowsDisplay.Length == 0)
            {
                if (WorksheetQueryStatus == QueryStatus.QueryComplete || WorksheetQueryStatus == QueryStatus.Idle)
                    EditorGUILayout.LabelField(
                        Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_EMPTY_WORKSHEET));
            }
            else
            {
                if (DoInitialSizeCheck)
                {
                    DoInitialSizeCheck = false;
                    for (var i = 0; i < ColOptions.Count; ++i)
                    {
                        CalculateColumnWidth(i, this);
                    }
                }

                if ((DateTime.Now - LastUpdateTime).TotalSeconds > 15)
                {
                    DoUpdateQuery = true;
                }

                if (ActiveCell != null)
                {
                    if (!ActiveCell.SkipValidation && !string.IsNullOrEmpty(ActiveCell.Tooltip))
                    {
                        EditorGUILayoutEx.SetColor(Color.red);
                        GUILayout.Label(ActiveCell.Tooltip);
                        EditorGUILayoutEx.ResetColor();
                    }
                    if (ActiveCell.DrawCellValue(in_layout))
                    {
                        WorksheetQueryStatus = QueryStatus.Idle;
                        LastUpdateTime = DateTime.MinValue;
                        DoUpdateQuery = true;
                        UpdateValidation = true;
                    }
                }
                else
                {
                    var old = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.TextField(string.Empty);
                    GUI.enabled = old;
                }

                // Calculate the total width and height of the scroll area
                var totalHeight = Math.Max(120, 22 + (24*RowsDisplay.Length));
                var totalWidth = 40 + ColOptions.Sum(in_colOption => in_colOption.Width);

                EditorGUILayout.Separator();
                if (Event.current.type == EventType.Repaint)
                    _LastRect = GUILayoutUtility.GetLastRect();
                var scrollHeight = Screen.height - _LastRect.y - 30;
                var screenRect = new Rect(0f, _LastRect.y, Screen.width, scrollHeight);
                var viewRect = new Rect(0f, 0f, totalWidth, totalHeight);

                _MyScrollPos = GUI.BeginScrollView(screenRect, _MyScrollPos, viewRect);

                var curRect = new Rect(0.0f, 0.0f, 40.0f, 22.0f);

                // Blank
                GUI.Label(curRect, string.Empty, in_layout.CellHeader);

                // Column Letters (Resizable Columns)
                for (var i = 0; i < RowsDisplay[0].Count; i++)
                {
                    var label = GetColumnName(i + 1);
                    curRect.x += curRect.width;
                    curRect.width = ColOptions[i].Width;
                    GUI.Label(curRect, label, in_layout.CellHeader);

                    ColOptions[i].ColumnRect = curRect;
                    ColOptions[i].ColumnRect.width = ColOptions[i].Width;

                    if (ColOptions[i].ColumnRect.Contains(Event.current.mousePosition))
                        ColOptions[i].HasMouse = true;

                    if (!ColOptions[i].HasMouse)
                        continue;

                    if ((Event.current.type == EventType.mouseDown) && (Event.current.clickCount >= 2))
                    {
                        // Doubleclick
                        CalculateColumnWidth(i, this);
                    }
                    if (Event.current.type == EventType.mouseDrag)
                    {
                        ColOptions[i].CurPos = Event.current.mousePosition;

                        if (!ColOptions[i].Dragging)
                        {
                            ColOptions[i].Dragging = true;
                            ColOptions[i].StartPos = ColOptions[i].CurPos;
                        }
                    }

                    if (Event.current.type == EventType.mouseUp)
                    {
                        ColOptions[i].Dragging = false;
                        ColOptions[i].HasMouse = false;
                    }

                    if (!ColOptions[i].Dragging)
                        continue;

                    if (Event.current.isMouse)
                        Event.current.Use();

                    ColOptions[i].Width +=
                        Convert.ToInt32((ColOptions[i].CurPos.x - ColOptions[i].StartPos.x));
                    ColOptions[i].StartPos = ColOptions[i].CurPos;
                    ColOptions[i].Width = Math.Max(26, ColOptions[i].Width);
                }


                curRect = new Rect(0.0f, 22.0f, 40.0f, 24.0f);

                // The rest of the rows
                for (var i = 0; i < RowsDisplay.Length; i++)
                {
                    if (i == 1)
                    {
                        // Could be type row
                        if (GUI.Button(curRect, UseTypeRow ? "Type" : "2", in_layout.CellTypeButton))
                        {
                            if (UseTypeRow == false)
                            {
                                if (EditorUtility.DisplayDialog(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_TYPEROWBOX_HEADER),
                                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_TYPEROWBOX_MESSAGE), "OK", "Cancel"))
                                {
                                    UseTypeRow = !UseTypeRow;
                                    UpdateCellTypes = true;
                                }
                            }
                            else
                            {
                                UseTypeRow = !UseTypeRow;
                                UpdateCellTypes = true;
                            }
                            
                        }
                        
                    }
                    else
                    {
                        // Row Number
                        GUI.Label(curRect, Convert.ToString(i + 1), in_layout.CellHeader);
                    }

                    // Cell Values

                    if (i == 1 && UseTypeRow)
                    {
                        for (var j = 0; j < RowsDisplay[i].Count; j++)
                        {
                            curRect.x += curRect.width;
                            curRect.width = ColOptions[j].Width;

                            var myCell = RowsDisplay[i][j];

                            if (myCell.MyType == SupportedType.Unrecognized)
                            {
                                myCell.SetTypeFromValue();
                            }

                            var cellType = myCell.MyType;
                            var curSelected = 0;
                            foreach (var guiContent in _ComboBoxList)
                            {
                                if (guiContent.text.Equals(Convert.ToString(cellType)))
                                    break;
                                curSelected++;
                            }
                            if (curSelected >= _ComboBoxList.Length)
                                curSelected = 0;

                            Google2uGUIUtil.ComboBox comboBoxControl;
                            if (!_ComboBoxes.ContainsKey(j))
                            {
                                comboBoxControl = new Google2uGUIUtil.ComboBox(curRect, _ComboBoxList[curSelected],
                                    _ComboBoxList, in_layout.CellTypeButton, in_layout.OuterBox, in_layout.CellHeader);
                                _ComboBoxes.Add(j, comboBoxControl);
                            }
                            else
                            {
                                comboBoxControl = _ComboBoxes[j];
                            }
                            comboBoxControl.width = curRect.width;
                            comboBoxControl.height = curRect.height;
                            comboBoxControl.x = curRect.x;
                            comboBoxControl.y = curRect.y;
                            var newSelected = comboBoxControl.Show();
                            if (newSelected != curSelected)
                            {
                                var newType =
                                    (SupportedType)
                                        Enum.Parse(typeof (SupportedType), _ComboBoxList[newSelected].text, true);
                                myCell.MyType = newType;
                                myCell.SetValueFromType();
                                UpdateCellTypes = true;
                                UpdateValidation = true;
                            }
                        }
                    }
                    else
                    {
                        for (var j = 0; j < RowsDisplay[i].Count; j++)
                        {
                            curRect.x += curRect.width;
                            curRect.width = ColOptions[j].Width;

                            if (curRect.x + curRect.width > _MyScrollPos.x && curRect.x < _MyScrollPos.x + Screen.width &&
                                curRect.y + curRect.height > _MyScrollPos.y && curRect.y < _MyScrollPos.y + scrollHeight)
                            {
                                if (i < 2 || i > 5 || !_ComboBoxes.ContainsKey(j) || _ComboBoxes[j].IsShown == false)
                                {
                                    var newCell = RowsDisplay[i][j].DrawGUI(in_layout, curRect, ActiveCell);
                                    if (newCell != null)
                                    {
                                        GUI.FocusControl("Blank");
                                        ActiveCell = newCell;
                                    }
                                }
                            }
                        }
                    }


                    curRect.x = 0.0f;
                    curRect.width = 40.0f;
                    curRect.y += curRect.height;
                }

                GUI.EndScrollView();
            }
        }

        public static void CalculateColumnWidth(int in_col, Google2uWorksheet in_worksheet)
        {
            var longest = 0;
            var longestString = string.Empty;
            foreach (var row in in_worksheet.RowsDisplay)
            {
                var cell = row[in_col];
                var len = cell.CellValueLength;
                if (len <= longest)
                    continue;

                longest = len;
                longestString = cell.CellValueString;
            }
            var strlen = GUIStyle.none.CalcSize(new GUIContent(longestString));
            in_worksheet.ColOptions[in_col].Width = Math.Max(26, (int) Math.Round(strlen.x + 0.5f) + 16);
        }

        public void ExportThread(object in_instance)
        {

            // Do all of our exporting here
            switch (WorksheetExportType)
            {

                case ExportType.CSV:
                {
                    Google2u.ExportCsv(this, _CSVPath, MyWorkbook.ExportOptions);
                    Google2u.Instance.InstanceData.Commands.Add(GFCommand.AssetDatabaseRefresh);
                break;
                }
                case ExportType.JSON:
                {
                    Google2u.ExportJson(this, _JSONPath, MyWorkbook.ExportOptions);
                    Google2u.Instance.InstanceData.Commands.Add(GFCommand.AssetDatabaseRefresh);
                }
                    break;
                case ExportType.NGUI:
                {
                    if (MyWorkbook.ExportOptions.NGUILegacyExport)
                        Google2u.ExportNGUILegacy(this, _NGUIPath, MyWorkbook.ExportOptions);
                    else
                        Google2u.ExportNGUI(this, _NGUIPath, MyWorkbook.ExportOptions);
                    Google2u.Instance.InstanceData.Commands.Add(GFCommand.AssetDatabaseRefresh);
                }
                    break;
                case ExportType.StaticDatabase:
                {
                    var respath = Path.Combine(_StaticDbPath, WorksheetName).Replace('\\', '/');
                    Google2u.ExportStaticDB(this, respath, WorksheetName, MyWorkbook.ExportOptions);
                    Google2u.Instance.InstanceData.Commands.Add(GFCommand.AssetDatabaseRefresh);
                }
                    break;
                case ExportType.XML:
                {
                    Google2u.ExportXML(this, _XMLPath, MyWorkbook.ExportOptions);
                    Google2u.Instance.InstanceData.Commands.Add(GFCommand.AssetDatabaseRefresh);
                }
                    break;
                case ExportType.ObjectDatabase:
                {
                    var respath = Path.Combine(_ObjdbResourcesPath, WorksheetName).Replace('\\', '/');
                    var edpath = Path.Combine(_ObjdbEditorPath, WorksheetName).Replace('\\', '/');
                    Google2u.ExportObjectDb(this, respath, edpath, _PlaymakerPath, MyWorkbook.ExportOptions);
                }
                    break;

                default:
                    return;
            }
        }

        public void DoExport()
        {

            switch (WorksheetExportType)
            {
                default:
                    return;
                case ExportType.CSV:
                    _CSVPath = Google2uGUIUtil.GetString("g2ucsvDirectory", Google2u.Google2uGenPath("CSV"));
                    break;
                case ExportType.JSON:
                    _JSONPath = Google2uGUIUtil.GetString("g2ujsonDirectory", Google2u.Google2uGenPath("JSON"));
                    break;
                case ExportType.NGUI:
                    _NGUIPath = Google2uGUIUtil.GetString("g2unguiDirectory", Google2u.Google2uGenPath("NGUI"));
                    break;
                case ExportType.StaticDatabase:
                    _StaticDbPath = Google2uGUIUtil.GetString("g2uStaticDBResourcesDirectory", Google2u.Google2uGenPath("STATICDBRESOURCES"));
                    break;
                case ExportType.XML:
                    _XMLPath = Google2uGUIUtil.GetString("g2uxmlDirectory", Google2u.Google2uGenPath("XML"));
                    break;
                case ExportType.ObjectDatabase:
                    _ObjdbResourcesPath = Google2uGUIUtil.GetString("g2uobjDBResourcesDirectory", Google2u.Google2uGenPath("OBJDBRESOURCES"));
                    _ObjdbEditorPath = Google2uGUIUtil.GetString("g2uobjDBEditorDirectory", Google2u.Google2uGenPath("OBJDBEDITOR"));
                    _PlaymakerPath = Google2uGUIUtil.GetString("g2uplaymakerDirectory", Google2u.Google2uGenPath("PLAYMAKER"));
                    break;


            }
            
            


            if (_ExportThread == null || _ExportThread.IsAlive == false)
            {
                _ExportThread = new Thread(ExportThread) {Name = "ExportThread"};
                _ExportThread.Start(this);
            }
        }

        internal void HighlightFirstInvalidCell()
        {
            UpdateValidation = true;
            _SetInvalidCellActive = true;
        }

        [Serializable]
        public class ColumnOption
        {
            public Rect ColumnRect;
            public Vector2 CurPos;
            public bool Dragging;
            public bool Expanded;
            public bool HasMouse;
            public Vector2 StartPos;
            public int Width;

            public ColumnOption(int in_width)
            {
                Width = in_width;
            }
        }

        public class RowOption
        {
            public int Height;

            public RowOption(int in_height)
            {
                Height = in_height;
            }
        }

        public class DataValidationParams
        {
            public Google2uExportOptions Options;
            public Google2uWorksheet Worksheet;

            public DataValidationParams(Google2uWorksheet in_worksheet, Google2uExportOptions in_options)
            {
                Worksheet = in_worksheet;
                Options = in_options;
            }
        }
    }
}