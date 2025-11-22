using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Interfaces;

namespace Core.Services
{
    /// <summary>
    /// Default implementation of <see cref="IEventsManager"/> providing a thread-safe,
    /// queued publish–subscribe event system.
    /// </summary>
    /// <remarks>
    /// This event manager serializes all listener modifications and event invocations
    /// via an internal <see cref="ConcurrentQueue{T}"/>.  
    /// 
    /// The queue ensures:
    /// <list type="bullet">
    /// <item>No modification of the listeners dictionary while invoking events.</item>
    /// <item>No recursive event execution issues.</item>
    /// <item>Consistent order of listener additions, removals, and event dispatch.</item>
    /// </list>
    /// 
    /// <para>
    /// This implementation is not tied to Unity's main thread; however, if event
    /// callbacks interact with Unity objects, they must still be executed on the
    /// Unity thread by the caller.
    /// </para>
    /// </remarks>
    public class EventsManager : IEventsManager
    {
        /// <summary>
        /// Queue storing pending listener modifications and event invocations.
        /// </summary>
        private readonly ConcurrentQueue<Action> actionQueue = new();

        /// <summary>
        /// Guards against re-entrant processing of the action queue.
        /// </summary>
        private bool isProcessingQueue = false;

        /// <summary>
        /// Dictionary of event types mapped to their registered listener callbacks.
        /// </summary>
        public Dictionary<EventType, EventListenerData.EventListenersData> ListenersData = new();

        /// <summary>
        /// Registers a listener callback for the specified event type.
        /// </summary>
        /// <param name="eventType">The event identifier.</param>
        /// <param name="additionalData">
        /// The callback invoked when the event is fired. The callback receives an optional payload.
        /// </param>
        /// <remarks>
        /// Listener addition is enqueued for serialized processing.  
        /// This avoids modifying the listeners dictionary while events are being invoked.
        /// </remarks>
        public void AddListener(EventType eventType, Action<object> additionalData)
        {
            actionQueue.Enqueue(() =>
            {
                if (ListenersData.TryGetValue(eventType, out var value))
                {
                    value.ActionsOnInvoke.Add(additionalData);
                }
                else
                {
                    ListenersData[eventType] = new EventListenerData.EventListenersData(additionalData);
                }
            });

            TryProcessQueue();
        }

        /// <summary>
        /// Removes a listener callback for the specified event type.
        /// </summary>
        /// <param name="eventType">The event identifier.</param>
        /// <param name="actionToRemove">The previously registered callback to remove.</param>
        /// <remarks>
        /// Listener removal is queued to maintain thread-safety and avoid modifying listener lists
        /// during invocation loops.
        /// </remarks>
        public void RemoveListener(EventType eventType, Action<object> actionToRemove)
        {
            actionQueue.Enqueue(() =>
            {
                if (ListenersData.TryGetValue(eventType, out var value))
                {
                    value.ActionsOnInvoke.Remove(actionToRemove);

                    // Remove the entry entirely if it has no listeners left.
                    if (!value.ActionsOnInvoke.Any())
                    {
                        ListenersData.Remove(eventType);
                    }
                }
            });

            TryProcessQueue();
        }

        /// <summary>
        /// Invokes an event, calling all registered listeners for the specified event type.
        /// </summary>
        /// <param name="eventType">The event identifier to dispatch.</param>
        /// <param name="dataToInvoke">Optional data payload for all listeners.</param>
        /// <remarks>
        /// Invocation is enqueued to ensure consistency with add/remove operations.
        /// </remarks>
        public void InvokeEvent(EventType eventType, object dataToInvoke)
        {
            actionQueue.Enqueue(() =>
            {
                if (ListenersData.TryGetValue(eventType, out var value))
                {
                    foreach (var method in value.ActionsOnInvoke)
                    {
                        method(dataToInvoke);
                    }
                }
            });

            TryProcessQueue();
        }

        /// <summary>
        /// Attempts to process the queued actions if they are not already being processed.
        /// Ensures serialized, non-reentrant execution of all pending listener additions,
        /// removals, and event invocations.
        /// </summary>
        private void TryProcessQueue()
        {
            if (isProcessingQueue)
                return;

            isProcessingQueue = true;

            while (actionQueue.TryDequeue(out var action))
            {
                action();
            }

            isProcessingQueue = false;
        }
    }
}