using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

namespace Core.Services
{
    /// <summary>
    /// Provides a factory for instantiating non-addressable prefabs.
    /// </summary>
    /// <remarks>
    /// Prefabs are resolved by a string key from an internal map that is supplied
    /// at construction time. Instances are created synchronously using
    /// <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/>,
    /// but returned through a <see cref="Task"/>-based API for interface compatibility.
    /// </remarks>
    public class PrefabFactory : IFactory
    {
        /// <summary>
        /// Lookup table mapping prefab keys to prefab <see cref="GameObject"/> instances.
        /// </summary>
        private readonly Dictionary<string, GameObject> _prefabs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabFactory"/> class.
        /// </summary>
        /// <param name="prefabMap">
        /// A dictionary mapping string keys to prefab <see cref="GameObject"/> instances.
        /// This map is used as the source for all instantiated objects.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="prefabMap"/> is <c>null</c>.
        /// </exception>
        public PrefabFactory(Dictionary<string, GameObject> prefabMap)
        {
            _prefabs = prefabMap ?? throw new ArgumentNullException(nameof(prefabMap));
        }

        /// <summary>
        /// Generates a list of instantiated components of type <typeparamref name="T"/>
        /// using the prefab registered under the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to be present on the prefab root.
        /// </typeparam>
        /// <param name="key">
        /// The lookup key used to resolve the prefab from the internal map.
        /// </param>
        /// <param name="amount">
        /// The number of instances to create. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A task that, when completed, returns a list of instantiated components.
        /// If the key is not found or the prefab is <c>null</c>, an empty list is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="amount"/> is less than or equal to zero.
        /// </exception>
        public Task<List<T>> GenerateObjectsAsync<T>(string key, int amount) where T : Component
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

            var list = new List<T>(amount);

            if (!_prefabs.TryGetValue(key, out var prefab) || prefab == null)
            {
                Logger.LogError($"No prefab registered for key '{key}'.");
                return Task.FromResult(list); // empty
            }

            for (int i = 0; i < amount; i++)
            {
                var instance = UnityEngine.Object.Instantiate(prefab);
                var component = instance.GetComponent<T>();

                if (component == null)
                {
                    Logger.LogError($"Prefab '{key}' has no {typeof(T).Name}");
                    UnityEngine.Object.Destroy(instance);
                    continue;
                }

                list.Add(component);
            }

            return Task.FromResult(list);
        }

        /// <summary>
        /// Generates a single instantiated component of type <typeparamref name="T"/>
        /// using the prefab registered under the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to be present on the prefab root.
        /// </typeparam>
        /// <param name="key">
        /// The lookup key used to resolve the prefab from the internal map.
        /// </param>
        /// <returns>
        /// A task that, when completed, returns the instantiated component of type
        /// <typeparamref name="T"/> if successful; otherwise <c>null</c>.
        /// </returns>
        public Task<T> GenerateObjectAsync<T>(string key) where T : Component
        {
            if (!_prefabs.TryGetValue(key, out var prefab) || prefab == null)
            {
                Logger.LogError($"No prefab registered for key '{key}'.");
                return Task.FromResult<T>(null);
            }

            var instance = UnityEngine.Object.Instantiate(prefab);
            var component = instance.GetComponent<T>();

            if (component == null)
            {
                Logger.LogError($"Prefab '{key}' has no {typeof(T).Name}");
                UnityEngine.Object.Destroy(instance);
                return Task.FromResult<T>(null);
            }

            return Task.FromResult(component);
        }
    }
}