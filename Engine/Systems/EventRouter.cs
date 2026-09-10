// ====================================================================================================
//  FILE: ECSRuntimeEvents.cs
//  PATH: ./Engine/Systems/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ECSRuntimeEvents module.
//
//  RESPONSIBILITIES:
//      - Provide RegisterEvent() behavior for the Core subsystem.
//      - Provide TriggerEvent() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//File: Engine/Systems/ECSRuntimeEvents.cs
//Purpose: Provides an event routing system for managing and triggering events.
//Features: Supports event registration and triggering with associated data.

using System;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    public class ECSRuntimeEvents
    {
        private readonly Dictionary<string, Action<object>> _eventHandlers = new();

        public void RegisterEvent(string eventName, Action<object> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = handler;
            }
        }

        public void TriggerEvent(string eventName, object eventData)
        {
            if (_eventHandlers.TryGetValue(eventName, out var handler))
            {
                handler(eventData);
            }
        }

        internal void Subscribe<T>(Action<T> onKillAttributed)
        {
            throw new NotImplementedException();
        }

        internal void Unsubscribe<T>(Action<T> onKillAttributed)
        {
            throw new NotImplementedException();
        }

        public static implicit operator ECSRuntimeEvents(EventRouting v)
        {
            throw new NotImplementedException();
        }
    }

    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator System.Numerics.Vector3(Vector3 v)
        {
            throw new NotImplementedException();
        }
    }
}

