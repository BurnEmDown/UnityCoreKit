using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Core.Utils.Logs
{
    /// <summary>
    /// Provides conditional logging utilities that compile only when the <c>LOGS</c>
    /// scripting define symbol is enabled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All methods in this logger are decorated with <see cref="ConditionalAttribute"/>
    /// using the <c>"LOGS"</c> symbol.  
    /// This means calls to these methods are **stripped entirely from the build**
    /// unless <c>LOGS</c> is defined in the project's scripting define symbols.
    /// </para>
    ///
    /// <para>
    /// This is useful for development builds, debugging, or enabling lightweight
    /// diagnostics without impacting the performance of release builds.
    /// </para>
    /// </remarks>
    public static class Logger
    {
        /// <summary>
        /// Logs a standard message to the Unity console using <see cref="Debug.Log"/>,
        /// but only when the <c>LOGS</c> define symbol is enabled.
        /// </summary>
        /// <param name="message">The message to print to the console.</param>
        [Conditional("LOGS")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Logs a warning message to the Unity console using <see cref="Debug.LogWarning"/>,
        /// but only when the <c>LOGS</c> define symbol is enabled.
        /// </summary>
        /// <param name="message">The warning message to print to the console.</param>
        [Conditional("LOGS")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Logs an error message to the Unity console using <see cref="Debug.LogError"/>,
        /// but only when the <c>LOGS</c> define symbol is enabled.
        /// </summary>
        /// <param name="message">The error message to print to the console.</param>
        [Conditional("LOGS")]
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Logs an exception to the Unity console using <see cref="Debug.LogException"/>,
        /// but only when the <c>LOGS</c> define symbol is enabled.
        /// </summary>
        /// <param name="exc">The exception object to display.</param>
        [Conditional("LOGS")]
        public static void LogException(Exception exc)
        {
            Debug.LogException(exc);
        }
    }
}