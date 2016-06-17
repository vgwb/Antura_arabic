// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------


namespace Google2u
{
    #region Using Directives

    using UnityEditor;
    using UnityEngine;
    using System.IO;

    #endregion

    public class Google2uDocs : EditorWindow
    {
        public delegate void FadeOpenCallback();

        public static Vector2 DocsScroll = Vector2.zero;
        public static string EditorAssets = "Assets/Google2u/Editor/EditorAssets";
        public GUIStyle HelpBoxHeader;
        public GUIStyle HelpBoxText;
        public Google2uLocalization LocalizationInfo;
        public EditorGUILayoutEx MyGUILayout;

        public static void ShowWindow(EditorGUILayoutEx in_layout, Google2uLocalization in_localization)
        {
            //Show existing window instance. If one doesn't exist, make one.
            var Google2uDocsWindow = GetWindow<Google2uDocs>("Google2u Docs");
            Google2uDocsWindow.MyGUILayout = in_layout;
            Google2uDocsWindow.LocalizationInfo = in_localization;
        }

        public void OnEnable()
        {
            EditorApplication.update += Update;

            //Correct paths if necessary

            var projectPath = Application.dataPath;
            if (projectPath.EndsWith("/Assets"))
            {
                projectPath = projectPath.Remove(projectPath.Length - ("Assets".Length));
            }

            if (!File.Exists(projectPath + EditorAssets + "/Google2uAssets"))
            {
                var foundPath = string.Empty;
                if (Google2uGUIUtil.FindPathContaining("Google2uAssets", ref foundPath))
                {
                    foundPath = foundPath.Replace(projectPath, "");

                    if (foundPath.StartsWith("/"))
                    {
                        foundPath = foundPath.Remove(0, 1);
                    }

                    EditorAssets = foundPath;
                    Debug.Log("Located editor assets folder to '" + EditorAssets + "'");
                }
                else
                {
                    Debug.LogWarning("Could not locate editor assets folder\nGoogle2u");
                    return;
                }
            }

            GUISkin google2USkin;
            if (EditorGUIUtility.isProSkin)
            {
                google2USkin =
                    AssetDatabase.LoadAssetAtPath(EditorAssets + "/DarkSkin/Google2uDark.guiskin", typeof (GUISkin)) as
                        GUISkin;
            }
            else
            {
                google2USkin =
                    AssetDatabase.LoadAssetAtPath(EditorAssets + "/LightSkin/Google2uLight.guiskin", typeof (GUISkin))
                        as GUISkin;
            }


            HelpBoxHeader = google2USkin.FindStyle("HelpHeaderStyle");
            HelpBoxText = google2USkin.FindStyle("HelpTextStyle");
        }

        public void OnDestroy()
        {
            EditorApplication.update -= Update;
        }

        public void Update()
        {
            Repaint();
        }

        public void OnGUI()
        {
            if (LocalizationInfo == null)
                return;

            DocsScroll = GUILayout.BeginScrollView(DocsScroll);

            DrawGettingStarted();
            DrawUsingGoogle2u();

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            EditorGUILayout.Separator();
            Google2u.ShowDocsAtStartup =
                EditorGUILayoutEx.ToggleInput(LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_SHOWONSTARTUP),
                    Google2u.ShowDocsAtStartup, "ShowDocsAtStartup");
        }

        public void ShowFade(string in_label, string in_savedVar, bool in_isOuter, FadeOpenCallback[] in_callbacks)
        {
            var savedVar = Google2uGUIUtil.GetBool(in_savedVar, false);
            var fadeVar = MyGUILayout.BeginFadeArea(savedVar, in_label, in_savedVar,
                in_isOuter ? MyGUILayout.OuterBox : MyGUILayout.InnerBox,
                in_isOuter ? MyGUILayout.OuterBoxHeader : MyGUILayout.InnerBoxHeader);
            Google2uGUIUtil.SetBool(in_savedVar, fadeVar.Open);
            if (fadeVar.Show())
            {
                foreach (var fadeOpenCallback in in_callbacks)
                {
                    fadeOpenCallback();
                }
            }
            MyGUILayout.EndFadeArea();
        }

        public void DrawGettingStarted()
        {
            ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_GETTINGSTARTED), "ShowGettingStarted", true,
                new FadeOpenCallback[]
                {
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_GOOGLESETUP),
                            "ShowSettingUpGoogle", false, new FadeOpenCallback[]
                            {
                                DrawSettingUpGoogle
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_GOOGLEDRIVE), "ShowGoogleDrive",
                            false, new FadeOpenCallback[]
                            {
                                DrawGoogleDrive
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_SPREADSHEETSETUP),
                            "ShowSettingUpASpreadsheet", false, new FadeOpenCallback[]
                            {
                                DrawSettingUpAGoogleSpreadsheet
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_DATATYPES), "ShowDataTypes",
                            false, new FadeOpenCallback[]
                            {
                                DrawDataTypes
                            })
                });
        }

        public void DrawSettingUpGoogle()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_GOOGLESETUP),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_GOOGLESETUP), HelpBoxText);

            var content = new GUIContent(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_GOOGLEACCOUNTCREATION));
            if (GUILayout.Button(content))
            {
                Application.OpenURL("http://accounts.google.com/SignUp");
            }

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_GOOGLEACCOUNTPROTECTION),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_GOOGLEACCOUNTPROTECTION),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_GOOGLETWOSTEP),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_GOOGLETWOSTEP), HelpBoxText);

            content = new GUIContent(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_GOOGLETWOSTEP));
            if (GUILayout.Button(content))
            {
                Application.OpenURL("https://security.google.com/settings/security/apppasswords");
            }

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_GOOGLETHIRDPARTY),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_GOOGLETHIRDPARTY),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocGoogleSecurity);
            GUILayout.Label(content);

            content = new GUIContent(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_GOOGLETHIRDPARTY));
            if (GUILayout.Button(content))
            {
                Application.OpenURL("https://www.google.com/settings/security/lesssecureapps");
            }
        }

        public void DrawGoogleDrive()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_GOOGLEDRIVE),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_GOOGLEDRIVE), HelpBoxText);

            var content = new GUIContent(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_GOOGLEDRIVE));
            if (GUILayout.Button(content))
            {
                Application.OpenURL("https://drive.google.com/");
            }
        }

        public void DrawSettingUpAGoogleSpreadsheet()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_SETUPSPREADSHEET),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SETUPSPREADSHEET),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_WORKSHEETNAME),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_WORKSHEETNAME), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_ROWHEADER), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_ROWHEADER), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_FIRSTROW), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_FIRSTROW), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_TYPEROW), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_TYPEROW), HelpBoxText);

            EditorGUILayout.Separator();

            var content = new GUIContent(MyGUILayout.DocSpreadsheetFormat);
            GUILayout.Label(content);
        }

        public void DrawDataTypes()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BASICTYPES), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BASICTYPES), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_COMPLEXTYPES),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_COMPLEXTYPES), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BASICARRAYTYPES),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BASICARRAYTYPES),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_COMPLEXARRAYTYPES),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_COMPLEXARRAYTYPES),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_SPECIALTYPES),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SPECIALTYPES), HelpBoxText);

            EditorGUILayout.Separator();
        }

        public void DrawUsingGoogle2u()
        {
            ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGGOOGLE2U), "ShowUsingGoogle2u", true,
                new FadeOpenCallback[]
                {
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_INTERFACE), "ShowUsingInterface",
                            false, new FadeOpenCallback[]
                            {
                                DrawUsingInterface
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGCREDENTIALS),
                            "ShowUsingCredentials", false, new FadeOpenCallback[]
                            {
                                DrawUsingCredentials
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGACCOUNTWORKBOOKS),
                            "ShowUsingAccountWorkbooks", false, new FadeOpenCallback[]
                            {
                                DrawUsingAccountWorkbooks
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGMANUALWORKBOOKS),
                            "ShowUsingManualWorkbooks", false, new FadeOpenCallback[]
                            {
                                DrawUsingManualWorkbooks
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGUPLOAD), "ShowUsingUpload",
                            false, new FadeOpenCallback[]
                            {
                                DrawUsingUpload
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_LISTVIEW), "ShowListView", false,
                            new FadeOpenCallback[]
                            {
                                DrawEditingListView
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_FULLVIEW), "ShowFullView", false,
                            new FadeOpenCallback[]
                            {
                                DrawEditingFullView
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_EXPORTOPTIONS),
                            "ShowExportOptions", false, new FadeOpenCallback[]
                            {
                                DrawExportOptions
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_EDITINGWORKSHEETS),
                            "ShowEditingWorksheets", false, new FadeOpenCallback[]
                            {
                                DrawEditingWorksheets
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_EXPORTINGWORKSHEETS),
                            "ShowExportingWorksheets", false, new FadeOpenCallback[]
                            {
                                DrawExportingWorksheets
                            }),
                    () =>
                        ShowFade(LocalizationInfo.Localize(Documentation.rowIds.ID_FADE_USINGSETTINGS),
                            "ShowUsingSettings", false, new FadeOpenCallback[]
                            {
                                DrawUsingSettings
                            })
                });
        }

        public void DrawUsingInterface()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_MAININTERFACE),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_MAININTERFACE), HelpBoxText);

            EditorGUILayout.Separator();

            var content = new GUIContent(MyGUILayout.DocMainInterface);
            GUILayout.Label(content);
        }

        public void DrawUsingCredentials()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_CREDENTIALS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_CREDENTIALS), HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_PRELOGIN), HelpBoxText);
            var content = new GUIContent(MyGUILayout.DocLoginInterface1);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_LABEL_POSTLOGIN), HelpBoxText);
            content = new GUIContent(MyGUILayout.DocLoginInterface2);
            GUILayout.Label(content);
        }

        public void DrawUsingAccountWorkbooks()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_ACCOUNTWORKBOOKS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_ACCOUNTWORKBOOKS),
                HelpBoxText);

            var content = new GUIContent(MyGUILayout.DocAccountWorkbooks);
            GUILayout.Label(content);
        }

        public void DrawUsingManualWorkbooks()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_MANUALWORKBOOKS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_MANUALWORKBOOKS),
                HelpBoxText);

            var content = new GUIContent(MyGUILayout.DocManualWorkbooks);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_SHARINGWORKBOOKS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SHARINGWORKBOOKS),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocGoogleSharing1);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SHARINGWORKBOOKS2),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocGoogleSharing2);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SHARINGWORKBOOKS3),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_PUBLISHINGWORKBOOKS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PUBLISHINGWORKBOOKS),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocGooglePublishing1);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PUBLISHINGWORKBOOKS2),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocGooglePublishing2);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PUBLISHINGWORKBOOKS3),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocManualWorkbooks2);
            GUILayout.Label(content);
        }

        public void DrawUsingUpload()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_UPLOAD), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_UPLOAD), HelpBoxText);

            var content = new GUIContent(MyGUILayout.DocUploadWorkbook);
            GUILayout.Label(content);
        }

        public void DrawEditingListView()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_LISTVIEW), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EDITINGLISTVIEW),
                HelpBoxText);

            var content = new GUIContent(MyGUILayout.DocListView);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EDITINGLISTVIEW2),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocIcons0);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_TOOLBAR_REFRESH),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_TOOLBAR_REFRESH),
                HelpBoxText);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_TOOLBAR_VALIDATE),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_TOOLBAR_VALIDATE),
                HelpBoxText);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_TOOLBAR_EDIT),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_TOOLBAR_EDIT), HelpBoxText);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_TOOLBAR_EXPORT),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_TOOLBAR_EXPORT), HelpBoxText);
        }

        public void DrawEditingFullView()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_FULLVIEW), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_FULLVIEW), HelpBoxText);
            var content = new GUIContent(MyGUILayout.DocFullView);
            GUILayout.Label(content);
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNREFRESH), HelpBoxHeader);
            EditorGUILayout.BeginHorizontal();
            content = new GUIContent(MyGUILayout.DocBtnRefresh);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNREFRESH), HelpBoxText);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNVALIDATE),
                HelpBoxHeader);
            EditorGUILayout.BeginHorizontal();
            content = new GUIContent(MyGUILayout.DocBtnValidate);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNVALIDATE), HelpBoxText);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNOPEN), HelpBoxHeader);
            EditorGUILayout.BeginHorizontal();
            content = new GUIContent(MyGUILayout.DocBtnOpen);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNOPEN), HelpBoxText);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNEXPORT), HelpBoxHeader);
            EditorGUILayout.BeginHorizontal();
            content = new GUIContent(MyGUILayout.DocBtnExport);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNEXPORT), HelpBoxText);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNACTIVE), HelpBoxHeader);
            content = new GUIContent(MyGUILayout.DocBtnActive);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNACTIVE), HelpBoxText);
            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_BTNEXPORTAS),
                HelpBoxHeader);
            content = new GUIContent(MyGUILayout.DocBtnExportAs);
            GUILayout.Label(content);
            content = new GUIContent(MyGUILayout.DocExportTypes);
            GUILayout.Label(content);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_BTNEXPORTAS), HelpBoxText);
            EditorGUILayout.Separator();
        }

        public void DrawExportOptions()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_EXPORTOPTIONS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EXPORTOPTIONS), HelpBoxText);

            var content = new GUIContent(MyGUILayout.DocExportOptions);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_WHITESPACE), HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_WHITESPACE), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocWhiteSpace);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_ARRAYDELIMITERS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_ARRAYDELIMITERS),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocDelimiters);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_OPTIONS_OBJDB),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_OPTIONS_OBJDB), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocOptionsObjDB);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_OPTIONS_STATICDB),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_OPTIONS_STATICDB),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocOptionsStaticDB);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_OPTIONS_CSV),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_OPTIONS_CSV), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocOptionsCSV);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_PREVIEW_CSV),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PREVIEW_CSV), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocPreviewCSV);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_OPTIONS_XML),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_OPTIONS_XML), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocOptionsXML);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_PREVIEW_XML),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PREVIEW_XML), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocPreviewXML);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_OPTIONS_JSON),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_OPTIONS_JSON), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocOptionsJSON);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_PREVIEW_JSONOBJECT),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PREVIEW_JSONOBJECT),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocPreviewJSONObj);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_PREVIEW_JSONCLASS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_PREVIEW_JSONCLASS),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocPreviewJSONClass);
            GUILayout.Label(content);
        }

        public void DrawEditingWorksheets()
        {
            var content = new GUIContent(MyGUILayout.DocEditCells);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_EDITINGWORKSHEETS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EDITINGWORKSHEETS),
                HelpBoxText);

            content = new GUIContent(MyGUILayout.DocEditCellsUpdate);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_EDITINGTYPES),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EDITINGTYPES1), HelpBoxText);

            content = new GUIContent(MyGUILayout.DocEditCellsType);
            GUILayout.Label(content);

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EDITINGTYPES2), HelpBoxText);
        }

        public void DrawExportingWorksheets()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_EXPORTINGWORKSHEETS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_EXPORTINGWORKSHEETS),
                HelpBoxText);
        }

        public void DrawUsingSettings()
        {
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_SETTINGSLANGUAGE),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SETTINGSLANGUAGE),
                HelpBoxText);

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HEADER_SETTINGSPATHS),
                HelpBoxHeader);
            EditorGUILayout.TextArea(LocalizationInfo.Localize(Documentation.rowIds.ID_HELP_SETTINGSPATHS), HelpBoxText);

            EditorGUILayout.Separator();
        }
    }
}