using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

#if FIREBASE_INSTALLED
using Firebase.Analytics;
#endif

namespace Core.DefaultServices.Firebase
{
    /// <summary>
    /// Analytics service implementation backed by Firebase Analytics.
    /// </summary>
    /// <remarks>
    /// This class provides a concrete implementation of <see cref="IAnalyticsService"/>
    /// that integrates with Firebase Analytics when the 
    /// <c>FIREBASE_INSTALLED</c> scripting define is enabled.
    ///
    /// <para><b>When Firebase Analytics is installed:</b></para>
    /// <list type="bullet">
    ///     <item>All analytics events are forwarded to Firebase.</item>
    ///     <item>Event parameters are converted into Firebase <see cref="Parameter"/> instances.</item>
    /// </list>
    ///
    /// <para><b>When Firebase is NOT installed:</b></para>
    /// <list type="bullet">
    ///     <item>Analytics calls fall back to the shared <see cref="Logger"/>.</item>
    ///     <item>No Firebase packages are required to compile or run the project.</item>
    ///     <item>Consumer code does not need conditional compilation.</item>
    /// </list>
    ///
    /// <para>
    /// This makes the service safe to use across all build configurations while
    /// still enabling production-grade analytics when Firebase is available.
    /// </para>
    ///
    /// <para>Example usage:</para>
    /// <code>
    /// var analytics = CoreServices.Get&lt;IAnalyticsService&gt;();
    /// analytics.LogEvent("level_complete");
    ///
    /// analytics.LogEvent(
    ///     "purchase",
    ///     new Dictionary&lt;string, object&gt; {
    ///         { "item_id", "sword_01" },
    ///         { "amount", 1 },
    ///     });
    /// </code>
    /// </remarks>
    public class FirebaseAnalyticsService : IAnalyticsService
    {
        /// <summary>
        /// Logs an analytics event without parameters.
        /// </summary>
        /// <param name="eventName">The name of the event to record.</param>
        /// <remarks>
        /// If Firebase Analytics is installed, the event is sent via
        /// <see cref="FirebaseAnalytics.LogEvent(string)"/>.  
        /// Otherwise, the event is recorded using <see cref="Logger.Log(string)"/> 
        /// as a harmless stub.
        /// </remarks>
        public void LogEvent(string eventName)
        {
#if FIREBASE_INSTALLED
            FirebaseAnalytics.LogEvent(eventName);
#else
            Logger.Log($"[Analytics Stub] Event: {eventName}");
#endif
        }

        /// <summary>
        /// Logs an analytics event with a set of key–value parameters.
        /// </summary>
        /// <param name="eventName">The name of the event to record.</param>
        /// <param name="parameters">
        /// A dictionary of event parameters such as item IDs, durations, or quantities.
        /// </param>
        /// <remarks>
        /// <para><b>Firebase Mode:</b></para>
        /// Parameters are converted to Firebase <see cref="Parameter"/> objects.
        /// Supported types include:
        /// <list type="bullet">
        ///     <item><description><see cref="string"/></description></item>
        ///     <item><description><see cref="int"/>, <see cref="long"/></description></item>
        ///     <item><description><see cref="float"/>, <see cref="double"/></description></item>
        ///     <item><description><see cref="bool"/> (converted to 0/1)</description></item>
        /// </list>
        ///
        /// Unsupported types fall back to <c>.ToString()</c> serialization.
        ///
        /// <para><b>Stub Mode:</b></para>
        /// If Firebase is not installed, this method logs a readable
        /// representation of the event and parameters using <see cref="Logger"/>.
        ///
        /// <para>
        /// This ensures consistent behavior regardless of whether Firebase is 
        /// included in the build.
        /// </para>
        /// </remarks>
        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
#if FIREBASE_INSTALLED
            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
                return;
            }

            var paramList = new List<Parameter>(parameters.Count);

            foreach (var kvp in parameters)
            {
                switch (kvp.Value)
                {
                    case string s:
                        paramList.Add(new Parameter(kvp.Key, s));
                        break;
                    case int i:
                        paramList.Add(new Parameter(kvp.Key, i));
                        break;
                    case long l:
                        paramList.Add(new Parameter(kvp.Key, l));
                        break;
                    case float f:
                        paramList.Add(new Parameter(kvp.Key, f));
                        break;
                    case double d:
                        paramList.Add(new Parameter(kvp.Key, d));
                        break;
                    case bool b:
                        paramList.Add(new Parameter(kvp.Key, b ? 1 : 0));
                        break;
                    default:
                        paramList.Add(new Parameter(kvp.Key, kvp.Value.ToString()));
                        break;
                }
            }

            FirebaseAnalytics.LogEvent(eventName, paramList.ToArray());
#else
            if (parameters == null || parameters.Count == 0)
            {
                Logger.Log($"[Analytics Stub] Event: {eventName} (no parameters)");
                return;
            }

            var paramText = string.Join(", ",
                System.Linq.Enumerable.Select(parameters, kvp => $"{kvp.Key}={kvp.Value}"));

            Logger.Log($"[Analytics Stub] Event: {eventName} | {paramText}");
#endif
        }
    }
}