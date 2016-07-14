
#if NETFX_CORE
using UnityEngine.Windows;
#endif

namespace SRDebugger.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Services;
    using SRF;
    using UnityEngine;

    public class BugReportApi
    {
        private readonly string _apiKey;
        private readonly BugReport _bugReport;
        private bool _isBusy;
        private WWW _www;

        public BugReportApi(BugReport report, string apiKey)
        {
            _bugReport = report;
            _apiKey = apiKey;
        }

        public bool IsComplete { get; private set; }
        public bool WasSuccessful { get; private set; }
        public string ErrorMessage { get; private set; }

        public float Progress
        {
            get
            {
                if (_www == null)
                {
                    return 0;
                }

                return Mathf.Clamp01(_www.progress + _www.uploadProgress);
            }
        }

        public IEnumerator Submit()
        {
            //Debug.Log("[BugReportApi] Submit()");

            if (_isBusy)
            {
                throw new InvalidOperationException("BugReportApi is already sending a bug report");
            }

            // Reset state
            _isBusy = true;
            ErrorMessage = "";
            IsComplete = false;
            WasSuccessful = false;
            _www = null;

            try
            {
                var json = BuildJsonRequest(_bugReport);

                var jsonBytes = Encoding.UTF8.GetBytes(json);

                var headers = new Dictionary<string, string>();
                headers["Content-Type"] = "application/json";
                headers["Accept"] = "application/json";
                headers["Method"] = "POST";
                headers["X-ApiKey"] = _apiKey;

                _www = new WWW(SRDebugApi.BugReportEndPoint, jsonBytes, headers);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            if (_www == null)
            {
                SetCompletionState(false);
                yield break;
            }

            yield return _www;

            if (!string.IsNullOrEmpty(_www.error))
            {
                ErrorMessage = _www.error;
                SetCompletionState(false);

                yield break;
            }

            if (!_www.responseHeaders.ContainsKey("X-STATUS"))
            {
                ErrorMessage = "Completion State Unknown";
                SetCompletionState(false);
                yield break;
            }

            var status = _www.responseHeaders["X-STATUS"];

            if (!status.Contains("200"))
            {
                ErrorMessage = SRDebugApiUtil.ParseErrorResponse(_www.text, status);
                SetCompletionState(false);

                yield break;
            }

            SetCompletionState(true);
        }

        private void SetCompletionState(bool wasSuccessful)
        {
            _bugReport.ScreenshotData = null;
            WasSuccessful = wasSuccessful;
            IsComplete = true;
            _isBusy = false;

            if (!wasSuccessful)
            {
                Debug.LogError("Bug Reporter Error: " + ErrorMessage);
            }
        }

        private static string BuildJsonRequest(BugReport report)
        {
            var ht = new Hashtable();

            ht.Add("userEmail", report.Email);
            ht.Add("userDescription", report.UserDescription);

            ht.Add("console", CreateConsoleDump());
            ht.Add("systemInformation", report.SystemInformation);

            if (report.ScreenshotData != null)
            {
                ht.Add("screenshot", Convert.ToBase64String(report.ScreenshotData));
            }

            var json = Json.Serialize(ht);

            return json;
        }

        private static IList<IList<string>> CreateConsoleDump()
        {
            var list = new List<IList<string>>();

            var consoleLog = Service.Console.Entries;

            foreach (var consoleEntry in consoleLog)
            {
                var entry = new List<string>();

                entry.Add(consoleEntry.LogType.ToString());
                entry.Add(consoleEntry.Message);
                entry.Add(consoleEntry.StackTrace);

                if (consoleEntry.Count > 1)
                {
                    entry.Add(consoleEntry.Count.ToString());
                }

                list.Add(entry);
            }

            return list;
        }
    }
}
