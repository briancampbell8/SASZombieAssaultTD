// ====================================================================================================
//  FILE: AnimationEventECSIntegration.cs
//  PATH: ./Engine/Animation/Integration/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationEventECSIntegration module.
//
//  RESPONSIBILITIES:
//      - Provide RegisterEntityHandler() behavior for the Core subsystem.
//      - Provide DeregisterEntityHandler() behavior for the Core subsystem.
//      - Provide RegisterGlobalHandler() behavior for the Core subsystem.
//      - Provide DeregisterGlobalHandler() behavior for the Core subsystem.
//      - Provide DispatchToECS() behavior for the Core subsystem.
//      - Provide ClearEntityHandlers() behavior for the Core subsystem.
//      - Provide ClearGlobalHandlers() behavior for the Core subsystem.
//      - Provide GetStatistics() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    AnimationEventECSIntegration.cs
Purpose: P11-19-12 - ECS integration for animation events.
Provides deterministic event dispatching to ECS systems without modifying ECS.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation.Integration

//
{
    /// <summary>
    /// P11-19-12: ECS integration for animation events. Provides deterministic event dispatching to ECS systems without
    /// modifying core ECS.
    /// </summary>
    public static class AnimationEventECSIntegration
    {
        private static readonly Dictionary<uint, List<IAnimationEventECSHandler>> _ECSEntityCoreHandlers = new();
        private static readonly List<IAnimationEventECSHandler> _globalHandlers = new();
        private static readonly object _ecsLock = new();

        /// <summary>
        /// Registers an ECS event handler for a specific ECSEntityCore.
        /// </summary>
        public static bool RegisterEntityHandler(uint ECSEntityCoreId, IAnimationEventECSHandler handler)
        {
            if (ECSEntityCoreId == 0 || handler == null)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Invalid ECSEntityCore ID or handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_ECSEntityCoreHandlers.TryGetValue(ECSEntityCoreId, out var handlers))
                {
                    handlers = new List<IAnimationEventECSHandler>();
                    _ECSEntityCoreHandlers[ECSEntityCoreId] = handlers;
                }

                if (handlers.Contains(handler))
                {
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning, $"Handler '{handler.HandlerName}' already registered for ECSEntityCore {ECSEntityCoreId}.");
                    return false;
                }

                handlers.Add(handler);
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Registered handler '{handler.HandlerName}' for ECSEntityCore {ECSEntityCoreId}.");
                return true;
            }
        }

        /// <summary>
        /// Deregisters an ECS event handler for a specific ECSEntityCore.
        /// </summary>
        public static bool DeregisterEntityHandler(uint ECSEntityCoreId, IAnimationEventECSHandler handler)
        {
            if (ECSEntityCoreId == 0 || handler == null)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Invalid ECSEntityCore ID or handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_ECSEntityCoreHandlers.TryGetValue(ECSEntityCoreId, out var handlers) || !handlers.Remove(handler))
                {
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning, $"Handler '{handler.HandlerName}' not found for ECSEntityCore {ECSEntityCoreId}.");
                    return false;
                }

                if (handlers.Count == 0)
                {
                    _ECSEntityCoreHandlers.Remove(ECSEntityCoreId);
                }

                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Deregistered handler '{handler.HandlerName}' for ECSEntityCore {ECSEntityCoreId}.");
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
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Cannot register null global handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (_globalHandlers.Contains(handler))
                {
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Warning, $"Global handler '{handler.HandlerName}' already registered.");
                    return false;
                }

                _globalHandlers.Add(handler);
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Registered global handler '{handler.HandlerName}'.");
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
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Cannot deregister null global handler.");
                return false;
            }

            lock (_ecsLock)
            {
                if (!_globalHandlers.Remove(handler))
                {
                    DLogger.Log(
                        LogSubsystems.Animation,
                        LogEnums.LogLevel.Warning,
                        $"Global handler '{handler.HandlerName}' not found.");
                    return false;
                }

                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Deregistered global handler '{handler.HandlerName}'.");
                return true;
            }
        }

        /// <summary>
        /// Dispatches an animation event to ECS handlers.
        /// </summary>
        public static int DispatchToECS(uint ECSEntityCoreId, AnimationEvent animationEvent, AnimationEventContext context)
        {
            if (animationEvent == null)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Cannot dispatch null animation event.");
                return 0;
            }

            var handlersProcessed = 0;

            lock (_ecsLock)
            {
                if (_ECSEntityCoreHandlers.TryGetValue(ECSEntityCoreId, out var ECSEntityCoreHandlers))
                {
                    handlersProcessed += ProcessHandlers(ECSEntityCoreHandlers, ECSEntityCoreId, animationEvent, context);
                }

                handlersProcessed += ProcessHandlers(_globalHandlers, ECSEntityCoreId, animationEvent, context);
            }

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Dispatched event '{animationEvent.EventName}' to {handlersProcessed} handlers.");
            return handlersProcessed;
        }

        private static int ProcessHandlers(IEnumerable<IAnimationEventECSHandler> handlers, uint ECSEntityCoreId, AnimationEvent animationEvent, AnimationEventContext context)
        {
            var processedCount = 0;

            foreach (var handler in handlers)
            {
                try
                {
                    handler.HandleEvent(ECSEntityCoreId,
                        animationEvent,
                        context);

                    DLogger.Log(
                        LogSubsystems.Animation,
                        LogEnums.LogLevel.Debug,
                        $"Handler '{handler.HandlerName}' processed event" +
                        $" '{animationEvent.EventName}' for ECSEntityCore {ECSEntityCoreId}.");
                }
                catch (Exception ex)
                {
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, $"Handler '{handler.HandlerName}' failed to process event '{animationEvent.EventName}': {ex.Message}");
                }
            }

            return processedCount;
        }

        /// <summary>
        /// Clears all handlers for a specific ECSEntityCore.
        /// </summary>
        public static int ClearEntityHandlers(uint ECSEntityCoreId)
        {
            if (ECSEntityCoreId == 0)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error, "Cannot clear handlers for ECSEntityCore ID 0.");
                return 0;
            }

            lock (_ecsLock)
            {
                if (!_ECSEntityCoreHandlers.Remove(ECSEntityCoreId, out var handlers))
                {
                    DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"No handlers to clear for ECSEntityCore {ECSEntityCoreId}.");
                    return 0;
                }

                var count = handlers.Count;
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Cleared {count} handlers for ECSEntityCore {ECSEntityCoreId}.");
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
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"Cleared {count} global handlers.");
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
                var ECSEntityCoreHandlerCounts = new Dictionary<uint, int>();
                foreach (var kvp in _ECSEntityCoreHandlers)
                {
                    ECSEntityCoreHandlerCounts[kvp.Key] = kvp.Value.Count;
                }

                return new AnimationEventECSStatistics
                {
                    TotalEntitiesWithHandlers = _ECSEntityCoreHandlers.Count,
                    TotalGlobalHandlers = _globalHandlers.Count,
                    EntityHandlerCounts = ECSEntityCoreHandlerCounts,
                    GlobalHandlerNames = _globalHandlers.ConvertAll(h => h.HandlerName)
                };
            }
        }
    }
}
