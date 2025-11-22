using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using UnityEngine;

namespace Core.Services
{
    /// <summary>
    /// A factory that delegates object creation to either an
    /// <see cref="AddressablesFactory"/> or a <see cref="PrefabFactory"/>,
    /// depending on the format of the provided key.
    /// </summary>
    /// <remarks>
    /// This class allows you to use a single unified factory interface
    /// (<see cref="IObjectFactory"/>) while supporting both Addressables-based
    /// and prefab-based object creation.
    /// 
    /// Determining which factory to use is handled by <see cref="IsAddressablesKey"/>,
    /// which can be customized to support your own key conventions, tags, or rules.
    /// </remarks>
    public class CompositeFactory : IObjectFactory
    {
        private readonly AddressablesFactory _addressablesFactory;
        private readonly PrefabFactory _prefabFactory;

        /// <summary>
        /// Creates a new <see cref="CompositeFactory"/> instance.
        /// </summary>
        /// <param name="addressablesFactory">
        /// Factory used for Addressables-based object instantiation.
        /// </param>
        /// <param name="prefabFactory">
        /// Factory used for non-Addressables prefab instantiation.
        /// </param>
        public CompositeFactory(AddressablesFactory addressablesFactory,
                                PrefabFactory prefabFactory)
        {
            _addressablesFactory = addressablesFactory;
            _prefabFactory = prefabFactory;
        }

        /// <summary>
        /// Creates a single object of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="key">
        /// The identifier used to look up the prefab or Addressable asset.
        /// Keys that match <see cref="IsAddressablesKey"/> are treated as
        /// Addressables keys; otherwise, the prefab factory is used.
        /// </param>
        /// <typeparam name="T">The component type expected on the created object.</typeparam>
        /// <returns>A task resolving to the created component, or <c>null</c> if creation failed.</returns>
        public Task<T> CreateAsync<T>(string key) where T : Component
        {
            return IsAddressablesKey(key) ?
                _addressablesFactory.GenerateObjectAsync<T>(key) :
                _prefabFactory.GenerateObjectAsync<T>(key);
        }

        /// <summary>
        /// Creates multiple objects of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <param name="key">
        /// The identifier used to look up each prefab or Addressable asset.
        /// Keys that match <see cref="IsAddressablesKey"/> are treated as
        /// Addressables keys; otherwise, the prefab factory is used.
        /// </param>
        /// <param name="amount">The number of instances to create.</param>
        /// <typeparam name="T">The component type expected on each created object.</typeparam>
        /// <returns>A task resolving to a list of created components.</returns>
        public Task<List<T>> CreateManyAsync<T>(string key, int amount) where T : Component
        {
            return IsAddressablesKey(key) ?
                _addressablesFactory.GenerateObjectsAsync<T>(key, amount) :
                _prefabFactory.GenerateObjectsAsync<T>(key, amount);
        }

        /// <summary>
        /// Determines whether a given key represents an Addressables asset key.
        /// </summary>
        /// <param name="key">The key to inspect.</param>
        /// <returns>
        /// <c>true</c> if the key is considered an Addressables key; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The default implementation checks whether the key starts with <c>"addr:"</c>.
        /// You may override this method and use any convention you prefer, including
        /// checking a configuration file, static dictionary, or metadata.
        /// </remarks>
        public virtual bool IsAddressablesKey(string key)
        {
            // Customize this convention as needed.
            return key.StartsWith("addr:");
        }
    }
}