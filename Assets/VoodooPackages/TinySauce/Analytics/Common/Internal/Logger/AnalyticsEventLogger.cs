using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal class AnalyticsEventLogger
    {
        private const string TAG = "AnalyticsEventLogger";
        private const string VOODOO_ANALYTICS_WRAPPER_NAME = "VoodooAnalytics";
        private static AnalyticsEventLogger _instance;
        private readonly List<DebugAnalyticsLog> _logsList = new List<DebugAnalyticsLog>(500);
        private readonly HashSet<string> _logsIdList = new HashSet<string>();
        private static bool _isAnalyticsDebuggingEnabled;
        private static bool _isAnalyticsLoggingEnabled;
        internal List<DebugAnalyticsLog> GetLocalAnalyticsLog(string wrapperNameFilter = null)
        {
            return string.IsNullOrEmpty(wrapperNameFilter) 
                ? _logsList 
                : _logsList.Where(nameInList => nameInList.WrapperName.Contains(wrapperNameFilter)).ToList();
        }

        internal static event Action<DebugAnalyticsLog, bool> OnAnalyticsEventStateChanged;

        internal static AnalyticsEventLogger GetInstance() => _instance ?? (_instance = new AnalyticsEventLogger());

        private void LogEventLocally(string wrapperName, string eventName, DebugAnalyticsStateEnum state, string eventId, string param = null, string error = "")
        {
            if (!_isAnalyticsDebuggingEnabled) return;
            var localAnalyticsLog = new DebugAnalyticsLog(wrapperName, eventName, param ?? "", state, eventId, error);
            var isUpdateFromExisting = false;

            if (_isAnalyticsLoggingEnabled) {
                if (!_logsIdList.Contains(localAnalyticsLog.EventId))
                {
                    _logsList.Add(localAnalyticsLog);
                    _logsIdList.Add(localAnalyticsLog.EventId);
                } else {
                    var index = _logsList.FindIndex(logItem => logItem.EventId.Contains(localAnalyticsLog.EventId));
                    _logsList[index] = localAnalyticsLog;
                    isUpdateFromExisting = true;
                }
            }
            OnAnalyticsEventStateChanged?.Invoke(localAnalyticsLog, isUpdateFromExisting);
        }

        internal void LogEventSentTo3rdParty(string wrapperName, string eventName, string eventId, [CanBeNull] Dictionary<string, object> param = null)
        {
            if (!_isAnalyticsDebuggingEnabled) return;
            LogEventLocally(wrapperName, eventName, DebugAnalyticsStateEnum.ForwardedTo3rdParty, eventId, param != null ? DictionaryToString(param) : "");
        }

        internal void LogEventException(string wrapperName, string eventName, string eventId, [CanBeNull] Dictionary<string, object> param, Exception e)
        {
            if (!_isAnalyticsDebuggingEnabled) return;
            LogEventLocally(wrapperName, eventName, DebugAnalyticsStateEnum.Error, eventId, param != null ? DictionaryToString(param) : "", e != null ? e.ToString() : "");
        }

        internal void LogEventsSentSuccessfully(List<string> eventJsons)
        {
            if (!_isAnalyticsDebuggingEnabled) return;
            LogAnalyticsSentOrErrorEvent(VOODOO_ANALYTICS_WRAPPER_NAME, eventJsons, DebugAnalyticsStateEnum.Sent);
        }
        
        internal void LogEventsSentError(List<string> eventJsons)
        {
            if (!_isAnalyticsDebuggingEnabled) return;
            LogAnalyticsSentOrErrorEvent(VOODOO_ANALYTICS_WRAPPER_NAME, eventJsons, DebugAnalyticsStateEnum.SentButErrorFromServer);
        }

        private void LogAnalyticsSentOrErrorEvent(string wrapperName,
                                             List<string> eventJsons,
                                             DebugAnalyticsStateEnum stateEnum)
        {
            foreach (string eventJson in eventJsons) {
                var eventName = GetEventNameFromJson(eventJson);
                if(string.IsNullOrEmpty(eventName)) continue;
                var eventId = GetEventIdFromJson(eventJson);
                LogEventLocally(wrapperName, eventName, stateEnum, eventId, eventJson);
            }
        }
        
        private static string GetEventNameFromJson(string json)
        {
            return GetValueFromJsonWithRegex(json, AnalyticsEventLoggerConstant.EVENT_NAME_JSON_REGEX_PATTERN);
        }

        private static string GetEventIdFromJson(string json)
        {
            return GetValueFromJsonWithRegex(json, AnalyticsEventLoggerConstant.EVENT_ID_JSON_REGEX_PATTERN);
        }

        private static string GetValueFromJsonWithRegex(string json, string regex)
        {
            Match regexMatch = Regex.Match(json, regex);
            if (!regexMatch.Success) return "";
            string match = regexMatch.Groups[1].Value;
            int matchSubstrIndex =
                match.IndexOf(AnalyticsEventLoggerConstant.JSON_SEPARATOR, StringComparison.Ordinal);
            if (matchSubstrIndex < 1) return "";
            return match.Substring(0, matchSubstrIndex);
        }

        internal void SetAnalyticsDebugging(bool enabled)
        {
            _isAnalyticsDebuggingEnabled = enabled;
        }

        internal void SetAnalyticsLogging(bool enabled)
        {
            _isAnalyticsLoggingEnabled = enabled;
        }

        internal void FlushAnalyticsLogs()
        {
            _logsIdList.Clear();
            _logsList.Clear();
        }
        
        private string DictionaryToString(Dictionary < string, object > dictionary) {  
            string dictionaryString = "{";  
            foreach(KeyValuePair < string, object > keyValues in dictionary) {  
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", ";  
            }  
            return dictionaryString.TrimEnd(',', ' ') + "}";  
        } 
    }
}