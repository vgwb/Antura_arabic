// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.IO;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static string AutoQuote(string in_string)
        {
            var doAutoQuote = in_string.Contains(",") || in_string.Contains("\"") ||
                              in_string.Contains(Environment.NewLine) || in_string.Contains("\n");

            if (doAutoQuote)
            {
                in_string = SanitizeDf(in_string);
            }
            return in_string;
        }

        public static string SanitizeDf(string in_string)
        {
            in_string = in_string.Replace("\"", "\"\"");
            return "\"" + in_string + "\"";
        }

        public static string ConvertLineBreaks(string in_string)
        {
            return in_string.Replace("\n", "\\n").Replace(Environment.NewLine, "\\n");
        }

        public static string ExportCsvString(Google2uWorksheet in_sheet, Google2uExportOptions in_options)
        {
            // for each page
            var ret = string.Empty;
            var rowCt = in_sheet.Rows.Count;
            if (rowCt <= 0) return ret;
            // Iterate through each row, printing its cell values.
            foreach (var row in in_sheet.Rows)
            {
                var rowType = row[0].GetTypeFromValue();
                var rowHeader = row[0].CellValueString;
                if (string.IsNullOrEmpty(rowHeader))
                    // if this header is empty
                {
                    if (in_options.CSVCullRows)
                        break;
                    continue;
                }

                if (rowType == SupportedType.Void ||
                    rowHeader.Equals("void", StringComparison.InvariantCultureIgnoreCase))
                    // if this cell is void, then skip the row completely
                {
                    continue;
                }

                // Iterate over the remaining columns, and print each cell value
                for (var i = 0; i < in_sheet.Rows[0].Count; i++)
                {
                    if ((row[i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(row[0].CellValueString) ||
                             in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(in_sheet.Rows[0][i].CellValueString) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase) ||
                             (in_options.CSVCullColumns && i >= in_sheet.FirstBlankCol)))
                    {
                        continue;
                    }

                    var tmpRet = in_options.EscapeCSVStrings
                        ? SanitizeDf(row[i].CellValueString)
                        : AutoQuote(row[i].CellValueString);

                    if (in_options.CSVConvertLineBreaks)
                        tmpRet = ConvertLineBreaks(tmpRet);

                    ret += tmpRet;

                    ret += ",";
                }
                ret = ret.Remove(ret.Length - 1); // remove the last comma
                ret += Environment.NewLine;
            }
            return ret;
        }

        public static void ExportCsv(Google2uWorksheet in_sheet, string in_path, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, in_sheet.WorksheetName);

            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, in_sheet.WorksheetName + ".csv").Replace('\\', '/');

            using (
                var fs = File.Open(in_path,
                    File.Exists(in_path) ? FileMode.Truncate : FileMode.OpenOrCreate,
                    FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = ExportCsvString(in_sheet, in_options);
                    sw.Write(fileString);
                    sw.Flush();
                }
            }

            PushNotification("Saving to: " + in_path);
        }
    }
}