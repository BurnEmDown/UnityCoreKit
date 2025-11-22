using System;
using System.Collections.Generic;
using Core.Events;
using Core.Interfaces;

namespace Core.Services
{
    /// <summary>
    /// Manages event listener registrations for objects, allowing grouped
    /// add/remove operations and automatic cleanup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This manager acts as a convenience layer on top of <see cref="IEventsManager"/>,
    /// providing the ability to register listeners associated with a specific owner object.
    /// When an owner unregisters or is destroyed, all associated listeners can be removed
    /// in one call.
    /// </para>
    ///
    /// <para>
    /// Internally, the class stores a mapping of owner objects to their registered
    /// event listeners. Each entry tracks both the event type and the callback.
    /// </para>
    /// </remarks>
    public class EventListenerManager : IEventListenerManager
    {
        /// <summary>
        /// Stores all registered listeners grouped by their owner object.
        /// Each owner maps to a list of (event type, callback) pairs.
        /// </summary>
        private static readonly Dictionary<object, List<(EventType eventType, Action<object> method)>> listeners 
            = new();

        /// <summary>
        /// The events manager used to register and unregister event callbacks.
        /// </summary>
        private readonly IEventsManager _eventsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListenerManager"/> class.
        /// </summary>
        /// <param name="eventsManager">
        /// The <see cref="IEventsManager"/> implementation used to manage event subscriptions.
        /// Must not be <c>null</c>.
        /// </param>
        public EventListenerManager(IEventsManager eventsManager)
        {
            _eventsManager = eventsManager ?? throw new ArgumentNullException(nameof(eventsManager));
        }

        /// <summary>
        /// Registers a listener callback for a specific event type, associated with the given owner object.
        /// </summary>
        /// <param name="listener">The object owning the listener (typically <c>this</c> from a behaviour).</param>
        /// <param name="eventType">The event identifier to listen for.</param>
        /// <param name="action">The callback to invoke when the event fires.</param>
        /// <remarks>
        /// This method both:
        /// <list type="bullet">
        /// <item>Registers the callback with <see cref="_eventsManager"/>.</item>
        /// <item>Stores the callback internally so it can be removed later.</item>
        /// </list>
        /// </remarks>
        public void AddListener(object listener, EventType eventType, Action<object> action)
        {
            if (!listeners.ContainsKey(listener))
            {
                listeners[listener] = new List<(EventType, Action<object>)>();
            }

            _eventsManager.AddListener(eventType, action);
            listeners[listener].Add((eventType, action));
        }
        
        /// <summary>
        /// Removes a previously registered listener callback for a specific event type.
        /// </summary>
        /// <param name="listener">The owner object associated with the listener.</param>
        /// <param name="eventType">The event identifier to unregister from.</param>
        /// <param name="action">The specific callback to remove.</param>
        /// <remarks>
        /// If the owner object has no more listeners after removal, it is removed from the internal map.
        /// </remarks>
        public void RemoveListener(object listener, EventType eventType, Action<object> action)
        {
            if (!listeners.TryGetValue(listener, out var listenersToRemove)) return;

            var listenerToRemove = (eventType, action);
            if (listenersToRemove.Remove(listenerToRemove))
            {
                _eventsManager.RemoveListener(eventType, action);
            }

            if (listenersToRemove.Count == 0)
            {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Removes all listeners associated with a specific owner object.
        /// </summary>
        /// <param name="listener">The owner object whose listeners should be removed.</param>
        /// <remarks>
        /// Commonly used when a MonoBehaviour is destroyed to ensure no orphaned callbacks remain.
        /// </remarks>
        public void RemoveListener(object listener)
        {
            if (!listeners.TryGetValue(listener, out var listenersToRemove)) return;

            foreach (var (eventType, method) in listenersToRemove)
            {
                _eventsManager.RemoveListener(eventType, method);
            }

            listeners.Remove(listener);
        }

        /// <summary>
        /// Removes all event listeners for all owners from the event system.
        /// </summary>
        /// <remarks>
            /// Useful for global resets, such as returning to the main menu or reinitializing
            /// the entire event system.
        /// </remarks>
        public void RemoveAllListeners()
        {
            foreach (var listenersToRemove in listeners.Values)
            {
                foreach (var (eventType, method) in listenersToRemove)
                {
                    _eventsManager.RemoveListener(eventType, method);
                }
            }

            listeners.Clear();
        }
    }
}