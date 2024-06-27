using System;
using System.Runtime.CompilerServices;

namespace Voodoo.Tiny.Sauce.Internal.Analytics {
    internal struct DebugAnalyticsLog
    {
        private const string TAG = "DebugAnalyticsLog";
        internal string EventId { get; }
        internal string WrapperName { get; }
        internal string EventName { get; }
        internal string Parameters { get; }
        internal string Error { get; }
        internal DebugAnalyticsStateEnum StateEnum { get; }
        internal DateTime Timestamp { get; }
        
        internal DebugAnalyticsLog(string wrapperName, string eventName, string param, 
            DebugAnalyticsStateEnum stateEnum, string eventId, string error)
        {
            WrapperName = wrapperName;
            EventName = eventName;
            Parameters = param;
            StateEnum = stateEnum;
            Timestamp = DateTime.Now;
            EventId = eventId;
            Error = error;
        }

        public override string ToString()
        {
            return
                $"EventId: {EventId}, WrapperName: {WrapperName}, " +
                $"EventName: {EventName}, StateEnum: {StateEnum}, " +
                $"Timestamp: {Timestamp}, Parameters: {Parameters}" +
                $"Error: {Error}";
        }
    }

    public enum DebugAnalyticsStateEnum
    {
        ForwardedTo3rdParty = 1,
        ErrorSending = 2,
        Sent = 3,
        SentButErrorFromServer = 4,
        Error = 5
    }
}