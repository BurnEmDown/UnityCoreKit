using System;
using System.Collections.Generic;
using UnityCoreKit.Runtime.Core.Interfaces;

namespace UnityCoreKit.Runtime.UserInteractions.Services
{
    /// <summary>
    /// Typed adapter around the UnityCoreKit events system for user interactions.
    /// </summary>
    /// <remarks>
    /// Uses:
    /// <list type="bullet">
    /// <item><see cref="IEventsManager"/> for publishing (invoking) interaction events.</item>
    /// <item><see cref="IEventListenerManager"/> for subscribing/unsubscribing with automatic listener bookkeeping.</item>
    /// </list>
    /// </remarks>
    public sealed class UserInteractionsService : IUserInteractions
    {
        private readonly IEventsManager events;
        private readonly IEventListenerManager listenerManager;

        // We must keep wrapper delegates so we can remove the exact same Action<object> instance later.
        private readonly Dictionary<Action<UserInteractionEvent>, Action<object>> wrappers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInteractionsService"/> class.
        /// </summary>
        /// <param name="events">Core events manager used for publishing interaction events.</param>
        /// <param name="listenerManager">
        /// Listener manager used to register/unregister subscriptions with listener ownership tracking.
        /// </param>
        public UserInteractionsService(IEventsManager events, IEventListenerManager listenerManager)
        {
            this.events = events ?? throw new ArgumentNullException(nameof(events));
            this.listenerManager = listenerManager ?? throw new ArgumentNullException(nameof(listenerManager));
        }

        /// <summary>
        /// Publishes a user interaction event through the core event bus.
        /// </summary>
        public void Publish(in UserInteractionEvent evt)
        {
            // Boxed because EventsManager payload is object.
            events.InvokeEvent(UserInteractionEventType.Interaction, evt);
        }

        /// <summary>
        /// Subscribes a callback to interaction events.
        /// </summary>
        /// <remarks>
        /// The caller is treated as the "listener owner" so it can be mass-unsubscribed later.
        /// If you want a different ownership model, pass an explicit listener owner object.
        /// </remarks>
        public void Subscribe(object listenerOwner, Action<UserInteractionEvent> listener)
        {
            if (listenerOwner == null) throw new ArgumentNullException(nameof(listenerOwner));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            if (wrappers.ContainsKey(listener))
                return;

            void Wrapper(object payload)
            {
                if (payload is UserInteractionEvent interactionEvent)
                    listener(interactionEvent);
            }

            wrappers[listener] = Wrapper;
            listenerManager.AddListener(listenerOwner, UserInteractionEventType.Interaction, Wrapper);
        }

        /// <summary>
        /// Unsubscribes a callback from interaction events.
        /// </summary>
        public void Unsubscribe(object listenerOwner, Action<UserInteractionEvent> listener)
        {
            if (listenerOwner == null) throw new ArgumentNullException(nameof(listenerOwner));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            if (!wrappers.TryGetValue(listener, out var wrapper))
                return;

            listenerManager.RemoveListener(listenerOwner, UserInteractionEventType.Interaction, wrapper);
            wrappers.Remove(listener);
        }

        /// <summary>
        /// Removes all interaction listeners registered under the given owner.
        /// </summary>
        public void UnsubscribeAll(object listenerOwner)
        {
            if (listenerOwner == null) throw new ArgumentNullException(nameof(listenerOwner));
            listenerManager.RemoveListener(listenerOwner);

            // Also clean our wrapper cache entries that may belong to that owner.
        }
    }
}