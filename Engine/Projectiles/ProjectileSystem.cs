using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;
// audio system using removed; AudioSystem lives in Engine.Systems
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Performance;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    /// <summary>
    /// Projectile system for SAS Zombie Assault TD.
    /// Manages all active projectiles, spawning, and collision detection.
    /// </summary>
    public class ProjectileSystem
    {
        private readonly List<Projectile> _activeProjectiles = new();
        private readonly Dictionary<ProjectileType, ProjectilePool> _pools = new();
        private bool _isInitialized = false;
        private static ProjectileSystem _instance;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static ProjectileSystem Instance => _instance ??= new ProjectileSystem();

        private ProjectileSystem() { }

        /// <summary>
        /// Initialize the projectile system.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            System.Diagnostics.Debug.WriteLine("Initializing Projectile System");

            try
            {
                // Initialize projectile pools
                InitializePools();

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("Projectile System initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize Projectile System: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update all active projectiles.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            try
            {
                // Update all projectiles
                for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
                {
                    var projectile = _activeProjectiles[i];

                    if (!projectile.IsActive)
                    {
                        _activeProjectiles.RemoveAt(i);
                        continue;
                    }

                    projectile.Update(deltaTime);
                }

                // Clean up inactive projectiles
                CleanupInactiveProjectiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating Projectile System: {ex.Message}");
            }
        }

        /// <summary>
        /// Render all active projectiles.
        /// </summary>
        public void Render()
        {
            if (!_isInitialized) return;

            try
            {
                foreach (var projectile in _activeProjectiles)
                {
                    if (projectile.IsActive)
                    {
                        projectile.Render();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering Projectile System: {ex.Message}");
            }
        }

        /// <summary>
        /// Fire a projectile from a tower.
        /// </summary>
        /// <param name="tower">The tower firing the projectile.</param>
        /// <param name="target">The target enemy.</param>
        /// <param name="projectileType">Type of projectile to fire.</param>
        /// <returns>The fired projectile, or null if failed.</returns>
        public Projectile FireProjectile(Tower tower, Enemy target, ProjectileType projectileType)
        {
            if (!_isInitialized || tower == null || target == null)
                return null;

            try
            {
                // Get projectile from pool
                var projectile = GetProjectileFromPool(projectileType);
                if (projectile == null)
                    return null;

                // Calculate firing position and direction
                var firePosition = GetFirePosition(tower);
                var direction = CalculateFireDirection(firePosition, target.Position, projectileType);

                // Set projectile data (use Tower.Data property)
                projectile.SetFromTowerData(tower.Data);
                projectile.Source = tower;
                projectile.Target = target;
                projectile.Type = projectileType;

                // Activate projectile
                projectile.Activate(firePosition, direction, tower, target);

                // Add to active list
                _activeProjectiles.Add(projectile);

                // Notify tower of projectile fired
                tower.OnProjectileFired(projectile);

                return projectile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error firing projectile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Fire a projectile in a direction (for area targeting).
        /// </summary>
        /// <param name="tower">The tower firing the projectile.</param>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="projectileType">Type of projectile to fire.</param>
        /// <returns>The fired projectile, or null if failed.</returns>
        public Projectile FireProjectileAt(Tower tower, Vector3 targetPosition, ProjectileType projectileType)
        {
            if (!_isInitialized || tower == null)
                return null;

            try
            {
                // Get projectile from pool
                var projectile = GetProjectileFromPool(projectileType);
                if (projectile == null)
                    return null;

                // Calculate firing position and direction
                var firePosition = GetFirePosition(tower);
                var direction = CalculateFireDirection(firePosition, targetPosition, projectileType);

                // Set projectile data (use Tower.Data property)
                projectile.SetFromTowerData(tower.Data);
                projectile.Source = tower;
                projectile.Target = null; // No specific target for area fire
                projectile.Type = projectileType;

                // Activate projectile
                projectile.Activate(firePosition, direction, tower);

                // Add to active list
                _activeProjectiles.Add(projectile);

                // Notify tower of projectile fired
                tower.OnProjectileFired(projectile);

                return projectile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error firing projectile at position: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Fire multiple projectiles (for shotgun or spread attacks).
        /// </summary>
        /// <param name="tower">The tower firing projectiles.</param>
        /// <param name="target">The target enemy.</param>
        /// <param name="projectileType">Type of projectile to fire.</param>
        /// <param name="count">Number of projectiles to fire.</param>
        /// <param name="spreadAngle">Spread angle in degrees.</param>
        /// <returns>List of fired projectiles.</returns>
        public List<Projectile> FireProjectileSpread(Tower tower, Enemy target, ProjectileType projectileType, int count, float spreadAngle)
        {
            var projectiles = new List<Projectile>();

            if (!_isInitialized || tower == null || target == null || count <= 0)
                return projectiles;

            try
            {
                var firePosition = GetFirePosition(tower);
                var baseDirection = CalculateFireDirection(firePosition, target.Position, projectileType);
                var spreadRadians = MathF.PI * spreadAngle / 180f;

                for (int i = 0; i < count; i++)
                {
                    // Calculate spread direction
                    var angleOffset = (i - (count - 1) / 2f) * spreadRadians / (count - 1);
                    var spreadDirection = RotateDirection(baseDirection, angleOffset);

                    // Get projectile from pool
                    var projectile = GetProjectileFromPool(projectileType);
                    if (projectile == null)
                        continue;

                    // Set projectile data
                    projectile.SetFromTowerData((TowerData)tower.TowerData());
                    projectile.Source = tower;
                    projectile.Target = target;
                    projectile.Type = projectileType;

                    // Activate projectile
                    projectile.Activate(firePosition, spreadDirection, tower, target);

                    // Add to active list
                    _activeProjectiles.Add(projectile);
                    projectiles.Add(projectile);
                }

                // Notify tower of projectiles fired
                tower.OnProjectilesFired(projectiles);

                return projectiles;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error firing projectile spread: {ex.Message}");
                return projectiles;
            }
        }

        /// <summary>
        /// Get the number of active projectiles.
        /// </summary>
        public int GetActiveProjectileCount()
        {
            return _activeProjectiles.Count;
        }

        /// <summary>
        /// Get projectiles by type.
        /// </summary>
        public List<Projectile> GetProjectilesByType(ProjectileType type)
        {
            var result = new List<Projectile>();
            foreach (var projectile in _activeProjectiles)
            {
                if (projectile.Type == type && projectile.IsActive)
                {
                    result.Add(projectile);
                }
            }
            return result;
        }

        /// <summary>
        /// Clear all active projectiles.
        /// </summary>
        public void ClearAllProjectiles()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.Deactivate();
            }
            _activeProjectiles.Clear();
        }

        /// <summary>
        /// Get system statistics.
        /// </summary>
        public string GetStats()
        {
            var stats = $"Projectile System Stats:\n";
            stats += $"Active Projectiles: {_activeProjectiles.Count}\n";
            stats += $"Total Pools: {_pools.Count}\n";

            foreach (var kvp in _pools)
            {
                stats += $"  {kvp.Key}: {kvp.Value.GetStats()}\n";
            }

            return stats;
        }

        /// <summary>
        /// Initialize projectile pools.
        /// </summary>
        private void InitializePools()
        {
            // Create pools for each projectile type
            _pools[ProjectileType.Bullet] = new ProjectilePool(ProjectileType.Bullet);
            _pools[ProjectileType.Rocket] = new ProjectilePool(ProjectileType.Rocket);
            _pools[ProjectileType.Grenade] = new ProjectilePool(ProjectileType.Grenade);
            _pools[ProjectileType.Laser] = new ProjectilePool(ProjectileType.Laser);
            _pools[ProjectileType.Plasma] = new ProjectilePool(ProjectileType.Plasma);
            _pools[ProjectileType.Arrow] = new ProjectilePool(ProjectileType.Arrow);
            _pools[ProjectileType.Magic] = new ProjectilePool(ProjectileType.Magic);

            System.Diagnostics.Debug.WriteLine($"Initialized {_pools.Count} projectile pools");
        }

        /// <summary>
        /// Get a projectile from the appropriate pool.
        /// </summary>
        private Projectile GetProjectileFromPool(ProjectileType type)
        {
            if (_pools.TryGetValue(type, out var pool))
            {
                return pool.Get();
            }

            // Fallback to generic projectile pool
            return PerformanceManager.Instance?.Get<Projectile>();
        }

        /// <summary>
        /// Get the firing position for a tower.
        /// </summary>
        private Vector3 GetFirePosition(Tower tower)
        {
            // Get tower's fire point (could be barrel position, etc.)
            return (Vector3)(tower.GetFirePosition() ?? Vector3.Zero);
        }

        /// <summary>
        /// Calculate firing direction with prediction for moving targets.
        /// </summary>
        private Vector3 CalculateFireDirection(Vector3 firePosition, Vector3 targetPosition, ProjectileType projectileType)
        {
            var direction = (targetPosition - firePosition).Normalized;

            // Add prediction for moving targets if needed
            // This would require target velocity information
            // For now, use simple direct targeting

            return direction;
        }

        /// <summary>
        /// Rotate a 2D direction by an angle.
        /// </summary>
        private Vector3 RotateDirection(Vector3 direction, float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            return new Vector3(
                direction.X * cos - direction.Y * sin,
                direction.X * sin + direction.Y * cos,
                0
            );
        }

        /// <summary>
        /// Clean up inactive projectiles.
        /// </summary>
        private void CleanupInactiveProjectiles()
        {
            for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
            {
                if (!_activeProjectiles[i].IsActive)
                {
                    _activeProjectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Shutdown the projectile system.
        /// </summary>
        public void Shutdown()
        {
            System.Diagnostics.Debug.WriteLine("Shutting down Projectile System");

            ClearAllProjectiles();

            // Clear pools
            _pools.Clear();

            _isInitialized = false;
            System.Diagnostics.Debug.WriteLine("Projectile System shutdown complete");
        }
    }

    /// <summary>
    /// Specialized projectile pool for different projectile types.
    /// </summary>
    public class ProjectilePool : ObjectPool<Projectile>
    {
        private readonly ProjectileType _projectileType;

        public ProjectilePool(ProjectileType type) : base(
            createFunc: () => CreateProjectile(type),
            resetFunc: projectile => projectile.Reset(),
            activateFunc: projectile => projectile.Activate()
        )
        {
            _projectileType = type;

            // Pre-warm pool based on projectile type
            var prewarmCount = GetPrewarmCount(type);
            Prewarm(prewarmCount);
        }

        /// <summary>
        /// Create a projectile of the specified type.
        /// </summary>
        private static Projectile CreateProjectile(ProjectileType type)
        {
            var projectile = new Projectile();
            projectile.Type = type;

            // Set type-specific properties
            switch (type)
            {
                case ProjectileType.Bullet:
                    projectile.Size = 0.2f;
                    projectile.Color = Color.Yellow;
                    projectile.Speed = 15f;
                    break;

                case ProjectileType.Rocket:
                    projectile.Size = 0.4f;
                    projectile.Color = Color.Red;
                    projectile.Speed = 8f;
                    projectile.HasSplash = true;
                    projectile.SplashRadius = 3f;
                    break;

                case ProjectileType.Grenade:
                    projectile.Size = 0.3f;
                    projectile.Color = Color.Orange;
                    projectile.Speed = 6f;
                    projectile.HasSplash = true;
                    projectile.SplashRadius = 2.5f;
                    break;

                case ProjectileType.Laser:
                    projectile.Size = 0.1f;
                    projectile.Color = Color.Cyan;
                    projectile.Speed = 30f;
                    break;

                case ProjectileType.Plasma:
                    projectile.Size = 0.35f;
                    projectile.Color = Color.Purple;
                    projectile.Speed = 12f;
                    break;

                case ProjectileType.Arrow:
                    projectile.Size = 0.25f;
                    projectile.Color = Color.Brown;
                    projectile.Speed = 18f;
                    break;

                case ProjectileType.Magic:
                    projectile.Size = 0.3f;
                    projectile.Color = Color.Magenta;
                    projectile.Speed = 10f;
                    break;
            }

            // Create trail effect for certain projectile types
            if (type == ProjectileType.Laser || type == ProjectileType.Plasma)
            {
                projectile.Trail = new TrailEffect(projectile.Color, projectile.Size * 0.5f);
            }

            return projectile;
        }

        /// <summary>
        /// Get prewarm count based on projectile type.
        /// </summary>
        private static int GetPrewarmCount(ProjectileType type)
        {
            return type switch
            {
                ProjectileType.Bullet => 100,  // High fire rate
                ProjectileType.Rocket => 20,   // Lower fire rate
                ProjectileType.Grenade => 15,
                ProjectileType.Laser => 50,
                ProjectileType.Plasma => 30,
                ProjectileType.Arrow => 40,
                ProjectileType.Magic => 25,
                _ => 30
            };
        }
    }
}
