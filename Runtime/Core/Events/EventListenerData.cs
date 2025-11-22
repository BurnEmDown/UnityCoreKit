using System;
using System.Collections.Generic;

namespace Core.Events
{
    /// <summary>
    /// Represents data related to event listeners.
    /// </summary>
    public class EventListenerData
    {
        /// <summary>
        /// Contains data for event listeners, including the actions to invoke when the event is triggered.
        /// </summary>
        public class EventListenersData
        {
            /// <summary>
            /// A list of actions to be invoked when the associated event is triggered.
            /// </summary>
            public List<Action<object>> ActionsOnInvoke;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventListenersData"/> class.
            /// </summary>
            /// <param name="additionalData">The initial action to add to the list of actions.</param>
            public EventListenersData(Action<object> additionalData)
            {
                ActionsOnInvoke = new List<Action<object>>
                {
                    additionalData
                };
            }
        }
    }
}