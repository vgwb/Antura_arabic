// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Xml;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static string ExportXMLString(Google2uWorksheet in_sheet, Google2uExportOptions in_options)
        {
            var bConvertArrays = !in_options.XMLCellArrayToString;

            // Create the System.Xml.XmlDocument.
            var xmlDoc = new XmlDocument();
            var rootNode = xmlDoc.CreateElement("Sheets");
            xmlDoc.AppendChild(rootNode);

            var sheetNode = xmlDoc.CreateElement("sheet");
            var sheetName = xmlDoc.CreateAttribute("name");
            sheetName.Value = in_sheet.WorksheetName;


            var curRow = 0;

            sheetNode.Attributes.Append(sheetName);
            rootNode.AppendChild(sheetNode);

            // Iterate through each row, printing its cell values.
            foreach (var row in in_sheet.Rows)
            {
                if (curRow < 1)
                {
                    curRow++;
                    continue;
                }
                if (in_sheet.UseTypeRow == true && curRow == 1)
                {
                    curRow++;
                    continue;
                }

                var rowType = row[0].GetTypeFromValue();
                var rowHeader = row[0].CellValueString;
                if (string.IsNullOrEmpty(rowHeader))
                    // if this header is empty
                {
                    if (in_options.XMLCullRows)
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


                if (in_options.XMLColsAsChildTags)
                {
                    XmlNode rowNode = xmlDoc.CreateElement("row");
                    var rowName = xmlDoc.CreateAttribute("name");
                    rowName.Value = row[0].CellValueString;
                    if (rowNode.Attributes == null) continue;
                    rowNode.Attributes.Append(rowName);
                    sheetNode.AppendChild(rowNode);


                    // Iterate over the remaining columns, and print each cell value
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        // Don't process rows or columns marked for ignore
                        if ((row[i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(row[0].CellValueString) ||
                             in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(in_sheet.Rows[0][i].CellValueString) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("void",
                                 StringComparison.InvariantCultureIgnoreCase) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("ignore",
                                 StringComparison.InvariantCultureIgnoreCase) ||
                             (in_options.XMLCullColumns && i >= in_sheet.FirstBlankCol)))
                        {
                            continue;
                        }

                        XmlNode colNode = xmlDoc.CreateElement(in_sheet.Rows[0][i].CellValueString);

                        var colType = xmlDoc.CreateAttribute("type");
                        colType.Value = row[i].CellTypeString;
                        if (colNode.Attributes != null) colNode.Attributes.Append(colType);

                        if (bConvertArrays && IsSupportedArrayType(row[i].MyType))
                        {
                            var delim = in_options.DelimiterOptions[in_options.ArrayDelimiters].ToCharArray();

                            if (row[i].MyType == SupportedType.StringArray)
                                delim = in_options.DelimiterOptions[in_options.StringArrayDelimiters].ToCharArray();

                            var value = row[i].CellValueString.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var s in value)
                            {
                                XmlNode arrNode = xmlDoc.CreateElement("entry");
                                if (row[i].MyType == SupportedType.BoolArray)
                                {
                                    var val = s.ToLower();
                                    if (val == "1")
                                        val = "true";
                                    if (val == "0")
                                        val = "false";
                                    arrNode.InnerText = val;
                                }
                                else
                                    arrNode.InnerText = s;

                                colNode.AppendChild(arrNode);
                            }
                        }
                        else
                        {
                            colNode.InnerText = row[i].CellValueString;
                        }

                        rowNode.AppendChild(colNode);
                    }
                    curRow++;
                }
                else
                {
                    XmlNode rowNode = xmlDoc.CreateElement("row");
                    if (rowNode.Attributes == null) continue;

                    var rowAttribute = xmlDoc.CreateAttribute("UID");
                    rowAttribute.Value = row[0].CellValueString;
                    rowNode.Attributes.Append(rowAttribute);

                    // Iterate over the remaining columns, and print each cell value
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        // Don't process rows or columns marked for ignore
                        if ((row[i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(row[0].CellValueString) ||
                             in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                             string.IsNullOrEmpty(in_sheet.Rows[0][i].CellValueString) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("void",
                                 StringComparison.InvariantCultureIgnoreCase) ||
                             in_sheet.Rows[0][i].CellValueString.Equals("ignore",
                                 StringComparison.InvariantCultureIgnoreCase) ||
                             (in_options.XMLCullColumns && i >= in_sheet.FirstBlankCol)))
                        {
                            continue;
                        }

                        rowAttribute = xmlDoc.CreateAttribute(in_sheet.Rows[0][i].CellValueString);
                        rowAttribute.Value = row[i].CellValueString;

                        rowNode.Attributes.Append(rowAttribute);
                    }


                    sheetNode.AppendChild(rowNode);

                    curRow++;
                }
            }

            string retstring;
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlDoc.WriteTo(xmlTextWriter);
                    retstring = stringWriter.ToString();
                }
            }

            return retstring;
        }

        public static void ExportXML(Google2uWorksheet in_sheet, string in_path, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, in_sheet.WorksheetName);

            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            in_path = Path.Combine(in_path, in_sheet.WorksheetName + ".xml").Replace('\\', '/');

            using (
                var fs = File.Open(in_path,
                    File.Exists(in_path) ? FileMode.Truncate : FileMode.OpenOrCreate,
                    FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = ExportXMLString(in_sheet, in_options);
                    sw.Write(fileString);
                    sw.Flush();
                }
            }

            PushNotification("Saving to: " + in_path);
        }
    }
}