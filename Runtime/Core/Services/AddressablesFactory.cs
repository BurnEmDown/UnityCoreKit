using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Logger = Core.Utils.Logs.Logger;

namespace Core.Services
{
    /// <summary>
    /// Factory implementation that creates prefabs using Unity Addressables.
    /// </summary>
    /// <remarks>
    /// This factory loads prefabs asynchronously from the Addressables system using a
    /// string key. Loaded prefabs are cached in memory to avoid repeated load operations.
    /// </remarks>
    public class AddressablesFactory : IFactory
    {
        /// <summary>
        /// Internal cache storing already-loaded prefabs keyed by their addressable key.
        /// Helps avoid redundant <see cref="Addressables.LoadAssetAsync{TObject}(object)"/> calls.
        /// </summary>
        private readonly Dictionary<string, GameObject> _prefabCache = new();
        
        /// <summary>
        /// Generates multiple instances of a prefab registered in Addressables under the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected on the instantiated prefab's root object.
        /// </typeparam>
        /// <param name="addressableKey">
        /// The key used to load the prefab from the Addressables system.
        /// </param>
        /// <param name="amount">
        /// The number of instances to create. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A task that resolves to a list of instantiated components of type <typeparamref name="T"/>,
        /// or <c>null</c> if the prefab could not be loaded.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="amount"/> is less than or equal to zero.
        /// </exception>
        public async Task<List<T>> GenerateObjectsAsync<T>(string addressableKey, int amount)
            where T : Component
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");

            var prefab = await GetPrefab(addressableKey);
            if (prefab == null)
            {
                Logger.LogError($"[Addressables Factory] Failed to load prefab for key '{addressableKey}'.");
                return new List<T>();
            }

            var created = new List<T>(amount);

            for (int i = 0; i < amount; i++)
            {
                var instance = GameObject.Instantiate(prefab);
                var component = instance.GetComponent<T>();

                if (component == null)
                {
                    Logger.LogError(
                        $"[Addressables Factory] Prefab for key '{addressableKey}' does not contain component of type {typeof(T).Name}."
                    );
                    GameObject.Destroy(instance);
                    continue;
                }

                created.Add(component);
            }

            return created;
        }
        
        /// <summary>
        /// Generates a single instance of a prefab registered under the given Addressables key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected on the instantiated prefab's root object.
        /// </typeparam>
        /// <param name="addressableKey">
        /// The key used to load the prefab from the Addressables system.
        /// </param>
        /// <returns>
        /// A task that resolves to an instantiated component of type <typeparamref name="T"/>,
        /// or <c>null</c> if the prefab failed to load or does not contain the required component.
        /// </returns>
        public async Task<T> GenerateObjectAsync<T>(string addressableKey) where T : Component
        {
            var prefab = await GetPrefab(addressableKey);
            if (prefab == null)
            {
                Logger.LogError($"[Addressables Factory] Failed to load prefab for key '{addressableKey}'.");
                return null;
            }

            var instance = GameObject.Instantiate(prefab);
            var component = instance.GetComponent<T>();

            if (component == null)
            {
                Logger.LogError(
                    $"[Addressables Factory] Prefab for key '{addressableKey}' does not contain component of type {typeof(T).Name}."
                );
                GameObject.Destroy(instance);
                return null;
            }

            return component;
        }
    
        /// <summary>
        /// Loads a prefab from Addressables and caches it for future use.
        /// </summary>
        /// <param name="addressableKey">
        /// The key used to load the prefab.
        /// </param>
        /// <returns>
        /// A task that resolves to the loaded prefab <see cref="GameObject"/>,
        /// or <c>null</c> if loading failed.
        /// </returns>
        /// <remarks>
        /// This method will return a cached version of the prefab if it has already been loaded.
        /// </remarks>
        private async Task<GameObject> GetPrefab(string addressableKey)
        {
            if (string.IsNullOrEmpty(addressableKey))
            {
                Logger.LogError("[Addressables Factory] Addressable key is null or empty.");
                return null;
            }

            if (_prefabCache.TryGetValue(addressableKey, out var cached))
            {
                return cached;
            }

            AsyncOperationHandle<GameObject> handle =
                Addressables.LoadAssetAsync<GameObject>(addressableKey);

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var prefab = handle.Result;
                _prefabCache[addressableKey] = prefab;
                return prefab;
            }

            Logger.LogError($"[Addressables Factory] Addressables failed to load asset for key '{addressableKey}'.");
            return null;
        }
    }
}