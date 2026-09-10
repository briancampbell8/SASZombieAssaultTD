// =====================================================================================================
//  FILE: AnimationECSIntegration.cs
//  PATH: Engine/Animation/Integration/AnimationECSIntegration.cs
//  SUBSYSTEM: Animation Integration Module
//
//  ROLE:
//      Bridges animation systems and animation events into the ECSRuntime subsystem.
//      Registers animation systems and subscribes to animation-related ECS events.
//      Provides deterministic integration behavior for animation-triggered ECS operations.
//
//  RESPONSIBILITIES:
//      - Register animation systems into ECSRuntimeSys.
//      - Subscribe to GameEventData events using ECSRuntimeEventss.
//      - Provide deterministic event dispatch into animation systems.
//      - Provide clean unregister behavior.
//
//  NON-RESPONSIBILITIES:
//      - Executing animation logic directly.
//      - Managing ECS ECSEntityCore lifecycle or component storage.
//      - Performing spatial or collision queries.
//      - Managing update-loop sequencing.
//
//  ARCHITECTURAL NOTES:
//      - Updated to match the new ECSRuntimeCore subsystem architecture.
//      - Replaces legacy AddSystem / Subscribe / Unsubscribe calls.
//      - Uses ECSRuntimeSys and ECSRuntimeEventss instead of monolithic ECSRuntimeCore methods.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Concurrent;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine;
using SASZombieAssaultTD.Engine.Animation.Systems;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.ECS.ESCSystem;
using SASZombieAssaultTD.Engine.Events;

public static class AnimationECSIntegration
{
    private static readonly HashSet<Type> _registeredSystems = new();
    private static readonly Dictionary<string, Action<GameEventData>> _eventHandlers = new();
    private static readonly ConcurrentDictionary<Type, Action<IECSRuntimeCore>> _systemHandlers = new();
    private static EventRouting _routing;





    // -------------------------------------------------------------------------------------------------
    // Register
    // -------------------------------------------------------------------------------------------------
    public static void Register(IECSRuntimeCore world)
    {
        if (world == null)
            throw new ArgumentNullException(nameof(world), "ECSRuntimeCore cannot be null.");

        // FIX: Cast the interface to the concrete class that contains the subsystems
        if (world is not ECSRuntimeCore runtimeCore)
            throw new InvalidCastException("Provided world runtime does not match the concrete ECSRuntimeCore.");

        // Register animation systems
        var systems = new[]
        {
            typeof(AnimationTriggerSystem),
            typeof(AnimationUpdateSystem)
        };

        foreach (var systemType in systems)
        {
            if (_registeredSystems.Contains(systemType))
                continue;

            _registeredSystems.Add(systemType);

            _systemHandlers.TryAdd(systemType, runtime =>
            {
                var instance = (ISystemCore)Activator.CreateInstance(systemType);

                // FIX: Access the Systems manager via the concrete instance
                runtimeCore.Systems.Add(instance);
            });
        }

        // Register event handlers
        if (!_eventHandlers.ContainsKey(nameof(GameEventData)))
            _eventHandlers[nameof(GameEventData)] = OnGameEventReceived;

        // FIX: Access the Events manager via the concrete instance
        _routing.Subscribe<GameEventData>(OnGameEventReceived);

        DLogger.Log(LogSubsystems.ResourcesPipeline, "AnimationECSIntegration", 1, "Register", "Animation systems and event handlers registered.");
    }

    // -------------------------------------------------------------------------------------------------
    // Unregister
    // -------------------------------------------------------------------------------------------------
    public static void Unregister(IECSRuntimeCore world)
    {
        if (world == null)
            throw new ArgumentNullException(nameof(world), "ECSRuntimeCore cannot be null.");

        // FIX: Cast the interface to the concrete class
        if (world is ECSRuntimeCore runtimeCore)
        {
            // Remove system handlers
            foreach (var systemType in _registeredSystems)
                _systemHandlers.TryRemove(systemType, out _);

            // FIX: Access the Events manager via the concrete instance
            _routing.Unsubscribe<GameEventData>(OnGameEventReceived);
        }

        _registeredSystems.Clear();
        _eventHandlers.Clear();

        DLogger.Log(LogSubsystems.ResourcesPipeline, "AnimationECSIntegration", 2, "Unregister", "Animation systems and event handlers unregistered.");
    }


    // -------------------------------------------------------------------------------------------------
    // Event Dispatch
    // -------------------------------------------------------------------------------------------------
    private static void OnGameEventReceived(GameEventData gameEvent)
    {
        if (gameEvent == null)
            throw new ArgumentNullException(nameof(gameEvent), "GameEvent cannot be null.");

        DLogger.Log(LogSubsystems.ResourcesPipeline, "AnimationECSIntegration", 3, "Event",
            $"GameEvent received → {gameEvent.EventName}");
    }
}
