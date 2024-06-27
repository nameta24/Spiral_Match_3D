namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    internal abstract class GameAnalyticsEvent : BaseAnalyticsEvent
    {
        protected override string GetAnalyticsProviderName() => "GameAnalytics";
        protected GameAnalyticsEvent(string eventName) : base(eventName) { }
    }
}