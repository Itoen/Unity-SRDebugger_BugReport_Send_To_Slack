
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

    public class MyBugReportApi
    {
        #region Enums

        public enum ReportType : int
        {
            ScreenShot = 0,
            SystemInfo,
            Console,
        }

        #endregion //Enums

        #region Constants

        /// <summary>
        /// Slack API 
        /// </summary>
        private static readonly string SlackApiUrl = "https://slack.com/api";
        private static readonly string UploadEndPoint = "/files.upload";
        /// @note attachment上手行かない。。。
        //private static readonly string PostMessageEndPoint = "/chat.postMessage";
        private static readonly string AccessToken = "YourToken";
        private static readonly string WebHookUrl = "https://hooks.slack.com/services/Txxxxxxxx/Bxxxxxxx/xxxxxxxxx";
        private static readonly string Channels = "general";
        private static readonly string UserName = "SRDebuggerBugReport";

        /// <summary>
        /// Attachmentのカラー
        /// </summary>
        private static readonly string SystemInfoColor = "#000000";

        /// <summary>
        /// Log毎にAttachmentを分ける場合のカラー
        /// </summary>
        private static readonly string ErrorLogColor = "#FF0000";
        private static readonly string WarningLogColor = "#FFD700";
        private static readonly string NormalLogColor = "#708090";

        #endregion // Constants

        #region Variables

        private readonly string _apiKey;
        private readonly BugReport _bugReport;
        private bool _isBusy;
        private WWW _www;
        private static uint _logId;

        #endregion // Variables

        #region Constructor

        public MyBugReportApi (BugReport report, string apiKey)
        {
            _bugReport = report;
            _apiKey = apiKey;
        }

        #endregion // Constructor

        #region Properties

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

        #endregion // Properties

        #region Methods

        public IEnumerator Submit ()
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
            _logId = (uint)System.Security.Cryptography.MD5.Create().GetHashCode();

            foreach (var type in Enum.GetValues(typeof(ReportType)))
            {
                try
                {
                    if ((int)ReportType.ScreenShot == (int)type && _bugReport.ScreenshotData != null)
                    {
                        var form = BuildScrrenShotUploadRequest(_bugReport);
                        var url = SlackApiUrl + UploadEndPoint;

                        _www = new WWW(url, form);
                    }
                    else
                    {
                        string json = BuildJsonRequest((int)type, _bugReport);

                        var jsonBytes = Encoding.UTF8.GetBytes(json);
                        var headers = new Dictionary<string, string>();
                        headers["Content-type"] = "application/json";
                        headers["Accept"] = "application/json";
                        headers["Method"] = "POST";
                        headers["data"] = json;

                        _www = new WWW(WebHookUrl, jsonBytes, headers);
                    }

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

                if (!_www.responseHeaders.ContainsKey("STATUS"))
                {
                    ErrorMessage = "Completion State Unknown";
                    SetCompletionState(false);
                    yield break;
                }

                var status = _www.responseHeaders["STATUS"];

                if (!status.Contains("200"))
                {
                    ErrorMessage = SRDebugApiUtil.ParseErrorResponse(_www.text, status);
                    SetCompletionState(false);

                    yield break;
                }
            }

            SetCompletionState(true);
        }

        private void SetCompletionState (bool wasSuccessful)
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

        static WWWForm BuildScrrenShotUploadRequest (BugReport report)
        {
            var form = new WWWForm();

            form.AddField("token", AccessToken);
            form.AddField("channels", Channels);
            form.AddField("username", UserName);
            var title = string.Format("ScreenShot ({0})", _logId);
            form.AddField("title", title);
            form.AddBinaryData("file", report.ScreenshotData, "screenShot", "imag/png");
            form.AddField("initial_comment", report.UserDescription);

            return form;
        }

        static string BuildJsonRequest (int typeValue, BugReport report)
        {
            string json = "";
            switch (typeValue)
            {
                case (int)ReportType.ScreenShot:
                    var ht = new Hashtable();
                    ht.Add("text", report.UserDescription);
                    json = Json.Serialize(ht);
                    break;
                case (int)ReportType.SystemInfo:
                    json = BuildSystemInfoJsonRequest(report);
                    break;
                case (int)ReportType.Console:
                    json = BuildConsoleLogJsonRequest(report);
                    break;
            }
            return json;
        }

        static string BuildSystemInfoJsonRequest (BugReport report)
        {
            var ht = new Hashtable();
            ht.Add("username", UserName);
            var messageTitle = string.Format("SystemInfo ({0})", _logId);
            ht.Add("text", messageTitle);

            List<Hashtable> attachmentList = new List<Hashtable>();
            List<Hashtable> systemInfoList = new List<Hashtable>();
            foreach (var systemInfos in report.SystemInformation)
            {
                var systemInfoHashTable = new Hashtable();
                systemInfoHashTable.Add("color", SystemInfoColor);
                systemInfoHashTable.Add("title", systemInfos.Key);
                var sb = new StringBuilder();
                foreach (var obj in systemInfos.Value)
                {
                    sb.AppendLine(obj.ToString());
                }
                systemInfoHashTable.Add("text", sb.ToString());

                attachmentList.Add(systemInfoHashTable);
            }

            ht.Add("attachments", attachmentList);

            var json = Json.Serialize(ht);

            return json;
        }

        private static string BuildConsoleLogJsonRequest (BugReport report)
        {
            var ht = new Hashtable();
            ht.Add("username", UserName);
            var messageTitle = string.Format("Console ({0})", _logId);
            ht.Add("text", messageTitle);

            List<Hashtable> attachmentList = new List<Hashtable>();

            foreach (var logs in CreateConsoleDump())
            {
                StringBuilder sb = new StringBuilder();
                var consoleHashTable = new Hashtable();

                switch (logs[0])
                {
                    case "Assert":
                    case "Exception":
                    case "Error":
                        consoleHashTable.Add("color", ErrorLogColor);
                        break;
                    case "Warning":
                        consoleHashTable.Add("color", WarningLogColor);
                        break;
                    default:
                        consoleHashTable.Add("color", NormalLogColor);
                        break;
                }

                // LogType + Log
                var title = logs[0] + " : " + logs[1];
                consoleHashTable.Add("title", title);
                // StackTrace
                for (int i = 2; i < logs.Count; i++)
                {
                    sb.Append(logs[i]);
                }
                sb.AppendLine();
                consoleHashTable.Add("text", sb.ToString());
                attachmentList.Add(consoleHashTable);
            }

            ht.Add("attachments", attachmentList);

            var json = Json.Serialize(ht);

            return json;
        }

        private static IList<IList<string>> CreateConsoleDump ()
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

    #endregion // Methods
}
