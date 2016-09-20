// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

using System.Runtime.Serialization.Formatters.Binary;

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static void ExportObjectDb(Google2uWorksheet in_sheet, string in_resourcesPath, string in_editorPath,
            string in_playmakerPath, Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_resourcesPath))
                Directory.CreateDirectory(in_resourcesPath);

            if (!Directory.Exists(in_editorPath))
                Directory.CreateDirectory(in_editorPath);


            var arrayDelimiter = in_options.DelimiterOptions[in_options.ArrayDelimiters];
            var stringArrayDelimiter = in_options.DelimiterOptions[in_options.StringArrayDelimiters];
            var complexTypeDelimiter = in_options.DelimiterOptions[in_options.ComplexTypeDelimiters];
            var complexTypeArrayDelimiters = in_options.DelimiterOptions[in_options.ComplexArrayDelimiters];

            var typesInFirstRow = in_sheet.UseTypeRow;

            ///////////////////////////////////////////////
            // open the file 
            var className = Path.GetInvalidFileNameChars()
                .Aggregate(in_sheet.WorksheetName, (in_current, in_c) => in_current.Replace(in_c, '_'));

            var tmpPath = Path.GetTempFileName();

            var binaryData = new List<List<string>>();

            using (var fs = File.Open(tmpPath, FileMode.Create, FileAccess.Write))
            {

                // Export the data as rows with values for each cell
                for (var row = in_sheet.UseTypeRow ? 2 : 1; row < in_sheet.Rows.Count; row++)
                {
                    var binaryDataRow = new List<string>();

                    var rowHeader = in_sheet.Rows[row][0].CellValueString;
                    if (rowHeader == string.Empty ||
                        rowHeader.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                        rowHeader.Equals("ignore", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (in_options.ObjectDBCullRows)
                            break;
                        continue;
                    }

                    binaryDataRow.Add(rowHeader);

                    var colCount = in_sheet.Rows[row].Count;
                    for (var col = 1; col < colCount; col++)
                    {
                        var headerValue = in_sheet.Rows[0][col].CellValueString;
                        var cellValue = in_sheet.Rows[row][col].CellValueString;
                        if (headerValue == string.Empty ||
                            headerValue.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                            headerValue.Equals("ignore", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (in_options.ObjectDBCullColumns)
                                break;
                            continue;
                        }
                        if (in_sheet.UseTypeRow)
                        {
                            var colType = in_sheet.Rows[1][col].CellValueString;
                            if (colType == string.Empty ||
                            colType.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                            colType.Equals("ignore", StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }
                        }
                        binaryDataRow.Add(cellValue);
                    }

                    binaryData.Add(binaryDataRow);
                }

                var bin = new BinaryFormatter();
                bin.Serialize(fs, binaryData);

            }
            

            var exportInfo = new Google2uObjDbExport { DataLocation = tmpPath, CullEmptyRows = in_options.ObjectDBCullRows, CullEmptyCols = in_options.ObjectDBCullColumns };

            var overrideName = in_options.GetOverrideObjectDatabaseGameObjectName(in_sheet.WorksheetName);

            exportInfo.ObjectName = string.IsNullOrEmpty(overrideName)
                ? (string.IsNullOrEmpty(in_options.ExportDatabaseGameObjectName)
                    ? "Google2uDatabase"
                    : in_options.ExportDatabaseGameObjectName)
                : overrideName;

            exportInfo.ScriptName = className;

            using (var fs = File.Open(Path.Combine(in_resourcesPath, className + ".cs"),
                File.Exists(Path.Combine(in_resourcesPath, className + ".cs"))
                    ? FileMode.Truncate
                    : FileMode.OpenOrCreate,
                FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = string.Empty;

                    fileString += FormatLine("//----------------------------------------------");
                    fileString += FormatLine("//    Google2u: Google Doc Unity integration");
                    fileString += FormatLine("//         Copyright © 2015 Litteratus");
                    fileString += FormatLine("//");
                    fileString += FormatLine("//        This file has been auto-generated");
                    fileString += FormatLine("//              Do not manually edit");
                    fileString += FormatLine("//----------------------------------------------");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("using UnityEngine;");
                    fileString += FormatLine("using System.Globalization;");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("namespace Google2u");
                    fileString += FormatLine("{");
                    fileString += FormatLine("	[System.Serializable]");
                    fileString += FormatLine("	public class " + className + "Row : IGoogle2uRow");
                    fileString += FormatLine("	{");

                    // variable declarations
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                        {
                        }
                        else if (IsSupportedArrayType(in_sheet.Rows[0][i].MyType))
                        {
                            fileString +=
                                FormatLine("		public System.Collections.Generic.List<" +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + "> " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = new System.Collections.Generic.List<" +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + ">();");
                        }
                        else
                            fileString +=
                                FormatLine("		public " + StringSupportedType(in_sheet.Rows[0][i].MyType) + " " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ";");
                    }

                    // constructor parameter list
                    fileString += ("		public " + className + "Row(");
                    {
                        var firstItem = true;
                        for (var i = 0; i < in_sheet.Rows[0].Count; i++)
                        {
                            if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                                string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                    in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            if (!firstItem)
                                fileString += (", ");
                            firstItem = false;
                            fileString += ("string _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames));
                        }
                    }
                    fileString += FormatLine(") " + Environment.NewLine + "		{");

                    // processing each of the input parameters and copying it into the members
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        //nightmare time
                        if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                        {
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.GameObject)
                        {
                            fileString +=
                                FormatLine("			" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = GameObject.Find(\"" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\");");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Bool)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(in_sheet.Rows[0][i].MyType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(in_sheet.Rows[0][i].MyType) + ".TryParse(_" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ", out res))");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to bool\");");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Byte)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(in_sheet.Rows[0][i].MyType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(in_sheet.Rows[0][i].MyType) + ".TryParse(_" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to byte\");");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Float)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(in_sheet.Rows[0][i].MyType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(in_sheet.Rows[0][i].MyType) + ".TryParse(_" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " +\" to " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + "\");");
                            fileString += FormatLine("			}");
                        }
                        else if ((in_sheet.Rows[0][i].MyType == SupportedType.ByteArray)
                                 || (in_sheet.Rows[0][i].MyType == SupportedType.BoolArray)
                                 || (in_sheet.Rows[0][i].MyType == SupportedType.FloatArray))
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("				" + StringSupportedType(in_sheet.Rows[0][i].MyType) + " res;");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           arrayDelimiter +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");
                            if (in_sheet.Rows[0][i].MyType == SupportedType.BoolArray)
                                fileString +=
                                    FormatLine("					if(" + StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                               ".TryParse(result[i], out res))");
                            else
                                fileString +=
                                    FormatLine("					if(" + StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                               ".TryParse(result[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");

                            fileString +=
                                FormatLine("						" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add(res);");
                            fileString += FormatLine("					else");
                            fileString += FormatLine("					{");
                            if (in_sheet.Rows[0][i].MyType == SupportedType.ByteArray)
                                fileString +=
                                    FormatLine("						" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( 0 );");
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.BoolArray)
                                fileString +=
                                    FormatLine("						" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( false );");
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.FloatArray)
                                fileString +=
                                    FormatLine("						" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( float.NaN );");
                            fileString +=
                                FormatLine("						Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ result[i] +\" to " +
                                           (StringSupportedType(in_sheet.Rows[0][i].MyType)) + "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Int)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(in_sheet.Rows[0][i].MyType) + " res;");
                            fileString +=
                                FormatLine("				if(int.TryParse(_" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to int\");");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.IntArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				" + (StringSupportedType(in_sheet.Rows[0][i].MyType)) + " res;");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           arrayDelimiter +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");
                            fileString +=
                                FormatLine(
                                    "					if(int.TryParse(result[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("						" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( res );");
                            fileString += FormatLine("					else");
                            fileString += FormatLine("					{");
                            fileString +=
                                FormatLine("						" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( 0 );");
                            fileString +=
                                FormatLine("						Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ result[i] +\" to " +
                                           (StringSupportedType(in_sheet.Rows[0][i].MyType)) + "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.String)
                        {
                            if (in_options.TrimStrings)
                                fileString +=
                                    FormatLine("			" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               " = _" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Trim();");
                            else
                                fileString +=
                                    FormatLine("			" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               " = _" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ";");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.StringArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           stringArrayDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");
                            if (in_options.TrimStringArrays)
                                fileString +=
                                    FormatLine("					" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( result[i].Trim() );");
                            else
                                fileString +=
                                    FormatLine("					" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( result[i] );");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 2)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            fileString += FormatLine("				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("				for(int i = 0; i < 2; i++)");
                            fileString += FormatLine("				{");
                            fileString += FormatLine("					float res;");
                            fileString +=
                                FormatLine(
                                    "					if(float.TryParse(splitpath[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						results[i] = res;");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("					else ");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2Array)
                        {
                            fileString += FormatLine("			{");

                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeArrayDelimiters +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            fileString += FormatLine("                  {");
                            fileString +=
                                FormatLine("      				string [] splitpath = result[i].Split(\"" + complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("      				if(splitpath.Length != 2)");
                            fileString +=
                                FormatLine("      					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                           " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " );");
                            fileString += FormatLine("      				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("      				for(int j = 0; j < splitpath.Length; j++)");
                            fileString += FormatLine("      				{");
                            fileString += FormatLine("				            float [] temp = new float[splitpath.Length];");
                            fileString +=
                                FormatLine(
                                    "      					if(float.TryParse(splitpath[j], NumberStyles.Any, CultureInfo.InvariantCulture, out temp[j]))");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("      						results[j] = temp[j];");
                            fileString += FormatLine("      					}");
                            fileString += FormatLine("      					else ");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("	        					Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1] ));");
                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            fileString += FormatLine("				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("				for(int i = 0; i < 3; i++)");
                            fileString += FormatLine("				{");
                            fileString += FormatLine("					float res;");
                            fileString +=
                                FormatLine(
                                    "					if(float.TryParse(splitpath[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						results[i] = res;");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("					else ");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".z = results[2];");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3Array)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeArrayDelimiters +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            fileString += FormatLine("      			{");
                            fileString +=
                                FormatLine("      				string [] splitpath = result[i].Split(\"" + complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("      				if(splitpath.Length != 3)");
                            fileString +=
                                FormatLine("      					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                           " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " );");
                            fileString += FormatLine("      				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("      				for(int j = 0; j < splitpath.Length; j++)");
                            fileString += FormatLine("      				{");
                            fileString += FormatLine("				            float [] temp = new float[splitpath.Length];");
                            fileString +=
                                FormatLine(
                                    "      					if(float.TryParse(splitpath[j], NumberStyles.Any, CultureInfo.InvariantCulture, out temp[j]))");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("      						results[j] = temp[j];");
                            fileString += FormatLine("      					}");
                            fileString += FormatLine("      					else ");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("	        					Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        	" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Color)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            fileString += FormatLine("				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("				for(int i = 0; i < splitpath.Length; i++)");
                            fileString += FormatLine("				{");
                            fileString += FormatLine("					float res;");
                            fileString +=
                                FormatLine(
                                    "					if(float.TryParse(splitpath[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						results[i] = res;");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("					else ");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".r = results[0];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".g = results[1];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".b = results[2];");
                            fileString += FormatLine("				if(splitpath.Length == 4)");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".a = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.ColorArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeArrayDelimiters +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            fileString += FormatLine("      			{");
                            fileString +=
                                FormatLine("      				string [] splitpath = result[i].Split(\"" + complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("      				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("      					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                           " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " );");
                            fileString += FormatLine("      				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("      				for(int j = 0; j < splitpath.Length; j++)");
                            fileString += FormatLine("      				{");
                            fileString += FormatLine("				            float [] temp = new float[splitpath.Length];");
                            fileString +=
                                FormatLine(
                                    "      					if(float.TryParse(splitpath[j], NumberStyles.Any, CultureInfo.InvariantCulture, out temp[j]))");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("      						results[j] = temp[j];");
                            fileString += FormatLine("      					}");
                            fileString += FormatLine("      					else ");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("	        					Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString += FormatLine("		        		if(splitpath.Length == 3)");
                            fileString +=
                                FormatLine("		        		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2] ));");
                            fileString += FormatLine("		        		else");
                            fileString +=
                                FormatLine("		        		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2], results[3] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Color32)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            fileString += FormatLine("				byte []results = new byte[splitpath.Length];");
                            fileString += FormatLine("				for(int i = 0; i < splitpath.Length; i++)");
                            fileString += FormatLine("				{");
                            fileString += FormatLine("					byte res;");
                            fileString +=
                                FormatLine(
                                    "					if(byte.TryParse(splitpath[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						results[i] = res;");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("					else ");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".r = results[0];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".g = results[1];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".b = results[2];");
                            fileString += FormatLine("				if(splitpath.Length == 4)");
                            fileString +=
                                FormatLine("					" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".a = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Color32Array)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeArrayDelimiters +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            fileString += FormatLine("      			{");
                            fileString +=
                                FormatLine("      				string [] splitpath = result[i].Split(\"" + complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("      				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("      					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                           " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " );");
                            fileString += FormatLine("      				byte []results = new byte[splitpath.Length];");
                            fileString += FormatLine("      				for(int j = 0; j < splitpath.Length; j++)");
                            fileString += FormatLine("      				{");
                            fileString += FormatLine("				            byte [] temp = new byte[splitpath.Length];");
                            fileString +=
                                FormatLine(
                                    "      					if(byte.TryParse(splitpath[j], NumberStyles.Any, CultureInfo.InvariantCulture, out temp[j]))");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("      						results[j] = temp[j];");
                            fileString += FormatLine("      					}");
                            fileString += FormatLine("      					else ");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("	        					Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString += FormatLine("		        		if(splitpath.Length == 3)");
                            fileString +=
                                FormatLine("		        		    " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Add( new " +
                                           (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2], System.Convert.ToByte(0) ));");
                            fileString += FormatLine("		        		else");
                            fileString +=
                                FormatLine("		        		    " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Add( new " +
                                           (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2], results[3] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.Quaternion)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) + " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            fileString += FormatLine("				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("				for(int i = 0; i < 4; i++)");
                            fileString += FormatLine("				{");
                            fileString += FormatLine("					float res;");
                            fileString +=
                                FormatLine(
                                    "					if(float.TryParse(splitpath[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						results[i] = res;");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("					else ");
                            fileString += FormatLine("					{");
                            fileString += FormatLine("						Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".z = results[2];");
                            fileString +=
                                FormatLine("				" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".w = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (in_sheet.Rows[0][i].MyType == SupportedType.QuaternionArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeArrayDelimiters +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            fileString += FormatLine("      			{");
                            fileString +=
                                FormatLine("      				string [] splitpath = result[i].Split(\"" + complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("      				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("      					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(in_sheet.Rows[0][i].MyType) +
                                           " in \" + _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " );");
                            fileString += FormatLine("      				float []results = new float[splitpath.Length];");
                            fileString += FormatLine("      				for(int j = 0; j < splitpath.Length; j++)");
                            fileString += FormatLine("      				{");
                            fileString += FormatLine("				            float [] temp = new float[splitpath.Length];");
                            fileString +=
                                FormatLine(
                                    "      					if(float.TryParse(splitpath[j], NumberStyles.Any, CultureInfo.InvariantCulture, out temp[j]))");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("      						results[j] = temp[j];");
                            fileString += FormatLine("      					}");
                            fileString += FormatLine("      					else ");
                            fileString += FormatLine("      					{");
                            fileString += FormatLine("	        					Debug.LogError(\"Error parsing \" + "
                                                     + "_" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(in_sheet.Rows[0][i].MyType)) +
                                           "(results[0], results[1], results[2], results[3] ));");
                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else
                        {
                            fileString +=
                                FormatLine("			" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " = _" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ";");
                        }
                    }
                    fileString += FormatLine("		}");

                    fileString += FormatLine(string.Empty);
                    {
                        var colCount = 0;

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                                string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                    in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;
                            colCount++;
                        }
                        fileString += FormatLine("		public int Length { get { return " + colCount + "; } }");
                    }
                    fileString += FormatLine(string.Empty);

                    // allow indexing by []
                    fileString += FormatLine("		public string this[int i]");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("		    get");
                    fileString += FormatLine("		    {");
                    fileString += FormatLine("		        return GetStringDataByIndex(i);");
                    fileString += FormatLine("		    }");
                    fileString += FormatLine("		}");
                    fileString += FormatLine(string.Empty);
                    // get string data by index lets the user use an int field rather than the name to retrieve the data
                    fileString += FormatLine("		public string GetStringDataByIndex( int index )");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			string ret = System.String.Empty;");
                    fileString += FormatLine("			switch( index )");
                    fileString += FormatLine("			{");
                    {
                        var colNum = 0;
                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                                string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                    in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;
                            fileString += FormatLine("				case " + colNum++ + ":");
                            fileString +=
                                FormatLine("					ret = " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".ToString();");
                            fileString += FormatLine("					break;");
                        }
                    }
                    fileString += FormatLine("			}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");
                    fileString += FormatLine(string.Empty);

                    // get the data by column name rather than index
                    fileString += FormatLine("		public string GetStringData( string colID )");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			var ret = System.String.Empty;");

                    if (in_options.LowercaseHeader)
                        fileString += FormatLine("			switch( colID.ToLower() )");
                    else
                        fileString += FormatLine("			switch( colID )");
                    fileString += FormatLine("			{");

                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                            continue;
                        fileString +=
                            FormatLine("				case \"" +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\":");
                        fileString +=
                            FormatLine("					ret = " +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                       ".ToString();");
                        fileString += FormatLine("					break;");
                    }

                    fileString += FormatLine("			}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");

                    fileString += FormatLine("		public override string ToString()");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			string ret = System.String.Empty;");
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                            continue;
                        fileString +=
                            FormatLine("			ret += \"{\" + \"" +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                       "\" + \" : \" + " +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                       ".ToString() + \"} \";");
                    }
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");

                    fileString += FormatLine("	}");

                    fileString +=
                        FormatLine("	public class " + className + " :  Google2uComponentBase, IGoogle2uDB");
                    fileString += FormatLine("	{");

                    // this is the enums, the enum matches the name of the row
                    fileString += FormatLine("		public enum rowIds {");
                    fileString += ("			");

                    {
                        var curRow = 0;
                        // Iterate through each row, printing its cell values to be the enum names.
                        foreach (var row in in_sheet.Rows)
                        {
                            if (curRow == 0 || (typesInFirstRow && curRow == 1))
                            {
                                curRow++;
                                continue;
                            }

                            var rowType = row[0].MyType;
                            var rowHeader = row[0].CellValueString;
                            if (string.IsNullOrEmpty(rowHeader))
                                // if this header is empty
                            {
                                if (in_options.ObjectDBCullRows)
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

                            fileString += row[0].CellValueString;
                            if ((curRow + 1)%20 == 0)
                                fileString += Environment.NewLine + "			";
                            fileString += (", ");
                            curRow++;
                        }
                    }
                    fileString = fileString.Remove(fileString.Length - 2); // remove the last comma

                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("		};");


                    fileString += FormatLine("		public string [] rowNames = {");
                    fileString += "			";
                    // Iterate through each row, printing its cell values for the row names strings.
                    {
                        var curRow = 0;
                        foreach (var row in in_sheet.Rows)
                        {
                            if (curRow == 0 || (typesInFirstRow && curRow == 1))
                            {
                                curRow++;
                                continue;
                            }

                            var rowType = row[0].MyType;
                            var rowHeader = row[0].CellValueString;
                            if (string.IsNullOrEmpty(rowHeader))
                                // if this header is empty
                            {
                                if (in_options.ObjectDBCullRows)
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

                            fileString += "\"" + row[0].CellValueString + "\"";
                            if ((curRow + 1)%20 == 0)
                                fileString += Environment.NewLine + "			";
                            fileString += ", ";
                            curRow++;
                        }
                    }
                    fileString = fileString.Remove(fileString.Length - 2); // remove the last comma
                    fileString += FormatLine(Environment.NewLine + "		};");
                    // the declaration of the storage for the row data
                    fileString +=
                        FormatLine("		public System.Collections.Generic.List<" + className +
                                   "Row> Rows = new System.Collections.Generic.List<" + className + "Row>();");


                    {
                        // the dont destroy on awake flag
                        if (in_options.UseDoNotDestroy)
                        {
                            fileString += FormatLine(string.Empty);
                            fileString += FormatLine("		void Awake()");
                            fileString += FormatLine("		{");
                            fileString += FormatLine("			DontDestroyOnLoad(this);");
                            fileString += FormatLine("		}");
                        }

                        // this is the processing that actually gets the data into the object itself later on, 
                        // this loops through the generic input and seperates it into strings for the above
                        // row class to handle and parse into its members
                        fileString +=
                            FormatLine(
                                "		public override void AddRowGeneric (System.Collections.Generic.List<string> input)");
                        fileString += FormatLine("		{");
                        fileString += ("			Rows.Add(new " + className + "Row(");
                        {
                            var typeCount = 1;
                            // variable declarations
                            for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                            {
                                if (in_sheet.Rows[0][i].MyType == SupportedType.Void ||
                                    string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                        in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                    (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                    continue;
                                typeCount++;
                            }
                            var firstItem = true;
                            for (var i = 0; i < typeCount; i++)
                            {
                                if (!firstItem)
                                    fileString += (",");
                                firstItem = false;
                                fileString += ("input[" + i + "]");
                            }
                        }
                        fileString += FormatLine("));");
                        fileString += FormatLine("		}");

                        fileString += FormatLine("		public override void Clear ()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			Rows.Clear();");
                        fileString += FormatLine("		}");
                    }

                    fileString += FormatLine("		public IGoogle2uRow GetGenRow(string in_RowString)");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			IGoogle2uRow ret = null;");
                    fileString += FormatLine("			try");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			catch(System.ArgumentException) {");
                    fileString +=
                        FormatLine(
                            "				Debug.LogError( in_RowString + \" is not a member of the rowIds enumeration.\");");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");

                    fileString += FormatLine("		public IGoogle2uRow GetGenRow(rowIds in_RowID)");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			IGoogle2uRow ret = null;");
                    fileString += FormatLine("			try");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				ret = Rows[(int)in_RowID];");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			catch( System.Collections.Generic.KeyNotFoundException ex )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Debug.LogError( in_RowID + \" not found: \" + ex.Message );");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");

                    fileString += FormatLine("		public " + className + "Row GetRow(rowIds in_RowID)");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			" + className + "Row ret = null;");
                    fileString += FormatLine("			try");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				ret = Rows[(int)in_RowID];");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			catch( System.Collections.Generic.KeyNotFoundException ex )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Debug.LogError( in_RowID + \" not found: \" + ex.Message );");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");


                    fileString += FormatLine("		public " + className + "Row GetRow(string in_RowString)");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			" + className + "Row ret = null;");
                    fileString += FormatLine("			try");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			catch(System.ArgumentException) {");
                    fileString +=
                        FormatLine(
                            "				Debug.LogError( in_RowString + \" is not a member of the rowIds enumeration.\");");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("	}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("}");


                    sw.Write(fileString);
                    sw.Flush();
                } // using StreamWriter
            } // Using Filestream

            exportInfo.LastAttempt = DateTime.Now;
            Instance.ObjDbExport.Add(exportInfo);

            using (var fs = File.Open(Path.Combine(in_editorPath, className + ".cs"),
                File.Exists(Path.Combine(in_editorPath, className + ".cs")) ? FileMode.Truncate : FileMode.OpenOrCreate,
                FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var fileString = string.Empty;
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Writing out the custom inspector
                    //////////////////////////////////////////////////////////////////////////////////////////////////////

                    fileString += FormatLine("using UnityEngine;");
                    fileString += FormatLine("using UnityEditor;");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("namespace Google2u");
                    fileString += FormatLine("{");
                    fileString += FormatLine("	[CustomEditor(typeof(" + className + "))]");
                    fileString += FormatLine("	public class " + className + "Editor : Editor");
                    fileString += FormatLine("	{");
                    fileString += FormatLine("		public int Index = 0;");
                    // sneaky time, count all the arrays and make an index for each of them within the inspector
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        if (in_sheet.Rows[0][i].IsArrayType)
                        {
                            fileString +=
                                FormatLine("		public int " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index = 0;");
                        }
                    }
                    fileString += FormatLine("		public override void OnInspectorGUI ()");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			" + className + " s = target as " + className + ";");
                    fileString += FormatLine("			" + className + "Row r = s.Rows[ Index ];");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                    fileString += FormatLine("			if ( GUILayout.Button(\"<<\") )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Index = 0;");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			if ( GUILayout.Button(\"<\") )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Index -= 1;");
                    fileString += FormatLine("				if ( Index < 0 )");
                    fileString += FormatLine("					Index = s.Rows.Count - 1;");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			if ( GUILayout.Button(\">\") )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Index += 1;");
                    fileString += FormatLine("				if ( Index >= s.Rows.Count )");
                    fileString += FormatLine("					Index = 0;");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			if ( GUILayout.Button(\">>\") )");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				Index = s.Rows.Count - 1;");
                    fileString += FormatLine("			}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                    fileString += FormatLine("			GUILayout.Label( \"ID\", GUILayout.Width( 150.0f ) );");
                    fileString += FormatLine("			{");
                    fileString += FormatLine("				EditorGUILayout.LabelField( s.rowNames[ Index ] );");
                    fileString += FormatLine("			}");
                    fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                    fileString += FormatLine(string.Empty);

                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        if (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol)
                            continue;

                        fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");

                        if (in_sheet.Rows[0][i].IsArrayType)
                        {
                            fileString +=
                                FormatLine("			if ( r." +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Count == 0 )");
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("			    GUILayout.Label( \"" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "\", GUILayout.Width( 150.0f ) );");
                            fileString += FormatLine("			    {");
                            fileString += FormatLine("			    	EditorGUILayout.LabelField( \"Empty Array\" );");
                            fileString += FormatLine("			    }");
                            fileString += FormatLine("			}");
                            fileString += FormatLine("			else");
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("			    GUILayout.Label( \"" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "\", GUILayout.Width( 130.0f ) );");
                            // when you switch the row you are examining, they may have different array sizes... therefore, we may actually be past the end of the list
                            fileString +=
                                FormatLine("			    if ( " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index >= r." +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Count )");
                            fileString +=
                                FormatLine("				    " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index = 0;");
                            // back button
                            fileString += FormatLine("			    if ( GUILayout.Button(\"<\", GUILayout.Width( 18.0f )) )");
                            fileString += FormatLine("			    {");
                            fileString +=
                                FormatLine("			    	" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index -= 1;");
                            fileString +=
                                FormatLine("			    	if ( " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index < 0 )");
                            fileString +=
                                FormatLine("			    		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index = r." +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Count - 1;");
                            fileString += FormatLine("			    }");

                            fileString += FormatLine("			    EditorGUILayout.LabelField(" +
                                                     MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                         in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "_Index.ToString(), GUILayout.Width( 15.0f ));");

                            // fwd button
                            fileString += FormatLine("			    if ( GUILayout.Button(\">\", GUILayout.Width( 18.0f )) )");
                            fileString += FormatLine("			    {");
                            fileString +=
                                FormatLine("			    	" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index += 1;");
                            fileString +=
                                FormatLine("			    	if ( " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index >= r." +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Count )");
                            fileString +=
                                FormatLine("		        		" +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           "_Index = 0;");
                            fileString += FormatLine("				}");
                        }
                        if (in_sheet.Rows[0][i].MyType != SupportedType.Void)
                        {
                            if (in_sheet.Rows[0][i].MyType == SupportedType.Float)
                            {
                                fileString += FormatLine("			GUILayout.Label( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString += FormatLine("				EditorGUILayout.FloatField( (float)r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Byte) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Int))
                            {
                                fileString += FormatLine("			GUILayout.Label( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString += FormatLine("				EditorGUILayout.IntField( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.BoolArray)
                            {
                                fileString += FormatLine("				EditorGUILayout.Toggle( System.Convert.ToBoolean( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index] ) );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.StringArray)
                            {
                                fileString += FormatLine("				EditorGUILayout.TextField( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.FloatArray)
                            {
                                fileString += FormatLine("				EditorGUILayout.FloatField( (float)r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if (IsSupportedArrayType(in_sheet.Rows[0][i].MyType) &&
                                     (in_sheet.Rows[0][i].MyType == SupportedType.ByteArray) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.IntArray))
                            {
                                fileString += FormatLine("				EditorGUILayout.IntField( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Byte)
                            {
                                fileString += FormatLine("			GUILayout.Label( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString += FormatLine("				EditorGUILayout.TextField( System.Convert.ToString( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         " ) );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Bool)
                            {
                                fileString +=
                                    FormatLine("			GUILayout.Label( \"" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString += FormatLine("				EditorGUILayout.Toggle( System.Convert.ToBoolean( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " ) );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.String)
                            {
                                fileString += FormatLine("			GUILayout.Label( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString +=
                                    FormatLine("				EditorGUILayout.TextField( r." +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.GameObject)
                            {
                                fileString +=
                                    FormatLine("			GUILayout.Label( \"" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString +=
                                    FormatLine("				EditorGUILayout.ObjectField( r." +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2)
                            {
                                fileString +=
                                    FormatLine("			EditorGUILayout.Vector2Field( \"" +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\", r." +
                                               MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                   in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2Array)
                            {
                                fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.Vector2Field( \"\", r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3)
                            {
                                fileString += FormatLine("			EditorGUILayout.Vector3Field( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\", r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3Array)
                            {
                                fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.Vector3Field( \"\", r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Color) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Color32))
                            {
                                fileString += FormatLine("			GUILayout.Label( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "\", GUILayout.Width( 150.0f ) );");
                                fileString += FormatLine("			{");
                                fileString += FormatLine("				EditorGUILayout.ColorField( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.ColorArray) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Color32Array))
                            {
                                fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.ColorField( \"\", r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index] );");
                                fileString += FormatLine("			}");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Quaternion)
                            {
                                fileString += FormatLine("          Vector4 converted" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                         " = new Vector4( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".x, "
                                                         + "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".y, "
                                                         + "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".z, "
                                                         + "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".w ); ");
                                fileString += FormatLine("			EditorGUILayout.Vector4Field( \"" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\", converted" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.QuaternionArray)
                            {
                                fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                                fileString += FormatLine("			EditorGUILayout.BeginHorizontal();");
                                fileString += FormatLine("          Vector4 converted" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " = new Vector4( r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index].x, " +
                                                         "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index].y, " +
                                                         "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index].z, " +
                                                         "r." +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "[" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "_Index].w ); ");
                                fileString += FormatLine("			EditorGUILayout.Vector4Field( \"\", converted" +
                                                         MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                                             in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
                                fileString += FormatLine("			}");
                            }
                        }

                        fileString += FormatLine("			EditorGUILayout.EndHorizontal();");
                        fileString += FormatLine(string.Empty);
                    }

                    fileString += FormatLine("		}");
                    fileString += FormatLine("	}");
                    fileString += FormatLine("}");


                    sw.Write(fileString);

                    ///////////////////////////////////
                    // done writing, clean up
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }

            ///////////////////////////////////
            // export playmaker actions (check if playmaker is installed first)
            if (in_options.GeneratePlaymakerActions)
            {
                var playmakerPath = in_playmakerPath;

                using (var fs = File.Open(Path.Combine(playmakerPath, "Get" + className + "DataByID.cs"),
                    File.Exists(Path.Combine(playmakerPath, "Get" + className + "DataByID.cs"))
                        ? FileMode.Truncate
                        : FileMode.OpenOrCreate,
                    FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        /////////////////////////////
                        // Generate the Action for Get*DataByID

                        var fileString = string.Empty;
                        fileString += FormatLine("using UnityEngine;");
                        fileString += FormatLine(string.Empty);

                        fileString += FormatLine("namespace HutongGames.PlayMaker.Actions");
                        fileString += FormatLine("{");
                        fileString += FormatLine("	[ActionCategory(\"Google2u\")]");
                        fileString +=
                            FormatLine("	[Tooltip(\"Gets the specified entry in the " + className +
                                       " Database.\")]");
                        fileString +=
                            FormatLine("	public class Get" + className + "DataByID : FsmStateAction");
                        fileString += FormatLine("	{");
                        fileString += FormatLine("		[RequiredField]");
                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString +=
                            FormatLine("		[Tooltip(\"The object that contains the " + className +
                                       " database.\")]");
                        fileString += FormatLine("		public FsmGameObject databaseObj;");

                        fileString += FormatLine("		[RequiredField]");
                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString += FormatLine("		[Tooltip(\"Row name of the entry you wish to retrieve.\")]");
                        fileString += FormatLine("		public FsmString rowName;");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var fsmvarType = string.Empty;
                            var varType = StringSupportedType(in_sheet.Rows[0][i].MyType);
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                (IsSupportedArrayType(in_sheet.Rows[0][i].MyType)) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                            {
                                continue;
                            }

                            if (in_sheet.Rows[0][i].MyType == SupportedType.Float)
                            {
                                fsmvarType = "FsmFloat";
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Int) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Byte))
                            {
                                fsmvarType = "FsmInt";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Bool)
                            {
                                fsmvarType = "FsmBool";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.String)
                            {
                                fsmvarType = "FsmString";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.GameObject)
                            {
                                fsmvarType = "FsmGameObject";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2)
                            {
                                fsmvarType = "FsmVector2";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3)
                            {
                                fsmvarType = "FsmVector3";
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Color) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Color32))
                            {
                                fsmvarType = "FsmColor";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Quaternion)
                            {
                                fsmvarType = "FsmQuaternion";
                            }

                            fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                            fileString +=
                                FormatLine("		[Tooltip(\"Store the " + varName + " in a " + varType +
                                           " variable.\")]");

                            fileString += FormatLine("		public " + fsmvarType + " " + varName + ";");
                        }

                        fileString += FormatLine("		public override void Reset()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			databaseObj = null;");
                        fileString += FormatLine("			rowName = null;");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                IsSupportedArrayType(in_sheet.Rows[0][i].MyType) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            var tmpVarName =
                                varName.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)[0];

                            if (string.Compare(tmpVarName, "IGNORE", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(tmpVarName, "VOID", StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            fileString += FormatLine("			" + varName + " = null;");
                        }

                        fileString += FormatLine("		}");

                        fileString += FormatLine("		public override void OnEnter()");
                        fileString += FormatLine("		{");
                        fileString +=
                            FormatLine(
                                "			if ( databaseObj != null && rowName != null && rowName.Value != System.String.Empty )");
                        fileString += FormatLine("			{");
                        fileString +=
                            FormatLine("				Google2u." + className +
                                       " db = databaseObj.Value.GetComponent<Google2u." +
                                       className + ">();");
                        fileString +=
                            FormatLine("				Google2u." + className +
                                       "Row row = db.GetRow( rowName.Value );");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                IsSupportedArrayType(in_sheet.Rows[0][i].MyType) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            var tmpVarName =
                                varName.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)[0];


                            if (string.Compare(tmpVarName, "IGNORE", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(tmpVarName, "VOID", StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            fileString += FormatLine("				if ( " + varName + " != null )");
                            fileString += FormatLine("				" + varName + ".Value = row." + varName + ";");
                        }

                        fileString += FormatLine("			}");
                        fileString += FormatLine("			Finish();");
                        fileString += FormatLine("		}");
                        fileString += FormatLine("	}");
                        fileString += FormatLine("}");

                        sw.Write(fileString);

                        ///////////////////////////////////
                        // done writing, clean up
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }

                using (var fs = File.Open(Path.Combine(playmakerPath, "Get" + className + "DataByIndex.cs"),
                    File.Exists(Path.Combine(playmakerPath, "Get" + className + "DataByIndex.cs"))
                        ? FileMode.Truncate
                        : FileMode.OpenOrCreate,
                    FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        var fileString = string.Empty;

                        /////////////////////////////
                        // Generate the Action for Get*DataByIndex
                        fileString += FormatLine("using UnityEngine;");
                        fileString += FormatLine(string.Empty);

                        fileString += FormatLine("namespace HutongGames.PlayMaker.Actions");
                        fileString += FormatLine("{");
                        fileString += FormatLine("	[ActionCategory(\"Google2u\")]");
                        fileString +=
                            FormatLine("	[Tooltip(\"Gets the specified entry in the " + className +
                                       " Database By Index.\")]");
                        fileString +=
                            FormatLine("	public class Get" + className + "DataByIndex : FsmStateAction");
                        fileString += FormatLine("	{");
                        fileString += FormatLine("		[RequiredField]");
                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString +=
                            FormatLine("		[Tooltip(\"The object that contains the " + className +
                                       " database.\")]");
                        fileString += FormatLine("		public FsmGameObject databaseObj;");

                        fileString += FormatLine("		[RequiredField]");
                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString += FormatLine("		[Tooltip(\"Row index of the entry you wish to retrieve.\")]");
                        fileString += FormatLine("		public FsmInt rowIndex;");

                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString += FormatLine("		[Tooltip(\"Row ID of the entry.\")]");
                        fileString += FormatLine("		public FsmString rowName;");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var fsmvarType = string.Empty;
                            var varType = StringSupportedType(in_sheet.Rows[0][i].MyType);
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                IsSupportedArrayType(in_sheet.Rows[0][i].MyType) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                            {
                                continue;
                            }

                            if (in_sheet.Rows[0][i].MyType == SupportedType.Float)
                            {
                                fsmvarType = "FsmFloat";
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Byte) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Int))
                            {
                                fsmvarType = "FsmInt";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Bool)
                            {
                                fsmvarType = "FsmBool";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.String)
                            {
                                fsmvarType = "FsmString";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.GameObject)
                            {
                                fsmvarType = "FsmGameObject";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector2)
                            {
                                fsmvarType = "FsmVector2";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Vector3)
                            {
                                fsmvarType = "FsmVector3";
                            }
                            else if ((in_sheet.Rows[0][i].MyType == SupportedType.Color) ||
                                     (in_sheet.Rows[0][i].MyType == SupportedType.Color32))
                            {
                                fsmvarType = "FsmColor";
                            }
                            else if (in_sheet.Rows[0][i].MyType == SupportedType.Quaternion)
                            {
                                fsmvarType = "FsmQuaternion";
                            }

                            fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                            fileString +=
                                FormatLine("		[Tooltip(\"Store the " + varName + " in a " + varType +
                                           " variable.\")]");

                            fileString += FormatLine("		public " + fsmvarType + " " + varName + ";");
                        }

                        fileString += FormatLine("		public override void Reset()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			databaseObj = null;");
                        fileString += FormatLine("			rowIndex = null;");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                IsSupportedArrayType(in_sheet.Rows[0][i].MyType) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            var tmpVarName =
                                varName.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)[0];

                            if (string.Compare(tmpVarName, "IGNORE", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(tmpVarName, "VOID", StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            fileString += FormatLine("			" + varName + " = null;");
                        }

                        fileString += FormatLine("		}");

                        fileString += FormatLine("		public override void OnEnter()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			if ( databaseObj != null && rowIndex != null )");
                        fileString += FormatLine("			{");
                        fileString +=
                            FormatLine("				Google2u." + className +
                                       " db = databaseObj.Value.GetComponent<Google2u." +
                                       className + ">();");

                        fileString +=
                            FormatLine("				// For sanity sake, we are going to do an auto-wrap based on the input");
                        fileString += FormatLine("				// This should prevent accessing the array out of bounds");
                        fileString += FormatLine("				int i = rowIndex.Value;");
                        fileString += FormatLine("				int L = db.Rows.Count;");
                        fileString += FormatLine("				while ( i < 0 )");
                        fileString += FormatLine("					i += L;");
                        fileString += FormatLine("				while ( i > L-1 )");
                        fileString += FormatLine("					i -= L;");
                        fileString += FormatLine("				Google2u." + className + "Row row = db.Rows[i];");

                        fileString += FormatLine("				if ( rowName != null )");
                        fileString += FormatLine("					rowName.Value = db.rowNames[i];");

                        for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                        {
                            var varName = MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames);

                            if ((in_sheet.Rows[0][i].MyType == SupportedType.Void) ||
                                IsSupportedArrayType(in_sheet.Rows[0][i].MyType) ||
                                (in_options.ObjectDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            var tmpVarName =
                                varName.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries)[0];

                            if (string.Compare(tmpVarName, "IGNORE", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(tmpVarName, "VOID", StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            fileString += FormatLine("				if ( " + varName + " != null )");
                            fileString += FormatLine("				" + varName + ".Value = row." + varName + ";");
                        }

                        fileString += FormatLine("			}");
                        fileString += FormatLine("			Finish();");
                        fileString += FormatLine("		}");
                        fileString += FormatLine("	}");
                        fileString += FormatLine("}");

                        sw.Write(fileString);

                        ///////////////////////////////////
                        // done writing, clean up
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }

                using (var fs = File.Open(Path.Combine(playmakerPath, "Get" + className + "Count.cs"),
                    File.Exists(Path.Combine(playmakerPath, "Get" + className + "Count.cs"))
                        ? FileMode.Truncate
                        : FileMode.OpenOrCreate,
                    FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        /////////////////////////////
                        // Generate the Action for Get*DataByIndex
                        var fileString = string.Empty;

                        fileString += FormatLine("using UnityEngine;");
                        fileString += FormatLine(string.Empty);

                        fileString += FormatLine("namespace HutongGames.PlayMaker.Actions");
                        fileString += FormatLine("{");
                        fileString += FormatLine("	[ActionCategory(\"Google2u\")]");
                        fileString +=
                            FormatLine("	[Tooltip(\"Gets the specified entry in the " + className +
                                       " Database By Index.\")]");
                        fileString += FormatLine("	public class Get" + className + "Count : FsmStateAction");
                        fileString += FormatLine("	{");
                        fileString += FormatLine("		[RequiredField]");
                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString +=
                            FormatLine("		[Tooltip(\"The object that contains the " + className +
                                       " database.\")]");
                        fileString += FormatLine("		public FsmGameObject databaseObj;");

                        fileString += FormatLine("		[UIHint(UIHint.Variable)]");
                        fileString += FormatLine("		[Tooltip(\"Row Count of the database.\")]");
                        fileString += FormatLine("		public FsmInt rowCount;");

                        fileString += FormatLine("		public override void Reset()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			databaseObj = null;");
                        fileString += FormatLine("			rowCount = null;");
                        fileString += FormatLine("		}");

                        fileString += FormatLine("		public override void OnEnter()");
                        fileString += FormatLine("		{");
                        fileString += FormatLine("			if ( databaseObj != null && rowCount != null )");
                        fileString += FormatLine("			{");
                        fileString +=
                            FormatLine("				Google2u." + className +
                                       " db = databaseObj.Value.GetComponent<Google2u." +
                                       className + ">();");
                        fileString += FormatLine("				rowCount.Value = db.Rows.Count;");
                        fileString += FormatLine("			}");
                        fileString += FormatLine("			Finish();");
                        fileString += FormatLine("		}");
                        fileString += FormatLine("	}");
                        fileString += FormatLine("}");

                        sw.Write(fileString);

                        ///////////////////////////////////
                        // done writing, clean up
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }
            } // /found playmaker

            PushNotification("Saving to: " + in_editorPath);
        } // ExportObjectDb
    }
}