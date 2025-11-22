using System;

namespace Core.Events
{
    /// <summary>
    /// Represents a base class for event types. 
    /// Each event type has a unique identifier.
    /// </summary>
    public abstract class EventType
    {
        /// <summary>
        /// Gets the unique identifier for the event type.
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// Returns a string representation of the event type.
        /// </summary>
        /// <returns>A string containing the name and ID of the event type.</returns>
        public override string ToString() => $"{GetType().Name}({Id})";
    }
    
    /// <summary>
    /// Represents a generic event type based on an enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type used for the event.</typeparam>
    public class EventType<TEnum> : EventType where TEnum : Enum
    {
        /// <summary>
        /// Gets or sets the enumeration value associated with the event type.
        /// </summary>
        public TEnum Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventType{TEnum}"/> class.
        /// </summary>
        /// <param name="value">The enumeration value for the event type.</param>
        protected EventType(TEnum value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the unique identifier for the event type, derived from the underlying enumeration value.
        /// </summary>
        public override int Id => Convert.ToInt32(Value);
    }
    
    // Example implementation
    public class ExampleEventType : EventType<ExampleEventType.GameEventEnum>
    {
        public enum GameEventEnum
        {
            PlayerDied,
            LevelUp
            // Add more as needed
        }

        // Static instances for convenience (optional but nice)
        public static readonly ExampleEventType PlayerDied =
            new ExampleEventType(GameEventEnum.PlayerDied);

        public static readonly ExampleEventType LevelUp =
            new ExampleEventType(GameEventEnum.LevelUp);

        // Constructor
        public ExampleEventType(GameEventEnum value) : base(value) { }
    }
}