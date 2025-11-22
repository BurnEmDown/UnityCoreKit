using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

namespace Core.StubServices
{
    /// <summary>
    /// A placeholder implementation of <see cref="IRemoteConfigService"/> designed for
    /// development and prototyping when no real remote configuration system is available.
    /// </summary>
    /// <remarks>
    /// This stub does **not** perform network requests or fetch configuration values
    /// from any remote service.
    ///
    /// Instead, it:
    /// <list type="bullet">
    ///   <item><description>Always returns the provided default values</description></item>
    ///   <item><description>Logs calls for visibility during development</description></item>
    ///   <item><description>Allows optional overrides for testing specific behaviors</description></item>
    /// </list>
    ///
    /// The purpose of this class is to:
    /// <list type="bullet">
    ///   <item><description>Enable code paths that rely on remote config without requiring a backend</description></item>
    ///   <item><description>Prevent null-reference errors early in the project</description></item>
    ///   <item><description>Provide a template for implementing a real service later</description></item>
    /// </list>
    ///
    /// **Do not ship this in production.**
    /// Replace it with a real remote configuration provider (e.g., Firebase Remote Config,
    /// Unity Remote Config, a custom CDN fetcher, etc.).
    /// </remarks>
    public class StubRemoteConfigService : IRemoteConfigService
    {
        /// <summary>
        /// Optional map of developer-defined overrides for testing and UI preview.
        /// Keys map to raw values representing pretend remote configuration entries.
        /// </summary>
        private readonly Dictionary<string, object> _overrides = new();

        /// <summary>
        /// Simulates the initialization process. No remote fetch happens.
        /// </summary>
        public Task InitializeAsync()
        {
            Logger.Log("[RemoteConfig Stub] InitializeAsync called (no remote fetch).");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns a boolean configuration value or the provided default if unavailable.
        /// </summary>
        public bool GetBool(string key, bool defaultValue = false)
        {
            if (_overrides.TryGetValue(key, out var value) && value is bool b)
                return b;

            Logger.Log($"[RemoteConfig Stub] GetBool('{key}') → {defaultValue} (default)");
            return defaultValue;
        }

        /// <summary>
        /// Returns an integer configuration value or the provided default.
        /// Supports integer and float overrides.
        /// </summary>
        public int GetInt(string key, int defaultValue = 0)
        {
            if (_overrides.TryGetValue(key, out var value))
            {
                if (value is int i) return i;
                if (value is float f) return Mathf.RoundToInt(f);
            }

            Logger.Log($"[RemoteConfig Stub] GetInt('{key}') → {defaultValue} (default)");
            return defaultValue;
        }

        /// <summary>
        /// Returns a double configuration value or the provided default.
        /// Supports double and float overrides.
        /// </summary>
        public double GetDouble(string key, double defaultValue = 0)
        {
            if (_overrides.TryGetValue(key, out var value))
            {
                if (value is double d) return d;
                if (value is float f) return f;
            }

            Logger.Log($"[RemoteConfig Stub] GetDouble('{key}') → {defaultValue} (default)");
            return defaultValue;
        }

        /// <summary>
        /// Returns a string configuration value or the provided default.
        /// </summary>
        public string GetString(string key, string defaultValue = "")
        {
            if (_overrides.TryGetValue(key, out var value) && value is string s)
                return s;

            Logger.Log($"[RemoteConfig Stub] GetString('{key}') → \"{defaultValue}\" (default)");
            return defaultValue;
        }

        /// <summary>
        /// Adds or updates an override value for testing or simulating remote config changes.
        /// </summary>
        /// <param name="key">The configuration key to override.</param>
        /// <param name="value">The local value to return instead of default values.</param>
        public void SetOverride(string key, object value)
        {
            _overrides[key] = value;
        }
    }
}