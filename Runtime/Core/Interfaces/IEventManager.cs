using System;
using Core.Events;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a contract for a publish–subscribe event system.
    /// </summary>
    /// <remarks>
    /// The event manager allows systems to register listeners for specific event types,
    /// remove listeners, and invoke events with optional payload data.
    /// 
    /// This interface abstracts the communication layer between gameplay systems,
    /// promoting decoupled architecture where events can be raised without direct references
    /// to listeners.
    /// </remarks>
    public interface IEventsManager
    {
        /// <summary>
        /// Registers a listener callback for the specified event type.
        /// </summary>
        /// <param name="eventType">
        /// The identifier of the event to listen for. Must be a subclass of <see cref="EventType"/>.
        /// </param>
        /// <param name="additionalData">
        /// The callback invoked when the event is raised. The callback receives an <c>object</c>
        /// payload which can be <c>null</c> or cast to the appropriate data type for the event.
        /// </param>
        /// <remarks>
        /// Multiple listeners may be registered for the same event type.
        /// If the same listener is added more than once, implementations may ignore duplicates.
        /// </remarks>
        void AddListener(EventType eventType, Action<object> additionalData);

        /// <summary>
        /// Removes a previously registered listener from the specified event type.
        /// </summary>
        /// <param name="eventType">
        /// The event identifier whose listener list should be modified.
        /// </param>
        /// <param name="actionToRemove">
        /// The callback previously registered via <see cref="AddListener"/>.
        /// </param>
        /// <remarks>
        /// If the listener is not registered or the event type has no listeners,
        /// the implementation should fail silently.
        /// </remarks>
        void RemoveListener(EventType eventType, Action<object> actionToRemove);

        /// <summary>
        /// Invokes an event, executing all listener callbacks registered
        /// for the specified event type.
        /// </summary>
        /// <param name="eventType">
        /// The identifier of the event to invoke.
        /// </param>
        /// <param name="dataToInvoke">
        /// Optional payload passed to listeners. Can be <c>null</c>.
        /// </param>
        /// <remarks>
        /// Implementations should guarantee that modifying listener lists during invocation
        /// (adding/removing) does not break enumeration.
        /// </remarks>
        void InvokeEvent(EventType eventType, object dataToInvoke);
    }
}