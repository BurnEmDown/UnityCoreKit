namespace Core.Interfaces
{
    /// <summary>
    /// Provides methods for managing localization and retrieving localized strings.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Retrieves a localized string for the specified key.
        /// </summary>
        /// <param name="key">The key identifying the localized string.</param>
        /// <returns>The localized string associated with the key.</returns>
        string Get(string key);

        /// <summary>
        /// Retrieves a formatted localized string for the specified key, using the provided arguments.
        /// </summary>
        /// <param name="key">The key identifying the localized string.</param>
        /// <param name="args">The arguments to format the localized string.</param>
        /// <returns>The formatted localized string associated with the key.</returns>
        string Get(string key, params object[] args);

        /// <summary>
        /// Gets or sets the current language for localization.
        /// </summary>
        string CurrentLanguage { get; set; }
    }
}