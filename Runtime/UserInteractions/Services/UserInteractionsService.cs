using System;
using System.Collections.Generic;
using UnityCoreKit.Runtime.Core.Interfaces;

namespace UnityCoreKit.Runtime.UserInteractions.Services
{
    /// <summary>
    /// Typed adapter around <see cref="IEventsManager"/> for publishing and subscribing to interactions.
    /// </summary>
    public sealed class InteractionsService : IInteractions
    {
        private readonly IEventsManager events;

        // Required so RemoveListener can remove the exact same delegate instance that was added.
        private readonly Dictionary<Action<UserInteractionEvent>, Action<object>> wrappers = new();

        public InteractionsService(IEventsManager events)
        {
            this.events = events;
        }

        public void Publish(in UserInteractionEvent evt)
        {
            // Boxed because EventsManager uses object payloads.
            events.InvokeEvent(UserInteractionEventType.Interaction, evt);
        }

        public void Subscribe(Action<UserInteractionEvent> listener)
        {
            if (wrappers.ContainsKey(listener))
                return;

            void Wrapper(object payload)
            {
                if (payload is UserInteractionEvent interactionEvent)
                    listener(interactionEvent);
            }

            wrappers[listener] = Wrapper;
            events.AddListener(UserInteractionEventType.Interaction, Wrapper);
        }

        public void Unsubscribe(Action<UserInteractionEvent> listener)
        {
            if (!wrappers.TryGetValue(listener, out var wrapper))
                return;

            events.RemoveListener(UserInteractionEventType.Interaction, wrapper);
            wrappers.Remove(listener);
        }
    }

    /// <summary>
    /// Public API for the interactions module.
    /// </summary>
    public interface IInteractions
    {
        void Publish(in UserInteractionEvent evt);
        void Subscribe(Action<UserInteractionEvent> listener);
        void Unsubscribe(Action<UserInteractionEvent> listener);
    }
}
