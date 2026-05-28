using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Memory
{
    /// <summary>
    /// Memory tracking and leak detection system.
    /// P30-02-08: Add memory usage tracking.
    /// P30-02-09: Add memory leak detection hooks.
    /// </summary>
    public class MemoryTracker
    {
        private readonly ConcurrentDictionary<string, MemoryAllocation> _allocations;
        private readonly ConcurrentDictionary<Type, TypeMemoryInfo> _typeInfo;
        private readonly object _snapshotLock = new object();
        private bool _enabled;
        private long _totalAllocated;
        private long _totalFreed;
        private long _peakMemoryUsage;
        private int _allocationCount;
        private int _freeCount;
        private float _lastGCTime;
        private List<MemorySnapshot> _snapshots;
        private int _maxSnapshots;

        /// <summary>
        /// Gets or sets whether memory tracking is enabled.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Gets the total allocated memory in bytes.
        /// </summary>
        public long TotalAllocated => _totalAllocated;

        /// <summary>
        /// Gets the total freed memory in bytes.
        /// </summary>
        public long TotalFreed => _totalFreed;

        /// <summary>
        /// Gets the current memory usage in bytes.
        /// </summary>
        public long CurrentMemoryUsage => _totalAllocated - _totalFreed;

        /// <summary>
        /// Gets the peak memory usage in bytes.
        /// </summary>
        public long PeakMemoryUsage => _peakMemoryUsage;

        /// <summary>
        /// Gets the number of active allocations.
        /// </summary>
        public int ActiveAllocationCount => _allocationCount - _freeCount;

        /// <summary>
        /// Gets the last GC time in milliseconds.
        /// </summary>
        public float LastGCTime => _lastGCTime;

        /// <summary>
        /// Event fired when memory usage exceeds a threshold.
        /// </summary>
        public event EventHandler<long>? MemoryThresholdExceeded;

        /// <summary>
        /// Event fired when a potential memory leak is detected.
        /// </summary>
        public event EventHandler<MemoryAllocation>? MemoryLeakDetected;

        /// <summary>
        /// Event fired when a memory snapshot is taken.
        /// </summary>
        public event EventHandler<MemorySnapshot>? SnapshotTaken;

        /// <summary>
        /// Initializes a new memory tracker.
        /// </summary>
        public MemoryTracker()
        {
            _allocations = new ConcurrentDictionary<string, MemoryAllocation>();
            _typeInfo = new ConcurrentDictionary<Type, TypeMemoryInfo>();
            _enabled = true;
            _totalAllocated = 0;
            _totalFreed = 0;
            _peakMemoryUsage = 0;
            _allocationCount = 0;
            _freeCount = 0;
            _lastGCTime = 0f;
            _snapshots = new List<MemorySnapshot>();
            _maxSnapshots = 100;

            Engine.Diagnostics.DebugLogger.Log("INFO", "MemoryTracker: Initialized");
        }

        /// <summary>
        /// Tracks a memory allocation.
        /// </summary>
        public void TrackAllocation(string id, long size, Type type, string? stackTrace = null)
        {
            if (!_enabled || string.IsNullOrEmpty(id))
                return;

            var allocation = new MemoryAllocation
            {
                Id = id,
                Size = size,
                Type = type,
                StackTrace = stackTrace ?? Environment.StackTrace,
                Timestamp = DateTime.UtcNow,
                Freed = false
            };

            if (_allocations.TryAdd(id, allocation))
            {
                Interlocked.Add(ref _totalAllocated, size);
                Interlocked.Increment(ref _allocationCount);

                var typeInfo = _typeInfo.GetOrAdd(type, _ => new TypeMemoryInfo { Type = type });

                // Fixes CS0206 on Lines 130 & 131: Use lock for thread-safe property modification
                lock (typeInfo)
                {
                    typeInfo.TotalAllocated += size;
                    typeInfo.AllocationCount++;
                }

                UpdatePeakMemoryUsage();

                Engine.Diagnostics.DebugLogger.Log("TRACE", $"MemoryTracker: Tracked allocation {id} ({size} bytes, {type.Name})");
            }
        }

        /// <summary>
        /// Tracks a memory deallocation.
        /// </summary>
        public void TrackDeallocation(string id)
        {
            if (!_enabled || string.IsNullOrEmpty(id))
                return;

            if (_allocations.TryRemove(id, out var allocation))
            {
                allocation.Freed = true;
                allocation.FreeTimestamp = DateTime.UtcNow;
                allocation.Lifetime = allocation.FreeTimestamp - allocation.Timestamp;

                Interlocked.Add(ref _totalFreed, allocation.Size);
                Interlocked.Increment(ref _freeCount);

                if (_typeInfo.TryGetValue(allocation.Type, out var typeInfo))
                {
                    // Fixes CS0206 on Lines 158 & 159: Use lock for thread-safe property modification
                    lock (typeInfo)
                    {
                        typeInfo.TotalFreed += allocation.Size;
                        typeInfo.FreeCount++;
                    }
                }


                Engine.Diagnostics.DebugLogger.Log("TRACE", $"MemoryTracker: Tracked deallocation {id} ({allocation.Size} bytes)");
            }
            else
            {
                Engine.Diagnostics.DebugLogger.Log("WARNING", $"MemoryTracker: Deallocation tracked for unknown allocation {id}");
            }
        }

        /// <summary>
        /// Takes a memory snapshot asynchronously.
        /// </summary>
        public async Task TakeSnapshotAsync(string? label = null)
        {
            if (!_enabled)
                return;

            await Task.Run(() =>
            {
                lock (_snapshotLock)
                {
                    var snapshot = new MemorySnapshot
                    {
                        Label = label ?? $"Snapshot_{_snapshots.Count + 1}",
                        Timestamp = DateTime.UtcNow,
                        TotalAllocated = _totalAllocated,
                        TotalFreed = _totalFreed,
                        CurrentUsage = CurrentMemoryUsage,
                        PeakUsage = _peakMemoryUsage,
                        ActiveAllocations = ActiveAllocationCount,
                        GCCount = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2),
                        TypeInfo = new Dictionary<Type, TypeMemoryInfo>(_typeInfo)
                    };

                    _snapshots.Add(snapshot);

                    while (_snapshots.Count > _maxSnapshots)
                    {
                        _snapshots.RemoveAt(0);
                    }

                    SnapshotTaken?.Invoke(this, snapshot);
                    Engine.Diagnostics.DebugLogger.Log("INFO", $"MemoryTracker: Took snapshot '{snapshot.Label}' (usage: {snapshot.CurrentUsage:N0} bytes)");
                }
            });
        }

        /// <summary>
        /// Forces garbage collection asynchronously.
        /// </summary>
        public async Task ForceGCAsync()
        {
            if (!_enabled)
                return;

            await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                stopwatch.Stop();

                _lastGCTime = (float)stopwatch.Elapsed.TotalMilliseconds;
                Engine.Diagnostics.DebugLogger.Log("INFO", $"MemoryTracker: Forced GC in {_lastGCTime:F2}ms");
            });
        }

        /// <summary>
        /// Updates the peak memory usage if the current usage exceeds the previous peak.
        /// </summary>
        private void UpdatePeakMemoryUsage()
        {
            var currentUsage = CurrentMemoryUsage;
            if (currentUsage > _peakMemoryUsage)
            {
                Interlocked.Exchange(ref _peakMemoryUsage, currentUsage);
            }
        }

        /// <summary>
        /// Detects potential memory leaks.
        /// </summary>
        /// <param name="maxAgeMinutes">Maximum age in minutes before considering an allocation a leak.</param>
        /// <returns>List of potential memory leaks.</returns>
        public List<MemoryAllocation> DetectMemoryLeaks(int maxAgeMinutes = 10)
        {
            if (!_enabled)
                return new List<MemoryAllocation>();

            lock (_snapshotLock)
            {
                var leaks = new List<MemoryAllocation>();
                var cutoffTime = DateTime.UtcNow.AddMinutes(-maxAgeMinutes);

                foreach (var allocation in _allocations.Values.Where(a => !a.Freed))
                {
                    if (allocation.Timestamp < cutoffTime)
                    {
                        leaks.Add(allocation);
                        MemoryLeakDetected?.Invoke(this, allocation);
                    }
                }

                Engine.Diagnostics.DebugLogger.Log("INFO", $"MemoryTracker: Detected {leaks.Count} potential memory leaks");
                return leaks;
            }
        }

        /// <summary>
        /// Gets memory statistics by type.
        /// </summary>
        /// <returns>Dictionary of type memory information.</returns>
        public Dictionary<Type, TypeMemoryInfo> GetTypeStatistics()
        {
            lock (_snapshotLock)
            {
                return _typeInfo.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        /// <summary>
        /// Gets all memory snapshots.
        /// </summary>
        /// <returns>List of memory snapshots.</returns>
        public List<MemorySnapshot> GetSnapshots()
        {
            lock (_snapshotLock)
            {
                return new List<MemorySnapshot>(_snapshots);
            }
        }

        /// <summary>
        /// Resets all tracking data.
        /// </summary>
        public void Reset()
        {
            lock (_snapshotLock)
            {
                _allocations.Clear();
                _typeInfo.Clear();
                _snapshots.Clear();
                _totalAllocated = 0;
                _totalFreed = 0;
                _peakMemoryUsage = 0;
                _allocationCount = 0;
                _freeCount = 0;
                _lastGCTime = 0f;

                Engine.Diagnostics.DebugLogger.Log("INFO", "MemoryTracker: Reset all tracking data");
            }
        }

        /// <summary>
        /// Gets memory usage summary.
        /// </summary>
        /// <returns>Memory usage summary as a string.</returns>
        public string GetMemorySummary()
        {
            lock (_snapshotLock)
            {
                var summary = new List<string>
                {
                    "Memory Usage Summary",
                    $"Current Usage: {CurrentMemoryUsage:N0} bytes ({CurrentMemoryUsage / 1024.0 / 1024.0:F2} MB)",
                    $"Peak Usage: {_peakMemoryUsage:N0} bytes ({_peakMemoryUsage / 1024.0 / 1024.0:F2} MB)",
                    $"Total Allocated: {_totalAllocated:N0} bytes",
                    $"Total Freed: {_totalFreed:N0} bytes",
                    $"Active Allocations: {ActiveAllocationCount}",
                    $"Last GC Time: {_lastGCTime:F2}ms",
                    ""
                };

                // Add top 5 types by memory usage
                var topTypes = _typeInfo.Values
                .OrderByDescending(t => t.CurrentUsage)
                .Take(5);

                summary.Add("Top 5 Types by Memory Usage:");
                foreach (var typeInfo in topTypes)
                {
                    summary.Add($"  {typeInfo.Type.Name}: {typeInfo.CurrentUsage:N0} bytes ({typeInfo.AllocationCount} allocations)");
                }

                return string.Join(Environment.NewLine, summary);
            }
        }

        /// <summary>
        /// Gets memory tracker information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"MemoryTracker: Enabled={_enabled}, Usage={CurrentMemoryUsage:N0}, " +
            $"Peak={_peakMemoryUsage:N0}, Allocations={ActiveAllocationCount}, " +
            $"Types={_typeInfo.Count}";
        }
    }

    /// <summary>
    /// Memory allocation information.
    /// </summary>
    public class MemoryAllocation
    {
        public string Id { get; set; }
        public long Size { get; set; }
        public Type Type { get; set; }
        public string StackTrace { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FreeTimestamp { get; set; }
        public TimeSpan Lifetime { get; set; }
        public bool Freed { get; set; }

        public override string ToString()
        {
            return $"Allocation: {Id} ({Size} bytes, {Type.Name}, {(Freed ? "Freed" : "Active")})";
        }
    }

    /// <summary>
    /// Type-specific memory information.
    /// </summary>
    public class TypeMemoryInfo
    {
        public Type Type { get; set; }
        public long TotalAllocated { get; set; }
        public long TotalFreed { get; set; }
        public int AllocationCount { get; set; }
        public int FreeCount { get; set; }

        public long CurrentUsage => TotalAllocated - TotalFreed;
        public int ActiveCount => AllocationCount - FreeCount;

        public override string ToString()
        {
            return $"TypeMemoryInfo: {Type.Name} - Usage: {CurrentUsage:N0}, Active: {ActiveCount}";
        }
    }

    /// <summary>
    /// Memory snapshot.
    /// </summary>
    public class MemorySnapshot
    {
        public string Label { get; set; }
        public DateTime Timestamp { get; set; }
        public long TotalAllocated { get; set; }
        public long TotalFreed { get; set; }
        public long CurrentUsage { get; set; }
        public long PeakUsage { get; set; }
        public int ActiveAllocations { get; set; }
        public int GCCount { get; set; }
        public Dictionary<Type, TypeMemoryInfo> TypeInfo { get; set; }

        public override string ToString()
        {
            return $"MemorySnapshot: {Label} - Usage: {CurrentUsage:N0}, Active: {ActiveAllocations}";
        }
    }
}







