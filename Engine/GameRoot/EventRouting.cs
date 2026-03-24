/*
File:    EventRouting.cs
Path:    Engine/GameRoot/EventRouting.cs
Purpose: P11-09-01 - Handles all event-based communication between subsystems.
         Ensures events are subscribed, routed, and dispatched correctly.

Role:     Event communication specialist.
         - Event subscription functions
         - Event dispatch functions
         - Event routing logic
         - Cross-system event coordination
         - Event queue management

Notes:    Contains all event routing logic extracted from GameRoot.
         Works with the engine's event system for loose coupling.
         Event routing is centralized for better debugging and monitoring.
*/

using System;
using SASZombieAssaultTD.Engine.Core;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Simple event router for managing event subscriptions and dispatching.
    /// </summary>
    public class EventRouter
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var eventType = typeof(T);
            if (!_subscriptions.ContainsKey(eventType))
                _subscriptions[eventType] = new List<Delegate>();

            _subscriptions[eventType].Add(handler);
        }

        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            var eventType = typeof(T);
            if (_subscriptions.ContainsKey(eventType))
                _subscriptions[eventType].Remove(handler);
        }

        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="eventData">The event data.</param>
        public void Publish<T>(T eventData) where T : class
        {
            var eventType = typeof(T);
            if (_subscriptions.ContainsKey(eventType))
            {
                foreach (var handler in _subscriptions[eventType])
                {
                    if (handler is Action<T> typedHandler)
                        typedHandler(eventData);
                }
            }
        }

        /// <summary>
        /// Processes event queue (placeholder).
        /// </summary>
        public void ProcessQueue()
        {
            // Event queue processing logic would go here
        }

        /// <summary>
        /// Clears all subscriptions.
        /// </summary>
        public void ClearSubscriptions()
        {
            _subscriptions.Clear();
        }

        /// <summary>
        /// Gets subscription count.
        /// </summary>
        /// <returns>The number of active subscriptions.</returns>
        public int GetSubscriptionCount()
        {
            return _subscriptions.Values.Sum(list => list.Count);
        }
    }

    /// <summary>
    /// Partial class containing event routing logic for GameRoot.
    /// </summary>
    public partial class GameRoot
    {
        private readonly EventRouter _eventRouter = new();

        /// <summary>
        /// Gets the event router for event operations.
        /// </summary>
        public EventRouter EventRouter => _eventRouter;

        /// <summary>
        /// Subscribes to an event with the specified handler.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void SubscribeToEvent<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventRouter.Subscribe(handler);
            ModernLoggingSystem.LogInfo($"Subscribed to event of type {typeof(T).Name}");
        }

        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void UnsubscribeFromEvent<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventRouter.Unsubscribe(handler);
            ModernLoggingSystem.LogInfo($"Unsubscribed from event of type {typeof(T).Name}");
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="eventData">The event data.</param>
        public void PublishEvent<T>(T eventData) where T : class
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            try
            {
                _eventRouter.Publish(eventData);
                ModernLoggingSystem.LogDebug($"Published event of type {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Failed to publish event of type {typeof(T).Name}: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Event publish");
            }
        }

        /// <summary>
        /// Processes all pending events in the event queue.
        /// </summary>
        public void ProcessEventQueue()
        {
            try
            {
                _eventRouter.ProcessQueue();
                ModernLoggingSystem.LogDebug("Event queue processed successfully");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Failed to process event queue: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Event queue processing");
            }
        }

        /// <summary>
        /// Clears all event subscriptions.
        /// </summary>
        public void ClearEventSubscriptions()
        {
            _eventRouter.ClearSubscriptions();
            ModernLoggingSystem.LogInfo("All event subscriptions cleared");
        }

        /// <summary>
        /// Gets the number of active event subscriptions.
        /// </summary>
        /// <returns>The number of active subscriptions.</returns>
        public int GetEventSubscriptionCount()
        {
            return _eventRouter.GetSubscriptionCount();
        }
    }
}
