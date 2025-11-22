using System.Collections.Generic;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines methods for logging analytics events.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Logs an analytics event with the specified name.
        /// </summary>
        /// <param name="eventName">The name of the event to log.</param>
        void LogEvent(string eventName);

        /// <summary>
        /// Logs an analytics event with the specified name and additional parameters.
        /// </summary>
        /// <param name="eventName">The name of the event to log.</param>
        /// <param name="parameters">A dictionary containing key-value pairs of additional parameters for the event.</param>
        void LogEvent(string eventName, Dictionary<string, object> parameters);
    }
}