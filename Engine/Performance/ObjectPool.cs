using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Projectiles;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// Generic object pool for performance optimization in SAS Zombie Assault TD.
    /// Reduces garbage collection overhead by reusing objects.
    /// </summary>
    /// <typeparam name="T">The type of object to pool.</typeparam>
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Queue<T> _pool = new();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _resetFunc;
        private readonly Action<T> _activateFunc;
        private int _maxSize = 100;
        private int _currentCount = 0;

        /// <summary>
        /// Number of objects currently in the pool.
        /// </summary>
        public int PoolSize => _pool.Count;

        /// <summary>
        /// Total number of objects created by this pool.
        /// </summary>
        public int TotalCreated => _currentCount;

        /// <summary>
        /// Maximum pool size before objects are discarded.
        /// </summary>
        public int MaxSize
        {
            get => _maxSize;
            set => _maxSize = System.Math.Max(1, value); // Corrected Math namespace
        }

        /// <summary>
        /// Create a new object pool with default factory functions.
        /// </summary>
        public ObjectPool() : this(() => new T(), obj => { }, obj => { })
        {
        }

        /// <summary>
        /// Create a new object pool with custom factory functions.
        /// </summary>
        /// <param name="createFunc">Function to create new objects.</param>
        /// <param name="resetFunc">Function to reset objects when returned to pool.</param>
        /// <param name="activateFunc">Function to activate objects when retrieved from pool.</param>
        public ObjectPool(Func<T> createFunc, Action<T> resetFunc, Action<T> activateFunc)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _resetFunc = resetFunc ?? throw new ArgumentNullException(nameof(resetFunc));
            _activateFunc = activateFunc ?? throw new ArgumentNullException(nameof(activateFunc));
        }

        /// <summary>
        /// Get an object from the pool or create a new one if empty.
        /// </summary>
        /// <returns>An object from the pool or a new instance.</returns>
        public T Get()
        {
            T item;

            if (_pool.Count > 0)
            {
                item = _pool.Dequeue();
            }
            else
            {
                item = _createFunc();
                _currentCount++;
            }

            // Activate the object
            _activateFunc?.Invoke(item);

            return item;
        }

        /// <summary>
        /// Return an object to the pool for reuse.
        /// </summary>
        /// <param name="item">The object to return to the pool.</param>
        public void Return(T item)
        {
            if (item == null)
                return;

            // Don't return to pool if it's at max capacity
            if (_pool.Count >= _maxSize)
                return;

            try
            {
                // Reset the object for reuse
                _resetFunc?.Invoke(item);

                // Return to pool
                _pool.Enqueue(item);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error returning object to pool: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear all objects from the pool.
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }

        /// <summary>
        /// Activates an object with the specified parameters.
        /// Adapts multi-parameter activation calls to the object's activation system.
        /// </summary>
        /// <param name="obj">Object to activate.</param>
        /// <param name="position">Activation position.</param>
        /// <param name="direction">Activation direction.</param>
        /// <param name="source">Source tower.</param>
        /// <param name="target">Target enemy.</param>
        public void Activate(T obj, Vector3 position, Vector3 direction, Tower source, Enemy target)
        {
            if (obj is Projectile projectile)
            {
                projectile.Activate(position, direction, source, target);
            }
            else
            {
                // Fallback to parameterless activation for other object types
                _activateFunc?.Invoke(obj);
            }
        }

        /// <summary>
        /// Pre-warm the pool with a specified number of objects.
        /// </summary>
        /// <param name="count">Number of objects to create and add to pool.</param>
        public void Prewarm(int count)
        {
            for (int i = 0; i < count && _pool.Count < _maxSize; i++)
            {
                var item = _createFunc();
                _resetFunc?.Invoke(item);
                _pool.Enqueue(item);
                _currentCount++;
            }
        }

        /// <summary>
        /// Get pool statistics for debugging.
        /// </summary>
        /// <returns>String containing pool statistics.</returns>
        public string GetStats()
        {
            return $"Pool<{typeof(T).Name}>: Size={_pool.Count}/{_maxSize}, TotalCreated={_currentCount}";
        }
    }

    /// <summary>
    /// Specialized object pool for projectiles in SAS TD.
    /// Integrates with an optional ECS world: when a projectile is activated
    /// an ECS entity is created and a `ProjectileComponent` is added.
    /// </summary>
    public class ProjectilePool : ObjectPool<Projectile>
    {
        // Optional ECS world instance (use your real world object when constructing)
        private readonly dynamic _ecsWorld;

        /// <summary>
        /// Create a projectile pool. If an ECS world is provided, an entity will be created
        /// and components will be added when a projectile is activated.
        /// </summary>
        /// <param name="ecsWorld">Optional ECS world instance (can be null).</param>
        public ProjectilePool(dynamic ecsWorld = null) : base(
            createFunc: () => new Projectile(),
            resetFunc: projectile => projectile.Reset(),
            activateFunc: projectile =>
            {
                // Keep the existing projectile activation behavior if available.
                try
                {
                    // If your Projectile.Activate signature differs, this call may need adjustment.
                    projectile.Activate(Vector3.Zero, Vector3.UnitX, null, null);
                }
                catch
                {
                    // Swallow activation exceptions to avoid breaking pool retrieval when signatures differ.
                }

                // If an ECS world was passed, create an entity and add components.
                if (ecsWorld != null)
                {
                    try
                    {
                        var entity = ecsWorld.CreateEntity();

                        // Add the provided ProjectileComponent (user-supplied struct).
                        // Populate fields with values we safely know exist on the Projectile instance.
                        // Fields not explicitly set will remain at their default values.
                        ecsWorld.AddComponent(entity, new ProjectileComponent
                        {
                            Speed = projectile.Speed,
                            Lifetime = 0f,               // set to a sensible default or map to a projectile property if available
                            Damage = 0f,                 // set to a sensible default or map to a projectile property if available
                            Direction = Vector3.UnitX    // default; replace with actual direction if available (e.g., projectile.Direction)
                        });

                        // If you have concrete `VelocityComponent` and `DamageComponent` types in the project,
                        // uncomment and adapt the lines below to add them. Provide actual field/property names
                        // to map from your `Projectile` instance.

                        // ecsWorld.AddComponent(entity, new VelocityComponent
                        // {
                        //     Velocity = Vector3.UnitX * projectile.Speed
                        // });

                        // ecsWorld.AddComponent(entity, new DamageComponent
                        // {
                        //     Amount = /* map from projectile.Damage or other source */
                        // });

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ECS integration failed when activating projectile: {ex.Message}");
                    }
                }
            }
        )
        {
            _ecsWorld = ecsWorld;
            // Pre-warm with projectiles for better performance
            Prewarm(50);
        }

        private class ProjectileComponent
        {
            public Func<float> Speed { get; set; }
            public float Lifetime { get; set; }
            public float Damage { get; set; }
            public Vector3 Direction { get; set; }
        }
    }

    public class Projectile
    {
        private object TheType;
        private object TheMember;

        internal void Activate(Vector3 position, Vector3 direction, Tower source, Enemy target)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        internal void Reset()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Specialized object pool for enemies in SAS TD.
    /// </summary>
    public class EnemyPool : ObjectPool<Enemy>
    {
        public EnemyPool() : base(
            createFunc: () => new Enemy(),
            resetFunc: enemy => { /* Reset logic not implemented */ }, // Placeholder for Reset logic
            activateFunc: enemy => { /* Activation logic not implemented */ } // Placeholder for Activation logic
        )
        {
            // Pre-warm with enemies for better performance
            Prewarm(30);
        }
    }
}
