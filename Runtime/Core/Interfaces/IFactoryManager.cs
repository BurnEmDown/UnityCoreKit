using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Provides methods for generating objects asynchronously.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Generates a specified number of objects of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the objects to generate, which must inherit from <see cref="Component"/>.</typeparam>
        /// <param name="addressableKey">The key used to locate the addressable asset.</param>
        /// <param name="amount">The number of objects to generate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of generated objects.</returns>
        Task<List<T>> GenerateObjectsAsync<T>(string addressableKey, int amount) where T : Component;

        /// <summary>
        /// Generates a single object of type <typeparamref name="T"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to generate, which must inherit from <see cref="Component"/>.</typeparam>
        /// <param name="addressableKey">The key used to locate the addressable asset.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated object.</returns>
        Task<T> GenerateObjectAsync<T>(string addressableKey) where T : Component;
    }
}