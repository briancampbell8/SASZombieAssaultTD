// =====================================================================================================
//  FILE: EventRouting.cs
//  PATH: Engine/GameRoot/EventRouting.cs
//  SUBSYSTEM: Event System
//
//  ROLE:
//      Provides centralized event subscription, routing, and dispatching for all engine subsystems.
//      ECSRuntimeEvents enables loose coupling between systems by allowing them to publish and subscribe
//      to strongly-typed events without direct references.
//
//  RESPONSIBILITIES:
//      - Manage event subscriptions.
//      - Dispatch events to all registered handlers.
//      - Provide cross-system event coordination.
//      - Maintain deterministic event routing behavior.
//      - Support future queued event processing.
//
//  NON-RESPONSIBILITIES:
//      - GameRoot lifecycle orchestration.
//      - Rendering or update logic.
//      - Asset or state management.
//
//  ARCHITECTURAL NOTES:
//      - Extracted from GameRoot to reduce monolithic responsibilities.
//      - Now a standalone subsystem, consistent with Option B architecture.
//      - GameRootMain composes and owns an ECSRuntimeEvents instance.
//      - Future expansion: queued events, priority routing, async dispatch.
//
//  AUTHOR: BDC
//  CREATED: 2026-07-17
//  LAST UPDATED: 2026-07-17
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Centralized event router for managing event subscriptions and dispatching.
    /// </summary>
    public sealed class EventRouting
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

        // -------------------------------------------------------------------------------------------------
        //  SUBSCRIBE
        // -------------------------------------------------------------------------------------------------

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            if (!_subscriptions.ContainsKey(eventType))
                _subscriptions[eventType] = new List<Delegate>();

            _subscriptions[eventType].Add(handler);

            DLogger.Log(LogSubsystems.Events, LogEnums.LogLevel.Debug,
                $"Subscribed handler to event type {eventType.Name}");
        }

        // -------------------------------------------------------------------------------------------------
        //  UNSUBSCRIBE
        // -------------------------------------------------------------------------------------------------

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            if (_subscriptions.ContainsKey(eventType))
            {
                _subscriptions[eventType].Remove(handler);

                DLogger.Log(LogSubsystems.Events, LogEnums.LogLevel.Debug,
                    $"Unsubscribed handler from event type {eventType.Name}");
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  PUBLISH
        // -------------------------------------------------------------------------------------------------

        public void Publish<T>(T eventData) where T : class
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            var eventType = typeof(T);

            try
            {
                if (_subscriptions.ContainsKey(eventType))
                {
                    foreach (var handler in _subscriptions[eventType])
                    {
                        if (handler is Action<T> typedHandler)
                            typedHandler(eventData);
                    }
                }

                DLogger.Log(LogSubsystems.Events, LogEnums.LogLevel.Debug,
                    $"Published event of type {eventType.Name}");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Events, LogEnums.LogLevel.Error,
                    $"Event publish failure for {eventType.Name}: {ex.Message}");
                throw;
            }
        }

        // -------------------------------------------------------------------------------------------------
        //  QUEUED PROCESSING (FUTURE EXPANSION)
        // -------------------------------------------------------------------------------------------------

        public void ProcessQueue()
        {
            // Placeholder for future queued event processing
        }

        // -------------------------------------------------------------------------------------------------
        //  MAINTENANCE
        // -------------------------------------------------------------------------------------------------

        public void ClearSubscriptions()
        {
            _subscriptions.Clear();
            DLogger.Log(LogSubsystems.Events, LogEnums.LogLevel.Debug,
                "All event subscriptions cleared");
        }

        public int GetSubscriptionCount()
        {
            return _subscriptions.Values.Sum(list => list.Count);
        }
    }
}
