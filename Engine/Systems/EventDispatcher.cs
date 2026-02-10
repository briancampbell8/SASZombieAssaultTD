/*
    File:    EventDispatcher.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Simple publish/subscribe event dispatcher for engine-wide messaging.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Simple event dispatcher for engine-wide messaging.
    /// </summary>
    public sealed class EventDispatcher
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            if (handler is null)
                return;

            var type = typeof(T);

            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }

            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler is null)
                return;

            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
            }
        }

        public void Publish<T>(T evt)
        {
            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var list))
            {
                // Snapshot the list to guard against subscribe/unsubscribe during dispatch
                var snapshot = list.ToArray();
                foreach (var handler in snapshot)
                {
                    if (handler is Action<T> action)
                    {
                        action(evt);
                    }
                }
            }
        }
    }
}