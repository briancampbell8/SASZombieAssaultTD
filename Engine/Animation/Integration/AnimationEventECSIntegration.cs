/*
File:    AnimationEventECSIntegration.cs
Purpose: P11-19-12 - ECS integration for animation events.
Provides deterministic event dispatching to ECS systems without modifying ECS.
*/
using SASZombieAssaultTD.Engine.Animation.Events;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Animation.Integration
{
    /// <summary>
    /// P11-19-12: ECS integration for animation events.
    /// Provides deterministic event dispatching to ECS systems without modifying core ECS.
    /// </summary>
    public static class AnimationEventECSIntegration
    {
        private static readonly Dictionary<uint, List<IAnimationEventECSHandler>> _entityHandlers = new();
        private static readonly List<IAnimationEventECSHandler> _globalHandlers = new();
        private static readonly object _ecsLock = new();

        /// <summary>
        /// Registers an ECS event handler for a specific entity.
        /// </summary>
        public static bool RegisterEntityHandler(uint entityId, IAnimationEventECSHandler handler)
        {
            if (entityId == 0 || handler == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Invalid entity ID or handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_entityHandlers.TryGetValue(entityId, out var handlers))
                {
                    handlers = new List<IAnimationEventECSHandler>();
                    _entityHandlers[entityId] = handlers;
                }

                if (handlers.Contains(handler))
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"Handler '{handler.HandlerName}' already registered for entity {entityId}.");
                    return false;
                }

                handlers.Add(handler);
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Registered handler '{handler.HandlerName}' for entity {entityId}.");
                return true;
            }
        }

        /// <summary>
        /// Deregisters an ECS event handler for a specific entity.
        /// </summary>
        public static bool DeregisterEntityHandler(uint entityId, IAnimationEventECSHandler handler)
        {
            if (entityId == 0 || handler == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Invalid entity ID or handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_entityHandlers.TryGetValue(entityId, out var handlers) || !handlers.Remove(handler))
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"Handler '{handler.HandlerName}' not found for entity {entityId}.");
                    return false;
                }

                if (handlers.Count == 0)
                {
                    _entityHandlers.Remove(entityId);
                }

                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Deregistered handler '{handler.HandlerName}' for entity {entityId}.");
                return true;
            }
        }

        /// <summary>
        /// Registers a global ECS event handler.
        /// </summary>
        public static bool RegisterGlobalHandler(IAnimationEventECSHandler handler)
        {
            if (handler == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot register null global handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (_globalHandlers.Contains(handler))
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"Global handler '{handler.HandlerName}' already registered.");
                    return false;
                }

                _globalHandlers.Add(handler);
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Registered global handler '{handler.HandlerName}'.");
                return true;
            }
        }

        /// <summary>
        /// Deregisters a global ECS event handler.
        /// </summary>
        public static bool DeregisterGlobalHandler(IAnimationEventECSHandler handler)
        {
            if (handler == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot deregister null global handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_globalHandlers.Remove(handler))
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("WARNING", $"Global handler '{handler.HandlerName}' not found.");
                    return false;
                }

                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Deregistered global handler '{handler.HandlerName}'.");
                return true;
            }
        }

        /// <summary>
        /// Dispatches an animation event to ECS handlers.
        /// </summary>
        public static int DispatchToECS(uint entityId, AnimationEvent animationEvent, AnimationEventContext context)
        {
            if (animationEvent == null)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot dispatch null animation event.");
                return 0;
            }

            var handlersProcessed = 0;

            lock (_ecsLock)
            {
                if (_entityHandlers.TryGetValue(entityId, out var entityHandlers))
                {
                    handlersProcessed += ProcessHandlers(entityHandlers, entityId, animationEvent, context);
                }

                handlersProcessed += ProcessHandlers(_globalHandlers, entityId, animationEvent, context);
            }

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Dispatched event '{animationEvent.EventName}' to {handlersProcessed} handlers.");
            return handlersProcessed;
        }

        private static int ProcessHandlers(IEnumerable<IAnimationEventECSHandler> handlers, uint entityId, AnimationEvent animationEvent, AnimationEventContext context)
        {
            var processedCount = 0;

            foreach (var handler in handlers)
            {
                try
                {
                    handler.HandleEvent(entityId, animationEvent);
                    processedCount++;
                    Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Handler '{handler.HandlerName}' processed event '{animationEvent.EventName}' for entity {entityId}.");
                }
                catch (Exception ex)
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("ERROR", $"Handler '{handler.HandlerName}' failed to process event '{animationEvent.EventName}': {ex.Message}");
                }
            }

            return processedCount;
        }

        /// <summary>
        /// Clears all handlers for a specific entity.
        /// </summary>
        public static int ClearEntityHandlers(uint entityId)
        {
            if (entityId == 0)
            {
                Engine.Diagnostics.DebugLogger.LogDebug("ERROR", "Cannot clear handlers for entity ID 0.");
                return 0;
            }

            lock (_ecsLock)
            {
                if (!_entityHandlers.Remove(entityId, out var handlers))
                {
                    Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"No handlers to clear for entity {entityId}.");
                    return 0;
                }

                var count = handlers.Count;
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Cleared {count} handlers for entity {entityId}.");
                return count;
            }
        }

        /// <summary>
        /// Clears all global handlers.
        /// </summary>
        public static int ClearGlobalHandlers()
        {
            lock (_ecsLock)
            {
                var count = _globalHandlers.Count;
                _globalHandlers.Clear();
                Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"Cleared {count} global handlers.");
                return count;
            }
        }

        /// <summary>
        /// Gets ECS integration statistics.
        /// </summary>
        public static AnimationEventECSStatistics GetStatistics()
        {
            lock (_ecsLock)
            {
                var entityHandlerCounts = new Dictionary<uint, int>();
                foreach (var kvp in _entityHandlers)
                {
                    entityHandlerCounts[kvp.Key] = kvp.Value.Count;
                }

                return new AnimationEventECSStatistics
                {
                    TotalEntitiesWithHandlers = _entityHandlers.Count,
                    TotalGlobalHandlers = _globalHandlers.Count,
                    EntityHandlerCounts = entityHandlerCounts,
                    GlobalHandlerNames = _globalHandlers.ConvertAll(h => h.HandlerName)
                };
            }
        }
    }
}




