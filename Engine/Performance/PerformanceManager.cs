using SASZombieAssaultTD.Engine.Gameplay.Towers;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Towers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// Performance manager for SAS Zombie Assault TD.
    /// Manages object pools, performance monitoring, and budgeting.
    /// </summary>
    public class PerformanceManager
    {
        readonly Dictionary<string, object> _pools = new();
        readonly PerformanceMetrics _metrics = new();
        readonly PerformanceBudget _budget = new();
        bool _isInitialized;

        // Performance targets
        const int TARGET_FPS = 60;
        const int MIN_ACCEPTABLE_FPS = 55, MAX_ENEMIES = 100, MAX_PROJECTILES = 200;

        /// <summary>
        /// Current performance metrics.
        /// </summary>
        public PerformanceMetrics Metrics => _metrics;

        /// <summary>
        /// Performance budget settings.
        /// </summary>
        public PerformanceBudget Budget => _budget;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static PerformanceManager Instance { get; } = new PerformanceManager();

        private PerformanceManager() { }

        /// <summary>
        /// Initialize performance systems and object pools.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Performance Manager");

            try
            {
                // Initialize object pools
                InitializeObjectPools();

                // Initialize performance monitoring
                InitializeMonitoring();

                // Set initial budget
                SetInitialBudget();

                _isInitialized = true;
                Console.WriteLine("Performance Manager initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Performance Manager: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update performance monitoring and adjust pools as needed.
        /// </summary>
        public void Update()
        {
            if (!_isInitialized) return;

            try
            {
                // Update performance metrics
                _metrics.Update();

                // Check if we need to adjust performance
                if (_metrics.AverageFPS < MIN_ACCEPTABLE_FPS)
                {
                    HandlePerformanceIssue();
                }
                else if (_metrics.AverageFPS > TARGET_FPS + 5)
                {
                    OptimizeForPerformance();
                }

                // Update object pools
                UpdateObjectPools();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Performance Manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Get an object from a specific pool.
        /// </summary>
        /// <typeparam name="T">Type of object to get.</typeparam>
        /// <returns>Object from pool or new instance.</returns>
        public T Get<T>() where T : class
        {
            var poolName = typeof(T).Name;

            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                return typedPool.Get();
            }

            // Create pool if it doesn't exist
            var newPool = new ObjectPool<T>(() => Activator.CreateInstance<T>());
            _pools[poolName] = newPool;
            return newPool.Get();
        }

        /// <summary>
        /// Return an object to its pool.
        /// </summary>
        /// <typeparam name="T">Type of object to return.</typeparam>
        /// <param name="item">Object to return to pool.</param>
        public void Return<T>(T item) where T : class, new()
        {
            if (item == null) return;

            var poolName = typeof(T).Name;

            if (_pools.TryGetValue(poolName, out var pool) && pool is ObjectPool<T> typedPool)
            {
                typedPool.Return(item);
            }
        }

        /// <summary>
        /// Get performance statistics for debugging.
        /// </summary>
        /// <returns>Performance statistics string.</returns>
        public string GetStats()
        {
            var stats = $"Performance Stats:\n";
            stats += $"FPS: {_metrics.AverageFPS:F1} (Target: {TARGET_FPS})\n";
            stats += $"Memory: {_metrics.MemoryUsageMB:F1} MB\n";
            stats += $"Enemies: {_metrics.ActiveEnemies}/{MAX_ENEMIES}\n";
            stats += $"Projectiles: {_metrics.ActiveProjectiles}/{MAX_PROJECTILES}\n";
            stats += $"Draw Calls: {_metrics.DrawCalls}\n";
            stats += $"Update Time: {_metrics.AverageUpdateTime:F2}ms\n";
            stats += $"Render Time: {_metrics.AverageRenderTime:F2}ms\n";

            stats += "\nObject Pools:\n";

            foreach (var kvp in _pools)
            {
                if (kvp.Value is ObjectPool<object> pool)
                {
                    stats += $"  {kvp.Key}: {pool.GetStats()}\n";
                }
            }

            return stats;
        }

        /// <summary>
        /// Initialize all object pools for SAS TD.
        /// </summary>
        void InitializeObjectPools()
        {
            Console.WriteLine("Initializing object pools");

            // Create specialized pools for common game objects
            _pools["Projectile"] = new ProjectilePool();
            _pools["Enemy"] = new EnemyPool();
            _pools["Effect"] = new ObjectPool<object>(() => new object());

            // Create generic pools for other objects
            _pools["Tower"] = new ObjectPool<MGLTurret>(() => new MGLTurret());
      //     _pools["Grenade"] = new ObjectPool<Grenade>()) => new Grenade());
        //     _pools["Particle"] = new ObjectPool<Particle>();

            Console.WriteLine($"Initialized {_pools.Count} object pools");
        }

        /// <summary>
        /// Initialize performance monitoring systems.
        /// </summary>
        void InitializeMonitoring() => _metrics.StartMonitoring();

        /// <summary>
        /// Set initial performance budget based on system capabilities.
        /// </summary>
        void SetInitialBudget()
        {
            // Detect system capabilities and set budget accordingly
            var systemMemory = GC.GetTotalMemory(false) / (1024 * 1024); // MB

            if (systemMemory < 2048) // Less than 2GB RAM
            {
                _budget.MaxEnemies = 50;
                _budget.MaxProjectiles = 100;
                _budget.MaxEffects = 30;
            }
            else if (systemMemory < 4096) // Less than 4GB RAM
            {
                _budget.MaxEnemies = 75;
                _budget.MaxProjectiles = 150;
                _budget.MaxEffects = 50;
            }
            else // 4GB+ RAM
            {
                _budget.MaxEnemies = MAX_ENEMIES;
                _budget.MaxProjectiles = MAX_PROJECTILES;
                _budget.MaxEffects = 100;
            }

            Console.WriteLine($"Performance budget set: {_budget}");
        }

        /// <summary>
        /// Handle performance issues by reducing quality or pool sizes.
        /// </summary>
        void HandlePerformanceIssue()
        {
            Console.WriteLine("Performance issue detected, optimizing...");

            // Reduce pool sizes
            foreach (var kvp in _pools)
            {
                if (kvp.Value is ObjectPool<object> pool)
                {
                    pool.MaxSize = System.Math.Max(10, pool.MaxSize / 2);
                }
            }

            // Reduce budget limits
            _budget.MaxEnemies = System.Math.Max(20, _budget.MaxEnemies / 2);
            _budget.MaxProjectiles = System.Math.Max(30, _budget.MaxProjectiles / 2);
            _budget.MaxEffects = System.Math.Max(10, _budget.MaxEffects / 2);

            // Notify other systems to reduce quality
            OnPerformanceDegraded?.Invoke();
        }

        /// <summary>
        /// Optimize for better performance when FPS is high.
        /// </summary>
        void OptimizeForPerformance()
        {
            // Gradually increase pool sizes if performance is good
            foreach (var kvp in _pools)
            {
                if (kvp.Value is ObjectPool<object> pool)
                {
                    pool.MaxSize = System.Math.Min(200, pool.MaxSize + 10);
                }
            }

            // Gradually increase budget limits
            _budget.MaxEnemies = System.Math.Min(MAX_ENEMIES, _budget.MaxEnemies + 5);
            _budget.MaxProjectiles = System.Math.Min(MAX_PROJECTILES, _budget.MaxProjectiles + 10);
            _budget.MaxEffects = System.Math.Min(100, _budget.MaxEffects + 5);
        }

        /// <summary>
        /// Update object pools based on current usage.
        /// </summary>
        void UpdateObjectPools()
        {
            // Update metrics for active objects
            _metrics.ActiveEnemies = GetActiveCount<Enemy>();
            _metrics.ActiveProjectiles = GetActiveCount<Projectile>();
        }

        /// <summary>
        /// Get estimated count of active objects of type T.
        /// </summary>
        int GetActiveCount<T>() where T : class, new()
        {
            // This is a simplified estimate - in a real implementation,
            // you'd track actual active objects
            return 0;
        }

        /// <summary>
        /// Event fired when performance degrades and optimization is needed.
        /// </summary>
        public event Action OnPerformanceDegraded;
    }

    /// <summary>
    /// Performance metrics tracking.
    /// </summary>
    public class PerformanceMetrics
    {
        readonly Queue<float> _fpsSamples = new(60);
        readonly Queue<float> _updateTimeSamples = new(60);
        readonly Queue<float> _renderTimeSamples = new(60);
        Stopwatch _frameTimer = new();
        float _lastFrameTime;

        public float AverageFPS { get; private set; }
        public float MemoryUsageMB { get; private set; }
        public int ActiveEnemies { get; set; }
        public int ActiveProjectiles { get; set; }
        public int DrawCalls { get; set; }
        public float AverageUpdateTime { get; private set; }
        public float AverageRenderTime { get; private set; }

        public void StartMonitoring() => _frameTimer.Start();

        public void Update()
        {
            // Calculate FPS
            var currentTime = (float)_frameTimer.Elapsed.TotalSeconds;
            var deltaTime = currentTime - _lastFrameTime;
            _lastFrameTime = currentTime;

            if (deltaTime > 0)
            {
                var fps = 1.0f / deltaTime;
                _fpsSamples.Enqueue(fps);
                if (_fpsSamples.Count > 60) _fpsSamples.Dequeue();

                AverageFPS = CalculateAverage(_fpsSamples);
            }

            // Update memory usage
            MemoryUsageMB = GC.GetTotalMemory(false) / (1024f * 1024f);
        }

        public void RecordUpdateTime(float time)
        {
            _updateTimeSamples.Enqueue(time);

            if (_updateTimeSamples.Count > 60)
                _updateTimeSamples.Dequeue();

            AverageUpdateTime = CalculateAverage(_updateTimeSamples);
        }

        public void RecordRenderTime(float time)
        {
            _renderTimeSamples.Enqueue(time);

            if (_renderTimeSamples.Count > 60)
                _renderTimeSamples.Dequeue();

            AverageRenderTime = CalculateAverage(_renderTimeSamples);
        }

        float CalculateAverage(Queue<float> samples)
        {
            if (samples.Count == 0) return 0f;

            var sum = 0f;

            foreach (var sample in samples)
                sum += sample;

            return sum / samples.Count;
        }
    }

    /// <summary>
    /// Performance budget settings.
    /// </summary>
    public class PerformanceBudget
    {
        public int MaxEnemies { get; set; } = 100;
        public int MaxProjectiles { get; set; } = 200;
        public int MaxEffects { get; set; } = 100;
        public int MaxDrawCalls { get; set; } = 1000;

        public override string ToString()
        {
            return $"Budget: Enemies={MaxEnemies}, Projectiles={MaxProjectiles}, Effects={MaxEffects}, DrawCalls={MaxDrawCalls}";
        }
    }
}