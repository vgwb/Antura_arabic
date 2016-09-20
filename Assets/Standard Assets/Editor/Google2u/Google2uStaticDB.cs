// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Linq;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        private static string MakeValidVariableName(string in_string, bool in_makeLower, bool in_prependUnderscore)
        {
            var ret = in_makeLower ? in_string.ToLowerInvariant() : in_string;
            if (string.IsNullOrEmpty(ret))
            {
                return ret;
            }

            string[] invalidStarts =
            {
                "0", "1", "2", "3", "4",
                "5", "6", "7", "8", "9"
            };

            if (invalidStarts.Any(start => in_string.StartsWith(start)))
            {
                ret = "_" + in_string;
            }

            string[] invalidCharacters =
            {
                " ", ",", ".", "?", "\"", ";", ":",
                "\'", "[", "]", "{", "}", "!", "@", "#",
                "$", "%", "^", "&", "*", "(", ")", "-",
                "/", "\\"
            };

            ret = invalidCharacters.Aggregate(ret, (in_current, in_x) => in_current.Replace(in_x, "_"));

            ret = in_prependUnderscore ? "_" + ret : "" + ret;

            return ret;
        }

        private static string FormatLine(string in_line)
        {
            return in_line + Environment.NewLine;
        }

        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public static string StringSupportedType(SupportedType in_type)
        {
            switch (in_type)
            {
                case SupportedType.String:
                case SupportedType.StringArray:
                    return "string";
                case SupportedType.Int:
                case SupportedType.IntArray:
                    return "int";
                case SupportedType.Float:
                case SupportedType.FloatArray:
                    return "float";
                case SupportedType.Bool:
                case SupportedType.BoolArray:
                    return "bool";
                case SupportedType.Byte:
                case SupportedType.ByteArray:
                    return "byte";
                case SupportedType.Vector2:
                case SupportedType.Vector2Array:
                    return "Vector2";
                case SupportedType.Vector3:
                case SupportedType.Vector3Array:
                    return "Vector3";
                case SupportedType.Color:
                case SupportedType.ColorArray:
                    return "Color";
                case SupportedType.Color32:
                case SupportedType.Color32Array:
                    return "Color32";
                case SupportedType.Quaternion:
                case SupportedType.QuaternionArray:
                    return "Quaternion";
                case SupportedType.GameObject:
                    return "GameObject";
                case SupportedType.Void:
                case SupportedType.Unrecognized:
                default:
                    return "Error";
            }
        }

        public static void ExportStaticDB(Google2uWorksheet in_sheet, string in_path, string in_fileName,
            Google2uExportOptions in_options)
        {
            if (!Directory.Exists(in_path))
                Directory.CreateDirectory(in_path);

            var className = Path.GetInvalidFileNameChars()
                .Aggregate(in_sheet.WorksheetName, (in_current, in_c) => in_current.Replace(in_c, '_'));

            var arrayDelimiter = in_options.DelimiterOptions[in_options.ArrayDelimiters];
            var stringArrayDelimiter = in_options.DelimiterOptions[in_options.StringArrayDelimiters];
            var complexTypeDelimiter = in_options.DelimiterOptions[in_options.ComplexTypeDelimiters];
            var complexTypeArrayDelimiters = in_options.DelimiterOptions[in_options.ComplexArrayDelimiters];

            var typesInFirstRow = in_sheet.UseTypeRow;


            ///////////////////////////////////////////////
            // open the file 

            using (var fs = File.Open(Path.Combine(in_path, className + ".cs"),
                File.Exists(Path.Combine(in_path, className + ".cs")) ? FileMode.Truncate : FileMode.OpenOrCreate,
                FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    ////////////////////////////////////////
                    // writing out the class
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
                        var rowType = in_sheet.Rows[0][i].MyType;
                        var rowHeader = in_sheet.Rows[0][i].CellValueString;

                        if (rowType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                        {
                            continue;
                        }

                        if (IsSupportedArrayType(rowType))
                            fileString +=
                                FormatLine("		public System.Collections.Generic.List<" + StringSupportedType(rowType) +
                                           "> " + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = new System.Collections.Generic.List<" + StringSupportedType(rowType) +
                                           ">();");
                        else
                            fileString +=
                                FormatLine("		public " + StringSupportedType(rowType) + " " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ";");
                    }

                    // constructor parameter list
                    fileString += ("		public " + className + "Row(");
                    {
                        var firstItem = true;
                        for (var i = 0; i < in_sheet.Rows[0].Count; i++)
                        {
                            var rowType = in_sheet.Rows[0][i].MyType;
                            var rowHeader = in_sheet.Rows[0][i].CellValueString;

                            if (rowType == SupportedType.Void ||
                                string.IsNullOrEmpty(MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;

                            if (!firstItem)
                                fileString += (", ");
                            firstItem = false;
                            fileString += ("string _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames));
                        }
                    }
                    fileString += FormatLine(") " + Environment.NewLine + "		{");


                    // processing each of the input parameters and copying it into the members
                    for (var i = 1; i < in_sheet.Rows[0].Count; i++)
                    {
                        var rowType = in_sheet.Rows[0][i].MyType;
                        var rowHeader = in_sheet.Rows[0][i].CellValueString;

                        //nightmare time
                        if (rowType == SupportedType.Void ||
                            string.IsNullOrEmpty(MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                            (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                        {
                            continue;
                        }

                        if (rowType == SupportedType.GameObject)
                        {
                            fileString +=
                                FormatLine("			" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = GameObject.Find(\"" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + "\");");
                        }
                        else if (rowType == SupportedType.Bool)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(rowType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(rowType) + ".TryParse(_" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ", out res))");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to bool\");");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Byte)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(rowType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(rowType) + ".TryParse(_" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to byte\");");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Float)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(rowType) + " res;");
                            fileString +=
                                FormatLine("				if(" + StringSupportedType(rowType) + ".TryParse(_" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " +\" to " +
                                           StringSupportedType(rowType) + "\");");
                            fileString += FormatLine("			}");
                        }
                        else if ((rowType == SupportedType.ByteArray)
                                 || (rowType == SupportedType.BoolArray)
                                 || (rowType == SupportedType.FloatArray))
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("				" + StringSupportedType(rowType) + " res;");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           arrayDelimiter +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");

                            if (rowType == SupportedType.BoolArray)
                                fileString +=
                                    FormatLine("					if(" + StringSupportedType(rowType) +
                                               ".TryParse(result[i], out res))");
                            else
                                fileString +=
                                    FormatLine("					if(" + StringSupportedType(rowType) +
                                               ".TryParse(result[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");

                            fileString +=
                                FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add(res);");
                            fileString += FormatLine("					else");
                            fileString += FormatLine("					{");
                            if (rowType == SupportedType.ByteArray)
                                fileString +=
                                    FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( 0 );");
                            else if (rowType == SupportedType.BoolArray)
                                fileString +=
                                    FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( false );");
                            else if (rowType == SupportedType.FloatArray)
                                fileString +=
                                    FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( float.NaN );");
                            fileString +=
                                FormatLine("						Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ result[i] +\" to " + (StringSupportedType(rowType)) + "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Int)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("			" + StringSupportedType(rowType) + " res;");
                            fileString +=
                                FormatLine("				if(int.TryParse(_" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ", NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " = res;");
                            fileString += FormatLine("				else");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " +\" to int\");");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.IntArray)
                        {
                            fileString += FormatLine("			{");
                            fileString += FormatLine("				" + (StringSupportedType(rowType)) + " res;");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           arrayDelimiter +
                                           "\".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");
                            fileString +=
                                FormatLine(
                                    "					if(int.TryParse(result[i], NumberStyles.Any, CultureInfo.InvariantCulture, out res))");
                            fileString +=
                                FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( res );");
                            fileString += FormatLine("					else");
                            fileString += FormatLine("					{");
                            fileString +=
                                FormatLine("						" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( 0 );");
                            fileString +=
                                FormatLine("						Debug.LogError(\"Failed To Convert " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           " string: \"+ result[i] +\" to " + (StringSupportedType(rowType)) + "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.String)
                        {
                            if (in_options.TrimStrings)
                                fileString +=
                                    FormatLine("			" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               " = _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Trim();");
                            else
                                fileString +=
                                    FormatLine("			" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               " = _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ";");
                        }
                        else if (rowType == SupportedType.StringArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           stringArrayDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				for(int i = 0; i < result.Length; i++)");
                            fileString += FormatLine("				{");
                            if (in_options.TrimStringArrays)
                                fileString +=
                                    FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( result[i].Trim() );");
                            else
                                fileString +=
                                    FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                               ".Add( result[i] );");
                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Vector2)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 2)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(rowType) + " in \" + _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Vector2Array)
                        {
                            fileString += FormatLine("			{");

                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
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
                                           StringSupportedType(rowType) +
                                           " in \" + _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        		" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(rowType)) +
                                           "(results[0], results[1] ));");
                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Vector3)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(rowType) + " in \" + _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".z = results[2];");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Vector3Array)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
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
                                           StringSupportedType(rowType) +
                                           " in \" + _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        	" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Color)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(rowType) + " in \" + _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".r = results[0];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".g = results[1];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".b = results[2];");
                            fileString += FormatLine("				if(splitpath.Length == 4)");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".a = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.ColorArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
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
                                           StringSupportedType(rowType) +
                                           " in \" + _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString += FormatLine("		        		if(splitpath.Length == 3)");
                            fileString +=
                                FormatLine("		        		" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2] ));");
                            fileString += FormatLine("		        		else");
                            fileString +=
                                FormatLine("		        		" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2], results[3] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Color32)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 3 && splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(rowType) + " in \" + _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".r = results[0];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".g = results[1];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".b = results[2];");
                            fileString += FormatLine("				if(splitpath.Length == 4)");
                            fileString +=
                                FormatLine("					" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".a = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Color32Array)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
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
                                           StringSupportedType(rowType) +
                                           " in \" + _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString += FormatLine("		        		if(splitpath.Length == 3)");
                            fileString +=
                                FormatLine("		        		    " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Add( new " +
                                           (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2], System.Convert.ToByte(0) ));");
                            fileString += FormatLine("		        		else");
                            fileString +=
                                FormatLine("		        		    " +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Add( new " +
                                           (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2], results[3] ));");

                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.Quaternion)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string [] splitpath = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
                                           complexTypeDelimiter +
                                           "\".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);");
                            fileString += FormatLine("				if(splitpath.Length != 4)");
                            fileString +=
                                FormatLine("					Debug.LogError(\"Incorrect number of parameters for " +
                                           StringSupportedType(rowType) + " in \" + _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " );");
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("					}");
                            fileString += FormatLine("				}");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".x = results[0];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".y = results[1];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".z = results[2];");
                            fileString +=
                                FormatLine("				" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".w = results[3];");
                            fileString += FormatLine("			}");
                        }
                        else if (rowType == SupportedType.QuaternionArray)
                        {
                            fileString += FormatLine("			{");
                            fileString +=
                                FormatLine("				string []result = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".Split(\"" +
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
                                           StringSupportedType(rowType) +
                                           " in \" + _" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
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
                                                     MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)
                                                     +
                                                     " + \" Component: \" + splitpath[i] + \" parameter \" + i + \" of variable "
                                                     + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                                     "\");");
                            fileString += FormatLine("		        			continue;");
                            fileString += FormatLine("		        		}");
                            fileString += FormatLine("		        	}");
                            fileString +=
                                FormatLine("		        		" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) +
                                           ".Add( new " + (StringSupportedType(rowType)) +
                                           "(results[0], results[1], results[2], results[3] ));");
                            fileString += FormatLine("		        	}");

                            fileString += FormatLine("				}");
                            fileString += FormatLine("			}");
                        }
                        else
                        {
                            fileString +=
                                FormatLine("			" + MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + " = _" +
                                           MakeValidVariableName(rowHeader, in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ";");
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
                                (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
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
                                (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                                continue;
                            fileString += FormatLine("				case " + colNum++ + ":");
                            fileString +=
                                FormatLine("					ret = " +
                                           MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                               in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".ToString();");
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
                            (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                            continue;
                        fileString += FormatLine("				case \"" + (in_sheet.Rows[0][i].CellValueString) + "\":");
                        fileString +=
                            FormatLine("					ret = " +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".ToString();");
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
                            (in_options.StaticDBCullColumns && i > in_sheet.FirstBlankCol))
                            continue;
                        fileString +=
                            FormatLine("			ret += \"{\" + \"" + (in_sheet.Rows[0][i].CellValueString) +
                                       "\" + \" : \" + " +
                                       MakeValidVariableName(in_sheet.Rows[0][i].CellValueString,
                                           in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames) + ".ToString() + \"} \";");
                    }
                    fileString += FormatLine("			return ret;");
                    fileString += FormatLine("		}");

                    fileString += FormatLine("	}");


                    ///////////////////////////////////////////////////////////////////////////////
                    // the database class itself, this contains the rows defined above
                    fileString += FormatLine("	public sealed class " + className + " : IGoogle2uDB");
                    fileString += FormatLine("	{");


                    // this is the enums, the enum matches the name of the row
                    fileString += FormatLine("		public enum rowIds {");
                    fileString += ("			");

                    {
                        var curRow = 0;
                        // Iterate through each row, printing its cell values to be the enum names.
                        foreach (var row in in_sheet.Rows)
                        {
                            if (curRow == 0 || (in_sheet.UseTypeRow && curRow == 1))
                            {
                                curRow++;
                                continue;
                            }

                            var rowType = row[0].MyType;
                            var rowHeader = row[0].CellValueString;
                            if (string.IsNullOrEmpty(rowHeader))
                                // if this header is empty
                            {
                                if (in_options.StaticDBCullRows)
                                    break;
                                continue;
                            }

                            if (rowType == SupportedType.Void ||
                                rowHeader.Equals("void", StringComparison.InvariantCultureIgnoreCase))
                                // if this cell is void, then skip the row completely
                            {
                                continue;
                            }

                            fileString += rowHeader;
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
                            if (curRow == 0 || (in_sheet.UseTypeRow && curRow == 1))
                            {
                                curRow++;
                                continue;
                            }

                            var rowType = row[0].MyType;
                            var rowHeader = row[0].CellValueString;
                            if (string.IsNullOrEmpty(rowHeader))
                                // if this header is empty
                            {
                                if (in_options.StaticDBCullRows)
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

                            fileString += "\"" + rowHeader + "\"";
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

                    // declare the instance as well as the get functionality
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("		public static " + className + " Instance");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			get { return Nested" + className + ".instance; }");
                    fileString += FormatLine("		}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("		private class Nested" + className + "");
                    fileString += FormatLine("		{");
                    fileString += FormatLine("			static Nested" + className + "() { }");
                    fileString +=
                        FormatLine("			internal static readonly " + className + " instance = new " + className + "();");
                    fileString += FormatLine("		}");
                    fileString += FormatLine(string.Empty);
                    fileString += FormatLine("		private " + className + "()");
                    fileString += FormatLine("		{");

                    var rowCt = 0;
                    // Iterate through each row, printing its cell values.

                    for (var i = 0; i < in_sheet.Rows.Count; i++)
                    {
                        // skip the first row. This is the title row, and we can get the values later
                        if (rowCt == 0 || (typesInFirstRow && rowCt == 1))
                        {
                            rowCt++;
                            continue;
                        }

                        var rowType = in_sheet.Rows[i][0].MyType;
                        var rowHeader = in_sheet.Rows[i][0].CellValueString;
                        if (string.IsNullOrEmpty(rowHeader))
                            // if this header is empty
                        {
                            if (in_options.StaticDBCullRows)
                                break;
                            rowCt++;
                            continue;
                        }

                        if (rowType == SupportedType.Void ||
                            rowHeader.Equals("void", StringComparison.InvariantCultureIgnoreCase))
                            // if this cell is void, then skip the row completely
                        {
                            rowCt++;
                            continue;
                        }

                        // Iterate over the remaining columns, and print each cell value

                        fileString += "			Rows.Add( new " + className + "Row(";

                        for (var j = 0; j < in_sheet.Rows[0].Count; j++)
                        {
                            if ((j != 0) &&
                                (in_sheet.Rows[i][j].MyType == SupportedType.Void ||
                                 string.IsNullOrEmpty(MakeValidVariableName(in_sheet.Rows[0][j].CellValueString,
                                     in_options.LowercaseHeader, in_options.PrependUnderscoreToVariableNames)) ||
                                  in_sheet.Rows[0][j].CellValueString.Equals("void", StringComparison.InvariantCultureIgnoreCase) ||
                                  in_sheet.Rows[0][j].CellValueString.Equals("ignore", StringComparison.InvariantCultureIgnoreCase) ||
                                 (in_options.StaticDBCullColumns && j >= in_sheet.FirstBlankCol)))
                            {
                                continue;
                            }

                            fileString += ToLiteral(in_sheet.Rows[i][j].CellValueString) + ", ";
                        }
                        fileString = fileString.Remove(fileString.Length - 2); // remove the last comma
                        fileString += FormatLine("));");

                        rowCt++;
                    }
                    fileString += FormatLine("		}");


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

                    ///////////////////////////////////
                    // done writing, clean up
                    sw.Flush();
                } // Using streamwriter
            } // Using FileSystem

            PushNotification("Saving to: " + in_path);
        }
    }
}