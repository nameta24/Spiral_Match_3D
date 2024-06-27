using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Voodoo.Analytics
{
    internal static class AnalyticsUtil
    {
        internal static string ConvertDictionaryToJson(Dictionary<string, object> eventCustomData,
                                                       string dataJson = null,
                                                       string customVariables = null,
                                                       string contextVariables = null)
        {
            var fields = new List<string>();
            foreach (KeyValuePair<string, object> keyValue in eventCustomData) {
                string value;
                switch (keyValue.Value) {
                    case null:
                        value = null;
                        break;
                    case string _:
                        value = $"\"{keyValue.Value}\"";
                        break;
                    case char _:
                        value = $"\"{keyValue.Value}\"";
                        break;
                    case bool _:
                        value = $"{keyValue.Value.ToString().ToLower()}";
                        break;
                    case float floatValue:
                        value = $"{floatValue.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US"))}";
                        break;
                    case double doubleValue:
                        value = $"{doubleValue.ToString(CultureInfo.CreateSpecificCulture("en-US"))}";
                        break;
                    default:
                        value = $"{keyValue.Value}";
                        break;
                }

                if (value != null) {
                    fields.Add($"\"{keyValue.Key}\":{value}");
                }
            }

            string result = "{" + string.Join(",", fields);

            if (ValidateJson(dataJson)) {
                result += ",\"" + AnalyticsConstant.DATA + "\":" + dataJson;
            }

            if (ValidateJson(customVariables)) {
                result += ",\"" + AnalyticsConstant.CUSTOM_VARIABLES + "\":" + customVariables;
            }

            if (ValidateJson(contextVariables)) {
                result += ",\"" + AnalyticsConstant.CONTEXT_VARIABLES + "\":" + contextVariables;
            }

            result += "}";

            return result;
        }

        private static string ToIsoFormat(this DateTime dateTime, IFormatProvider cultureInfo) => dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", cultureInfo);
        internal static string ToIsoFormat(this DateTime dateTime) => dateTime.ToIsoFormat(new CultureInfo("fr-FR"));

        private static bool ValidateJson(string json) => !string.IsNullOrEmpty(json) && json.StartsWith("{") && json.EndsWith("}");

        internal static string ConvertDictionaryToCustomVarJson(Dictionary<string, object> eventCustomVariables)
        {
            var result = "";
            var counter = 0;
            foreach (KeyValuePair<string, object> pair in eventCustomVariables) {
                if (!string.IsNullOrEmpty(result)) result += ",";
                result += $"\"c{counter}_key\":\"{pair.Key}\",";
                result += $"\"c{counter}_val\":\"{pair.Value}\"";
                counter++;
            }

            return "{" + result + "}";
        }

        internal static string ConvertDictionaryToContextVarJson(Dictionary<string, object> eventContextVariables)
        {
            var fields = new List<string>();
            foreach (KeyValuePair<string, object> keyValue in eventContextVariables) {
                //check key
                string key = keyValue.Key.ToLower();
                if (!Regex.IsMatch(key, "^[a-z_]+$")) {
                    continue;
                }

                //check value
                string value;
                switch (keyValue.Value) {
                    case null:
                        value = null;
                        break;
                    case string _:
                        value = $"\"{keyValue.Value}\"";
                        break;
                    case char _:
                        value = $"\"{keyValue.Value}\"";
                        break;
                    case bool _:
                        value = $"{keyValue.Value.ToString().ToLower()}";
                        break;
                    case float floatValue:
                        value = $"{floatValue.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US"))}";
                        break;
                    case double doubleValue:
                        value = $"{doubleValue.ToString(CultureInfo.CreateSpecificCulture("en-US"))}";
                        break;
                    case byte _:
                    case decimal _:
                    case int _:
                    case long _:
                    case sbyte _:
                    case short _:
                    case uint _:
                    case ulong _:
                    case ushort _:
                        value = $"{keyValue.Value}";
                        break;
                    default:
                        value = null;
                        break;
                }

                if (value != null) {
                    fields.Add($"\"{key}\":{value}");
                }
            }
            return "{" + string.Join(",", fields) + "}";
        }

        public static void AddParameterEnum(ref Dictionary<string, object> data, string key, Enum variable, string defaultValue)
        {
            if (variable == null) {
                if (defaultValue != null)
                    data.Add(key, defaultValue);
            } else {
                data.Add(key, variable.ToString());
            }
        }

        public static void AddParameterString(ref Dictionary<string, object> data, string key, string variable)
        {
            if (!string.IsNullOrEmpty(variable))
                data.Add(key, variable);
        }

        public static void AddParameterNullable<T>(ref Dictionary<string, object> data, string key, T? variable) where T : struct
        {
            if (variable.HasValue) {
                data.Add(key, variable.Value);
            }
        }
    }
}