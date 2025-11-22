using System.Collections.Generic;
using Core.Interfaces;
using Logger = Core.Utils.Logs.Logger;

namespace Core.StubServices
{
    /// <summary>
    /// A placeholder implementation of <see cref="ILocalizationService"/> intended for
    /// development, prototyping, and UI iteration.
    /// </summary>
    /// <remarks>
    /// This stub does **not** perform real localization lookups.
    /// Instead, it always returns:
    /// <list type="bullet">
    ///   <item><description>The original key unchanged</description></item>
    ///   <item><description>The formatted key, if formatting arguments are supplied</description></item>
    /// </list>
    ///
    /// The goal of this class is to:
    /// <list type="bullet">
    ///   <item><description>Allow UI and text systems to function even without a localization backend</description></item>
    ///   <item><description>Enable fast iteration during early development</description></item>
    ///   <item><description>Provide a reference structure for implementing a real localization system</description></item>
    /// </list>
    ///
    /// **Not for production use.**  
    /// Replace this with an actual localization solution (e.g., Unity Localization package,
    /// Google Sheets → JSON pipeline, or any custom implementation) when shipping a game.
    /// </remarks>
    public class StubLocalizationService : ILocalizationService
    {
        /// <summary>
        /// The language code currently in use by the stub.
        /// Defaults to <c>"en"</c>, but does not affect returned results.
        /// </summary>
        public string CurrentLanguage { get; set; } = "en";

        /// <summary>
        /// Optional override table that allows developers to force specific
        /// strings during UI testing or debugging.
        /// </summary>
        private readonly Dictionary<string, string> _overrides 
            = new Dictionary<string, string>();

        /// <summary>
        /// Returns a localized string for the given key.
        /// In this stub version, the key is simply returned unchanged unless an override exists.
        /// </summary>
        public string Get(string key)
        {
            if (_overrides.TryGetValue(key, out var value))
                return value;

#if UNITY_EDITOR
            Logger.Log($"[Localization Stub] Get('{key}')");
#endif

            return key; // fallback: return the key itself
        }

        /// <summary>
        /// Returns a formatted string using the provided arguments.
        /// Formatting is performed using <see cref="string.Format(string,object[])"/>.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="args">Arguments used to format the value associated with the key.</param>
        /// <returns>The formatted string, or the unformatted key if formatting fails.</returns>
        public string Get(string key, params object[] args)
        {
            string baseString = Get(key);

            try
            {
                return string.Format(baseString, args);
            }
            catch
            {
                Logger.LogWarning($"[Localization Stub] Format error for key '{key}'");
                return baseString;
            }
        }

        /// <summary>
        /// Adds or updates a forced override value for the given key.
        /// Useful when previewing UI or simulating localization changes.
        /// </summary>
        /// <param name="key">The localization key to override.</param>
        /// <param name="value">The forced replacement text.</param>
        public void SetOverride(string key, string value)
        {
            _overrides[key] = value;
        }
    }
}