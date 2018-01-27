namespace SRBugReportSendToSlack
{
    public static class SlackAPISetting
    {
        /// <summary>
        /// 送信種別
        /// </summary>
        public static SendTypes sendType = SendTypes.TextFile;

        public static readonly string AccessToken = "YourToken";
        public static readonly string Channels = "general";
        public static readonly string UserName = "SRDebuggerBugReport";

        // SendType.Planeのみ
        public static readonly string WebHookUrl = "https://hooks.slack.com/services/Txxxxxxxx/Bxxxxxxx/xxxxxxxxx";
    }
}
