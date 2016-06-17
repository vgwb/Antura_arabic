// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.GData.Spreadsheets;
    using UnityEditor;
    using UnityEngine;

    #endregion

    [Serializable]
    public class Google2uCell
    {
        [SerializeField] private CellEntry _MyCell;

        public int ColNum;
        public SupportedType MyType = SupportedType.Unrecognized;
        public string NewValue;
        public int RowNum;
        public string SavedValueString;
        public bool SkipValidation = false;
        public string Tooltip = string.Empty;

        public Google2uCell(CellEntry in_entry)
        {
            if (in_entry == null)
                return;

            _MyCell = in_entry;
            RowNum = (int) _MyCell.Row;
            ColNum = (int) _MyCell.Column;
        }

        public CellEntry MyCell
        {
            get { return _MyCell; }
            set
            {
                _MyCell = value;
                SavedValueString = _MyCell.Value;
                if (RowNum == 2)
                {
                    SetTypeFromValue();
                }
            }
        }

        public string CellValueString
        {
            get
            {
                if (_MyCell == null && !string.IsNullOrEmpty(SavedValueString))
                    return SavedValueString;


                if (_MyCell == null || string.IsNullOrEmpty(_MyCell.Value))
                {

                    switch (MyType)
                    {
                        case SupportedType.Bool:
                            return "false";

                        case SupportedType.Color:
                        case SupportedType.Color32:
                            return "0,0,0";

                        case SupportedType.Vector2:
                            return "0,0";

                        case SupportedType.Vector3:
                            return "0,0,0";

                        case SupportedType.Quaternion:
                            return "0,0,0,0";

                        case SupportedType.Byte:
                        case SupportedType.Int:
                        case SupportedType.Float:
                            return "0";

                        default:
                            return string.Empty;
                    }
                }

                SavedValueString = _MyCell.Value;
                return _MyCell.Value;
            }
        }

        public int CellValueLength
        {
            get
            {
                if (_MyCell == null || string.IsNullOrEmpty(_MyCell.Value))
                    return 0;
                return _MyCell.Value.Length;
            }
        }

        public bool IsArrayType
        {
            get
            {
                return ((MyType == SupportedType.StringArray) ||
                        (MyType == SupportedType.FloatArray) ||
                        (MyType == SupportedType.IntArray) ||
                        (MyType == SupportedType.BoolArray) ||
                        (MyType == SupportedType.QuaternionArray) ||
                        (MyType == SupportedType.ColorArray) ||
                        (MyType == SupportedType.Color32Array) ||
                        (MyType == SupportedType.ByteArray) ||
                        (MyType == SupportedType.Vector2Array) ||
                        (MyType == SupportedType.Vector3Array));
            }
        }

        public string CellTypeString
        {
            get
            {
                switch (MyType)
                {
                    case SupportedType.Bool:
                        return "bool";
                    case SupportedType.BoolArray:
                        return "bool[]";
                    case SupportedType.String:
                        return "string";
                    case SupportedType.Int:
                        return "int";
                    case SupportedType.Float:
                        return "float";
                    case SupportedType.Byte:
                        return "byte";
                    case SupportedType.Vector2:
                        return "Vector2";
                    case SupportedType.Vector3:
                        return "Vector3";
                    case SupportedType.Color:
                        return "Color";
                    case SupportedType.Color32:
                        return "Color32";
                    case SupportedType.Quaternion:
                        return "Quaternion";
                    case SupportedType.FloatArray:
                        return "float[]";
                    case SupportedType.IntArray:
                        return "int[]";
                    case SupportedType.ByteArray:
                        return "byte[]";
                    case SupportedType.StringArray:
                        return "string[]";
                    case SupportedType.Vector2Array:
                        return "Vector2[]";
                    case SupportedType.Vector3Array:
                        return "Vector3[]";
                    case SupportedType.ColorArray:
                        return "Color[]";
                    case SupportedType.Color32Array:
                        return "Color32[]";
                    case SupportedType.QuaternionArray:
                        return "Quaternion[]";
                    case SupportedType.GameObject:
                        return "GameObject";
                    case SupportedType.Void:
                        return "void";
                }
                return "unsupported";
            }
        }

        public bool DrawCellValue(EditorGUILayoutEx in_layout)
        {
            var ret = false;
            EditorGUILayout.BeginHorizontal();

            if (!_MyCell.ReadOnly)
            {
                var oldVal = _MyCell.InputValue;
                if (NewValue == null)
                    NewValue = oldVal;
                NewValue = EditorGUILayout.TextField(NewValue);

                var content = new GUIContent(Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_UPDATE),
                    Google2u.LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_SYNC));
                if (GUILayout.Button(content, GUILayout.Width(100)))
                {
                    if (oldVal != NewValue)
                    {
                        _MyCell.InputValue = NewValue;
                        try
                        {
                            _MyCell.Update();
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Something went wrong. Try logging out and back in again: " + ex);
                        }

                        ret = true;
                    }
                }
            }
            else
            {
                var old = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.LabelField(_MyCell.Value);
                GUI.enabled = old;
            }
            EditorGUILayout.EndHorizontal();
            return ret;
        }

        public Google2uCell DrawGUI(EditorGUILayoutEx in_layout, Rect in_rect, Google2uCell in_activeEntry)
        {
            if (in_activeEntry == this)
            {
                GUI.Label(in_rect, _MyCell.Value, in_layout.CellButtonActive);
            }
            else if (!SkipValidation && !string.IsNullOrEmpty(Tooltip))
            {
                var content = new GUIContent(_MyCell.Value, Tooltip);
                if (GUI.Button(in_rect, content, in_layout.CellInvalidButton))
                {
                    return this;
                }
            }
            else
            {
                if (GUI.Button(in_rect, _MyCell.Value, in_layout.CellButton))
                {
                    return this;
                }
            }
            return null;
        }

        public void SetValueFromType()
        {
            NewValue = CellTypeString;

            if (MyCell.Value != NewValue)
            {
                try
                {
                    _MyCell.InputValue = NewValue;
                    _MyCell.Update();
                }
                catch (Exception ex)
                {
                    Debug.Log("Unable to sync: " + ex.Message);
                }
            }
        }

        public void SetTypeFromValue()
        {
            MyType = GetTypeFromValue();
        }

        public SupportedType GetTypeFromValue()
        {
            var ret = SupportedType.Void;

            var valueString = CellValueString;
            if (string.IsNullOrEmpty(valueString))
                return ret;

            // Always consider the ID column a string
            if (RowNum == 2 && ColNum == 1)
            {
                return SupportedType.String;
            }

            valueString = valueString.Replace(" ", string.Empty).ToLower();
            switch (valueString)
            {
                case "string":
                    ret = SupportedType.String;
                    break;
                case "stringarray":
                case "string[]":
                    ret = SupportedType.StringArray;
                    break;
                case "integer":
                case "int":
                    ret = SupportedType.Int;
                    break;
                case "integerarray":
                case "integer[]":
                case "intarray":
                case "int[]":
                    ret = SupportedType.IntArray;
                    break;
                case "boolean":
                case "bool":
                    ret = SupportedType.Bool;
                    break;
                case "booleanarray":
                case "boolarray":
                case "boolean[]":
                case "bool[]":
                    ret = SupportedType.BoolArray;
                    break;
                case "float":
                case "double":
                    ret = SupportedType.Float;
                    break;
                case "floatarray":
                case "float[]":
                case "doublearray":
                case "double[]":
                    ret = SupportedType.FloatArray;
                    break;
                case "byte":
                case "char":
                    ret = SupportedType.Byte;
                    break;
                case "bytearray":
                case "byte[]":
                    ret = SupportedType.ByteArray;
                    break;
                case "color":
                    ret = SupportedType.Color;
                    break;
                case "color32":
                    ret = SupportedType.Color32;
                    break;
                case "colorarray":
                case "color[]":
                    ret = SupportedType.ColorArray;
                    break;
                case "color32array":
                case "color32[]":
                    ret = SupportedType.Color32Array;
                    break;
                case "vector2":
                    ret = SupportedType.Vector2;
                    break;
                case "vector2array":
                case "vector2[]":
                    ret = SupportedType.Vector2Array;
                    break;
                case "vector":
                case "vector3":
                    ret = SupportedType.Vector3;
                    break;
                case "vectorarray":
                case "vector[]":
                case "vector3array":
                case "vector3[]":
                    ret = SupportedType.Vector3Array;
                    break;
                case "quaternion":
                case "quat":
                    ret = SupportedType.Quaternion;
                    break;
                case "quaternionarray":
                case "quatarray":
                case "quaternion[]":
                case "quat[]":
                    ret = SupportedType.QuaternionArray;
                    break;
                case "void":
                case "ignore":
                case "":
                    ret = SupportedType.Void;
                    break;
                case "gameobject":
                    ret = SupportedType.GameObject;
                    break;
                default:
                    ret = SupportedType.Unrecognized;
                    break;
            }
            return ret;
        }

        public bool DoDataValidation(CellType in_cellType, List<Google2uRow> in_cells, WorkbookBase in_base)
        {
            switch (in_cellType)
            {
                case CellType.Null:
                    return true;
                case CellType.ColumnHeader:
                    return ValidateColumnHeader(in_cells);
                case CellType.RowHeader:
                    return ValidateRowHeader(in_cells);
                case CellType.Type:
                    return ValidateTypeValue();
                case CellType.Value:
                    return ValidateValue(in_base);
            }
            return true;
        }

        private bool ValidateValue(WorkbookBase in_base)
        {
            var testType = MyType;
            if (RowNum == 1 || RowNum == 2)
                testType = SupportedType.String;

            Tooltip = string.Empty;
            switch (testType)
            {
                case SupportedType.GameObject:
                    try
                    {
                        Convert.ToString(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To GameObject";
                        return false;
                    }
                    break;
                case SupportedType.String:
                    try
                    {
                        Convert.ToString(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To String";
                        return false;
                    }
                    break;
                case SupportedType.Int:
                    try
                    {
                        Convert.ToInt32(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Int";
                        return false;
                    }
                    break;
                case SupportedType.Float:
                    try
                    {
                        Convert.ToSingle(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Float";
                        return false;
                    }
                    break;
                case SupportedType.Bool:
                    try
                    {
                        Convert.ToBoolean(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Bool. You must use TRUE or FALSE";
                        return false;
                    }
                    break;
                case SupportedType.Byte:
                    try
                    {
                        Convert.ToByte(CellValueString);
                    }
                    catch (Exception)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Byte";
                        return false;
                    }
                    break;
                case SupportedType.Vector2:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters] +
                                 " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 2)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Vector2";
                        return false;
                    }
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToSingle(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Vector3";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Vector3:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters] +
                                 " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 3)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Vector3";
                        return false;
                    }
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToSingle(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Vector3";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Color:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters] +
                                 " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 4)
                    {
                        if (split.Length != 3)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color";
                            return false;
                        }
                        foreach (var s in split)
                        {
                            try
                            {
                                Convert.ToSingle(s);
                            }
                            catch (Exception)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Color";
                                return false;
                            }
                        }
                    }
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToSingle(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Color32:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters] +
                                 " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 4)
                    {
                        if (split.Length != 3)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color32";
                            return false;
                        }
                        foreach (var s in split)
                        {
                            try
                            {
                                Convert.ToByte(s);
                            }
                            catch (Exception)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Color32";
                                return false;
                            }
                        }
                    }
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToByte(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color32";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Quaternion:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters] +
                                 " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 4)
                    {
                        Tooltip = "Unable to convert " + CellValueString + " To Quaternion";
                        return false;
                    }
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToSingle(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Quaternion";
                            return false;
                        }
                    }
                }
                    break;

                case SupportedType.FloatArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ArrayDelimiters] + " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToSingle(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Float Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.IntArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ArrayDelimiters] + " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToInt32(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Int Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.BoolArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ArrayDelimiters] + " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToBoolean(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString +
                                      " To Bool Array. You must use TRUE or FALSE";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.ByteArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ArrayDelimiters] + " ";
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToByte(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Byte Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.StringArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.StringArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            Convert.ToString(s);
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To String Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Vector2Array:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            var innerdelims =
                                in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters];
                            var innersplit = s.Split(innerdelims.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);

                            if (innersplit.Length != 2)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Vector2 Array";
                                return false;
                            }

                            foreach (var inners in innersplit)
                            {
                                Convert.ToSingle(inners);
                            }
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Vector2 Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Vector3Array:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            var innerdelims =
                                in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters];
                            var innersplit = s.Split(innerdelims.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);

                            if (innersplit.Length != 3)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Vector3 Array";
                                return false;
                            }

                            foreach (var inners in innersplit)
                            {
                                Convert.ToSingle(inners);
                            }
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Vector3 Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.ColorArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            var innerdelims =
                                in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters];
                            var innersplit = s.Split(innerdelims.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);

                            if (innersplit.Length != 4)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Color Array";
                                return false;
                            }

                            foreach (var inners in innersplit)
                            {
                                Convert.ToSingle(inners);
                            }
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.Color32Array:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            var innerdelims =
                                in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters];
                            var innersplit = s.Split(innerdelims.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);

                            if (innersplit.Length != 4)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Color32 Array";
                                return false;
                            }

                            foreach (var inners in innersplit)
                            {
                                Convert.ToByte(inners);
                            }
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Color32 Array";
                            return false;
                        }
                    }
                }
                    break;
                case SupportedType.QuaternionArray:
                {
                    var delims = in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexArrayDelimiters];
                    var split = CellValueString.Split(delims.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in split)
                    {
                        try
                        {
                            var innerdelims =
                                in_base.ExportOptions.DelimiterOptions[in_base.ExportOptions.ComplexTypeDelimiters];
                            var innersplit = s.Split(innerdelims.ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);
                            if (innersplit.Length != 4)
                            {
                                Tooltip = "Unable to convert " + CellValueString + " To Quaternion Array";
                                return false;
                            }

                            foreach (var inners in innersplit)
                            {
                                Convert.ToSingle(inners);
                            }
                        }
                        catch (Exception)
                        {
                            Tooltip = "Unable to convert " + CellValueString + " To Quaternion Array";
                            return false;
                        }
                    }
                }
                    break;

                default:
                    return true;
            }

            return true;
        }

        private bool ValidateTypeValue()
        {
            if (GetTypeFromValue() == SupportedType.Unrecognized)
            {
                Tooltip = "Unable to recognize Type value";
                return false;
            }
            return true;
        }

        private bool ValidateColumnHeader(List<Google2uRow> in_cells)
        {
            var val = MyCell.Value;
            if (val == null)
                return true; // blank column headers are ok, these are simply skipped columns
            if (!char.IsLetter(val[0]))
            {
                Tooltip = "Must begin with a Letter";
                return false;
            }

            if (!ValidStart(val))
            {
                Tooltip = "Must begin with a Letter";
                return false;
            }

            if (!ContainsInvalidChars(val))
            {
                Tooltip = "Must only contain Letters, Numbers, and Underscores";
                return false;
            }

            if (ContainsKeyword(val))
            {
                Tooltip = "Must not be a C# Keyword";
                return false;
            }

            var duplicates = (from cell in in_cells[0].Cells
                where (MyCell.Value.Equals(cell.CellValueString) &&
                       cell.RowNum != RowNum && cell.ColNum != ColNum)
                select cell.MyCell.Title.Text).ToList();

            if (duplicates.Count > 0)
            {
                Tooltip = "Duplicate Row Header found: ";
                foreach (var d in duplicates)
                {
                    Tooltip += d + " ";
                }
                return false;
            }
            return true;
        }

        private bool ValidateRowHeader(List<Google2uRow> in_cells)
        {
            var val = MyCell.Value;
            if (val == null)
                return true; // blank rows are ok, these are simply skipped rows
            if (!char.IsLetter(val[0]))
            {
                Tooltip = "Must begin with a Letter";
                return false;
            }

            if (!ValidStart(val))
            {
                Tooltip = "Must begin with a Letter";
                return false;
            }

            if (!ContainsInvalidChars(val))
            {
                Tooltip = "Must only contain Letters, Numbers, and Underscores";
                return false;
            }

            if (ContainsKeyword(val))
            {
                Tooltip = "Must not be a C# Keyword";
                return false;
            }

            var duplicates = (from row in in_cells
                where (row[0].CellValueString.Equals(CellValueString) &&
                       row[0].ColNum != ColNum && row[0].RowNum != RowNum)
                select row[0].MyCell.Title.Text).ToList();

            if (duplicates.Count > 0)
            {
                Tooltip = "Duplicate Row Header found: ";
                foreach (var d in duplicates)
                {
                    Tooltip += d + " ";
                }
                return false;
            }

            return true;
        }

        public static bool ValidStart(string in_string)
        {
            if (string.IsNullOrEmpty(in_string))
                return false;

            if (in_string.StartsWith("_") || char.IsLetter(in_string, 0))
                return true;

            return false;
        }

        public static bool ContainsInvalidChars(string in_string)
        {
            if (string.IsNullOrEmpty(in_string))
                return false;

            return
                !in_string.Any(
                    in_c => in_c.Equals('_') == false && char.IsLetter(in_c) == false && char.IsDigit(in_c) == false);
        }

        public static bool ContainsKeyword(string in_string)
        {
            string[] stringArray =
            {
                "abstract", "event", "new", "struct",
                "as", "explicit", "null", "switch",
                "base", "extern", "object", "this",
                "bool", "false", "operator", "throw",
                "break", "finally", "out", "true",
                "byte", "fixed", "override", "try",
                "case", "float", "params", "typeof",
                "catch", "for", "private", "uint",
                "char", "foreach", "protected", "ulong",
                "checked", "goto", "public", "unchecked",
                "class", "if", "readonly", "unsafe",
                "const", "implicit", "ref", "ushort",
                "continue", "in", "return", "using",
                "decimal", "int", "sbyte", "virtual",
                "default", "interface", "sealed", "volatile",
                "delegate", "internal", "short",
                "do", "sizeof", "while",
                "double", "lock", "stackalloc",
                "else", "long", "static",
                "enum", "namespace", "string"
            };

            return stringArray.Any(in_x => in_x.Equals(in_string));
        }
    }
}