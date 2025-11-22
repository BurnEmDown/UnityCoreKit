using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for saving, loading, enumerating, and managing game data
    /// across multiple save slots.
    /// </summary>
    /// <remarks>
    /// Implementations may store data locally (file system, PlayerPrefs), remotely
    /// (cloud sync), or in hybrid solutions. Both synchronous and asynchronous APIs
    /// are provided to support a wide range of storage backends.
    /// </remarks>
    public interface ISaveSystem
    {
        /// <summary>
        /// Saves the specified data into the given slot synchronously.
        /// </summary>
        /// <typeparam name="T">The type of the data being saved.</typeparam>
        /// <param name="slot">A unique identifier for the save slot.</param>
        /// <param name="data">The data object to persist.</param>
        void Save<T>(string slot, T data);
        
        /// <summary>
        /// Loads data of the specified type from the given slot synchronously.
        /// </summary>
        /// <typeparam name="T">The expected type of the data.</typeparam>
        /// <param name="slot">The identifier of the save slot.</param>
        /// <param name="defaultValue">The value to return if loading fails.</param>
        /// <returns>The loaded data or <paramref name="defaultValue"/> if missing.</returns>
        T Load<T>(string slot, T defaultValue = default);
        
        /// <summary>
        /// Returns whether a save exists for the specified slot.
        /// </summary>
        /// <param name="slot">The identifier of the save slot.</param>
        /// <returns><c>true</c> if the slot has saved data; otherwise <c>false</c>.</returns>
        bool HasSave(string slot);
        
        #region Async

        /// <summary>
        /// Saves the specified data into the given slot asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the data being saved.</typeparam>
        /// <param name="slot">The identifier of the save slot.</param>
        /// <param name="data">The data object to persist.</param>
        /// <returns>A task that completes when the save operation has finished.</returns>
        Task SaveAsync<T>(string slot, T data);

        /// <summary>
        /// Loads data of the specified type from the given slot asynchronously.
        /// </summary>
        /// <typeparam name="T">The expected type of the data.</typeparam>
        /// <param name="slot">The identifier of the save slot.</param>
        /// <param name="defaultValue">The fallback value returned if loading fails.</param>
        /// <returns>
        /// A task resolving to the loaded data, or <paramref name="defaultValue"/> if the slot is missing.
        /// </returns>
        Task<T> LoadAsync<T>(string slot, T defaultValue = default);

        #endregion

        /// <summary>
        /// Retrieves the version number of the saved data in the specified slot.
        /// </summary>
        /// <param name="slot">The identifier of the save slot.</param>
        /// <returns>
        /// An integer representing the version of the save data.
        /// Implementations should return 0 or -1 if the slot does not exist or contains no version.
        /// </returns>
        int GetVersion(string slot);

        /// <summary>
        /// Deletes the save data stored under the specified slot.
        /// </summary>
        /// <param name="slot">The identifier of the save slot to remove.</param>
        void Delete(string slot);

        /// <summary>
        /// Deletes all save data for all slots managed by this save system.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Retrieves a list of all save slot identifiers currently stored.
        /// </summary>
        /// <returns>
        /// An enumerable sequence of slot names. The sequence may be empty but should not be <c>null</c>.
        /// </returns>
        IEnumerable<string> GetAllSlots();
    }
}