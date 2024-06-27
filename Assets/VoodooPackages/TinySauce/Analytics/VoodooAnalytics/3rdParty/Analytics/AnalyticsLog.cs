using System;
using System.Globalization;
using UnityEngine;

namespace Voodoo.Analytics
{
    public static class AnalyticsLog
    {
        private static AnalyticsLogLevel _logLevel;
        private const string TAG = "AnalyticsLog";

        public static void SetLogLevel(AnalyticsLogLevel level)
        {
            _logLevel = level;
        }

        public static void Log(string tag, string message)
        {
            if (_logLevel >= AnalyticsLogLevel.DEBUG)
                Debug.Log(Format(tag, message));
        }

        public static void LogE(string tag, string message)
        {
            if (_logLevel >= AnalyticsLogLevel.ERROR)
                Debug.LogError(Format(tag, message));
        }

        public static void LogW(string tag, string message)
        {
            if (_logLevel >= AnalyticsLogLevel.WARNING)
                Debug.LogWarning(Format(tag, message));
        }

        private static string Format(string tag, string message)
        {
            return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",new CultureInfo("fr-FR"))} - {TAG}/{tag}: {message}";
        }

        public enum AnalyticsLogLevel
        {
            DISABLED = 0,
            ERROR = 1,
            WARNING = 2,
            DEBUG = 3,
        }
    }
}