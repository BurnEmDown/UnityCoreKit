using UnityCoreKit.Runtime.Core.Events;

namespace UnityCoreKit.Runtime.UserInteractions
{
    /// <summary>
    /// Defines event types for the UserInteractions module.
    /// Uses the shared UnityCoreKit event bus (<see cref="UnityCoreKit.Runtime.Core.Interfaces.IEventsManager"/>).
    /// </summary>
    public class UserInteractionEventType : EventType<UserInteractionEventType.InteractionEventId>
    {
        public enum InteractionEventId 
        {
            Interaction,
            Click
        }

        /// <summary>
        /// Static instance used as the event key for all interaction events.
        /// IMPORTANT: because EventsManager keys by EventType instance, use this exact instance everywhere.
        /// </summary>
        public static readonly UserInteractionEventType Interaction =
            new UserInteractionEventType(InteractionEventId.Interaction);
        
        public static readonly UserInteractionEventType Click =
            new UserInteractionEventType(InteractionEventId.Click);

        private UserInteractionEventType(InteractionEventId value) : base(value) { }
    }
}