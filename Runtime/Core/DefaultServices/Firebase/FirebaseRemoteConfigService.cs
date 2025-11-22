using System.Threading.Tasks;
using Core.Interfaces;
using Logger = Core.Utils.Logs.Logger;
#if FIREBASE_INSTALLED
using Firebase.RemoteConfig;
#endif

namespace Core.DefaultServices.Firebase
{
    /// <summary>
    /// Remote config service implementation backed by Firebase Remote Config.
    /// </summary>
    /// <remarks>
    /// This class provides a concrete implementation of <see cref="IRemoteConfigService"/>
    /// that uses Firebase Remote Config when the <c>FIREBASE_INSTALLED</c> scripting
    /// define is enabled.
    ///
    /// <para><b>Behavior when Firebase is installed:</b></para>
    /// <list type="bullet">
    ///   <item>Defaults are applied via <c>SetDefaultsAsync</c>.</item>
    ///   <item>Remote values are fetched asynchronously and activated.</item>
    ///   <item>Getter methods (<c>GetBool</c>, <c>GetInt</c>, etc.) read from Firebase.</item>
    /// </list>
    ///
    /// <para><b>Behavior when Firebase is NOT installed:</b></para>
    /// <list type="bullet">
    ///   <item><c>InitializeAsync</c> logs a stub message and returns immediately.</item>
    ///   <item>All getters return the provided <c>defaultValue</c> and log stub usage.</item>
    /// </list>
    ///
    /// <para>
    /// This design allows game code to request <see cref="IRemoteConfigService"/> safely,
    /// with or without Firebase present, without needing any conditional compilation.
    /// </para>
    /// </remarks>
    public class FirebaseRemoteConfigService : IRemoteConfigService
    {
        private bool _initialized;

        /// <summary>
        /// Initializes the Firebase Remote Config system, applies defaults,
        /// fetches remote values, and activates them.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method should be called once during application startup, typically
        /// from a bootstrap or initialization step (e.g. via
        /// <see cref="FirebaseInitializer"/>).
        /// </para>
        ///
        /// <para>
        /// When Firebase is installed, the method:
        /// </para>
        /// <list type="bullet">
        ///   <item>Logs an initialization message.</item>
        ///   <item>Calls <c>SetDefaultsAsync</c> with the configured default values.</item>
        ///   <item>Performs a <c>FetchAsync</c> and then <c>ActivateAsync</c> on success.</item>
        ///   <item>Sets <see cref="_initialized"/> to <c>true</c> when activation completes.</item>
        /// </list>
        ///
        /// <para>
        /// If Firebase is not installed, this method simply logs a stub message
        /// and returns a completed task.
        /// </para>
        /// </remarks>
        public async Task InitializeAsync()
        {
#if FIREBASE_INSTALLED
            if (_initialized)
                return;

            Logger.Log("[RemoteConfig] Initializing Firebase Remote Config...");

            // Assumes a 'defaults' collection is defined and populated elsewhere.
            await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);

            // Fetch and activate
            var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync();
            await fetchTask;

            if (fetchTask.IsCompletedSuccessfully)
            {
                await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                _initialized = true;
                Logger.Log("[RemoteConfig] Fetch and activate succeeded.");
            }
            else
            {
                Logger.LogWarning("[RemoteConfig] Fetch failed, using defaults / last known values.");
            }
#else
            Logger.Log("[RemoteConfig Stub] InitializeAsync called (no Firebase).");
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Retrieves a boolean remote config value or a default if not available.
        /// </summary>
        /// <param name="key">The config key.</param>
        /// <param name="defaultValue">
        /// The value to use if no remote value exists or if Firebase is unavailable.
        /// </param>
        /// <returns>The resolved boolean value.</returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
#if FIREBASE_INSTALLED
            if (!_initialized)
                Logger.LogWarning($"[RemoteConfig] GetBool('{key}') before InitializeAsync. Returning default.");

            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (value.Source == ValueSource.DefaultValue && defaultValue != false)
                return defaultValue;

            return value.BooleanValue;
#else
            Logger.Log($"[RemoteConfig Stub] GetBool('{key}') → {defaultValue} (default)");
            return defaultValue;
#endif
        }

        /// <summary>
        /// Retrieves an integer remote config value or a default if not available.
        /// </summary>
        /// <param name="key">The config key.</param>
        /// <param name="defaultValue">
        /// The value to use if no remote value exists or if Firebase is unavailable.
        /// </param>
        /// <returns>The resolved integer value.</returns>
        public int GetInt(string key, int defaultValue = 0)
        {
#if FIREBASE_INSTALLED
            if (!_initialized)
                Logger.LogWarning($"[RemoteConfig] GetInt('{key}') before InitializeAsync. Returning default.");

            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (value.Source == ValueSource.DefaultValue && defaultValue != 0)
                return defaultValue;

            return (int)value.LongValue;
#else
            Logger.Log($"[RemoteConfig Stub] GetInt('{key}') → {defaultValue} (default)");
            return defaultValue;
#endif
        }

        /// <summary>
        /// Retrieves a double remote config value or a default if not available.
        /// </summary>
        /// <param name="key">The config key.</param>
        /// <param name="defaultValue">
        /// The value to use if no remote value exists or if Firebase is unavailable.
        /// </param>
        /// <returns>The resolved double value.</returns>
        public double GetDouble(string key, double defaultValue = 0)
        {
#if FIREBASE_INSTALLED
            if (!_initialized)
                Logger.LogWarning($"[RemoteConfig] GetDouble('{key}') before InitializeAsync. Returning default.");

            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (value.Source == ValueSource.DefaultValue && defaultValue != 0)
                return defaultValue;

            return value.DoubleValue;
#else
            Logger.Log($"[RemoteConfig Stub] GetDouble('{key}') → {defaultValue} (default)");
            return defaultValue;
#endif
        }

        /// <summary>
        /// Retrieves a string remote config value or a default if not available.
        /// </summary>
        /// <param name="key">The config key.</param>
        /// <param name="defaultValue">
        /// The value to use if no remote value exists or if Firebase is unavailable.
        /// </param>
        /// <returns>The resolved string value.</returns>
        public string GetString(string key, string defaultValue = "")
        {
#if FIREBASE_INSTALLED
            if (!_initialized)
                Logger.LogWarning($"[RemoteConfig] GetString('{key}') before InitializeAsync. Returning default.");

            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            if (value.Source == ValueSource.DefaultValue && !string.IsNullOrEmpty(defaultValue))
                return defaultValue;

            return value.StringValue;
#else
            Logger.Log($"[RemoteConfig Stub] GetString('{key}') → \"{defaultValue}\" (default)");
            return defaultValue;
#endif
        }
    }
}