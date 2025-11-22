using System;

namespace Core.Interfaces
{
    /// <summary>
    /// Provides methods for logging crash reports and exceptions.
    /// </summary>
    public interface ICrashReportingService
    {
        /// <summary>
        /// Logs a custom message to the crash reporting system.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);

        /// <summary>
        /// Logs an exception to the crash reporting system.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        void LogException(Exception ex);
    }
}