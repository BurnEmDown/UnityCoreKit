using System;
using Core.Interfaces;
using Logger = Core.Utils.Logs.Logger;

namespace Core.StubServices
{
    /// <summary>
    /// A placeholder implementation of <see cref="ICrashReportingService"/> used during development.
    /// </summary>
    /// <remarks>
    /// This stub does **not** send crash logs or exceptions to any external crash reporting provider.
    /// Instead, it logs messages locally using <see cref="Logger"/>.
    ///
    /// The purpose of this class is to:
    /// <list type="bullet">
    ///   <item><description>Prevent null references when a crash reporting service is required</description></item>
    ///   <item><description>Serve as an example of how a real crash reporting integration should be structured</description></item>
    ///   <item><description>Enable testing and prototyping without any platform SDKs</description></item>
    /// </list>
    ///
    /// **Do not** ship this implementation in production.
    /// Replace it with a real crash reporting provider such as Firebase Crashlytics,
    /// Unity Cloud Diagnostics, Sentry, or Backtrace.
    /// </remarks>
    public class StubCrashReportingService : ICrashReportingService
    {
        /// <summary>
        /// Logs a generic crash-related message for debugging purposes only.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            Logger.Log($"[Crash Stub] {message}");
        }

        /// <summary>
        /// Logs an exception for debugging purposes only.
        /// </summary>
        /// <param name="exception">The exception to log. No external service receives it.</param>
        public void LogException(Exception exception)
        {
            Logger.LogException(exception);
        }
    }
}