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
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static string ExportNGUIString(Google2uWorksheet in_sheet, Google2uExportOptions in_options)
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
                    if (in_options.NGUICullRows)
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
                             (in_options.NGUICullColumns && i >= in_sheet.FirstBlankCol)))
                    {
                        continue;
                    }

                    var tmpRet = in_options.EscapeNGUIStrings
                        ? SanitizeDf(row[i].CellValueString)
                        : AutoQuote(row[i].CellValueString);

                    if (in_options.NGUIConvertLineBreaks)
                        tmpRet = ConvertLineBreaks(tmpRet);


                    ret += tmpRet;

                    ret += ",";
                }
                ret = ret.Remove(ret.Length - 1); // remove the last comma
                ret += Environment.NewLine;
            }
            return ret;
        }

        public static string ExportNGUILegacyString(Google2uWorksheet in_sheet, Google2uExportOptions in_options)
        {
            return ExportNGUILegacyString(in_sheet, in_options, 1);
        }

        public static string ExportNGUILegacyString(Google2uWorksheet in_sheet, Google2uExportOptions in_options,
            int in_langIndex)
        {
            var ret = FormatLine("Flag = Flag-" + in_sheet.Rows[0][in_langIndex].CellValueString);
            for (var i = 1; i < in_sheet.Rows.Count - 1; ++i)
            {
                var row = in_sheet.Rows[i];
                if (string.IsNullOrEmpty(row.Cells[0].CellValueString) && in_options.NGUICullRows)
                    break;

                var tmpRet = in_options.EscapeNGUIStrings
                    ? SanitizeDf(row.Cells[in_langIndex].CellValueString)
                    : row.Cells[in_langIndex].CellValueString;

                if (in_options.NGUIConvertLineBreaks)
                    tmpRet = ConvertLineBreaks(tmpRet);

                ret += FormatLine(row.Cells[0].CellValueString + " = " + tmpRet);
            }
            return ret;
        }

        public static void ExportNGUILegacy(Google2uWorksheet in_sheet, string in_path, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            var finalCol = in_sheet.Rows[0].Count;
            if (in_options.NGUICullColumns)
                finalCol = in_sheet.FirstBlankCol;

            var languages = new List<string>();
            for (var j = 1; j < in_sheet.Rows[0].Count - 1; ++j)
            {
                if (j == finalCol)
                    break;
                languages.Add(in_sheet.Rows[0][j].CellValueString);
            }

            foreach (var lang in languages)
            {
                var filepath = in_path + "/" + lang + ".txt";


                using (
                    var fs = File.Open(filepath,
                        File.Exists(filepath) ? FileMode.Truncate : FileMode.OpenOrCreate,
                        FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        var langIndex = 0;
                        for (var index = 0; index < in_sheet.Rows[0].Cells.Count; index++)
                        {
                            var cell = in_sheet.Rows[0].Cells[index];
                            if (cell.CellValueString.Equals(lang))
                            {
                                langIndex = index;
                                break;
                            }
                        }
                        sw.Write(ExportNGUILegacyString(in_sheet, in_options, langIndex));
                        sw.Flush();
                    }
                }
            }
        }

        public static void ExportNGUI(Google2uWorksheet in_sheet, string in_path, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, "Localization.csv").Replace('\\', '/');

            using (
                var fs = File.Open(in_path,
                    File.Exists(in_path) ? FileMode.Truncate : FileMode.OpenOrCreate,
                    FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = ExportNGUIString(in_sheet, in_options);
                    sw.Write(fileString);
                    sw.Flush();
                }
            }

            PushNotification("Saving to: " + in_path);
        }
    }
}