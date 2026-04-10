/*
File:    RenderResourcePool.cs
Purpose: Resource pool for efficient memory management in SAS Zombie Assault TD.
Features: Object pooling, memory allocation optimization, and garbage collection reduction.
Standards: XML documentation with detailed method descriptions and usage examples.
Integration: Core UI rendering system for efficient memory management.
Performance: Optimized for minimal garbage collection impact and efficient allocation.
*/

using System.Collections.Generic;
using System;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Resource pool for efficient memory management and allocation.
    /// Pools frequently allocated objects like vertices, indices, and render commands.
    /// Reduces garbage collection impact and improves rendering performance.
    /// </summary>
    /// <remarks>
    /// The RenderResourcePool manages object pooling to reduce the impact of
    /// garbage collection on rendering performance. It pools frequently allocated
    /// objects and provides efficient allocation and deallocation mechanisms.
    /// 
    /// Resource Pooling Features:
    /// - Automatic object pooling and recycling
    /// - Efficient allocation and deallocation
    /// - Memory usage optimization
    /// - Garbage collection impact reduction
    /// 
    /// Performance Benefits:
    /// - Reduced garbage collection overhead
    /// - Improved memory allocation efficiency
    /// - Better rendering performance stability
    /// - Lower memory fragmentation
    /// </remarks>
    public class RenderResourcePool : IDisposable
    {
        private readonly Dictionary<System.Type, Queue<object>> _pools = new();

        /// <summary>
        /// Gets an object from the pool or creates a new one.
        /// </summary>
        /// <typeparam name="T">Type of object to get</typeparam>
        /// <returns>Pooled or new object</returns>
        public T Get<T>() where T : class, new()
        {
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var pool))
            {
                _pools[type] = pool = new Queue<object>();
            }

            if (pool.Count > 0)
            {
                return pool.Dequeue() as T;
            }

            return new T();
        }

        /// <summary>
        /// Returns an object to the pool for reuse.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="obj">Object to return to pool</param>
        public void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var pool))
            {
                _pools[type] = pool = new Queue<object>();
            }

            pool.Enqueue(obj);
        }

        /// <summary>
        /// Disposes the resource pool and clears all pools.
        /// </summary>
        public void Dispose()
        {
            ReleaseAllResources();
        }

        private void ReleaseAllResources()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
        }
    }
}
