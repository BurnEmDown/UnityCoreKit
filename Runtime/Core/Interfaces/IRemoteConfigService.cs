using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for accessing remote configuration values provided by
    /// an external service (e.g., Firebase Remote Config or a custom backend).
    /// </summary>
    /// <remarks>
    /// Remote configuration services allow runtime tuning of gameplay values,
    /// feature flags, A/B testing parameters, difficulty settings, and other
    /// live-operated game data without requiring a new app build.
    /// Implementations are expected to cache fetched values and provide fast,
    /// local retrieval of typed configuration entries.
    /// </remarks>
    public interface IRemoteConfigService
    {
        /// <summary>
        /// Initializes the remote configuration system and fetches the latest configuration
        /// values from the remote backend.
        /// </summary>
        /// <returns>
        /// A task that completes when initialization and any required fetch operations are finished.
        /// </returns>
        /// <remarks>
        /// Implementations may include throttling rules, caching, fallback logic, or platform-specific
        /// initialization steps. This method should be called at startup before accessing values.
        /// </remarks>
        Task InitializeAsync();
        
        /// <summary>
        /// Retrieves a boolean configuration value associated with the given key.
        /// </summary>
        /// <param name="key">
        /// The configuration key to look up.
        /// </param>
        /// <param name="defaultValue">
        /// The value returned if the key is not found or the stored value is not a boolean.
        /// </param>
        /// <returns>
        /// The resolved boolean configuration value.
        /// </returns>
        bool GetBool(string key, bool defaultValue = false);
        
        /// <summary>
        /// Retrieves an integer configuration value associated with the given key.
        /// </summary>
        /// <param name="key">
        /// The configuration key to look up.
        /// </param>
        /// <param name="defaultValue">
        /// The value returned if the key is not found or the stored value is not an integer.
        /// </param>
        /// <returns>
        /// The resolved integer configuration value.
        /// </returns>
        int GetInt(string key, int defaultValue = 0);
        
        /// <summary>
        /// Retrieves a floating-point configuration value associated with the given key.
        /// </summary>
        /// <param name="key">
        /// The configuration key to look up.
        /// </param>
        /// <param name="defaultValue">
        /// The value returned if the key is not found or the stored value cannot be represented
        /// as a <see cref="double"/>.
        /// </param>
        /// <returns>
        /// The resolved double configuration value.
        /// </returns>
        double GetDouble(string key, double defaultValue = 0);
        
        /// <summary>
        /// Retrieves a string configuration value associated with the given key.
        /// </summary>
        /// <param name="key">
        /// The configuration key to look up.
        /// </param>
        /// <param name="defaultValue">
        /// The value returned if the key is not found or the stored value is not a string.
        /// </param>
        /// <returns>
        /// The resolved string configuration value.
        /// </returns>
        string GetString(string key, string defaultValue = "");
    }
}
