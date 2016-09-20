// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Google.GData.Client;
    using Google.GData.Spreadsheets;
    using UnityEngine;
    using UnityEditor;

    #endregion

    public partial class Google2u : EditorWindow
    {
        public static string EditorAssets = "Assets/Standard Assets/Editor/Google2u/EditorAssets";
        private static OAuth2Parameters _authParameters;
        public EditorGUILayoutEx MyGUILayout;
        public Vector2 ScrollPos = Vector2.zero;

        public void LoadPathSettings()
        {
            InstanceData.ProjectPath = Application.dataPath;

            _ObjDbResourcesDirectory = Google2uGUIUtil.GetString("g2uobjDBResourcesDirectory", _ObjDbResourcesDirectory);
            _ObjDbEditorDirectory = Google2uGUIUtil.GetString("g2uobjDBEditorDirectory", _ObjDbEditorDirectory);
            _StaticDbResourcesDirectory = Google2uGUIUtil.GetString("g2uStaticDBResourcesDirectory",
                _StaticDbResourcesDirectory);
            _NguiDirectory = Google2uGUIUtil.GetString("g2unguiDirectory", _NguiDirectory);
            _XmlDirectory = Google2uGUIUtil.GetString("g2uxmlDirectory", _XmlDirectory);
            _JsonDirectory = Google2uGUIUtil.GetString("g2ujsonDirectory", _JsonDirectory);
            _CsvDirectory = Google2uGUIUtil.GetString("g2ucsvDirectory", _CsvDirectory);
            _PlaymakerDirectory = Google2uGUIUtil.GetString("g2uplaymakerDirectory", _PlaymakerDirectory);

            if (Directory.GetFiles(Application.dataPath, "NGUITools.cs", SearchOption.AllDirectories).Length > 0)
                FoundNgui = true;

            if (Directory.GetFiles(Application.dataPath, "PlayMaker.dll", SearchOption.AllDirectories).Length > 0)
                FoundPlaymaker = true;
        }

        public void LoadLanguageSettings()
        {
            LocalizationInfo.LanguageOptions = new string[Language.languageStrings.GetUpperBound(0)];
            for (var i = 0; i < Language.languageStrings.GetUpperBound(0); ++i)
            {
                LocalizationInfo.LanguageOptions[i] = Language.languageStrings[i, 0];
            }

            LocalizationInfo.LanguagesIndex = Google2uGUIUtil.GetInt("languagesIndex", LocalizationInfo.LanguagesIndex);
            LocalizationInfo.EditorLanguage = Google2uGUIUtil.GetString("editorLanguage",
                LocalizationInfo.EditorLanguage);
            if (Language.GetLanguageCode(LocalizationInfo.EditorLanguage) != Language.Code.INVALID)
                return;
            LocalizationInfo.EditorLanguage = "en";
            Google2uGUIUtil.SetString("editorLanguage", LocalizationInfo.EditorLanguage);
        }

        public void LoadManualWorkbooks()
        {
            // Manual Workbook Cache
            var manualWorkbookCache = Google2uGUIUtil.GetString(InstanceData.ProjectPath + "_ManualWorkbookCache",
                string.Empty);
            if (string.IsNullOrEmpty(manualWorkbookCache)) return;
            InstanceData.ManualWorkbookCache = manualWorkbookCache;

            InstanceData.Commands.Add(GFCommand.RetrieveManualWorkbooks);
        }

        public void LoadVisibilitySettings()
        {
            ShowRSS = Google2uGUIUtil.GetBool("ShowRSS", ShowRSS);
            ShowLogin = Google2uGUIUtil.GetBool("ShowLogin", ShowLogin);
            ShowLoginCredentials = Google2uGUIUtil.GetBool("ShowLogin", ShowLoginCredentials);

            ShowSettings = Google2uGUIUtil.GetBool("ShowSettings", ShowSettings);
            ShowSettingsLanguage = Google2uGUIUtil.GetBool("ShowSettingsLanguage", ShowSettingsLanguage);
            ShowSettingsPaths = Google2uGUIUtil.GetBool("ShowSettingsPaths", ShowSettingsPaths);

            ShowSettingsPathGameObjectExport = Google2uGUIUtil.GetBool("ShowSettingsPathGameObjectExport",
                ShowSettingsPathGameObjectExport);
            ShowSettingsPathStaticExport = Google2uGUIUtil.GetBool("ShowSettingsPathStaticExport",
                ShowSettingsPathStaticExport);
            ShowSettingsPathXMLExport = Google2uGUIUtil.GetBool("ShowSettingsPathXMLExport", ShowSettingsPathXMLExport);
            ShowSettingsPathJSONExport = Google2uGUIUtil.GetBool("ShowSettingsPathJSONExport",
                ShowSettingsPathJSONExport);
            ShowSettingsPathCSVExport = Google2uGUIUtil.GetBool("ShowSettingsPathCSVExport", ShowSettingsPathCSVExport);
            ShowSettingsPathNGUIExport = Google2uGUIUtil.GetBool("ShowSettingsPathNGUIExport",
                ShowSettingsPathNGUIExport);
            ShowSettingsPathPlaymakerExport = Google2uGUIUtil.GetBool("ShowSettingsPathPlaymakerExport",
                ShowSettingsPathPlaymakerExport);

            ShowWorkbooks = Google2uGUIUtil.GetBool("ShowWorkbooks", ShowWorkbooks);
            ShowWorkbooksAccount = Google2uGUIUtil.GetBool("ShowWorkbooksAccount", ShowWorkbooksAccount);
            ShowWorkbooksManual = Google2uGUIUtil.GetBool("ShowWorkbooksManual", ShowWorkbooksManual);
            ShowWorkbooksUpload = Google2uGUIUtil.GetBool("ShowWorkbooksUpload", ShowWorkbooksUpload);
            ShowHiddenWorkbooks = Google2uGUIUtil.GetBool("ShowHiddenWorkbooks", ShowHiddenWorkbooks);

            ShowTools = Google2uGUIUtil.GetBool("ShowTools", ShowTools);
            ShowToolsOptions = Google2uGUIUtil.GetBool("ShowToolsOptions", ShowToolsOptions);
            ShowToolsWorksheets = Google2uGUIUtil.GetBool("ShowToolsWorksheets", ShowToolsWorksheets);

            ShowHelp = Google2uGUIUtil.GetBool("ShowHelp", ShowHelp);
            ShowHelpContact = Google2uGUIUtil.GetBool("ShowHelpContact", ShowHelpContact);
            ShowHelpDocs = Google2uGUIUtil.GetBool("ShowHelpDocs", ShowHelpDocs);
            ShowHelpLog = Google2uGUIUtil.GetBool("ShowHelpLog", ShowHelpLog);
        }

        public void OnEnable()
        {
            EditorApplication.update += Update;
            MyGUILayout = new EditorGUILayoutEx();
            EditorGUILayoutEx.GuiEditor = this;

            LoadStyles();
            LoadVisibilitySettings();
            LoadLanguageSettings();
            LoadPathSettings();

            LoadManualWorkbooks();
        }

        private void DrawGoogleRSSGUI()
        {
            if (string.IsNullOrEmpty(GoogleRSSMessage))
                return;

            var rssFadeArea = MyGUILayout.BeginFadeArea(ShowRSS,
                "Important Information from Google",
                "GoogleRSS", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
            ShowRSS = rssFadeArea.Open;
            Google2uGUIUtil.SetBool("ShowRSS", ShowRSS);
            if (rssFadeArea.Show())
            {
                EditorGUILayout.HelpBox(GoogleRSSMessage, MessageType.Warning);
            }
            MyGUILayout.EndFadeArea();
        }

        private static void SetupParameters()
        {
            _authParameters = new OAuth2Parameters
            {
                ClientId = "1018826106343-cfe56meceiie1mjichtcs94dc70i00jl.apps.googleusercontent.com",
                ClientSecret = "szBJS798e4Jz8OXOKjjqdPss",
                RedirectUri = "urn:ietf:wg:oauth:2.0:oob",
                Scope =
                    "https://spreadsheets.google.com/feeds https://docs.google.com/feeds https://www.googleapis.com/auth/drive",
                RefreshToken = RefreshToken,
                AccessToken = OAuthToken,
                AccessType = "offline",
                TokenType = "refresh"
            };
        }

        private static void OpenAuthWindow()
        {
            if (_authParameters == null)
                SetupParameters();
            TempTokenURL = OAuthUtil.CreateOAuth2AuthorizationUrl(_authParameters);
            Application.OpenURL(TempTokenURL);
        }

        private void DrawLoginGUI()
        {
            if (Instance == null)
                Init();
            var loginFadeArea = InstanceData.Service == null
                ? MyGUILayout.BeginFadeArea(ShowLogin, LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_LOGIN),
                    "login", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader)
                : MyGUILayout.BeginFadeArea(ShowLogin,
                    LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_LOGGED_IN_AS), "login",
                    MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
            ShowLogin = loginFadeArea.Open;
            Google2uGUIUtil.SetBool("ShowLogin", ShowLogin);
            if (loginFadeArea.Show())
            {
                var showLoginCredentialsArea = MyGUILayout.BeginFadeArea(ShowLoginCredentials,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CREDENTIALS), "credentials",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowLoginCredentials = showLoginCredentialsArea.Open;
                Google2uGUIUtil.SetBool("ShowLoginCredentials", ShowLoginCredentials);
                if (InstanceData.Commands.Contains(GFCommand.WaitForLogin))
                {
                    EditorGUILayout.LabelField(
                        LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_PROCESSING_LOGIN) + Ellipses);
                }
                else if (InstanceData.Commands.Contains(GFCommand.WaitForRetrievingWorkbooks))
                {
                    EditorGUILayout.LabelField(
                        LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_RETRIEVING_WORKBOOKS) + Ellipses);
                }
                else
                {
                    if (InstanceData.Service == null)
                    {
                        var content = new GUIContent(MyGUILayout.LoginButton);
                        if (!string.IsNullOrEmpty(TempTokenURL))
                        {
                            EditorGUILayout.BeginHorizontal();
                            TempToken = EditorGUILayout.TextField("OAuth Token: ", TempToken);
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.CredentialButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.CredentialButtonWidth)))
                            {
                                if (!string.IsNullOrEmpty(TempToken))
                                {
                                    _authParameters.AccessCode = TempToken;
                                    TempToken = string.Empty;

                                    OAuthUtil.GetAccessToken(_authParameters);

                                    if (!string.IsNullOrEmpty(_authParameters.AccessToken))
                                    {
                                        OAuthToken = _authParameters.AccessToken;
                                        RefreshToken = _authParameters.RefreshToken;
                                        RefreshTimeout = _authParameters.TokenExpiry;
                                        Google2uGUIUtil.SetString("OAuthToken", OAuthToken);
                                        Google2uGUIUtil.SetString("RefreshToken", RefreshToken);
                                        Google2uGUIUtil.SetString("RefreshTimeout", RefreshTimeout.ToString("O"));

                                        var requestFactory = new GOAuth2RequestFactory("structuredcontent",
                                            "Google2Unity", _authParameters);
                                        InstanceData.Service = new SpreadsheetsService("Google2Unity")
                                        {
                                            RequestFactory = requestFactory
                                        };

                                        if (!InstanceData.Commands.Contains(GFCommand.RetrieveWorkbooks))
                                            InstanceData.Commands.Add(GFCommand.RetrieveWorkbooks);
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        content = new GUIContent(MyGUILayout.GoogleAuthButton);

                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.GoogleButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.GoogleButtonWidth)))
                        {
                            if (_authParameters == null)
                                SetupParameters();
                            if (!string.IsNullOrEmpty(_authParameters.AccessToken))
                            {
                                OAuthToken = _authParameters.AccessToken;
                                Google2uGUIUtil.SetString("OAuthToken", OAuthToken);

                                var requestFactory = new GOAuth2RequestFactory("structuredcontent", "Google2Unity",
                                    _authParameters);
                                InstanceData.Service = new SpreadsheetsService("Google2Unity")
                                {
                                    RequestFactory = requestFactory
                                };

                                Thread.Sleep(100);

                                if (!InstanceData.Commands.Contains(GFCommand.RetrieveWorkbooks))
                                    InstanceData.Commands.Add(GFCommand.RetrieveWorkbooks);
                            }
                            else
                            {
                                OpenAuthWindow();
                            }
                        }

                        content = new GUIContent(MyGUILayout.ClearButton);
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.CredentialButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.CredentialButtonWidth)))
                        {
                            OAuthToken = string.Empty;
                            TempTokenURL = string.Empty;
                            _authParameters = null;
                            Google2uGUIUtil.SetString("OAuthToken", string.Empty);
                            InstanceData.AccountWorkbooks.Clear();
                            InstanceData.Service = null;
                        }
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        var content = new GUIContent(MyGUILayout.LogoutButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_LOGOUT));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.CredentialButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.CredentialButtonWidth)))
                        {
                            if (!InstanceData.Commands.Contains(GFCommand.DoLogout))
                            {
                                InstanceData.Commands.Add(GFCommand.DoLogout);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                MyGUILayout.EndFadeArea(); // End Credentials
            }
            MyGUILayout.EndFadeArea();
        }

        private void DrawSettingsGUI()
        {
            var settingsFadeArea = MyGUILayout.BeginFadeArea(ShowSettings,
                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_SETTINGS), "settings", MyGUILayout.OuterBox,
                MyGUILayout.OuterBoxHeader);
            ShowSettings = settingsFadeArea.Open;
            Google2uGUIUtil.SetBool("ShowSettings", ShowSettings);
            if (settingsFadeArea.Show())
            {
                var showSettingsLanguage = MyGUILayout.BeginFadeArea(ShowSettingsLanguage,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_LANGUAGE), "settingsLanguage",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowSettingsLanguage = showSettingsLanguage.Open;
                Google2uGUIUtil.SetBool("ShowSettingsLanguage", ShowSettingsLanguage);
                if (showSettingsLanguage.Show())
                {
                    var oldIdx = LocalizationInfo.LanguagesIndex;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EDITOR_LANGUAGE) + ": ",
                        GUILayout.Width(100));


                    GUI.SetNextControlName("Clear");
                    LocalizationInfo.LanguagesIndex = EditorGUILayout.Popup(LocalizationInfo.LanguagesIndex,
                        LocalizationInfo.LanguageOptions);

                    if (oldIdx != LocalizationInfo.LanguagesIndex)
                    {
                        LocalizationInfo.EditorLanguage = Language.languageStrings[LocalizationInfo.LanguagesIndex, 1];
                        Google2uGUIUtil.SetString("editorLanguage", LocalizationInfo.EditorLanguage);
                        Google2uGUIUtil.SetInt("languagesIndex", LocalizationInfo.LanguagesIndex);
                        GUI.FocusControl("Clear");
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                MyGUILayout.EndFadeArea();

                var showSettingsPaths = MyGUILayout.BeginFadeArea(ShowSettingsPaths,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORTERS), "settingsPaths",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowSettingsPaths = showSettingsPaths.Open;
                Google2uGUIUtil.SetBool("ShowSettingsPaths", ShowSettingsPaths);
                if (showSettingsPaths.Show())
                {
                    GUI.SetNextControlName("PathRefocus");
                    if (GUILayout.Button(LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GENERATE_PATHS),
                        EditorStyles.toolbarButton))
                    {
                        Google2uGenPath("Google2uGen");
                        Google2uGenPath("ObjDBResources");
                        Google2uGenPath("ObjDBEditor");
                        Google2uGenPath("StaticDBResources");
                        Google2uGenPath("JSON");
                        Google2uGenPath("CSV");
                        Google2uGenPath("XML");
                        if (FoundNgui)
                            Google2uGenPath("NGUI");
                        if (FoundPlaymaker)
                            Google2uGenPath("PLAYMAKER");
                        GUI.FocusControl("PathRefocus");
                        Repaint();
                    }

                    var showSettingsPathGameObjectExport = MyGUILayout.BeginFadeArea(ShowSettingsPathGameObjectExport,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_GAME_OBJECT_DATABASE) + " " +
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT),
                        "showSettingsPathGameObjectExport", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
                    ShowSettingsPathGameObjectExport = showSettingsPathGameObjectExport.Open;
                    Google2uGUIUtil.SetBool("ShowSettingsPathGameObjectExport", ShowSettingsPathGameObjectExport);
                    if (showSettingsPathGameObjectExport.Show())
                    {
                        EditorGUILayout.Separator();
                        var bDoSave = true;

                        EditorGUILayout.LabelField(
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_OBJECT_DATABASE) + " " +
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_RESOURCES_DIR) + ":");
                        EditorGUILayout.BeginHorizontal();
                        var oldObjDbResourcesDirectory = _ObjDbResourcesDirectory;

                        var content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var objDbResourcesDirectory = EditorUtility.SaveFolderPanel(
                                oldObjDbResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((objDbResourcesDirectory.Length == 0) ||
                                (objDbResourcesDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0) ||
                                (objDbResourcesDirectory.IndexOf("/RESOURCES",
                                    StringComparison.InvariantCultureIgnoreCase) == -1))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_RESOURCES_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldObjDbResourcesDirectory != objDbResourcesDirectory))
                            {
                                Google2uGUIUtil.SetString("g2uobjDBResourcesDirectory", objDbResourcesDirectory);
                                _ObjDbResourcesDirectory = objDbResourcesDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_ObjDbResourcesDirectory);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Separator();

                        EditorGUILayout.LabelField(
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_OBJECT_DATABASE) + " " +
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EDITOR_DIR) + ": ");
                        EditorGUILayout.BeginHorizontal();
                        var oldObjDbEditorDirectory = _ObjDbEditorDirectory;

                        content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var objDbEditorDirectory = EditorUtility.SaveFolderPanel(
                                oldObjDbEditorDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((objDbEditorDirectory.Length == 0) ||
                                (objDbEditorDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0) ||
                                (objDbEditorDirectory.IndexOf("/EDITOR",
                                    StringComparison.InvariantCultureIgnoreCase) == -1))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_EDITOR_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldObjDbEditorDirectory != objDbEditorDirectory))
                            {
                                Google2uGUIUtil.SetString("g2uobjDBEditorDirectory", objDbEditorDirectory);
                                _ObjDbEditorDirectory = objDbEditorDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_ObjDbEditorDirectory);
                        EditorGUILayout.EndHorizontal();
                    }
                    MyGUILayout.EndFadeArea();

                    var showSettingsPathStaticExport = MyGUILayout.BeginFadeArea(ShowSettingsPathStaticExport,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_STATIC_DATABASE) + " " +
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT), "showSettingsPathStaticExport",
                        MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
                    ShowSettingsPathStaticExport = showSettingsPathStaticExport.Open;
                    Google2uGUIUtil.SetBool("ShowSettingsPathStaticExport", ShowSettingsPathStaticExport);
                    if (showSettingsPathStaticExport.Show())
                    {
                        EditorGUILayout.Separator();
                        var bDoSave = true;

                        EditorGUILayout.LabelField(
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_STATIC_DATABASE) + " " +
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_RESOURCES_DIR) + ": ");
                        EditorGUILayout.BeginHorizontal();
                        var oldStaticDbResourcesDirectory = _StaticDbResourcesDirectory;

                        var content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var staticDbResourcesDirectory = EditorUtility.SaveFolderPanel(
                                oldStaticDbResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((staticDbResourcesDirectory.Length == 0) ||
                                (staticDbResourcesDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0) ||
                                (staticDbResourcesDirectory.IndexOf("/RESOURCES",
                                    StringComparison.InvariantCultureIgnoreCase) == -1))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_RESOURCES_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldStaticDbResourcesDirectory != staticDbResourcesDirectory))
                            {
                                Google2uGUIUtil.SetString("g2uStaticDBResourcesDirectory", staticDbResourcesDirectory);
                                _StaticDbResourcesDirectory = staticDbResourcesDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_StaticDbResourcesDirectory);
                        EditorGUILayout.EndHorizontal();
                    }
                    MyGUILayout.EndFadeArea();

                    var showSettingsPathXMLExport = MyGUILayout.BeginFadeArea(ShowSettingsPathXMLExport,
                        "XML " + LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT),
                        "showSettingsPathXMLExport", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
                    ShowSettingsPathXMLExport = showSettingsPathXMLExport.Open;
                    Google2uGUIUtil.SetBool("ShowSettingsPathXMLExport", ShowSettingsPathXMLExport);
                    if (showSettingsPathXMLExport.Show())
                    {
                        EditorGUILayout.Separator();
                        var bDoSave = true;

                        EditorGUILayout.LabelField("XML " +
                                                   LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_DIR) +
                                                   ": ");
                        EditorGUILayout.BeginHorizontal();
                        var oldXMLResourcesDirectory = _XmlDirectory;

                        var content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var xmlDirectory = EditorUtility.SaveFolderPanel(
                                oldXMLResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((xmlDirectory.Length == 0) ||
                                (xmlDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldXMLResourcesDirectory != xmlDirectory))
                            {
                                Google2uGUIUtil.SetString("g2uxmlDirectory", xmlDirectory);
                                _XmlDirectory = xmlDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_XmlDirectory);
                        EditorGUILayout.EndHorizontal();
                    }
                    MyGUILayout.EndFadeArea();

                    var showSettingsPathJSONExport = MyGUILayout.BeginFadeArea(ShowSettingsPathJSONExport,
                        "JSON " + LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT),
                        "showSettingsPathJSONExport", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
                    ShowSettingsPathJSONExport = showSettingsPathJSONExport.Open;
                    Google2uGUIUtil.SetBool("ShowSettingsPathJSONExport", ShowSettingsPathJSONExport);
                    if (showSettingsPathJSONExport.Show())
                    {
                        EditorGUILayout.Separator();
                        var bDoSave = true;

                        EditorGUILayout.LabelField("JSON " +
                                                   LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_DIR) +
                                                   ": ");
                        EditorGUILayout.BeginHorizontal();
                        var oldJSONResourcesDirectory = _JsonDirectory;

                        var content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var jsonDirectory = EditorUtility.SaveFolderPanel(
                                oldJSONResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((jsonDirectory.Length == 0) ||
                                (jsonDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldJSONResourcesDirectory != jsonDirectory))
                            {
                                Google2uGUIUtil.SetString("g2ujsonDirectory", jsonDirectory);
                                _JsonDirectory = jsonDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_JsonDirectory);
                        EditorGUILayout.EndHorizontal();
                    }
                    MyGUILayout.EndFadeArea();

                    var showSettingsPathCSVExport = MyGUILayout.BeginFadeArea(ShowSettingsPathCSVExport,
                        "CSV " + LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT),
                        "showSettingsPathCSVExport", MyGUILayout.OuterBox, MyGUILayout.OuterBoxHeader);
                    ShowSettingsPathCSVExport = showSettingsPathCSVExport.Open;
                    Google2uGUIUtil.SetBool("ShowSettingsPathCSVExport", ShowSettingsPathCSVExport);
                    if (showSettingsPathCSVExport.Show())
                    {
                        var bDoSave = true;

                        EditorGUILayout.LabelField("CSV " +
                                                   LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_DIR) +
                                                   ": ");
                        EditorGUILayout.BeginHorizontal();
                        var oldCSVResourcesDirectory = _CsvDirectory;

                        var content = new GUIContent(MyGUILayout.BrowseButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            var csvDirectory = EditorUtility.SaveFolderPanel(
                                oldCSVResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                            if ((csvDirectory.Length == 0) ||
                                (csvDirectory.IndexOf(InstanceData.ProjectPath,
                                    StringComparison.InvariantCultureIgnoreCase) != 0))
                            {
                                InstanceData.Messages.Add(new G2GUIMessage(
                                    GFGUIMessageType.InvalidEditorDirectory,
                                    LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_DIR)));
                                bDoSave = false;
                            }

                            if ((bDoSave) && (oldCSVResourcesDirectory != csvDirectory))
                            {
                                Google2uGUIUtil.SetString("g2ucsvDirectory", csvDirectory);
                                _CsvDirectory = csvDirectory;
                            }
                        }

                        EditorGUILayout.LabelField(_CsvDirectory);
                        EditorGUILayout.EndHorizontal();
                    }
                    MyGUILayout.EndFadeArea();

                    if (FoundNgui)
                    {
                        var showSettingsPathNGUIExport = MyGUILayout.BeginFadeArea(ShowSettingsPathNGUIExport,
                            "NGUI " + LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT),
                            "showSettingsPathNGUIExport", MyGUILayout.OuterBox,
                            MyGUILayout.OuterBoxHeader);
                        ShowSettingsPathNGUIExport = showSettingsPathNGUIExport.Open;
                        Google2uGUIUtil.SetBool("ShowSettingsPathNGUIExport", ShowSettingsPathNGUIExport);
                        if (showSettingsPathNGUIExport.Show())
                        {
                            EditorGUILayout.Separator();
                            var bDoSave = true;

                            EditorGUILayout.LabelField("NGUI " +
                                                       LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_DIR) +
                                                       ": ");
                            EditorGUILayout.BeginHorizontal();
                            var oldNGUIResourcesDirectory = _NguiDirectory;

                            var content = new GUIContent(MyGUILayout.BrowseButton,
                                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                var nguiDirectory = EditorUtility.SaveFolderPanel(
                                    oldNGUIResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                                if ((nguiDirectory.Length == 0) ||
                                    (nguiDirectory.IndexOf(InstanceData.ProjectPath,
                                        StringComparison.InvariantCultureIgnoreCase) != 0))
                                {
                                    InstanceData.Messages.Add(new G2GUIMessage(
                                        GFGUIMessageType.InvalidEditorDirectory,
                                        LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_DIR)));
                                    bDoSave = false;
                                }

                                if ((bDoSave) && (oldNGUIResourcesDirectory != nguiDirectory))
                                {
                                    Google2uGUIUtil.SetString("g2unguiDirectory", nguiDirectory);
                                    _NguiDirectory = nguiDirectory;
                                }
                            }

                            EditorGUILayout.LabelField(_NguiDirectory);
                            EditorGUILayout.EndHorizontal();
                        }
                        MyGUILayout.EndFadeArea();
                    }

                    if (FoundPlaymaker)
                    {
                        var showSettingsPathPlaymakerExport = MyGUILayout.BeginFadeArea(ShowSettingsPathPlaymakerExport,
                            "Playmaker Export", "showSettingsPathPlaymakerExport", MyGUILayout.OuterBox,
                            MyGUILayout.OuterBoxHeader);
                        ShowSettingsPathPlaymakerExport = showSettingsPathPlaymakerExport.Open;
                        Google2uGUIUtil.SetBool("ShowSettingsPathPlaymakerExport", ShowSettingsPathPlaymakerExport);
                        if (showSettingsPathPlaymakerExport.Show())
                        {
                            EditorGUILayout.Separator();
                            var bDoSave = true;

                            EditorGUILayout.LabelField("Playmaker " +
                                                       LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_EXPORT_DIR) +
                                                       ": ");
                            EditorGUILayout.BeginHorizontal();
                            var oldPlaymakerResourcesDirectory = _PlaymakerDirectory;

                            var content = new GUIContent(MyGUILayout.BrowseButton,
                                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FOLDER));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                var playmakerDirectory = EditorUtility.SaveFolderPanel(
                                    oldPlaymakerResourcesDirectory, EditorApplication.applicationPath, string.Empty);

                                if ((playmakerDirectory.Length == 0) ||
                                    (playmakerDirectory.IndexOf(InstanceData.ProjectPath,
                                        StringComparison.InvariantCultureIgnoreCase) != 0))
                                {
                                    InstanceData.Messages.Add(new G2GUIMessage(
                                        GFGUIMessageType.InvalidEditorDirectory,
                                        LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_DIR)));
                                    bDoSave = false;
                                }

                                if ((bDoSave) && (oldPlaymakerResourcesDirectory != playmakerDirectory))
                                {
                                    Google2uGUIUtil.SetString("g2uplaymakerDirectory", playmakerDirectory);
                                    _PlaymakerDirectory = playmakerDirectory;
                                }
                            }

                            EditorGUILayout.LabelField(_PlaymakerDirectory);
                            EditorGUILayout.EndHorizontal();
                        }
                        MyGUILayout.EndFadeArea();
                    }
                }
                MyGUILayout.EndFadeArea();

                var showSettingsPrefs = MyGUILayout.BeginFadeArea(ShowSettingsPrefs, "Preferences", "settingsPrefs",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowSettingsPrefs = showSettingsPrefs.Open;
                Google2uGUIUtil.SetBool("ShowSettingsPrefs", ShowSettingsPrefs);
                if (showSettingsPrefs.Show())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Load Preferences From File: ");
                    if (GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.Width(100)))
                    {
                        var prefsPath = EditorUtility.OpenFilePanel(
                            "g2uPrefs", EditorApplication.applicationPath, "g2uPrefs.bin;g2uPrefs.bin");

                        if (!string.IsNullOrEmpty(prefsPath))
                        {
                            if (Google2uGUIUtil.Load(prefsPath))
                            {
                                LoadVisibilitySettings();
                                LoadLanguageSettings();
                                LoadPathSettings();

                                LoadManualWorkbooks();

                                PushNotification("Loaded Preferences");
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                MyGUILayout.EndFadeArea();
            }
            MyGUILayout.EndFadeArea();
        }

        private void DrawWorkbooksGUI()
        {
            // WORKBOOKS
            var workbooksFadeArea = MyGUILayout.BeginFadeArea(ShowWorkbooks,
                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_WORKBOOKS), "workbooks", MyGUILayout.OuterBox,
                MyGUILayout.OuterBoxHeader);
            ShowWorkbooks = workbooksFadeArea.Open;
            Google2uGUIUtil.SetBool("ShowWorkbooks", ShowWorkbooks);
            if (workbooksFadeArea.Show())
            {
                if (InstanceData.Service != null)
                {
                    // Account
                    var showWorkbooksAccount = MyGUILayout.BeginFadeArea(ShowWorkbooksAccount,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ACCOUNT_WORKBOOKS),
                        "workbooksAccount", MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                    ShowWorkbooksAccount = showWorkbooksAccount.Open;
                    Google2uGUIUtil.SetBool("ShowWorkbooksAccount", ShowWorkbooksAccount);
                    if (showWorkbooksAccount.Show())
                    {
                        if (InstanceData.Commands.Contains(GFCommand.WaitForRetrievingWorkbooks))
                        {
                            EditorGUILayout.LabelField(
                                LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_RETRIEVING_WORKBOOKS) +
                                Ellipses);
                        }
                        else
                        {
                            if (
                                GUILayout.Button(LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_RELOAD_WORKBOOKS)))
                            {
                                InstanceData.Commands.Add(GFCommand.RetrieveWorkbooks);
                            }
                            else
                            {
                                ShowHiddenWorkbooks = Google2uGUIUtil.GetBool("ShowHiddenWorkbooks", ShowHiddenWorkbooks);
                                var tmp = EditorGUILayout.Toggle("Show Hidden Workbooks", ShowHiddenWorkbooks);
                                if (tmp != ShowHiddenWorkbooks)
                                {
                                    ShowHiddenWorkbooks = tmp;
                                    Google2uGUIUtil.SetBool("ShowHiddenWorkbooks", ShowHiddenWorkbooks);
                                }

                                foreach (var google2UWorkbook in InstanceData.AccountWorkbooksDisplay)
                                {
                                    google2UWorkbook.DrawGUIList(MyGUILayout, ShowHiddenWorkbooks);
                                }
                            }
                        }
                    }
                    MyGUILayout.EndFadeArea();
                }

                // Manual
                var showWorkbooksManual = MyGUILayout.BeginFadeArea(ShowWorkbooksManual,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_MANUAL_WORKBOOKS), "workbooksManual",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowWorkbooksManual = showWorkbooksManual.Open;
                Google2uGUIUtil.SetBool("ShowWorkbooksManual", ShowWorkbooksManual);
                if (showWorkbooksManual.Show())
                {
                    if (InstanceData.Commands.Contains(GFCommand.WaitForRetrievingManualWorkbooks))
                    {
                        EditorGUILayout.LabelField(
                            LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_RETRIEVING_WORKBOOKS) + Ellipses);
                    }
                    else
                    {
                        var deleteWorkbooks =
                            InstanceData.ManualWorkbooksDisplay.Where(
                                in_google2USpreadsheet =>
                                    in_google2USpreadsheet.DrawGUIList(MyGUILayout, false) == false).ToList();

                        var refreshManualWorkbooks = false;
                        foreach (var google2USpreadsheet in deleteWorkbooks)
                        {
                            InstanceData.ManualWorkbooks.Remove(google2USpreadsheet);
                            refreshManualWorkbooks = true;
                        }

                        if (refreshManualWorkbooks)
                        {
                            InstanceData.ManualWorkbookCache = string.Empty;
                            foreach (var google2USpreadsheet in InstanceData.ManualWorkbooks)
                            {
                                InstanceData.ManualWorkbookCache += google2USpreadsheet.WorkbookUrl + "|";
                            }
                            Google2uGUIUtil.SetString(InstanceData.ProjectPath + "_ManualWorkbookCache",
                                InstanceData.ManualWorkbookCache);
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("URL: ", GUILayout.Width(30));
                        InstanceData.ManualWorkbookUrl = EditorGUILayout.TextField(InstanceData.ManualWorkbookUrl);

                        var content = new GUIContent(MyGUILayout.AddButton,
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_ADD_WORKBOOK));
                        if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                            GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                        {
                            InstanceData.Messages.Clear();
                            AddManualWorkbookByUrl(InstanceData.ManualWorkbookUrl, InstanceData);

                            Google2uGUIUtil.SetString(InstanceData.ProjectPath + "_ManualWorkbookCache",
                                InstanceData.ManualWorkbookCache);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                MyGUILayout.EndFadeArea();

                if (InstanceData.Service != null)
                {
                    // Upload
                    var showWorkbooksUpload = MyGUILayout.BeginFadeArea(ShowWorkbooksUpload,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_UPLOAD_WORKBOOK),
                        "workbooksUpload", MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                    ShowWorkbooksUpload = showWorkbooksUpload.Open;
                    Google2uGUIUtil.SetBool("ShowWorkbooksUpload", ShowWorkbooksUpload);
                    if (showWorkbooksUpload.Show())
                    {
                        if (InstanceData.Commands.Contains(GFCommand.WaitingForUpload))
                        {
                            EditorGUILayout.LabelField(
                                LocalizationInfo.Localize(Localization.rowIds.ID_MESSAGE_UPLOADING_WORKBOOK) + ": " +
                                InstanceData.WorkbookUploadProgress + "% " +
                                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_COMPLETE) + Ellipses);
                        }
                        else
                        {
                            EditorGUILayout.BeginHorizontal();
                            var content = new GUIContent(MyGUILayout.BrowseButton,
                                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CHOOSE_FILE));
                            GUI.SetNextControlName("UploadClear");
                            if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Width(24)))
                            {
                                var workbookpath = EditorUtility.OpenFilePanel(
                                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_SELECT_WORKBOOK_PATH),
                                    EditorApplication.applicationPath, "*.xls;*.xlsx;*.ods;*.csv;*.txt;*.tsv");

                                if (!string.IsNullOrEmpty(workbookpath) && IsValidWorkbookPath(workbookpath))
                                {
                                    InstanceData.WorkbookUploadPath = workbookpath;
                                    GUI.FocusControl("UploadClear");
                                }
                            }
                            EditorGUILayout.TextField(InstanceData.WorkbookUploadPath);
                            content = new GUIContent(MyGUILayout.UploadButton,
                                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_UPLOAD_WORKBOOK));
                            if (GUILayout.Button(content, GUILayout.Height(EditorGUILayoutEx.ButtonHeight),
                                GUILayout.Width(EditorGUILayoutEx.ButtonWidth)))
                            {
                                InstanceData.Messages.Clear();
                                if (IsValidWorkbookPath(InstanceData.WorkbookUploadPath))
                                    InstanceData.Commands.Add(GFCommand.DoUpload);
                                else
                                    InstanceData.Messages.Add(new G2GUIMessage(GFGUIMessageType.InvalidUploadDirectory,
                                        LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_UPLOAD_DIR)));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    MyGUILayout.EndFadeArea();
                }
            }
            MyGUILayout.EndFadeArea();
        }

        private static bool IsValidWorkbookPath(string in_path)
        {
            return (in_path.IndexOf(".xls", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                   (in_path.IndexOf(".xlsx", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                   (in_path.IndexOf(".ods", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                   (in_path.IndexOf(".csv", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                   (in_path.IndexOf(".txt", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                   (in_path.IndexOf(".tsv", StringComparison.InvariantCultureIgnoreCase) != -1);
        }

        private void DrawHelpGUI()
        {
            var helpFadeArea = MyGUILayout.BeginFadeArea(ShowHelp,
                LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_HELP), "Help", MyGUILayout.OuterBox,
                MyGUILayout.OuterBoxHeader);
            ShowHelp = helpFadeArea.Open;
            Google2uGUIUtil.SetBool("ShowHelp", ShowHelp);
            if (helpFadeArea.Show())
            {
                var showHelpContact = MyGUILayout.BeginFadeArea(ShowHelpContact,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CONTACT), "HelpContact", MyGUILayout.InnerBox,
                    MyGUILayout.InnerBoxHeader);
                ShowHelpContact = showHelpContact.Open;
                Google2uGUIUtil.SetBool("ShowHelpContact", ShowHelpContact);
                if (showHelpContact.Show())
                {
                    EditorGUILayout.BeginHorizontal();
                    var content = new GUIContent(MyGUILayout.LitteratusLogo,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_BROWSE_LITTERATUS));
                    if (GUILayout.Button(content))
                    {
                        Application.OpenURL("http://www.litteratus.net");
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    content = new GUIContent(MyGUILayout.UnityLogo,
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_BROWSE_UNITY));
                    if (GUILayout.Button(content))
                    {
                        Application.OpenURL("http://www.unity3d.com");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField(
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_CREATED_WITH_UNITY) + " \u00a9 " +
                        LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_COPYRIGHT_UNITY));
                }
                MyGUILayout.EndFadeArea();

                var showHelpDocs = MyGUILayout.BeginFadeArea(ShowHelpDocs,
                    LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_DOCUMENTATION), "HelpDocs",
                    MyGUILayout.InnerBox, MyGUILayout.InnerBoxHeader);
                ShowHelpDocs = showHelpDocs.Open;
                Google2uGUIUtil.SetBool("ShowHelpDocs", ShowHelpDocs);
                if (showHelpDocs.Show())
                {
                    if (GUILayout.Button("Show Documentation"))
                    {
                        Google2uDocs.ShowWindow(MyGUILayout, LocalizationInfo);
                    }

                    ShowDocsAtStartup =
                        EditorGUILayoutEx.ToggleInput(
                            LocalizationInfo.Localize(Localization.rowIds.ID_LABEL_SHOWONSTARTUP), ShowDocsAtStartup,
                            "ShowDocsAtStartup");
                }
                MyGUILayout.EndFadeArea();
            }
            MyGUILayout.EndFadeArea();
        }

        private void OnGUI()
        {
            FinishedRedraw = Event.current.type == EventType.Layout;

            //Do some loading and checking
            if (!stylesLoaded)
            {
                if (!LoadStyles())
                {
                    GUILayout.Label("The folder " + EditorAssets +
                                    "/ was not found or some custom styles in it do not exist.");
                    return;
                }
            }

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64 &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXIntel &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXIntel64 &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXUniversal &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinux &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinux64 &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinuxUniversal &&
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
#if UNITY_5
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS &&
#else
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.iPhone &&
#endif

#if !UNITY_5_4_OR_NEWER
                EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player
#else
                true
#endif
                )
            {
                EditorGUILayout.HelpBox(LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_BUILD_TARGET),
                    MessageType.Error);
                return;
            }

            // Prevent Google2u from doing anything while in play mode
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox(
                    "Google2u is an Editor-Only application. Functionality is unavailable in Play Mode.",
                    MessageType.Info);
                return;
            }

            foreach (var gfguiMessage in InstanceData.Messages)
            {
                EditorGUILayout.LabelField(gfguiMessage.Message);
            }

            MyGUILayout.ClearStack();

            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
            DrawGoogleRSSGUI();
            DrawLoginGUI();
            DrawWorkbooksGUI();
            DrawSettingsGUI();
            DrawHelpGUI();
            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.Repaint)
                InstanceData.EndOfFrame();
        }

        public bool LoadStyles()
        {
            //Correct paths if necessary

            var projectPath = Application.dataPath;
            if (projectPath.EndsWith("/Assets"))
            {
                projectPath = projectPath.Remove(projectPath.Length - ("Assets".Length));
            }

            if (!Directory.Exists(projectPath + EditorAssets))
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
                    Debug.Log("Located editor assets folder at '" + projectPath + EditorAssets + "'");
                }
                else
                {
                    Debug.LogWarning("Could not locate editor assets folder\nGoogle2u");
                    return false;
                }
            }

            if (EditorGUIUtility.isProSkin)
            {
                Google2uSkin =
                    AssetDatabase.LoadAssetAtPath(EditorAssets + "/DarkSkin/Google2uDark.guiskin", typeof (GUISkin)) as
                        GUISkin;
            }
            else
            {
                Google2uSkin =
                    AssetDatabase.LoadAssetAtPath(EditorAssets + "/LightSkin/Google2uLight.guiskin", typeof (GUISkin))
                        as GUISkin;
            }

            var inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            if (Google2uSkin != null)
            {
                Google2uSkin.button = inspectorSkin.button;
            }
            else
            {
                Debug.LogWarning("Couldn't find Google2u Skin at " + EditorAssets);
                return false;
            }

            EditorGUILayoutEx.DefaultAreaStyle = MyGUILayout.InnerBox = Google2uSkin.FindStyle("InnerBox");

            if (EditorGUILayoutEx.DefaultAreaStyle == null)
            {
                Debug.LogWarning("Incorrect settings for Google2u Skin at " + EditorAssets);
                return false;
            }

            EditorGUILayoutEx.DefaultLabelStyle = MyGUILayout.InnerBoxHeader = Google2uSkin.FindStyle("InnerBoxHeader");

            MyGUILayout.OuterBoxHeader = Google2uSkin.FindStyle("OuterBoxHeader");
            MyGUILayout.OuterBox = Google2uSkin.FindStyle("OuterBox");

            MyGUILayout.CellButton = Google2uSkin.FindStyle("CellButton");
            MyGUILayout.CellButtonActive = Google2uSkin.FindStyle("CellButtonActive");

            MyGUILayout.CellHeader = Google2uSkin.FindStyle("CellHeader");
            MyGUILayout.CellTypeButton = Google2uSkin.FindStyle("CellTypeButton");
            MyGUILayout.CellInvalidButton = Google2uSkin.FindStyle("CellInvalidButton");
            MyGUILayout.PlusButton = Google2uSkin.FindStyle("PlusButton");
            MyGUILayout.MinusButton = Google2uSkin.FindStyle("MinusButton");
            MyGUILayout.HelpButtonStyle = Google2uSkin.FindStyle("HelpButtonStyle");
            MyGUILayout.InvalidWorksheetStyle = Google2uSkin.FindStyle("InvalidWorksheet");
            MyGUILayout.VisibilityButton = Google2uSkin.FindStyle("VisibilityButton");
            MyGUILayout.VisibilityHiddenButton = Google2uSkin.FindStyle("VisibilityHiddenButton");
            MyGUILayout.LoadIcons();

            MyGUILayout.LoadDocImages();

            stylesLoaded = true;

            return true;
        }

        public static string Indent(int in_amount, string in_string)
        {
            var s = string.Empty;
            const int indentSpaces = 4;
            for (var i = 0; i < in_amount*indentSpaces; ++i)
                s += " ";
            return s + in_string;
        }

#region Styles

        public static bool StylesLoaded = false;
        public static bool UseDarkSkin = false;
        public static bool stylesLoaded;
        public static GUISkin Google2uSkin;

#endregion

#region Paths

        private string _ObjDbResourcesDirectory;
        private string _ObjDbEditorDirectory;
        private string _StaticDbResourcesDirectory;
        private string _NguiDirectory;
        private string _XmlDirectory;
        private string _JsonDirectory;
        private string _CsvDirectory;
        private string _PlaymakerDirectory;
        public static bool FoundPlaymaker;
        public static bool FoundNgui;

#endregion

#region Menu Visibility

        // RSS notification area
        public bool ShowRSS = true;

        // Login menu
        public bool ShowLogin = true;
        public bool ShowLoginCredentials = true;

        // Settings Menu
        public bool ShowSettings;
        public bool ShowSettingsLanguage;
        public bool ShowSettingsPaths;
        public bool ShowSettingsPathGameObjectExport;
        public bool ShowSettingsPathStaticExport;
        public bool ShowSettingsPathXMLExport;
        public bool ShowSettingsPathJSONExport;
        public bool ShowSettingsPathCSVExport;
        public bool ShowSettingsPathNGUIExport;
        public bool ShowSettingsPathPlaymakerExport;
        public bool ShowSettingsPrefs;

        // Workbooks Menu
        public bool ShowWorkbooks;
        public bool ShowWorkbooksAccount;
        public bool ShowWorkbooksManual;
        public bool ShowWorkbooksUpload;
        public bool ShowHiddenWorkbooks;

        // Tools Menu
        public bool ShowTools;
        public bool ShowToolsOptions;
        public bool ShowToolsWorksheets;

        // Help Menu
        public bool ShowHelp;
        public bool ShowHelpContact;
        public bool ShowHelpDocs;
        public bool ShowHelpLog;

#endregion
    }
}