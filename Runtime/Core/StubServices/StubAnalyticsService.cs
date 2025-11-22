using System.Collections.Generic;
using Core.Interfaces;
using Logger = Core.Utils.Logs.Logger;

namespace Core.StubServices
{
    /// <summary>
    /// A placeholder implementation of <see cref="IAnalyticsService"/> used for development,
    /// editor testing, and as an example of how to implement a real analytics integration.
    /// </summary>
    /// <remarks>
    /// This stub does **not** send analytics data to any external service.
    /// Instead, it simply logs messages to the console using <see cref="Logger"/>.
    /// 
    /// This class is intended only as:
    /// <list type="bullet">
    ///   <item><description>A safe default when no real analytics SDK is present</description></item>
    ///   <item><description>A reference implementation for how analytics services should behave</description></item>
    ///   <item><description>A way to avoid null checks during early development</description></item>
    /// </list>
    ///
    /// Replace this class with an actual implementation (e.g., Firebase Analytics,
    /// Unity Gaming Services Analytics, GameAnalytics, etc.) in production builds.
    /// </remarks>
    public class StubAnalyticsService : IAnalyticsService
    {
        /// <summary>
        /// Logs an event name to the console. No external analytics provider is contacted.
        /// </summary>
        /// <param name="eventName">The name of the event being logged.</param>
        public void LogEvent(string eventName)
        {
            Logger.Log($"[Analytics Stub] Event: {eventName}");
        }

        /// <summary>
        /// Logs an event name and a set of parameters to the console.
        /// No external analytics provider is contacted.
        /// </summary>
        /// <param name="eventName">The name of the event being logged.</param>
        /// <param name="parameters">Optional event parameters included for structure consistency.</param>
        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                Logger.Log($"[Analytics Stub] Event: {eventName} (no params)");
                return;
            }

            var paramText = string.Join(", ",
                System.Linq.Enumerable.Select(parameters, kvp => $"{kvp.Key}={kvp.Value}"));

            Logger.Log($"[Analytics Stub] Event: {eventName} | {paramText}");
        }
    }
}