using System;
using Core.Interfaces;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

#if FIREBASE_INSTALLED
using Firebase.Crashlytics;
#endif

namespace Core.DefaultServices.Firebase
{
    /// <summary>
    /// Crash reporting service implementation backed by Firebase Crashlytics.
    /// </summary>
    /// <remarks>
    /// This class provides a production-ready implementation of
    /// <see cref="ICrashReportingService"/>. When the
    /// <c>FIREBASE_INSTALLED</c> scripting define symbol is enabled, all crash
    /// reports and logged messages are forwarded to Firebase Crashlytics.
    ///
    /// <para>
    /// When Firebase is *not* installed, this service falls back to your
    /// project's shared <see cref="Logger"/> utility.  
    /// This ensures:
    /// </para>
    /// <list type="bullet">
    ///     <item>No hard dependency on Firebase packages</item>
    ///     <item>Consistent logging behavior across all environments</item>
    ///     <item>Consumers of <see cref="ICrashReportingService"/> never need conditional compilation</item>
    /// </list>
    ///
    /// <para>Typical usage:</para>
    /// <code>
    /// try
    /// {
    ///     // risky code
    /// }
    /// catch (Exception ex)
    /// {
    ///     CoreServices.Get&lt;ICrashReportingService&gt;().LogException(ex);
    /// }
    ///
    /// CoreServices.Get&lt;ICrashReportingService&gt;().Log("Player reached checkpoint X");
    /// </code>
    /// </remarks>
    public class FirebaseCrashReportingService : ICrashReportingService
    {
        /// <summary>
        /// Logs a non-fatal diagnostic message to Crashlytics or to the fallback logger.
        /// </summary>
        /// <param name="message">The descriptive message to record.</param>
        /// <remarks>
        /// If Firebase is installed, this method writes to Crashlytics using
        /// <see cref="Crashlytics.Log(string)"/>.  
        /// Otherwise, it falls back to <see cref="Logger.Log(string)"/> to ensure
        /// the call succeeds in all build configurations.
        /// </remarks>
        public void Log(string message)
        {
#if FIREBASE_INSTALLED
            Crashlytics.Log(message);
#else
            Logger.Log($"[Crash Stub] {message}");
#endif
        }

        /// <summary>
        /// Reports a handled exception to Crashlytics or logs it using the shared logger.
        /// </summary>
        /// <param name="exception">The exception instance to record.</param>
        /// <remarks>
        /// Intended for exceptions that the application can recover from but still
        /// wishes to capture for telemetry and diagnostics.
        ///
        /// <para>
        /// When Firebase is available, exceptions are reported to Crashlytics
        /// via <see cref="Crashlytics.LogException(Exception)"/>.  
        /// Otherwise, your project's <see cref="Logger.LogException(Exception)"/> is used.
        /// </para>
        /// </remarks>
        public void LogException(Exception exception)
        {
#if FIREBASE_INSTALLED
            Crashlytics.LogException(exception);
#else
            Logger.LogException(exception);
#endif
        }
    }
}
