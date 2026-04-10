using SASZombieAssaultTD.Engine.Performance;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Towers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    public sealed class ProjectileSystem
    {
        readonly List<Projectile> activeProjectiles = new();
        readonly Dictionary<ProjectileType, ProjectilePool> pools = new();
        bool isInitialized;
        private IEnumerable<object> _activeProjectiles;
        public ProjectileType Type { get; set; }
        public static ProjectileSystem Instance { get; } = new();
        private ProjectileSystem() { }
        private void ApplyExplosionDamage(Vector2 center, float radius, int damage)
        {
            // TODO: implement damage logic
        }

        private List<Enemy> GetTargetsInRadius(Vector2 center, float radius)
        {
            // TODO: implement radius search logic
            return new List<Enemy>();
        }

        private void SpawnExplosionEffect(Vector2 position)
        {
            // TODO: implement VFX/SFX logic
        }

        private void RemoveProjectile(Projectile projectile)
        {
            // TODO: implement projectile removal logic
        }
        public void Initialize()
        {
            if (isInitialized) return;
            Console.WriteLine("Initializing Projectile System");

            foreach (ProjectileType t in Enum.GetValues(typeof(ProjectileType)))
                pools[t] = new ProjectilePool(t);

            Console.WriteLine($"Initialized {pools.Count} projectile pools");
            isInitialized = true;
            Console.WriteLine("Projectile System initialized successfully");
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutting down Projectile System");
            ClearAllProjectiles();
            pools.Clear();
            isInitialized = false;
            Console.WriteLine("Projectile System shutdown complete");
        }

        public void Update(float deltaTime)
        {
            if (!isInitialized) return;

            try
            {
                for (int i = activeProjectiles.Count - 1; i >= 0; i--)
                {
                    var p = activeProjectiles[i];
                    if (!p.IsActive) { activeProjectiles.RemoveAt(i); continue; }
                    p.Update(deltaTime);

                    if (p.Type == ProjectileType.Grenade && !p.HasExploded)
                        Explode(p);
                    foreach (Projectile projectile in _activeProjectiles)
                    {
                        if (projectile.Type == ProjectileType.Grenade)
                        {
                            if (!projectile.HasExploded)
                            {
                                Explode(projectile);
                                continue;
                            }
                        }

                        // other projectile logic
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Projectile System: {ex.Message}");
            }
        }

        private void Explode(object projectile)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            if (!isInitialized) return;

            try
            {
                foreach (var p in activeProjectiles)
                    if (p.IsActive) p.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering Projectile System: {ex.Message}");
            }
        }

        public Projectile FireProjectile(Tower tower, Enemy target, ProjectileType type) =>
            FireInternal(tower, pos => (target.Position - pos).Normalized, type, target);

        public Projectile FireProjectileAt(Tower tower, Vector3 targetPos, ProjectileType type) =>
            FireInternal(tower, pos => (targetPos - pos).Normalized, type, null);

        public IList<Projectile> FireProjectileSpread(Tower tower, Enemy target, ProjectileType type, int count, float spreadAngle)
        {
            var list = new List<Projectile>();
            if (!isInitialized || tower == null || target == null || count <= 0) return list;

            var origin = GetFirePosition(tower);
            var baseDir = (target.Position - origin).Normalized;
            var total = MathF.PI * spreadAngle / 180f;
            var step = count > 1 ? total / (count - 1) : 0f;

            for (int i = 0; i < count; i++)
            {
                var offset = (i - (count - 1) / 2f) * step;
                var dir = RotateDirection(baseDir, offset);
                var p = GetPooled(type);
                if (p == null) continue;
                Initialize(p, tower, target, type, origin, dir);
                list.Add(p);
            }

            if (list.Count > 0)
                tower.OnProjectilesFired(list);

            return list;
        }

        public int ActiveCount => activeProjectiles.Count;

        public IList<Projectile> GetProjectilesByType(ProjectileType type) =>
            activeProjectiles.FindAll(p => p.Type == type && p.IsActive);

        public void ClearAllProjectiles()
        {
            foreach (var p in activeProjectiles) p.Deactivate();
            activeProjectiles.Clear();
        }

        public string GetStats()
        {
            var sb = new StringBuilder()
                .AppendLine("Projectile System Stats:")
                .AppendLine($"Active Projectiles: {activeProjectiles.Count}")
                .AppendLine($"Total Pools: {pools.Count}");

            foreach (var kv in pools)
                sb.AppendLine($"  {kv.Key}: {kv.Value.GetStats()}");

            return sb.ToString();
        }

        Projectile FireInternal(Tower tower, Func<Vector3, Vector3> dirProvider, ProjectileType type, Enemy target)
        {
            if (!isInitialized || tower == null) return null;
            var p = GetPooled(type);
            if (p == null) return null;
            var origin = GetFirePosition(tower);
            Initialize(p, tower, target, type, origin, dirProvider(origin));
            tower.OnProjectileFired(p);
            return p;
        }

        void Initialize(Projectile p, Tower tower, Enemy target, ProjectileType type, Vector3 origin, Vector3 dir)
        {
            p.SetFromTowerData(tower.Data);
            p.Source = tower;
            p.Target = target;
            p.Type = type;
            p.Activate(origin, dir, tower, target);
            activeProjectiles.Add(p);
        }

        Projectile GetPooled(ProjectileType type) =>
            pools.TryGetValue(type, out var pool) ? pool.Get() : PerformanceManager.Instance?.Get<Projectile>();

        Vector3 GetFirePosition(Tower tower) =>
            tower.GetFirePosition() ?? Vector3.Zero;

        Vector3 RotateDirection(Vector3 dir, float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);
            return new Vector3(dir.X * cos - dir.Y * sin, dir.X * sin + dir.Y * cos, 0f);
        }

        void Explode(Projectile p) => throw new NotImplementedException();

        internal Projectile GetProjectileFromPool(ProjectileType type)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ProjectilePool : ObjectPool<Projectile>
    {
        public ProjectilePool(ProjectileType type)
            : base(
                () => Create(type),
                p => p.Reset(),
                p => p.Activate())
        {
            Prewarm(GetPrewarmCount(type));
        }

        static Projectile Create(ProjectileType type)
        {
            var p = new Projectile { Type = type };

            switch (type)
            {
                case ProjectileType.Bullet:
                    p.Size = 0.2f; p.Color = Color.Yellow; p.Speed = 15f;
                    break;
                case ProjectileType.Rocket:
                    p.Size = 0.4f; p.Color = Color.Red; p.Speed = 8f;
                    p.HasSplash = true; p.SplashRadius = 3f;
                    break;
                case ProjectileType.Grenade:
                    p.Size = 0.3f; p.Color = Color.Orange; p.Speed = 6f;
                    p.HasSplash = true; p.SplashRadius = 2.5f;
                    p.HasGravity = true; p.GravityStrength = 9.8f;
                    p.HasFuse = true; p.FuseTime = 1.5f;
                    break;
                case ProjectileType.Laser:
                    p.Size = 0.1f; p.Color = Color.Cyan; p.Speed = 30f;
                    break;
                case ProjectileType.Plasma:
                    p.Size = 0.35f; p.Color = Color.Purple; p.Speed = 12f;
                    break;
                case ProjectileType.Arrow:
                    p.Size = 0.25f; p.Color = Color.Brown; p.Speed = 18f;
                    break;
                case ProjectileType.Magic:
                    p.Size = 0.3f; p.Color = Color.Magenta; p.Speed = 10f;
                    break;
            }

            if (type == ProjectileType.Laser || type == ProjectileType.Plasma)
                p.Trail = new TrailEffect(p.Color, p.Size * 0.5f);

            return p;
        }

        static int GetPrewarmCount(ProjectileType type) => type switch
        {
            ProjectileType.Bullet => 100,
            ProjectileType.Rocket => 20,
            ProjectileType.Grenade => 15,
            ProjectileType.Laser => 50,
            ProjectileType.Plasma => 30,
            ProjectileType.Arrow => 40,
            ProjectileType.Magic => 25,
            _ => 30
        };
    }
}