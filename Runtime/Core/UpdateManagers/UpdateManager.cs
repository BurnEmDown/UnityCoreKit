using System.Collections.Generic;
using Core.UpdateManagers.Interfaces;
using UnityEngine;

namespace Core.UpdateManagers
{
    /// <summary>
    /// Centralized update dispatcher for all objects that implement
    /// <see cref="IUpdateObserver"/>.  
    ///
    /// Instead of attaching many MonoBehaviours with their own <c>Update()</c> calls,
    /// this manager allows systems to register lightweight C# objects or components
    /// and receive update ticks through <see cref="IUpdateObserver.ObservedUpdate"/>.
    /// </summary>
    /// <remarks>
    /// Benefits of this pattern:
    /// <list type="bullet">
    ///   <item><description>Reduces overhead caused by Unity calling <c>Update()</c> on many MonoBehaviours.</description></item>
    ///   <item><description>Allows non-MonoBehaviour classes to receive update ticks.</description></item>
    ///   <item><description>Centralizes update flow for easier profiling and debugging.</description></item>
    ///   <item><description>Provides deterministic update ordering for registered observers.</description></item>
    /// </list>
    ///
    /// This manager uses two lists:
    /// <list type="bullet">
    ///   <item><description><c>observers</c> — the active, iterated list</description></item>
    ///   <item><description><c>pendingObservers</c> — observers waiting to be added safely</description></item>
    /// </list>
    ///
    /// To avoid modifying the collection while iterating, new observers are added to
    /// <c>pendingObservers</c> and merged after the main update loop.
    /// </remarks>
    public class UpdateManager : MonoBehaviour
    {
        /// <summary>
        /// The main list of active observers that receive update ticks.
        /// </summary>
        private static List<IUpdateObserver> observers = new List<IUpdateObserver>();

        /// <summary>
        /// Observers waiting to be added at the end of the update cycle.
        /// </summary>
        private static List<IUpdateObserver> pendingObservers = new List<IUpdateObserver>();

        /// <summary>
        /// Tracks the current index during iteration to safely support removal
        /// while iterating backward.
        /// </summary>
        private static int currentIndex;

        /// <summary>
        /// Unity Update loop — dispatches update notifications to all registered observers.
        /// </summary>
        private void Update()
        {
            // Iterate backwards so removing observers during update is safe
            for (currentIndex = observers.Count - 1; currentIndex >= 0; currentIndex--)
            {
                observers[currentIndex].ObservedUpdate();
            }

            // Add any observers registered during the update loop
            observers.AddRange(pendingObservers);
            pendingObservers.Clear();
        }

        /// <summary>
        /// Registers an observer to receive update ticks.
        /// Registration is delayed until the end of the current update loop.
        /// </summary>
        /// <param name="observer">The observer to register.</param>
        public static void RegisterObserver(IUpdateObserver observer)
        {
            if (!observers.Contains(observer) && !pendingObservers.Contains(observer))
            {
                pendingObservers.Add(observer);
            }
            else
            {
                Debug.LogWarning("Observer already registered.");
            }
        }

        /// <summary>
        /// Unregisters an observer so it no longer receives update ticks.
        /// </summary>
        /// <param name="observer">The observer to remove.</param>
        public static void UnregisterObserver(IUpdateObserver observer)
        {
            int index = observers.IndexOf(observer);
            if (index >= 0)
            {
                observers.RemoveAt(index);

                // Adjust iterator so removal does not skip elements
                if (index <= currentIndex)
                    currentIndex--;
            }
            else
            {
                Debug.LogWarning("Observer not found for removal.");
            }
        }
    }
}