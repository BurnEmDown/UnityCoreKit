using System.Collections.Generic;
using UnityCoreKit.Runtime.Core.UpdateManagers.Interfaces;
using UnityEngine;
using Logger = UnityCoreKit.Runtime.Core.Utils.Logs.Logger;

namespace UnityCoreKit.Runtime.Core.UpdateManagers
{
    /// <summary>
    /// Centralized dispatcher for all objects that implement
    /// <see cref="ILateUpdateObserver"/> and require LateUpdate callbacks.
    /// </summary>
    /// <remarks>
    /// This manager allows systems and non-MonoBehaviour objects to receive
    /// <c>LateUpdate</c> ticks via <see cref="ILateUpdateObserver.ObservedLateUpdate"/>.
    ///
    /// Benefits of this pattern:
    /// <list type="bullet">
    ///   <item><description>Reduces the number of MonoBehaviours calling <c>LateUpdate()</c></description></item>
    ///   <item><description>Allows pure C# classes to participate in LateUpdate without being components</description></item>
    ///   <item><description>Improves determinism and reduces per-object overhead</description></item>
    ///   <item><description>Centralizes where LateUpdate happens, simplifying debugging</description></item>
    /// </list>
    ///
    /// This implementation maintains two lists:
    /// <list type="bullet">
    ///   <item><description><c>observers</c> — active observers receiving LateUpdate</description></item>
    ///   <item><description><c>pendingObservers</c> — observers registered during iteration</description></item>
    /// </list>
    ///
    /// To avoid modifying collections while iterating, all new registrations are stored
    /// in <c>pendingObservers</c> and merged after the LateUpdate loop finishes.
    /// </remarks>
    public class LateUpdateManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the LateUpdateManager.
        /// Auto-created on first use if not already present in the scene.
        /// </summary>
        private static LateUpdateManager instance;

        /// <summary>
        /// The list of active observers that receive LateUpdate notifications.
        /// </summary>
        private static List<ILateUpdateObserver> observers = new List<ILateUpdateObserver>();

        /// <summary>
        /// Observers queued for addition at the end of the LateUpdate cycle.
        /// </summary>
        private static List<ILateUpdateObserver> pendingObservers = new List<ILateUpdateObserver>();

        /// <summary>
        /// Tracks the current iteration index to safely support removals during iteration.
        /// </summary>
        private static int currentIndex;

        /// <summary>
        /// Ensures a LateUpdateManager instance exists in the scene.
        /// If no instance exists, creates a new GameObject with the LateUpdateManager component
        /// and marks it as DontDestroyOnLoad to persist across scene transitions.
        /// </summary>
        /// <remarks>
        /// This method is called automatically when the first observer is registered,
        /// eliminating the need to manually add LateUpdateManager to the scene hierarchy.
        /// The created GameObject will persist for the lifetime of the application.
        /// </remarks>
        private static void EnsureInstance()
        {
            if (instance == null)
            {
                var go = new GameObject("LateUpdateManager");
                instance = go.AddComponent<LateUpdateManager>();
                DontDestroyOnLoad(go);
                Logger.Log("[LateUpdateManager] Auto-created LateUpdateManager instance");
            }
        }

        /// <summary>
        /// Unity LateUpdate loop — notifies all registered observers in reverse order.
        /// </summary>
        private void LateUpdate()
        {
            // Iterate backwards so removal during iteration is safe
            for (currentIndex = observers.Count - 1; currentIndex >= 0; currentIndex--)
            {
                observers[currentIndex].ObservedLateUpdate();
            }

            // Add any observers that registered during the loop
            observers.AddRange(pendingObservers);
            pendingObservers.Clear();
        }

        /// <summary>
        /// Registers an observer to receive LateUpdate ticks.
        /// Registration is deferred until the end of the current LateUpdate cycle.
        /// </summary>
        /// <param name="observer">The observer to register.</param>
        public static void RegisterObserver(ILateUpdateObserver observer)
        {
            EnsureInstance(); // Auto-create if needed
            
            if (!observers.Contains(observer) && !pendingObservers.Contains(observer))
            {
                pendingObservers.Add(observer);
            }
            else
            {
                Logger.LogWarning("Observer already registered.");
            }
        }

        /// <summary>
        /// Unregisters an observer, preventing it from receiving any more LateUpdate ticks.
        /// </summary>
        /// <param name="observer">The observer to unregister.</param>
        public static void UnregisterObserver(ILateUpdateObserver observer)
        {
            int index = observers.IndexOf(observer);
            if (index >= 0)
            {
                observers.RemoveAt(index);

                // Adjust the iteration index to avoid skipping an element
                if (index <= currentIndex)
                    currentIndex--;
            }
            else
            {
                Logger.LogWarning("Observer not found for removal.");
            }
        }
    }
}