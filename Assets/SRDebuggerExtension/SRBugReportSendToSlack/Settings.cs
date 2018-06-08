public static class SlackAPISetting
{
    public static readonly string AccessToken = "YourSlackAPIToken";

    public static readonly string Channels = "PostChannelName";

    /// PlaneMode Only
    public static readonly string WebHookUrl = "https://hooks.slack.com/services/Txxxxxxxx/Bxxxxxxx/xxxxxxxxx";
}

public static class PostModeSetting
{
    public enum PostModes
    {
        Plane,
        TextFile,
    }

    public static readonly PostModes PostMode = PostModes.TextFile;
}
