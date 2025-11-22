using System;
using Core.Events;

namespace Core.Interfaces
{
    /// <summary>
    /// Provides methods for managing event listeners.
    /// </summary>
    public interface IEventListenerManager
    {
        /// <summary>
        /// Adds a listener for a specific event type with action to invoke.
        /// </summary>
        /// <param name="listener">The Listener object</param>
        /// <param name="eventType">The type of event to listen for.</param>
        /// <param name="action">The action to invoke when the event is triggered.</param>
        void AddListener(object listener, EventType eventType, Action<object> action);

        /// <summary>
        /// Removes a specific event type listen for a specific listener.
        /// </summary>
        /// <param name="listener">The listener object</param>
        /// <param name="eventType">The type of event the listener is associated with.</param>
        /// <param name="action">The action to remove from the event listener.</param>
        void RemoveListener(object listener, EventType eventType, Action<object> action);

        /// <summary>
        /// Removes all listeners associated with a specific object.
        /// </summary>
        /// <param name="listener">The listener object whose listeners should be removed.</param>
        void RemoveListener(object listener);

        /// <summary>
        /// Removes all event listeners.
        /// </summary>
        void RemoveAllListeners();
    }
}