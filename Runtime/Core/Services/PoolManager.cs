using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Utils.Extensions;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;
using Object = UnityEngine.Object;

namespace Core.Services
{
    /// <summary>
    /// Manages pools of reusable objects to minimize instantiation and destruction costs.
    /// </summary>
    /// <remarks>
    /// Pools are identified by string keys and are populated using an <see cref="IObjectFactory"/>.
    /// Each pool has its own holder GameObject under a global <see cref="PoolsHolder"/> root.
    /// </remarks>
    public class PoolManager : IPoolManager
    {
        /// <summary>
        /// Internal lookup of all pools by pool name.
        /// </summary>
        private Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();
        
        /// <summary>
        /// Root GameObject under which all pool holder objects are parented.
        /// Marked as <see cref="Object.DontDestroyOnLoad(Object)"/>.
        /// </summary>
        public GameObject PoolsHolder;

        private readonly IObjectFactory objectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolManager"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor injects the <see cref="IObjectFactory"/> used to create
        /// pooled objects when a pool runs out of available instances. It also
        /// creates the persistent <see cref="PoolsHolder"/> GameObject, which serves
        /// as the root container for all pool-specific holders in the hierarchy.
        /// </remarks>
        /// <param name="objectFactory">
        /// The object factory used to create new pooled instances when required.
        /// Must not be <c>null</c>.
        /// </param>
        public PoolManager(IObjectFactory objectFactory)
        {
            PoolsHolder = new GameObject("PoolsHolder");
            Object.DontDestroyOnLoad(PoolsHolder);
            this.objectFactory = objectFactory;
        }

        
        /// <summary>
        /// Initializes a pool by creating a predefined number of objects using the
        /// configured <see cref="IObjectFactory"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The component type expected on each pooled object.
        /// </typeparam>
        /// <param name="originalName">
        /// The key used to create objects from the factory and to identify the pool.
        /// </param>
        /// <param name="amount">
        /// The number of objects to pre-instantiate into the pool.
        /// </param>
        public async void InitPool<T>(string originalName, int amount) where T : Component
        {
            var generatedObjects = await objectFactory.CreateManyAsync<T>(originalName, amount);

            if (generatedObjects == null || !generatedObjects.Any())
            {
                Logger.LogError($"Failed to generate objects for pool of item types {originalName}");
                return;
            }

            var poolHolder = new GameObject($"Pool_{originalName}");
            poolHolder.transform.SetParent(PoolsHolder.transform);

            pools[originalName] = new PoolData(generatedObjects.ToArray(), poolHolder);
        }

        /// <summary>
        /// Retrieves an object from the specified pool and returns it via a callback.
        /// If the pool is empty, an object is created on demand using the <see cref="IObjectFactory"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The component type required on the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The name of the pool from which to retrieve an object.
        /// </param>
        /// <param name="parentObject">
        /// The GameObject that will become the parent of the retrieved instance.
        /// </param>
        /// <param name="onObjectReady">
        /// Callback invoked when the object is ready for use. Receives <c>null</c> on failure.
        /// </param>
        public void GetFromPool<T>(string poolName, GameObject parentObject, Action<T> onObjectReady) where T : Component
        {
            if (!pools.ContainsKey(poolName))
            {
                Logger.LogError("No pool of type: " + poolName);
                onObjectReady(null);
                return;
            }

            PoolData poolData = pools[poolName];
            if (!poolData.availableItems.Any())
            {
                objectFactory.CreateAsync<T>(poolName).ContinueWithOnMainThread(
                    task =>
                    {
                        if (task.IsFaulted || task.Result == null)
                        {
                            Logger.LogError("Failed to generate object asynchronously: " + task.Exception?.ToString());
                            onObjectReady(null);
                        }
                        else
                        {
                            T generatedObject = task.Result;
                            poolData.AddNewToPool(generatedObject);
                            SetupAndReturnObject(poolData, parentObject, onObjectReady);
                        }
                    });
            }
            else
            {
                SetupAndReturnObject(poolData, parentObject, onObjectReady);
            }
        }

        /// <summary>
        /// Asynchronously retrieves an object from the specified pool.
        /// If the pool is empty, an object is created on demand using the <see cref="IObjectFactory"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The component type required on the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The name of the pool from which to retrieve an object.
        /// </param>
        /// <param name="parent">
        /// The GameObject that will become the parent of the retrieved instance.
        /// </param>
        /// <returns>
        /// A task that resolves to the retrieved component of type <typeparamref name="T"/>,
        /// or <c>null</c> if the pool does not exist or an instance could not be created.
        /// </returns>
        public async Task<T> GetFromPoolAsync<T>(string poolName, GameObject parent) where T : Component
        {
            if (!pools.TryGetValue(poolName, out var poolData))
            {
                Logger.LogError("No pool of type: " + poolName);
                return null;
            }

            // If we have available items, use one immediately
            if (poolData.availableItems.Any())
            {
                var availableItem = poolData.availableItems[0] as T;
                poolData.availableItems.RemoveAt(0);
                poolData.unavailableItems.Add(availableItem);

                if (availableItem != null)
                {
                    availableItem.transform.SetParent(parent.transform);
                    availableItem.gameObject.SetActive(true);
                }

                return availableItem;
            }

            // Otherwise, create a new one via the factory
            var created = await objectFactory.CreateAsync<T>(poolName);
            if (created == null)
            {
                Logger.LogError("Failed to generate object asynchronously.");
                return null;
            }

            // Add to pool (disabled, parented to PoolHolder)
            poolData.AddNewToPool(created);

            // Take it out of the available list and activate it for use
            if (!poolData.availableItems.Any())
            {
                Logger.LogError("Pool did not contain any available items after adding a new one.");
                return null;
            }

            var newItem = poolData.availableItems[0] as T;
            poolData.availableItems.RemoveAt(0);
            poolData.unavailableItems.Add(newItem);

            if (newItem != null)
            {
                newItem.transform.SetParent(parent.transform);
                newItem.gameObject.SetActive(true);
            }

            return newItem;
        }

        /// <summary>
        /// Internal helper that moves an available object from the pool to a parent
        /// and invokes the provided callback.
        /// </summary>
        private void SetupAndReturnObject<T>(PoolData poolData, GameObject parentObject, Action<T> onObjectReady) where T : Component
        {
            if (poolData.availableItems.Count > 0)
            {
                T availableItem = poolData.availableItems[0] as T;
                poolData.availableItems.RemoveAt(0);
                poolData.unavailableItems.Add(availableItem);
        
                availableItem.transform.SetParent(parentObject.transform);
                availableItem.gameObject.SetActive(true);
        
                onObjectReady(availableItem);
            }
            else
            {
                onObjectReady(null);
            }
        }
        
        /// <summary>
        /// Returns an object back to its pool so it can be reused later.
        /// </summary>
        /// <typeparam name="T">
        /// The component type of the pooled object.
        /// </typeparam>
        /// <param name="poolName">
        /// The name of the pool to which the object belongs.
        /// </param>
        /// <param name="returnedObject">
        /// The instance to return to the pool.
        /// </param>
        public void ReturnToPool<T>(string poolName, T returnedObject) where T : Component
        {
            returnedObject.gameObject.SetActive(false);
            pools[poolName].availableItems.Add(returnedObject);
            pools[poolName].unavailableItems.Remove(returnedObject);
        }
    }

    /// <summary>
    /// Holds metadata and object lists for a single pool.
    /// </summary>
    public class PoolData
    {
        /// <summary>
        /// All objects that belong to the pool (both available and unavailable).
        /// </summary>
        public List<Component> totalItems;

        /// <summary>
        /// Objects currently available to be retrieved from the pool.
        /// </summary>
        public List<Component> availableItems;

        /// <summary>
        /// Objects currently in use and not available for retrieval.
        /// </summary>
        public List<Component> unavailableItems;

        /// <summary>
        /// Parent GameObject under which all pooled instances are organized in the hierarchy.
        /// </summary>
        public GameObject PoolHolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolData"/> class,
        /// assigning all generated objects to the pool holder and marking them inactive.
        /// </summary>
        /// <param name="generatedObjects">
        /// The initial set of objects to populate the pool.
        /// </param>
        /// <param name="poolHolder">
        /// The GameObject that will act as the parent for all pooled instances.
        /// </param>
        public PoolData(Component[] generatedObjects, GameObject poolHolder)
        {
            totalItems = generatedObjects.ToList();
            availableItems = generatedObjects.ToList();
            unavailableItems = new List<Component>();

            PoolHolder = poolHolder;
            foreach (var generatedObject in generatedObjects)
            {
                generatedObject.gameObject.transform.SetParent(PoolHolder.transform);
                generatedObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Adds a newly created object to the pool, parenting it under the pool holder
        /// and disabling it so it starts as available.
        /// </summary>
        /// <typeparam name="T">
        /// The component type of the object being added.
        /// </typeparam>
        /// <param name="generatedObject">
        /// The newly generated object to add to the pool.
        /// </param>
        public void AddNewToPool<T>(T generatedObject) where T : Component
        {
            totalItems.Add(generatedObject);
            availableItems.Add(generatedObject);
            generatedObject.gameObject.transform.SetParent(PoolHolder.transform);
            generatedObject.gameObject.SetActive(false);
        }
    }
}