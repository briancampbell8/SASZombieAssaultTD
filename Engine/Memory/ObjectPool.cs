using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Memory
{
    /// <summary>
    /// Generic object pool for memory management optimization.
    /// Optimized for thread safety, asynchronous operations, and event-driven design.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool.</typeparam>
    public class ObjectPool<T> where T : class, new()
    {
        private readonly ConcurrentQueue<T> _available;
        private readonly ConcurrentDictionary<T, bool> _inUse;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _resetAction;
        private readonly Action<T> _disposeAction;
        private readonly int _maxCapacity;
        private int _totalCreated;
        private int _totalReused;
        private int _peakUsage;

        /// <summary>
        /// Gets the number of available objects.
        /// </summary>
        public int AvailableCount => _available.Count;

        /// <summary>
        /// Gets the number of objects currently in use.
        /// </summary>
        public int InUseCount => _inUse.Count;

        /// <summary>
        /// Gets the maximum capacity of the pool.
        /// </summary>
        public int MaxCapacity => _maxCapacity;

        /// <summary>
        /// Gets the total number of objects created.
        /// </summary>
        public int TotalCreated => _totalCreated;

        /// <summary>
        /// Gets the total number of objects reused.
        /// </summary>
        public int TotalReused => _totalReused;

        /// <summary>
        /// Gets the peak usage count.
        /// </summary>
        public int PeakUsage => _peakUsage;

        /// <summary>
        /// Gets the reuse efficiency (0.0 to 1.0).
        /// </summary>
        public float ReuseEfficiency => _totalCreated > 0 ? (float)_totalReused / _totalCreated : 0f;

        /// <summary>
        /// Event fired when an object is rented.
        /// </summary>
        public event EventHandler<T>? ObjectRented;

        /// <summary>
        /// Event fired when an object is returned.
        /// </summary>
        public event EventHandler<T>? ObjectReturned;

        /// <summary>
        /// Event fired when a new object is created.
        /// </summary>
        public event EventHandler<T>? ObjectCreated;

        /// <summary>
        /// Initializes a new object pool.
        /// </summary>
        /// <param name="maxCapacity">Maximum capacity of the pool.</param>
        /// <param name="createFunc">Factory function for creating new objects.</param>
        /// <param name="resetAction">Action to reset objects when returned.</param>
        /// <param name="disposeAction">Action to dispose objects when cleaned up.</param>
        public ObjectPool(int maxCapacity = 100, Func<T>? createFunc = null, Action<T>? resetAction = null, Action<T>? disposeAction = null)
        {
            _available = new ConcurrentQueue<T>();
            _inUse = new ConcurrentDictionary<T, bool>();
            _maxCapacity = System.Math.Max(1, maxCapacity);
            _createFunc = createFunc ?? (() => new T());
            _resetAction = resetAction;
            _disposeAction = disposeAction;
            ModernLoggingSystem.Log("INFO", $"ObjectPool<{typeof(T).Name}>: Initialized with max capacity {_maxCapacity}");
        }

        /// <summary>
        /// Rents an object from the pool asynchronously.
        /// </summary>
        /// <returns>An object from the pool.</returns>
        public async Task<T> RentAsync()
        {
            return await Task.Run(() => Rent());
        }

        /// <summary>
        /// Rents an object from the pool.
        /// </summary>
        /// <returns>An object from the pool.</returns>
        public T Rent()
        {
            if (_available.TryDequeue(out var obj))
            {
                Interlocked.Increment(ref _totalReused);
            }
            else
            {
                obj = _createFunc();
                Interlocked.Increment(ref _totalCreated);
                ObjectCreated?.Invoke(this, obj);
            }

            _inUse[obj] = true;
            UpdatePeakUsage();
            ObjectRented?.Invoke(this, obj);
            ModernLoggingSystem.Log("TRACE", $"ObjectPool<{typeof(T).Name}>: Rented object (available: {_available.Count}, in use: {_inUse.Count})");

            return obj;
        }

        /// <summary>
        /// Returns an object to the pool asynchronously.
        /// </summary>
        /// <param name="obj">The object to return.</param>
        public async Task ReturnAsync(T obj)
        {
            if (obj == null)
            {
                ModernLoggingSystem.Log("WARNING", $"ObjectPool<{typeof(T).Name}>: Cannot return null object");
                return;
            }

            await Task.Run(() => Return(obj));
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">The object to return.</param>
        public void Return(T obj)
        {
            if (!_inUse.TryRemove(obj, out _))
            {
                ModernLoggingSystem.Log("WARNING", $"ObjectPool<{typeof(T).Name}>: Object not found in use set");
                return;
            }

            _resetAction?.Invoke(obj);

            if (_available.Count < _maxCapacity)
            {
                _available.Enqueue(obj);
            }
            else
            {
                _disposeAction?.Invoke(obj);
            }

            ObjectReturned?.Invoke(this, obj);
            ModernLoggingSystem.Log("TRACE", $"ObjectPool<{typeof(T).Name}>: Returned object (available: {_available.Count}, in use: {_inUse.Count})");
        }

        /// <summary>
        /// Clears the pool and disposes all objects.
        /// </summary>
        public void Clear()
        {
            while (_available.TryDequeue(out var obj))
            {
                _disposeAction?.Invoke(obj);
            }

            foreach (var obj in _inUse.Keys)
            {
                _disposeAction?.Invoke(obj);
            }

            _inUse.Clear();
            ModernLoggingSystem.Log("INFO", $"ObjectPool<{typeof(T).Name}>: Cleared pool");
        }

        /// <summary>
        /// Pre-warms the pool with a specified number of objects.
        /// </summary>
        /// <param name="count">Number of objects to create.</param>
        public void PreWarm(int count)
        {
            count = System.Math.Min(count, _maxCapacity - _available.Count);

            for (int i = 0; i < count; i++)
            {
                var obj = _createFunc();
                Interlocked.Increment(ref _totalCreated);
                _available.Enqueue(obj);
                ObjectCreated?.Invoke(this, obj);
            }

            ModernLoggingSystem.Log("INFO", $"ObjectPool<{typeof(T).Name}>: Pre-warmed with {count} objects");
        }

        /// <summary>
        /// Updates the peak usage count.
        /// </summary>
        private void UpdatePeakUsage()
        {
            var currentUsage = _inUse.Count;
            if (currentUsage > _peakUsage)
            {
                Interlocked.Exchange(ref _peakUsage, currentUsage);
            }
        }

        /// <summary>
        /// Gets pool statistics.
        /// </summary>
        /// <returns>Pool statistics.</returns>
        public PoolStatistics GetStatistics()
        {
            return new PoolStatistics
            {
                AvailableCount = _available.Count,
                InUseCount = _inUse.Count,
                MaxCapacity = _maxCapacity,
                TotalCreated = _totalCreated,
                TotalReused = _totalReused,
                PeakUsage = _peakUsage,
                ReuseEfficiency = ReuseEfficiency
            };
        }

        /// <summary>
        /// Gets pool information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"ObjectPool<{typeof(T).Name}>: Available={_available.Count}, InUse={_inUse.Count}, " +
                   $"Max={_maxCapacity}, Created={_totalCreated}, Reused={_totalReused}, " +
                   $"Efficiency={ReuseEfficiency:P1}";
        }
    }

    /// <summary>
    /// Pool statistics.
    /// </summary>
    public class PoolStatistics
    {
        public int AvailableCount { get; set; }
        public int InUseCount { get; set; }
        public int MaxCapacity { get; set; }
        public int TotalCreated { get; set; }
        public int TotalReused { get; set; }
        public int PeakUsage { get; set; }
        public float ReuseEfficiency { get; set; }

        public override string ToString()
        {
            return $"Pool: Available={AvailableCount}, InUse={InUseCount}, " +
                   $"Created={TotalCreated}, Reused={TotalReused}, " +
                   $"Efficiency={ReuseEfficiency:P1}";
        }
    }
}




