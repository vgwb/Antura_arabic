// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

using System.Runtime.Serialization.Formatters.Binary;

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Threading;
    using System.Security.Cryptography.X509Certificates;
    using Google.GData.Client.ResumableUpload;
    using Google.GData.Documents;
    using Google.GData.Client;
    using Google.GData.Spreadsheets;
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    #endregion

    public class Google2uLocalization
    {
        private string _EditorLanguage = "en";

        public Google2uLocalization()
        {
            LanguagesIndex = 0;
        }

        public string EditorLanguage
        {
            get { return _EditorLanguage; }
            set { _EditorLanguage = value; }
        }

        public int LanguagesIndex { get; set; }
        public string[] LanguageOptions { get; set; }

        public string Localize(Localization.rowIds in_textID)
        {
            var row = Localization.Instance.GetRow(in_textID);
            if (row != null)
            {
                return row.GetStringData(EditorLanguage);
            }
            return "Unable to find string ID: " + in_textID;
        }

        public string Localize(Documentation.rowIds in_textID)
        {
            var row = Documentation.Instance.GetRow(in_textID);
            if (row != null)
            {
                return row.GetStringData(EditorLanguage);
            }
            return "Unable to find string ID: " + in_textID;
        }
    }

    [Serializable]
    public class Google2uData
    {
        private Google2uAccountWorkbook[] _AccountWorkbooksDisplay = new Google2uAccountWorkbook[0];
        private Google2uManualWorkbook[] _ManualWorkbooksDisplay = new Google2uManualWorkbook[0];

        public Google2uData()
        {
            AccountWorkbooks = new List<Google2uAccountWorkbook>();
            ManualWorkbooks = new List<Google2uManualWorkbook>();
            Messages = new List<G2GUIMessage>();
            Commands = new List<GFCommand>();
        }

        public SpreadsheetsService Service { get; set; }
        public SpreadsheetsService ManualService { get; set; }
        public string ProjectPath { get; set; }
        public string WorkbookUploadPath { get; set; }
        public int WorkbookUploadProgress { get; set; }
        public string ManualWorkbookUrl { get; set; }
        public string ManualWorkbookCache { get; set; }
        public List<GFCommand> Commands { get; private set; }
        public List<G2GUIMessage> Messages { get; private set; }
        public List<Google2uAccountWorkbook> AccountWorkbooks { get; private set; }

        public Google2uAccountWorkbook[] AccountWorkbooksDisplay
        {
            get { return _AccountWorkbooksDisplay; }
        }

        public List<Google2uManualWorkbook> ManualWorkbooks { get; private set; }

        public Google2uManualWorkbook[] ManualWorkbooksDisplay
        {
            get { return _ManualWorkbooksDisplay; }
        }

        public void EndOfFrame()
        {
            _AccountWorkbooksDisplay = AccountWorkbooks.ToArray();
            foreach (var wb in _AccountWorkbooksDisplay)
            {
                wb.EndOfFrame();
            }

            _ManualWorkbooksDisplay = ManualWorkbooks.ToArray();
            foreach (var wb in _ManualWorkbooksDisplay)
            {
                wb.EndOfFrame();
            }
        }
    }

    [Serializable]
    public class Google2uObjDbExport
    {
        public bool CullEmptyRows = false;
        public bool CullEmptyCols = false;
        public string DataLocation;
        public DateTime LastAttempt = DateTime.Now;
        public string ObjectName;
        public bool ReloadedAssets;
        public string ScriptName;
    }

    [Serializable]
    [InitializeOnLoad]
    public partial class Google2u : EditorWindow
    {
        public static string TempTokenURL;
        public static string TempToken;
        public static string OAuthToken;
        public static string RefreshToken;
        public static DateTime RefreshTimeout;
        public static bool ShowDocsAtStartup = true;
        public static bool FinishedRedraw = false;
        public static DateTime LastCheckedRSS = DateTime.MinValue;
        public static Google2uLocalization LocalizationInfo = new Google2uLocalization();
        public static string Ellipses = "...";
        public static DateTime LastEllipses = DateTime.Now;
        public static int EllipsesCount = 3;
        public static Google2uEditor ActiveWorkbookWindow = null;
        private static string _NotificationString = string.Empty;
        public static string UpdateMessage = string.Empty;
        public static string GoogleRSSMessage = string.Empty;
        public int _ImportTryCount;
        [SerializeField] private Google2uData _InstanceData;
        public bool IsDirty;

        public Google2uData InstanceData
        {
            get
            {
                if (_InstanceData == null)
                    _InstanceData = new Google2uData();
                return _InstanceData;
            }
        }

        public List<Google2uObjDbExport> ObjDbExport { get; private set; }
        public static Google2u Instance { get; private set; }

        public static void PushNotification(string in_notify)
        {
            _NotificationString = "Google2u: " + in_notify;
        }

        public static string PopNotification()
        {
            var ret = _NotificationString;
            _NotificationString = string.Empty;
            return ret;
        }

        public static void CheckForService()
        {
            var feed = new GoogleAppStatus();
            foreach (var item in feed.RowNews.Items)
            {
                if (item.Title.Equals("Google Sheets"))
                {
                    GoogleRSSMessage = item.Description;
                    break;
                }
            }
        }

        public static void CheckForUpdate()
        {
            try
            {
                var ma = 2;
                var mi = 1;
                var bu = 12;

                var request =
                    WebRequest.Create("http://www.litteratus.net/CheckUpdate.php?ma=" + ma + "&mi=" + mi + "&bu=" + bu);
                var response = request.GetResponse();
                var data = response.GetResponseStream();
                string html;
                if (data == null) return;
                using (var sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }

                UpdateMessage = html;
            }
            catch (Exception)
            {
                UpdateMessage = "Unable to check for updates. Try again later.";
            }
        }

        [MenuItem("Window/Google2u")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            var Google2uWindow = GetWindow(typeof (Google2u));
#if(UNITY_4)
            Google2uWindow.title = "Google2u";
#elif(UNITY_5_0)
            Google2uWindow.title = "Google2u";
#else
            Google2uWindow.titleContent.text = "Google2u";
#endif
        }

        private void Init()
        {
            var lastChecked =
                Convert.ToDateTime(Google2uGUIUtil.GetString("Google2uLastCheckedForUpdate",
                    Convert.ToString(DateTime.MinValue)));
            if ((DateTime.Now - lastChecked).Days >= 1)
            {
                Google2uGUIUtil.SetString("Google2uLastCheckedForUpdate", Convert.ToString(DateTime.Now));
                var t = new Thread(CheckForUpdate);
                t.Start();
            }

            ShowDocsAtStartup = Google2uGUIUtil.GetBool("ShowDocsAtStartup", ShowDocsAtStartup);
            if (ShowDocsAtStartup)
                Google2uDocs.ShowWindow(MyGUILayout, LocalizationInfo);

            ServicePointManager.ServerCertificateValidationCallback = Validator;
            OAuthToken = Google2uGUIUtil.GetString("OAuthToken", OAuthToken);
            RefreshToken = Google2uGUIUtil.GetString("RefreshToken", RefreshToken);
            RefreshTimeout =
                DateTime.ParseExact(Google2uGUIUtil.GetString("RefreshTimeout", RefreshTimeout.ToString("O")), "O",
                    CultureInfo.InvariantCulture);

            if (InstanceData.Service == null && !string.IsNullOrEmpty(OAuthToken))
            {
                SetupParameters();

                _authParameters.AccessToken = OAuthToken;


                var requestFactory = new GOAuth2RequestFactory("structuredcontent", "Google2Unity", _authParameters);
                InstanceData.Service = new SpreadsheetsService("Google2Unity") {RequestFactory = requestFactory};

                Thread.Sleep(100);

                if (!InstanceData.Commands.Contains(GFCommand.RetrieveWorkbooks))
                    InstanceData.Commands.Add(GFCommand.RetrieveWorkbooks);
            }

            // Close lingering editor windows
            var ed = GetWindow<Google2uEditor>();
            if (ed != null)
                ed.Close();

            if (ObjDbExport == null)
                ObjDbExport = new List<Google2uObjDbExport>();

            Instance = this;
        }

        public void OnDestroy()
        {
            Google2uGUIUtil.Save();

            InstanceData.AccountWorkbooks.Clear();
            InstanceData.Service = null;

            EditorApplication.update -= Update;
            var ed = GetWindow<Google2uEditor>();
            if (ed != null)
                ed.Close();
            var help = GetWindow<Google2uDocs>();
            if (help != null)
                help.Close();
        }

        public static Type GetAssemblyType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null) return type;
            }
            return null;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Instance == null)
                Init();

            var notification = PopNotification();
            if (!string.IsNullOrEmpty(notification))
            {
                Debug.Log(notification);
                ShowNotification(new GUIContent(notification));
            }

            if (!string.IsNullOrEmpty(UpdateMessage))
            {
                Debug.Log(UpdateMessage);
                UpdateMessage = string.Empty;
            }

            if ((DateTime.Now - LastEllipses).Milliseconds > 500)
            {
                LastEllipses = DateTime.Now;
                EllipsesCount += 1;
                if (EllipsesCount > 5)
                    EllipsesCount = 2;
                Ellipses = string.Empty;
                for (var i = 0; i < EllipsesCount; ++i)
                    Ellipses += ".";
                Repaint();
            }

            // Detect Skin Changes
            var oldUseDarkSkin = UseDarkSkin;
            if (EditorGUIUtility.isProSkin)
            {
                UseDarkSkin = true;
                if (oldUseDarkSkin != UseDarkSkin)
                    LoadStyles();
            }

            if (IsDirty)
            {
                Google2uGUIUtil.Save();
                IsDirty = false;
            }

            // Prevent Google2u from doing anything while in play mode
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;

            if ((DateTime.Now - LastCheckedRSS).Hours > 0)
            {
                LastCheckedRSS = DateTime.Now;
                var t = new Thread(CheckForService);
                t.Start();
            }

            if (InstanceData.Commands.Contains(GFCommand.AssetDatabaseRefresh))
            {
                InstanceData.Commands.Remove(GFCommand.AssetDatabaseRefresh);
                AssetDatabase.Refresh();
            }

            if (InstanceData.Commands.Contains(GFCommand.DoLogout))
            {
                InstanceData.Commands.Remove(GFCommand.DoLogout);
                var t = new Thread(DoLogout) {Name = "DoLogout"};
                t.Start(InstanceData);
            }

            if (InstanceData.Commands.Contains(GFCommand.RetrieveWorkbooks))
            {
                InstanceData.Commands.Remove(GFCommand.RetrieveWorkbooks);
                InstanceData.Commands.Add(GFCommand.WaitForRetrievingWorkbooks);
                InstanceData.AccountWorkbooks.Clear();
                var t = new Thread(DoWorkbookQuery) {Name = "RetrieveWorkbooks"};
                t.Start(InstanceData);
            }

            if (InstanceData.Commands.Contains(GFCommand.RetrieveManualWorkbooks))
            {
                InstanceData.Commands.Remove(GFCommand.RetrieveManualWorkbooks);
                InstanceData.Commands.Add(GFCommand.WaitForRetrievingManualWorkbooks);
                var t = new Thread(DoManualWorkbookRetrieval) {Name = "ManualWorkbookRetrieval"};
                t.Start(InstanceData);
            }

            if (InstanceData.Commands.Contains(GFCommand.ManualWorkbooksRetrievalComplete))
            {
                InstanceData.Commands.Remove(GFCommand.ManualWorkbooksRetrievalComplete);
                var manualWorkbooksString = InstanceData.ManualWorkbooks.Aggregate(string.Empty,
                    (in_current, in_wb) => in_current + (in_wb.WorkbookUrl + "|"));
                Google2uGUIUtil.SetString(InstanceData.ProjectPath + "_ManualWorkbookCache", manualWorkbooksString);
            }

            if (InstanceData.Commands.Contains(GFCommand.DoUpload))
            {
                InstanceData.Commands.Remove(GFCommand.DoUpload);
                InstanceData.Commands.Add(GFCommand.WaitingForUpload);
                var t = new Thread(DoWorkbookUpload) {Name = "WorkbookUpload"};
                t.Start(InstanceData);
            }

            if (InstanceData.Commands.Contains(GFCommand.UploadComplete))
            {
                InstanceData.Commands.Remove(GFCommand.UploadComplete);
                InstanceData.WorkbookUploadProgress = 0;
                InstanceData.AccountWorkbooks.Clear();
                InstanceData.Commands.Add(GFCommand.RetrieveWorkbooks);
            }

            foreach (var Google2uSpreadsheet in InstanceData.AccountWorkbooksDisplay)
            {
                Google2uSpreadsheet.Update();
            }

            foreach (var Google2uSpreadsheet in InstanceData.ManualWorkbooksDisplay)
            {
                Google2uSpreadsheet.Update();
            }

            if (ObjDbExport != null && ObjDbExport.Count > 0)
            {
                var dbInfo = ObjDbExport[0];

                if (dbInfo == null)
                {
                    ObjDbExport.RemoveAt(0);
                    return;
                }

                if ((DateTime.Now - dbInfo.LastAttempt).TotalSeconds < 5)
                    return;

                if (dbInfo.ReloadedAssets == false)
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    dbInfo.ReloadedAssets = true;
                    return;
                }

                dbInfo.LastAttempt = DateTime.Now;

                Component comp = null;
                var myAssetPath = string.Empty;


                Debug.Log("Looking for Database Script: " + dbInfo.ScriptName);
                var findAssetArray = AssetDatabase.FindAssets(dbInfo.ScriptName);
                if (findAssetArray.Length > 0)
                {
                    foreach (var s in findAssetArray)
                    {
                        var mypath = AssetDatabase.GUIDToAssetPath(s);

                        if (mypath.EndsWith(".cs"))
                        {
                            myAssetPath = mypath;
                            Debug.Log("Found Database Script at: " + mypath);
                        }
                    }

                    var myType = GetAssemblyType("Google2u." + dbInfo.ScriptName);

                    Debug.Log(dbInfo.ScriptName + ": GetAssemblyType returns " + myType);
                    if (myType != null)
                    {
                        var go = GameObject.Find(dbInfo.ObjectName) ?? new GameObject(dbInfo.ObjectName);


                        var toDestroy = go.GetComponent(dbInfo.ScriptName);
                        if (toDestroy != null)
                            DestroyImmediate(toDestroy);

#if UNITY_5
                        comp = go.AddComponent(myType);
#else
                        comp = go.AddComponent(dbInfo.ScriptName);
#endif
                    }
                }


                if (comp == null)
                {
                    if (!string.IsNullOrEmpty(myAssetPath))
                    {
                        Debug.Log("Attempting to compile: " + myAssetPath);
                        AssetDatabase.ImportAsset(myAssetPath,
                            ImportAssetOptions.ForceSynchronousImport |
                            ImportAssetOptions.ForceUpdate);
                    }

                    if (_ImportTryCount < 5)
                    {
                        _ImportTryCount++;
                        return;
                    }
                    Debug.LogError("Could not add Google2u component base " + dbInfo.ScriptName);
                    ObjDbExport.Clear();
                    _ImportTryCount = 0;
                    return;
                }

                _ImportTryCount = 0;
                Debug.Log("Database Script Attached!");
                ObjDbExport.Remove(dbInfo);


                var tmpFilePath = dbInfo.DataLocation;
                using (var fs = File.Open(tmpFilePath, FileMode.Open, FileAccess.Read))
                {
                    var bin = new BinaryFormatter();
                    var binData = (List<List<string>>)bin.Deserialize(fs);

                    foreach (var binDataRow in binData)
                    {
                        (comp as Google2uComponentBase).AddRowGeneric(binDataRow);
                    }
                }

                try
                {
                    File.Delete(tmpFilePath);
                }
                catch (Exception ex)
                {
                    Debug.Log("G2U is unable to delete the temporary file: " + tmpFilePath + " - " + ex.Message);
                }
            }
        }

        public static bool SetupManualService(object in_instance)
        {
            var ret = false;
            var instance = in_instance as Google2uData;
            if (instance == null)
                return ret;
            ret = true;

            if (instance.ManualService != null)
                return ret;

            var svc = new SpreadsheetsService("Google2Unity");
            instance.ManualService = svc;

            return ret;
        }

        public void DoManualWorkbookRetrieval(object in_instance)
        {
            var instance = in_instance as Google2uData;
            if (instance == null)
                return;

            var cacheSplit = instance.ManualWorkbookCache.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in cacheSplit)
            {
                AddManualWorkbookByUrl(s, InstanceData);
            }
            instance.Commands.Remove(GFCommand.WaitForRetrievingManualWorkbooks);
            instance.Commands.Add(GFCommand.ManualWorkbooksRetrievalComplete);
        }

        private static void AddManualWorkbookByUrl(string in_manualUrl, Google2uData in_instance)
        {
            WorkbookBase info;
            if (string.IsNullOrEmpty(in_manualUrl))
            {
                Debug.LogError(LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_EMPTY_URL));
                return;
            }

            var refreshManualWorkbookCache = false;
            try
            {
                var key =
                    in_manualUrl.Substring(in_manualUrl.IndexOf("key=", StringComparison.InvariantCultureIgnoreCase) + 4);
                key = key.Split('&')[0];

                var singleQuery = new WorksheetQuery(key, "public", "values");

                if (in_instance.ManualService == null && !SetupManualService(in_instance))
                    return;

                var feed = in_instance.ManualService.Query(singleQuery);

                var finalUrl = in_manualUrl.Split('&')[0];

                if (feed != null)
                {
                    info =
                        in_instance.ManualWorkbooks.Find(in_i => Google2uGUIUtil.GfuStrCmp(in_i.WorkbookUrl, finalUrl)) ??
                        in_instance.AccountWorkbooks.Find(in_i => Google2uGUIUtil.GfuStrCmp(in_i.WorkbookUrl, finalUrl))
                            as WorkbookBase;
                    if (info == null)
                    {
                        var newWorkbook = new Google2uManualWorkbook(feed, finalUrl, feed.Title.Text,
                            in_instance.ManualService);
                        in_instance.ManualWorkbooks.Add(newWorkbook);

                        refreshManualWorkbookCache = true;
                    }
                }
            }
            catch
            {
                try
                {
                    var key =
                        in_manualUrl.Substring(
                            in_manualUrl.IndexOf("spreadsheets/d/", StringComparison.InvariantCultureIgnoreCase) + 15);
                    key = key.Split('/')[0];

                    if (in_instance.ManualService == null && !SetupManualService(in_instance))
                        return;

                    var singleQuery = new WorksheetQuery(key, "public", "values");
                    var feed = in_instance.ManualService.Query(singleQuery);
                    var urlParts = in_manualUrl.Split('/');

                    var finalUrl = "";
                    var urlBuild = 0;
                    string urlPart;
                    do
                    {
                        urlPart = urlParts[urlBuild];
                        finalUrl += urlPart + '/';
                        urlBuild++;
                    } while (urlPart != key);
                    if (feed != null)
                    {
                        info =
                            in_instance.ManualWorkbooks.Find(
                                in_i => Google2uGUIUtil.GfuStrCmp(in_i.WorkbookUrl, finalUrl)) ??
                            in_instance.AccountWorkbooks.Find(
                                in_i => Google2uGUIUtil.GfuStrCmp(in_i.WorkbookUrl, finalUrl)) as WorkbookBase;
                        if (info == null)
                        {
                            var newWorkbook = new Google2uManualWorkbook(feed, finalUrl, feed.Title.Text,
                                in_instance.ManualService);
                            in_instance.ManualWorkbooks.Add(newWorkbook);

                            refreshManualWorkbookCache = true;
                        }
                    }
                }
                catch
                {
                    Debug.LogError(LocalizationInfo.Localize(Localization.rowIds.ID_ERROR_INVALID_URL));
                }
            }

            if (refreshManualWorkbookCache)
            {
                in_instance.ManualWorkbookCache = string.Empty;
                foreach (var Google2uSpreadsheet in in_instance.ManualWorkbooks)
                {
                    in_instance.ManualWorkbookCache += Google2uSpreadsheet.WorkbookUrl + "|";
                }
            }
        }

        public void DoWorkbookUpload(object in_instance)
        {
            var instance = in_instance as Google2uData;
            if (instance == null)
                return;

            if (!string.IsNullOrEmpty(instance.WorkbookUploadPath))
            {
                try
                {
                    // We need a DocumentService
                    var service = new DocumentsService("Google2Unity");
                    var mimeType = Google2uMimeType.GetMimeType(instance.WorkbookUploadPath);

                    var authenticator = new OAuth2Authenticator("Google2Unity", _authParameters);

                    // Instantiate a DocumentEntry object to be inserted.
                    var entry = new DocumentEntry
                    {
                        MediaSource = new MediaFileSource(instance.WorkbookUploadPath, mimeType)
                    };

                    // Define the resumable upload link
                    var createUploadUrl =
                        new Uri("https://docs.google.com/feeds/upload/create-session/default/private/full");
                    var link = new AtomLink(createUploadUrl.AbsoluteUri)
                    {
                        Rel = ResumableUploader.CreateMediaRelation
                    };

                    entry.Links.Add(link);

                    // Set the service to be used to parse the returned entry
                    entry.Service = service;


                    // Instantiate the ResumableUploader component.
                    var uploader = new ResumableUploader();

                    // Set the handlers for the completion and progress events
                    uploader.AsyncOperationCompleted += OnSpreadsheetUploadDone;
                    uploader.AsyncOperationProgress += OnSpreadsheetUploadProgress;

                    // Start the upload process
                    uploader.InsertAsync(authenticator, entry, instance);
                }
                catch (Exception)
                {
                    PushNotification(
                        "There is a problem with your credentials. Clear the credentials and Re-Authorize G2U");
                    //instance.Messages.Add(new G2GUIMessage(GFGUIMessageType.InvalidLogin, ex.Message));
                    instance.Commands.Remove(GFCommand.WaitingForUpload);
                }
            }
        }

        private static void OnSpreadsheetUploadDone(object in_sender, AsyncOperationCompletedEventArgs in_e)
        {
            Instance.InstanceData.Commands.Remove(GFCommand.WaitingForUpload);
            Instance.InstanceData.Commands.Add(GFCommand.UploadComplete);
        }

        private static void OnSpreadsheetUploadProgress(object in_sender, AsyncOperationProgressEventArgs in_e)
        {
            Instance.InstanceData.WorkbookUploadProgress = in_e.ProgressPercentage;
        }

        public void DoWorkbookQuery(object in_instance)
        {
            var instance = in_instance as Google2uData;
            if (instance == null || InstanceData.Service == null)
                return;

            try
            {
                instance.AccountWorkbooks.Clear();
                var spreadsheetQuery = new Google.GData.Spreadsheets.SpreadsheetQuery();
                var spreadsheetFeed = InstanceData.Service.Query(spreadsheetQuery);

                foreach (var entry in spreadsheetFeed.Entries)
                {
                    var workbook = new Google2uAccountWorkbook(entry as SpreadsheetEntry, instance.Service);

                    instance.AccountWorkbooks.Add(workbook);
                }
            }
            catch (Exception)
            {
                if (!instance.Commands.Contains(GFCommand.DoLogout))
                {
                    instance.Commands.Add(GFCommand.DoLogout);
                }

                PushNotification("There is a problem with your credentials. Clear the credentials and Re-Authorize G2U");
                //instance.Messages.Add(new G2GUIMessage(GFGUIMessageType.InvalidLogin, ex.Message));
            }

            instance.Commands.Remove(GFCommand.WaitForRetrievingWorkbooks);
        }

        public void DoLogout(object in_instance)
        {
            var instance = in_instance as Google2uData;
            if (instance == null)
                return;

            instance.AccountWorkbooks.Clear();
            instance.Service = null;
            TempTokenURL = string.Empty;
        }

        public static bool Validator(object in_sender, X509Certificate in_certificate, X509Chain in_chain,
            SslPolicyErrors in_sslPolicyErrors)
        {
            return true;
        }

        public static string Google2uGenPath(string in_pathType)
        {
            var retPath = string.Empty;

            if (Google2uGUIUtil.GfuStrCmp(in_pathType, "Google2uGEN"))
            {
                retPath = Path.Combine(Instance.InstanceData.ProjectPath, "Google2uGen").Replace('\\', '/');
                if (!Directory.Exists(retPath))
                {
                    Debug.Log("Generating: " + retPath);
                    Directory.CreateDirectory(retPath);
                }
            } // Standard Assets
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "OBJDB"))
            {
                {
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "ObjDB").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "OBJDBRESOURCES"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2uobjDBResourcesDirectory", retPath);
                    if (!Directory.Exists(retPath))
                    {
                        var objdbPath = Google2uGenPath("OBJDB");

                        retPath = Path.Combine(objdbPath, "Resources").Replace('\\', '/');
                        if (!Directory.Exists(retPath))
                        {
                            Debug.Log("Generating: " + retPath);
                            Directory.CreateDirectory(retPath);
                        }

                        Google2uGUIUtil.SetString("g2uobjDBResourcesDirectory", retPath);
                    }
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "OBJDBEDITOR"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2uobjDBEditorDirectory", retPath);
                    if (!Directory.Exists(retPath))
                    {
                        var objdbPath = Google2uGenPath("OBJDB");

                        retPath = Path.Combine(objdbPath, "Editor").Replace('\\', '/');
                        if (!Directory.Exists(retPath))
                        {
                            Debug.Log("Generating: " + retPath);
                            Directory.CreateDirectory(retPath);
                        }

                        Google2uGUIUtil.SetString("g2uobjDBEditorDirectory", retPath);
                    }
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "STATICDB"))
            {
                {
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "StaticDB").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "STATICDBRESOURCES"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2uStaticDBResourcesDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    var staticdbPath = Google2uGenPath("STATICDB");

                    retPath = Path.Combine(staticdbPath, "Resources").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }

                    Google2uGUIUtil.SetString("g2uStaticDBResourcesDirectory", retPath);
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "JSON"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2ujsonDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "JSON").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }

                    Google2uGUIUtil.SetString("g2ujsonDirectory", retPath);
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "CSV"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2ucsvDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "CSV").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }

                    Google2uGUIUtil.SetString("g2ucsvDirectory", retPath);
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "XML"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2uxmlDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "XML").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }

                    Google2uGUIUtil.SetString("g2uxmlDirectory", retPath);
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "NGUI"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2unguiDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    var Google2ugenPath = Google2uGenPath("Google2uGEN");

                    retPath = Path.Combine(Google2ugenPath, "NGUI").Replace('\\', '/');
                    if (!Directory.Exists(retPath))
                    {
                        Debug.Log("Generating: " + retPath);
                        Directory.CreateDirectory(retPath);
                    }

                    Google2uGUIUtil.SetString("g2unguiDirectory", retPath);
                }
            }
            else if (Google2uGUIUtil.GfuStrCmp(in_pathType, "PLAYMAKER"))
            {
                {
                    retPath = Google2uGUIUtil.GetString("g2uplaymakerDirectory", retPath);
                    if (Directory.Exists(retPath))
                        return retPath;
                    // attempt to find the playmaker actions directory
                    // We already know that the playmaker dll exists, but we need to find the actual path
                    var playmakerPaths = Directory.GetFiles(Application.dataPath, "PlayMaker.dll",
                        SearchOption.AllDirectories);
                    var playmakerPath = string.Empty;
                    if (playmakerPaths.Length > 0)
                    {
                        // We are just going to use the first entry. If there is more than 1 entry, there are bigger issues
                        var fileName = playmakerPaths[0];
                        var fileInfo = new FileInfo(fileName);
                        playmakerPath = fileInfo.DirectoryName;
                    }

                    if (playmakerPath != string.Empty)
                    {
                        if (playmakerPath != null) retPath = Path.Combine(playmakerPath, "Actions");
                        if (Directory.Exists(retPath))
                        {
                            // We have found the Playmaker Actions dir!
                            Google2uGUIUtil.SetString("g2uplaymakerDirectory", retPath);
                        }
                        else
                        {
                            // The actions subdirectory doesn't exist? Rather than making it in the playmaker directory,
                            // We will just use our Google2uGen path instead and let the user figure it out
                            var Google2ugenPath = Google2uGenPath("Google2uGEN");

                            retPath = Path.Combine(Google2ugenPath, "PlayMaker").Replace('\\', '/');
                            if (!Directory.Exists(retPath))
                                Directory.CreateDirectory(retPath);

                            Google2uGUIUtil.SetString("g2uplaymakerDirectory", retPath);
                        }
                    }
                }
            }

            return retPath;
        }
    }
}