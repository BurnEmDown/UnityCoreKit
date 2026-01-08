using System;

namespace UnityCoreKit.Runtime.UserInteractions
{
    /// <summary>
    /// High-level API for publishing and subscribing to interaction events.
    /// Uses the project's <see cref="Core.Interfaces.IEventsManager"/> internally.
    /// </summary>
    public interface IUserInteractions
    {
        void Publish(in UserInteractionEvent evt);

        void Subscribe(Action<UserInteractionEvent> listener);

        void Unsubscribe(Action<UserInteractionEvent> listener);
    }
}