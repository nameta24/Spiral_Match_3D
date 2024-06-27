namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public static class AnalyticsEventLoggerConstant
    {
        public const string DEFAULT_NOT_INITIALIZED_MESSAGE = "Provider is not initialized";
        public const string EVENT_NAME_JSON_REGEX_PATTERN = "\\\"name\\\":\\\"(.*)\\\",";
        public const string EVENT_ID_JSON_REGEX_PATTERN = ",\\\"id\\\":\\\"(.*)\\\",";
        public const string EVENT_TYPE_JSON_PATTERN = ",\"type\":";
        public const string EVENT_USER_ID_JSON_PATTERN = ",\"user_id\":";
        public const string JSON_SEPARATOR = "\",\"";
    }
}