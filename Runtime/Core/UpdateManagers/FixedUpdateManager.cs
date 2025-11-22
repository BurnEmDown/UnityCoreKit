using System.Collections.Generic;
using Core.UpdateManagers.Interfaces;
using UnityEngine;

namespace Core.UpdateManagers
{
    /// <summary>
    /// Centralized dispatcher for all objects that implement
    /// <see cref="IFixedUpdateObserver"/> and require FixedUpdate callbacks.
    /// </summary>
    /// <remarks>
    /// This manager delivers physics-timestep updates to registered observers via
    /// <see cref="IFixedUpdateObserver.ObservedFixedUpdate"/>.  
    ///
    /// Benefits of centralized fixed-update dispatching:
    /// <list type="bullet">
    ///   <item><description>Reduces overhead from many MonoBehaviours running <c>FixedUpdate()</c></description></item>
    ///   <item><description>Allows pure C# classes to run physics updates without being components</description></item>
    ///   <item><description>Improves determinism in update ordering</description></item>
    ///   <item><description>Keeps all FixedUpdate logic easy to profile and debug</description></item>
    /// </list>
    ///
    /// This manager uses:
    /// <list type="bullet">
    ///   <item><description><c>observers</c> — the active listener list</description></item>
    ///   <item><description><c>pendingObservers</c> — observers registered during iteration</description></item>
    /// </list>
    ///
    /// To prevent collection modification errors, new observers are added to
    /// <c>pendingObservers</c> and merged after the FixedUpdate loop completes.
    /// </remarks>
    public class FixedUpdateManager : MonoBehaviour
    {
        /// <summary>
        /// The active observers receiving FixedUpdate ticks.
        /// </summary>
        private static List<IFixedUpdateObserver> observers = new List<IFixedUpdateObserver>();

        /// <summary>
        /// Observers waiting to be added safely after iteration.
        /// </summary>
        private static List<IFixedUpdateObserver> pendingObservers = new List<IFixedUpdateObserver>();

        /// <summary>
        /// Tracks current index during iteration, allowing safe removal.
        /// </summary>
        private static int currentIndex;

        /// <summary>
        /// Unity FixedUpdate loop — dispatches notifications in reverse order.
        /// </summary>
        private void FixedUpdate()
        {
            for (currentIndex = observers.Count - 1; currentIndex >= 0; currentIndex--)
            {
                observers[currentIndex].ObservedFixedUpdate();
            }

            // Add any observers registered during update
            observers.AddRange(pendingObservers);
            pendingObservers.Clear();
        }

        /// <summary>
        /// Registers an observer to receive FixedUpdate ticks.
        /// Registration is deferred until the current loop ends.
        /// </summary>
        public static void RegisterObserver(IFixedUpdateObserver observer)
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
        /// Unregisters an observer, preventing further FixedUpdate ticks.
        /// </summary>
        public static void UnregisterObserver(IFixedUpdateObserver observer)
        {
            int index = observers.IndexOf(observer);
            if (index >= 0)
            {
                observers.RemoveAt(index);

                // Adjust iterator to avoid skipping an element
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