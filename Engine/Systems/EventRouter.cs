// File: Engine/Systems/EventRouter.cs
// Purpose: Provides an event routing system for managing and triggering events.
// Features: Supports event registration and triggering with associated data.

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    public class EventRouter
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
    }
}
