// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    #endregion

    [Serializable]
    public class Google2uExportOptions
    {
        private readonly string _Prefix;

        #region Legacy

        public bool LowercaseHeader;

        #endregion

        public Google2uExportOptions(string in_prefix)
        {
            _Prefix = in_prefix;

            LowercaseHeader = Google2uGUIUtil.GetBool(in_prefix + "LowercaseHeader", LowercaseHeader);

            TrimStrings = Google2uGUIUtil.GetBool(in_prefix + "TrimStrings", TrimStrings);
            TrimStringArrays = Google2uGUIUtil.GetBool(in_prefix + "TrimStringArrays", TrimStringArrays);

            ArrayDelimiters = Google2uGUIUtil.GetInt(in_prefix + "ArrayDelimiters", ArrayDelimiters);
            StringArrayDelimiters = Google2uGUIUtil.GetInt(in_prefix + "StringArrayDelimiters", StringArrayDelimiters);
            ComplexTypeDelimiters = Google2uGUIUtil.GetInt(in_prefix + "ComplexTypeDelimiters", ComplexTypeDelimiters);
            ComplexArrayDelimiters = Google2uGUIUtil.GetInt(in_prefix + "ComplexArrayDelimiters", ComplexArrayDelimiters);

            PrependUnderscoreToVariableNames = Google2uGUIUtil.GetBool(in_prefix + "PrependUnderscoreToVariableNames", PrependUnderscoreToVariableNames);

            #region ObjectDatabase Options

            var dbObjName = Google2uGUIUtil.GetString(_Prefix + "GameObjectDatabaseName", string.Empty);
            if (string.IsNullOrEmpty(dbObjName) == false)
            {
                var go = GameObject.Find(dbObjName);
                if (go)
                {
                    ExportDatabaseGameObjectName = dbObjName;
                    _ExportDatabaseGameObject = go;
                }
            }


            _OverrideObjectDatabaseNames = new Dictionary<string, string>();


            var tmpOverrides = Google2uGUIUtil.GetString(_Prefix + "OverrideObjectDatabaseNames",
                string.Empty);
            var tmpOverrideSplit = tmpOverrides.Split(',');
            foreach (var s in tmpOverrideSplit)
            {
                var sSplit = s.Split('|');
                if (sSplit.Length == 2)
                    _OverrideObjectDatabaseNames.Add(sSplit[0], sSplit[1]);
            }

            GeneratePlaymakerActions = Google2uGUIUtil.GetBool(in_prefix + "GeneratePlaymakerActions",
                GeneratePlaymakerActions);
            UseDoNotDestroy = Google2uGUIUtil.GetBool(in_prefix + "UseDoNotDestroy", UseDoNotDestroy);
            ObjectDBCullColumns = Google2uGUIUtil.GetBool(in_prefix + "ObjectDBCullColumns", ObjectDBCullColumns);
            ObjectDBCullRows = Google2uGUIUtil.GetBool(in_prefix + "ObjectDBCullRows", ObjectDBCullRows);

            #endregion

            #region Static DB Options

            StaticDBCullColumns = Google2uGUIUtil.GetBool(in_prefix + "StaticDBCullColumns", StaticDBCullColumns);
            StaticDBCullRows = Google2uGUIUtil.GetBool(in_prefix + "StaticDBCullRows", StaticDBCullRows);

            #endregion

            #region JSON Options

            EscapeUnicode = Google2uGUIUtil.GetBool(in_prefix + "EscapeUnicode", EscapeUnicode);
            JSONCellArrayToString = Google2uGUIUtil.GetBool(in_prefix + "JSONCellArrayToString", JSONCellArrayToString);
            JSONExportClass = Google2uGUIUtil.GetBool(in_prefix + "JSONExportClass", JSONExportClass);
            JSONExportType = Google2uGUIUtil.GetEnum(in_prefix + "JSONExportType", JSONExportType);
            JSONCullColumns = Google2uGUIUtil.GetBool(in_prefix + "JSONCullColumns", JSONCullColumns);
            JSONCullRows = Google2uGUIUtil.GetBool(in_prefix + "JSONCullRows", JSONCullRows);
            JSONIgnoreIDColumn = Google2uGUIUtil.GetBool(in_prefix + "JSONIgnoreIDColumn", JSONIgnoreIDColumn);
            JSONExportPretty = Google2uGUIUtil.GetBool(in_prefix + "JSONExportPretty", JSONExportPretty);
            #endregion

            #region XML Options

            XMLColsAsChildTags = Google2uGUIUtil.GetBool(in_prefix + "XMLColsAsChildTags", XMLColsAsChildTags);
            XMLCellArrayToString = Google2uGUIUtil.GetBool(in_prefix + "XMLCellArrayToString", XMLCellArrayToString);
            XMLCullColumns = Google2uGUIUtil.GetBool(in_prefix + "XMLCullColumns", XMLCullColumns);
            XMLCullRows = Google2uGUIUtil.GetBool(in_prefix + "XMLCullRows", XMLCullRows);

            #endregion

            #region CSV Options

            EscapeCSVStrings = Google2uGUIUtil.GetBool(in_prefix + "EscapeCSVStrings", EscapeCSVStrings);
            CSVCullColumns = Google2uGUIUtil.GetBool(in_prefix + "CSVCullColumns", CSVCullColumns);
            CSVCullRows = Google2uGUIUtil.GetBool(in_prefix + "CSVCullRows", CSVCullRows);
            CSVConvertLineBreaks = Google2uGUIUtil.GetBool(in_prefix + "CSVConvertLineBreaks", CSVConvertLineBreaks);

            #endregion

            #region NGUI Options

            EscapeNGUIStrings = Google2uGUIUtil.GetBool(in_prefix + "EscapeNGUIStrings", EscapeNGUIStrings);
            NGUICullColumns = Google2uGUIUtil.GetBool(in_prefix + "NGUICullColumns", NGUICullColumns);
            NGUICullRows = Google2uGUIUtil.GetBool(in_prefix + "NGUICullRows", NGUICullRows);
            NGUIConvertLineBreaks = Google2uGUIUtil.GetBool(in_prefix + "NGUIConvertLineBreaks", NGUIConvertLineBreaks);
            NGUILegacyExport = Google2uGUIUtil.GetBool(in_prefix + "NGUILegacyExport", NGUILegacyExport);

            #endregion
        }

        public string GetOverrideObjectDatabaseGameObjectName(string in_sheetName)
        {
            if (_OverrideObjectDatabaseNames.Count == 0)
                return string.Empty;

            var sheetName = in_sheetName.Replace(",", string.Empty);
            if (_OverrideObjectDatabaseNames.ContainsKey(_Prefix + sheetName))
                return _OverrideObjectDatabaseNames[_Prefix + sheetName];
            return string.Empty;
        }

        public GameObject GetOverrideObjectDatabaseGameObject(string in_sheetName)
        {
            if (_OverrideObjectDatabaseNames.Count == 0)
                return null;

            var findVar = GetOverrideObjectDatabaseGameObjectName(in_sheetName);

            return string.IsNullOrEmpty(findVar) ? null : GameObject.Find(findVar);
        }

        public void SetOverrideObjectDatabaseGameObject(string in_sheetName, GameObject in_gameObject)
        {
            var sheetName = in_sheetName.Replace(",", string.Empty);
            var goName = string.Empty;
            if (in_gameObject != null)
                goName = in_gameObject.name;

            if (!_OverrideObjectDatabaseNames.ContainsKey(_Prefix + sheetName))
                _OverrideObjectDatabaseNames.Add(_Prefix + sheetName, goName);
            _OverrideObjectDatabaseNames[_Prefix + sheetName] = goName;

            var namesString = string.Empty;
            var index = 0;
            foreach (var pair in _OverrideObjectDatabaseNames)
            {
                namesString += pair.Key + "|" + pair.Value;
                if (index < _OverrideObjectDatabaseNames.Count - 1)
                    namesString += ",";
                index++;
            }

            Google2uGUIUtil.SetString(_Prefix + "OverrideObjectDatabaseNames", namesString);
        }

        #region Whitespace

        public bool TrimStrings = true;
        public bool TrimStringArrays = true;

        #endregion

        #region Delimiters

        public string[] DelimiterOptionStrings = {", - Comma", "| - Pipe", "  - Space"};
        public string[] DelimiterOptions = {",", "|", " "};

        public int ArrayDelimiters;
        public int StringArrayDelimiters = 1;
        public int ComplexTypeDelimiters;
        public int ComplexArrayDelimiters = 1;

        #endregion

        #region Code Generation

        public bool PrependUnderscoreToVariableNames = true;

        #endregion

        #region ObjectDatabase Options

        [SerializeField] private GameObject _ExportDatabaseGameObject;

        public string ExportDatabaseGameObjectName;
        public bool ObjectDBCullColumns;
        public bool ObjectDBCullRows;

        public GameObject ExportDatabaseGameObject
        {
            get
            {
                if (string.IsNullOrEmpty(ExportDatabaseGameObjectName))
                    return null;

                var go = GameObject.Find(ExportDatabaseGameObjectName);
                if (go != null)
                {
                    _ExportDatabaseGameObject = go;
                }
                return _ExportDatabaseGameObject;
            }
            set
            {
                if (value == null)
                {
                    ExportDatabaseGameObjectName = string.Empty;
                    Google2uGUIUtil.SetString(_Prefix + "GameObjectDatabaseName", string.Empty);
                    _ExportDatabaseGameObject = null;
                }
                else
                {
                    ExportDatabaseGameObjectName = value.name;
                    Google2uGUIUtil.SetString(_Prefix + "GameObjectDatabaseName", value.name);
                    _ExportDatabaseGameObject = value;
                }
            }
        }

        private readonly Dictionary<string, string> _OverrideObjectDatabaseNames;

        public bool GeneratePlaymakerActions;
        public bool UseDoNotDestroy;

        #endregion

        #region StaticDatabase Options

        public bool StaticDBCullColumns;
        public bool StaticDBCullRows;

        #endregion

        #region JSON Options

        public enum ExportType
        {
            ExportObject,
            ExportArray
        }

        public bool EscapeUnicode;
        public bool JSONCellArrayToString;
        public bool JSONExportClass;
        public ExportType JSONExportType = ExportType.ExportObject;
        public bool JSONCullColumns;
        public bool JSONCullRows;
        public bool JSONIgnoreIDColumn;
        public bool JSONExportPretty;

        #endregion

        #region XML Options

        public bool XMLColsAsChildTags = true;
        public bool XMLCellArrayToString;
        public bool XMLCullColumns;
        public bool XMLCullRows;

        #endregion

        #region CSV Options

        public bool EscapeCSVStrings;
        public bool CSVCullColumns;
        public bool CSVCullRows;
        public bool CSVConvertLineBreaks;

        #endregion

        #region NGUI Options

        public bool EscapeNGUIStrings;
        public bool NGUICullColumns;
        public bool NGUICullRows;
        public bool NGUIConvertLineBreaks;
        public bool NGUILegacyExport;

        #endregion
    }
}