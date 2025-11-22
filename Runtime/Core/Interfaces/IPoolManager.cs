using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for a pooled object management system.
    /// </summary>
    /// <remarks>
    /// A pool manager pre-instantiates objects and stores them in memory so they can be
    /// reused without incurring additional allocation or instantiation cost.
    ///
    /// Pools are typically identified using a string key, and objects are retrieved or
    /// returned based on that key. Implementations may also organize pooled objects under
    /// parent GameObjects for scene hierarchy cleanliness.
    /// </remarks>
    public interface IPoolManager
    {
        /// <summary>
        /// Initializes a new pool by creating a predefined number of objects using the
        /// specified prefab key or name.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to exist on each pooled object.
        /// </typeparam>
        /// <param name="originalName">
        /// The identifier used to resolve the prefab or original object that will be cloned
        /// to populate the pool.
        /// </param>
        /// <param name="amount">
        /// The number of objects to pre-instantiate and store in the pool.
        /// </param>
        void InitPool<T>(string originalName, int amount) where T : Component;
        
        /// <summary>
        /// Retrieves an object from the requested pool and delivers it through a callback.
        /// </summary>
        /// <typeparam name="T">
        /// The component type required on the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The identifier of the pool from which to retrieve an instance.
        /// </param>
        /// <param name="parentObject">
        /// The transform or GameObject under which the retrieved object should be parented.
        /// Typically used to keep the hierarchy organized.
        /// </param>
        /// <param name="onObjectReady">
        /// A callback invoked when the object is ready for use. The callback receives the
        /// instantiated component of type <typeparamref name="T"/> or <c>null</c> if retrieval fails.
        /// </param>
        /// <remarks>
        /// Implementations may create a new pooled object on-demand if the pool is empty,
        /// depending on design choices.
        /// </remarks>
        void GetFromPool<T>(string poolName, GameObject parentObject, Action<T> onObjectReady)
            where T : Component;
        
        /// <summary>
        /// Retrieves an object from the specified pool asynchronously.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected to exist on the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The identifier of the pool from which to retrieve an instance.
        /// </param>
        /// <param name="parent">
        /// The GameObject under which the retrieved object should be parented.
        /// This is typically used to keep the hierarchy organized.
        /// </param>
        /// <returns>
        /// A task that resolves to the retrieved component of type <typeparamref name="T"/>.
        /// If the pool is empty and the implementation supports on-demand instantiation,
        /// the task may include the time required to asynchronously create a new instance.
        /// Returns <c>null</c> if the pool does not exist, the prefab cannot be created,
        /// or the retrieved instance does not contain the required component.
        /// </returns>
        /// <remarks>
        /// Implementations should place the returned object in an active state and assign
        /// its parent to the provided <paramref name="parent"/> GameObject before returning it.
        ///
        /// Async retrieval is useful when pooled objects may require asynchronous construction,
        /// such as when instantiating Addressables or remote-loaded prefabs.
        /// </remarks>
        Task<T> GetFromPoolAsync<T>(string poolName, GameObject parent) where T : Component;
        
        /// <summary>
        /// Returns a previously retrieved object back to its pool so it can be reused.
        /// </summary>
        /// <typeparam name="T">
        /// The component type required on the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The identifier of the pool to which the object belongs.
        /// </param>
        /// <param name="item">
        /// The instance being returned. Implementations typically disable the GameObject
        /// and move it back under the pool's hierarchy.
        /// </param>
        void ReturnToPool<T>(string poolName, T item) where T : Component;
    }
}