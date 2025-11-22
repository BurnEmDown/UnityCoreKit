using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for factories capable of creating Unity object instances
    /// (usually <see cref="GameObject"/> prefabs) asynchronously.
    /// </summary>
    /// <remarks>
    /// Implementations may load objects from various sources such as:
    /// <list type="bullet">
    /// <item><description>Unity Addressables</description></item>
    /// <item><description>Preloaded prefab dictionaries</description></item>
    /// <item><description>Resources folder (not recommended)</description></item>
    /// <item><description>Remote asset bundles</description></item>
    /// </list>
    ///
    /// The API is asynchronous to support loading systems that require asynchronous
    /// operations (e.g., Addressables). Implementations that load synchronously
    /// may return completed tasks.
    /// </remarks>
    public interface IObjectFactory
    {
        /// <summary>
        /// Creates a single instance of the object associated with the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to be present on the instantiated object.
        /// </typeparam>
        /// <param name="key">
        /// The lookup key used by the factory to identify the prefab or object to instantiate.
        /// </param>
        /// <returns>
        /// A task that resolves to the created component of type <typeparamref name="T"/>,
        /// or <c>null</c> if creation fails.
        /// </returns>
        Task<T> CreateAsync<T>(string key) where T : Component;

        /// <summary>
        /// Creates multiple instances of the object associated with the given key.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to be present on the instantiated objects.
        /// </typeparam>
        /// <param name="key">
        /// The lookup key used by the factory to identify the prefab or object to instantiate.
        /// </param>
        /// <param name="amount">
        /// The number of instances to create.
        /// </param>
        /// <returns>
        /// A task that resolves to a list of created components of type <typeparamref name="T"/>.
        /// Implementations may return an empty list (but should not return <c>null</c>)
        /// if creation fails for all objects.
        /// </returns>
        Task<List<T>> CreateManyAsync<T>(string key, int amount) where T : Component;
    }
}
