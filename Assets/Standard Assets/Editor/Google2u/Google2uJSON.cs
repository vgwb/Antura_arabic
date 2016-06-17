// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Text;
    using System.IO;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static bool IsSupportedArrayType(SupportedType in_type)
        {
            switch (in_type)
            {
                case SupportedType.FloatArray:
                case SupportedType.IntArray:
                case SupportedType.BoolArray:
                case SupportedType.ByteArray:
                case SupportedType.StringArray:
                case SupportedType.Vector2Array:
                case SupportedType.Vector3Array:
                case SupportedType.ColorArray:
                case SupportedType.Color32Array:
                case SupportedType.QuaternionArray:
                    return true;
            }
            return false;
        }

        public static bool IsBasicArrayType(SupportedType in_type)
        {
            switch (in_type)
            {
                case SupportedType.FloatArray:
                case SupportedType.IntArray:
                case SupportedType.BoolArray:
                case SupportedType.ByteArray:
                case SupportedType.StringArray:
                    return true;
            }
            return false;
        }

        public static bool IsComplexType(SupportedType in_type)
        {
            switch (in_type)
            {
                case SupportedType.Vector2:
                case SupportedType.Vector3:
                case SupportedType.Color:
                case SupportedType.Color32:
                case SupportedType.Quaternion:
                    return true;
            }
            return false;
        }

        public static bool IsComplexArrayType(SupportedType in_type)
        {
            switch (in_type)
            {
                case SupportedType.Vector2Array:
                case SupportedType.Vector3Array:
                case SupportedType.ColorArray:
                case SupportedType.Color32Array:
                case SupportedType.QuaternionArray:
                    return true;
            }
            return false;
        }

        private static string SanitizeJson(string in_value, bool in_escapeUnicode)
        {
            var sb = new StringBuilder();
            foreach (var c in in_value)
            {
                if ((c > 127) && (in_escapeUnicode))
                {
                    // change this character into a unicode escape
                    var encodedValue = "\\u" + ((int) c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    switch (c)
                    {
                        case '\n':
                            sb.Append("\\n");
                            break;
                        case '\r':
                            sb.Append("\\r");
                            break;
                        case '\t':
                            sb.Append("\\t");
                            break;
                        case '\b':
                            sb.Append("\\b");
                            break;
                        case '\a':
                            sb.Append("\\a");
                            break;
                        case '\f':
                            sb.Append("\\f");
                            break;
                        case '\\':
                            sb.Append("\\\\");
                            break;
                        case '\"':
                            sb.Append("\\\"");
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        public static string ExportJsonObjectClassString(Google2uWorksheet in_sheet, Google2uExportOptions in_options)
        {
            return ExportJsonObjectClassString(in_sheet, in_options, 0);
        }

        public static string ExportJsonObjectClassString(Google2uWorksheet in_sheet, Google2uExportOptions in_options,
            int in_indent)
        {
            var retString = string.Empty;

            if (in_sheet.Rows.Count <= 0)
                return retString;

            var headerRow = in_sheet.Rows[0];

            var indent = in_indent;

            retString += FormatLine("//----------------------------------------------");
            retString += FormatLine("//    Google2u: Google Doc Unity integration");
            retString += FormatLine("//         Copyright © 2015 Litteratus");
            retString += FormatLine("//");
            retString += FormatLine("//        This file has been auto-generated");
            retString += FormatLine("//              Do not manually edit");
            retString += FormatLine("//----------------------------------------------");
            retString += FormatLine(string.Empty);
            retString += FormatLine("using System.Collections.Generic;");
            retString += FormatLine("using UnityEngine;");
            retString += FormatLine(string.Empty);
            retString += FormatLine("namespace Google2u");
            retString += FormatLine("{");

            indent++;

            retString += Indent(indent, "public class " + in_sheet.WorksheetName + "Row" + Environment.NewLine);
            retString += Indent(indent, "{" + Environment.NewLine);

            indent++;

            var idCell = true;
            foreach (var cell in headerRow.Cells)
            {
                if (!idCell && (cell.MyType == SupportedType.Void || cell.MyType == SupportedType.Unrecognized))
                    // Don't process rows or columns marked for ignore
                {
                    if (in_options.JSONCullColumns)
                        break;
                    continue;
                }

                retString += Indent(indent, "");

                if (idCell)
                {
                    retString += "public string " + cell.CellValueString + " { get; set; }" + Environment.NewLine;
                }
                else
                {
                    // check the type
                    switch (cell.MyType)
                    {
                        case SupportedType.GameObject:
                        case SupportedType.String:
                            retString += "public string " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.Int:
                            retString += "public int " + cell.CellValueString + " { get; set; }" + Environment.NewLine;
                            break;
                        case SupportedType.Float:
                            retString += "public float " + cell.CellValueString + " { get; set; }" + Environment.NewLine;
                            break;
                        case SupportedType.Bool:
                            retString += "public bool " + cell.CellValueString + " { get; set; }" + Environment.NewLine;
                            break;
                        case SupportedType.Byte:
                            retString += "public byte  " + cell.CellValueString + " { get; set; }" + Environment.NewLine;
                            break;
                        case SupportedType.Vector2:
                            retString += "public List<float> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString += "public Vector2 " + cell.CellValueString +
                                         "_Vector2 { get { return new Vector2(" + cell.CellValueString + "[0], " +
                                         cell.CellValueString + "[1]); } }" + Environment.NewLine;
                            break;
                        case SupportedType.Vector3:
                            retString += "public List<float> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString += "public Vector3 " + cell.CellValueString +
                                         "_Vector3 { get { return new Vector3(" + cell.CellValueString + "[0], " +
                                         cell.CellValueString + "[1], " + cell.CellValueString + "[2]); } }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.Color:
                            retString += "public List<float> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString += "public Color " + cell.CellValueString + "_Color { get { return new Color(" +
                                         cell.CellValueString + "[0], " + cell.CellValueString + "[1], " +
                                         cell.CellValueString + "[2], " + cell.CellValueString + "[3]); } }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.Color32:
                            retString += "public List<int> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString += "public Color32 " + cell.CellValueString +
                                         "_Color32 { get { return new Color((byte)" + cell.CellValueString +
                                         "[0], (byte)" + cell.CellValueString + "[1], (byte)" + cell.CellValueString +
                                         "[2], (byte)" + cell.CellValueString + "[3]); } }" + Environment.NewLine;
                            break;
                        case SupportedType.Quaternion:
                            retString += "public List<float> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString += "public Quaternion " + cell.CellValueString +
                                         "_Quaternion { get { return new Color(" + cell.CellValueString + "[0], " +
                                         cell.CellValueString + "[1], " + cell.CellValueString + "[2], " +
                                         cell.CellValueString + "[3]); } }" + Environment.NewLine;
                            break;
                        case SupportedType.FloatArray:
                            retString += "public List<float> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.IntArray:
                            retString += "public List<int> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.BoolArray:
                            retString += "public List<bool> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.ByteArray:
                            retString += "public List<byte> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.StringArray:
                            retString += "public List<string> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            break;
                        case SupportedType.Vector2Array:
                            retString += "public List<List<float>> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString +=
                                string.Format(
                                    "public Vector2[] {0}_Vector2Array {{get {{var len = {0}.Count; var ret = new Vector2[len]; for (var i = 0; i < len; ++i) {{ ret[i] = new Vector2({0}[i][0], {0}[i][1]); }} return ret; }} }}",
                                    cell.CellValueString);
                            break;
                        case SupportedType.Vector3Array:
                            retString += "public List<List<float>>  " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString +=
                                string.Format(
                                    "public Vector3[] {0}_Vector3Array {{get {{var len = {0}.Count; var ret = new Vector3[len]; for (var i = 0; i < len; ++i) {{ ret[i] = new Vector3({0}[i][0], {0}[i][1], {0}[i][2]); }} return ret; }} }}",
                                    cell.CellValueString);
                            break;
                        case SupportedType.ColorArray:
                            retString += "public List<List<float>> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString +=
                                string.Format(
                                    "public Color[] {0}_ColorArray {{get {{var len = {0}.Count; var ret = new Color[len]; for (var i = 0; i < len; ++i) {{ ret[i] = new Color({0}[i][0], {0}[i][1], {0}[i][2], {0}[i][3]); }} return ret; }} }}",
                                    cell.CellValueString);
                            break;
                        case SupportedType.Color32Array:
                            retString += "public List<List<float>> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString +=
                                string.Format(
                                    "public Color32[] {0}_Color32Array {{get {{var len = {0}.Count; var ret = new Color32[len]; for (var i = 0; i < len; ++i) {{ ret[i] = new Color32({0}[i][0], {0}[i][1], {0}[i][2], {0}[i][3]); }} return ret; }} }}",
                                    cell.CellValueString);
                            break;
                        case SupportedType.QuaternionArray:
                            retString += "public List<List<float>> " + cell.CellValueString + " { get; set; }" +
                                         Environment.NewLine;
                            retString += Indent(indent, "");
                            retString +=
                                string.Format(
                                    "public Quaternion[] {0}_QuaternionArray {{get {{var len = {0}.Count; var ret = new Quaternion[len]; for (var i = 0; i < len; ++i) {{ ret[i] = new Quaternion({0}[i][0], {0}[i][1], {0}[i][2], {0}[i][3]); }} return ret; }} }}",
                                    cell.CellValueString);
                            break;
                    }
                }
                idCell = false;
            }

            indent--;

            retString += Indent(indent, "}" + Environment.NewLine);

            if (in_options.JSONExportType == Google2uExportOptions.ExportType.ExportObject)
            {
                retString += Environment.NewLine;
                retString += Indent(indent, "public class " + in_sheet.WorksheetName + "Database" + Environment.NewLine);
                retString += Indent(indent, "{" + Environment.NewLine);

                indent++;
                retString += Indent(indent,
                    "public List< " + in_sheet.WorksheetName + "Row > " + in_sheet.WorksheetName + "Row { get; set; }" +
                    Environment.NewLine);
                indent--;

                retString += Indent(indent, "}" + Environment.NewLine);
            }

            retString += FormatLine("}");

            return retString;
        }

        public static string ExportJsonObjectString(Google2uWorksheet in_sheet, Google2uExportOptions in_options,
            bool in_newlines)
        {
            var indent = 0;
            var retString = string.Empty;
            var escapeUnicode = in_options.EscapeUnicode;
            var bConvertArrays = !in_options.JSONCellArrayToString;

            if (in_options.JSONExportPretty)
                in_newlines = true;

            if (in_options.JSONExportType == Google2uExportOptions.ExportType.ExportObject)
            {
                retString += Indent(indent, "{");

                if (in_newlines)
                {
                    retString += Environment.NewLine;
                    indent++;
                }

                retString += Indent(indent, ("\"" + SanitizeJson(in_sheet.WorksheetName, escapeUnicode) + "Row\":"));
                // "sheetName":

                if (in_newlines)
                {
                    retString += Environment.NewLine;
                }
            }

            retString += Indent(indent, "["); // [

            if (in_newlines)
            {
                retString += Environment.NewLine;
                indent++;
            }

            var rowCt = in_sheet.Rows.Count;
            if (rowCt > 0)
            {
                var curRow = 0;
                var validRow = false;

                // Iterate through each row, printing its cell values.
                foreach (var row in in_sheet.Rows)
                {
                    // if we are skipping the type row, record the types and increment curRow now
                    if (curRow == 0 || (curRow == 1 && in_sheet.UseTypeRow))
                    {
                        curRow++;
                        continue;
                    }

                    var rowType = row[0].GetTypeFromValue();
                    var rowHeader = row[0].CellValueString;
                    if (string.IsNullOrEmpty(rowHeader))
                        // if this header is empty
                    {
                        if (in_options.JSONCullRows)
                            break;
                        curRow++;
                        continue;
                    }

                    if (rowType == SupportedType.Void ||
                        rowHeader.Equals("void", StringComparison.InvariantCultureIgnoreCase))
                        // if this cell is void, then skip the row completely
                    {
                        curRow++;
                        continue;
                    }

                    if (validRow)
                    {
                        retString += ",";
                        if (in_newlines)
                        {
                            retString += Environment.NewLine;
                        }
                    }

                    validRow = true;

                    retString += Indent(indent, "{");
                    if (in_newlines)
                    {
                        retString += Environment.NewLine;
                        indent++;
                    }

                    var firstCell = true;
                    // Iterate over the remaining columns, and print each cell value
                    for (var i = 0; i < in_sheet.Rows[0].Count; i++)
                    {
                        // Don't process rows or columns marked for ignore
                        if ((row[i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(row[0].CellValueString) ||
                             in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(in_sheet.Rows[0][i].CellValueString) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase) ||
                             (in_options.JSONCullColumns && i >= in_sheet.FirstBlankCol) ||
                             (in_options.JSONIgnoreIDColumn && i == 0)))
                        {
                            continue;
                        }

                        if (firstCell)
                            firstCell = false;
                        else
                        {
                            retString += ", ";
                            if (in_newlines)
                                retString += Environment.NewLine;
                        }

                        var myType = in_sheet.Rows[0].Cells[i].MyType;

                        if (IsComplexType(myType))
                        {
                            retString += Indent(indent,
                                "\"" + SanitizeJson(in_sheet.Rows[0][i].CellValueString, escapeUnicode) + "\":");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            retString += Indent(indent, "[");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            var complexTypeDelim =
                                in_options.DelimiterOptions[in_options.ComplexTypeDelimiters].ToCharArray();
                            var value = row[i].CellValueString.Split(complexTypeDelim,
                                StringSplitOptions.RemoveEmptyEntries);
                            var ct = 0;
                            foreach (var s in value)
                            {
                                retString += Indent(indent, SanitizeJson(s, escapeUnicode));

                                if (ct < value.Length - 1)
                                {
                                    retString += ",";
                                    if (in_newlines)
                                    {
                                        retString += Environment.NewLine;
                                    }
                                }
                                ct++;
                            }

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent--;
                            }
                            retString += Indent(indent, "]");

                            if (in_newlines)
                            {
                                indent--;
                            }
                        }
                        else if (IsComplexArrayType(myType))
                        {
                            retString += Indent(indent,
                                "\"" + SanitizeJson(in_sheet.Rows[0][i].CellValueString, escapeUnicode) + "\":");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            retString += Indent(indent, "[");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            var complexArrayDelim =
                                in_options.DelimiterOptions[in_options.ComplexArrayDelimiters].ToCharArray();
                            var complexArray = row[i].CellValueString.Split(complexArrayDelim,
                                StringSplitOptions.RemoveEmptyEntries);
                            var ctArray = 0;
                            foreach (var cv in complexArray)
                            {
                                if (in_newlines)
                                {
                                    retString += Environment.NewLine;
                                    indent++;
                                }

                                retString += Indent(indent, "[");

                                if (in_newlines)
                                {
                                    retString += Environment.NewLine;
                                    indent++;
                                }

                                var complexTypeDelim =
                                    in_options.DelimiterOptions[in_options.ComplexTypeDelimiters].ToCharArray();
                                var complexType = cv.Split(complexTypeDelim, StringSplitOptions.RemoveEmptyEntries);
                                var ctValue = 0;
                                foreach (var s in complexType)
                                {
                                    retString += Indent(indent, SanitizeJson(s, escapeUnicode));

                                    if (ctValue < complexType.Length - 1)
                                    {
                                        retString += ",";
                                        if (in_newlines)
                                        {
                                            retString += Environment.NewLine;
                                        }
                                    }
                                    ctValue++;
                                }

                                if (in_newlines)
                                {
                                    retString += Environment.NewLine;
                                    indent--;
                                }
                                retString += Indent(indent, "]");

                                if (in_newlines)
                                {
                                    indent--;
                                }

                                if (ctArray < complexArray.Length - 1)
                                {
                                    retString += ",";
                                    if (in_newlines)
                                    {
                                        retString += Environment.NewLine;
                                    }
                                }
                                ctArray++;
                            }

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent--;
                            }
                            retString += Indent(indent, "]");

                            if (in_newlines)
                            {
                                indent--;
                            }
                        }
                        else if (bConvertArrays && IsSupportedArrayType(myType))
                        {
                            var delim = in_options.DelimiterOptions[in_options.ArrayDelimiters].ToCharArray();

                            retString += Indent(indent,
                                "\"" + SanitizeJson(in_sheet.Rows[0][i].CellValueString, escapeUnicode) + "\":");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            retString += Indent(indent, "[");

                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent++;
                            }

                            var isString = false;

                            if (row[i].MyType == SupportedType.StringArray)
                            {
                                delim = in_options.DelimiterOptions[in_options.StringArrayDelimiters].ToCharArray();
                                isString = true;
                            }
                            if (i == 0)
                                isString = true;

                            var value = row[i].CellValueString.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                            var ct = 0;
                            foreach (var s in value)
                            {
                                if (isString)
                                {
                                    retString += Indent(indent, "\"" + SanitizeJson(s, escapeUnicode) + "\"");
                                }
                                else if (in_sheet.Rows[0].Cells[i].MyType == SupportedType.BoolArray)
                                {
                                    var val = s.ToLower();
                                    if (val == "1")
                                        val = "true";
                                    if (val == "0")
                                        val = "false";
                                    retString += Indent(indent, SanitizeJson(val, escapeUnicode));
                                }
                                else
                                    retString += Indent(indent, SanitizeJson(s, escapeUnicode));

                                if (ct < value.Length - 1)
                                {
                                    retString += ",";
                                    if (in_newlines)
                                    {
                                        retString += Environment.NewLine;
                                    }
                                }
                                ct++;
                            }
                            if (in_newlines)
                            {
                                retString += Environment.NewLine;
                                indent--;
                            }
                            retString += Indent(indent, "]");

                            if (in_newlines)
                            {
                                indent--;
                            }
                        }
                        else
                        {
                            if (in_sheet.UseTypeRow == false ||
                                in_sheet.Rows[0].Cells[i].MyType == SupportedType.String || (i == 0))
                            {
                                retString += Indent(indent, "\"" +
                                                            SanitizeJson(in_sheet.Rows[0][i].CellValueString,
                                                                escapeUnicode) +
                                                            "\":\"" +
                                                            SanitizeJson(row[i].CellValueString, escapeUnicode) + "\"");
                            }
                            else if (in_sheet.Rows[0].Cells[i].MyType == SupportedType.Bool)
                            {
                                var val = row[i].CellValueString.ToLower();
                                if (val == "1")
                                    val = "true";
                                if (val == "0")
                                    val = "false";
                                retString += Indent(indent, "\"" +
                                                            SanitizeJson(in_sheet.Rows[0][i].CellValueString,
                                                                escapeUnicode) +
                                                            "\":" +
                                                            SanitizeJson(val, escapeUnicode));
                            }

                            else if (in_sheet.Rows[0].Cells[i].MyType == SupportedType.BoolArray)
                            {
                                var val = row[i].CellValueString.ToLower();
                                if (val == "1")
                                    val = "true";
                                if (val == "0")
                                    val = "false";
                                retString += Indent(indent, "\"" +
                                                            SanitizeJson(in_sheet.Rows[0][i].CellValueString,
                                                                escapeUnicode) +
                                                            "\":" +
                                                            SanitizeJson(val, escapeUnicode));
                            }
                            else
                                retString += Indent(indent, "\"" +
                                                            SanitizeJson(in_sheet.Rows[0][i].CellValueString,
                                                                escapeUnicode) +
                                                            "\":" +
                                                            SanitizeJson(row[i].CellValueString, escapeUnicode) + "");
                        }

                        if (in_newlines)
                        {
                            //retString += Environment.NewLine;
                        }
                    }

                    if (in_newlines)
                    {
                        retString += Environment.NewLine;
                        indent--;
                    }

                    retString += Indent(indent, "}");

                    curRow++;
                }
            }

            if (in_newlines)
            {
                retString += Environment.NewLine;
                indent--;
            }
            retString += Indent(indent, "]");

            if (in_options.JSONExportType == Google2uExportOptions.ExportType.ExportObject)
            {
                if (in_newlines)
                {
                    retString += Environment.NewLine;
                    indent--;
                }
                retString += Indent(indent, "}");
            }
            return retString;
        }

        public static void ExportJson(Google2uWorksheet in_sheet, string in_path, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, in_sheet.WorksheetName);

            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            var jsonPath = Path.Combine(in_path, in_sheet.WorksheetName + ".json").Replace('\\', '/');

            using (
                var fs = File.Open(jsonPath,
                    File.Exists(jsonPath) ? FileMode.Truncate : FileMode.OpenOrCreate,
                    FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = ExportJsonObjectString(in_sheet, in_options, false);
                    sw.Write(fileString);
                    sw.Flush();
                }
            }

            if (in_options.JSONExportClass)
            {
                var jsonClassDir = in_path + "\\Resources";
                if (!Directory.Exists(jsonClassDir))
                    Directory.CreateDirectory(jsonClassDir);

                var jsonClassPath = Path.Combine(jsonClassDir, in_sheet.WorksheetName + ".cs").Replace('\\', '/');

                using (
                    var fs = File.Open(jsonClassPath,
                        File.Exists(jsonClassPath) ? FileMode.Truncate : FileMode.OpenOrCreate,
                        FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        var fileString = ExportJsonObjectClassString(in_sheet, in_options);
                        sw.Write(fileString);
                        sw.Flush();
                    }
                }
            }

            PushNotification("Saving to: " + in_path);
        }
    }
}