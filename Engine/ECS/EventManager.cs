using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    //Minimal event manager to satisfy Subscribe/Unsubscribe/Publish usage across the engine.
    public sealed class EventManager
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var list = _subscribers.GetOrAdd(typeof(T), _ => new List<Delegate>());
            lock (list)
            {
                list.Add(handler);
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (_subscribers.TryGetValue(typeof(T), out var list))
            {
                lock (list)
                {
                    list.Remove(handler);
                }
            }
        }

        public void Publish<T>(T evt)
        {
            if (_subscribers.TryGetValue(typeof(T), out var list))
            {
                Delegate[] copy;
                lock (list)
                {
                    copy = list.ToArray();
                }
                foreach (var d in copy)
                {
                    if (d is Action<T> action)
                    {
                        try { action(evt); } catch { }
                    }
                }
            }
        }
    }
}
