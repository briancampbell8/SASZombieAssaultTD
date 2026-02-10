using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Manages active projectiles and updates their movement.
    /// </summary>
    public sealed class ProjectileSystem
    {
        private readonly List<Projectile> _projectiles = new();
        private bool _initialized;

        /// <summary>
        /// Initializes the projectile system. Called once during engine startup.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _projectiles.Clear();
            _initialized = true;

            DebugLogger.Log(DebugLogger.Phase5,
                "[ProjectileSystem] Initialized.");
        }

        public void Spawn(Projectile projectile)
        {
            if (!_initialized)
                return;

            if (projectile is null)
                throw new ArgumentNullException(nameof(projectile));

            _projectiles.Add(projectile);
        }

        public void Update(float deltaSeconds)
        {
            if (!_initialized)
                return;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                var p = _projectiles[i];
                p.Update(deltaSeconds);

                if (p.IsExpired)
                {
                    _projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Tears down the projectile system. Called during engine shutdown.
        /// </summary>
        public void Shutdown()
        {
            _projectiles.Clear();
            _initialized = false;

            DebugLogger.Log(DebugLogger.Phase5,
                "[ProjectileSystem] Shutdown.");
        }
    }

    public sealed class Projectile
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float VelocityX { get; }
        public float VelocityY { get; }
        public float Lifetime { get; private set; }
        public bool IsExpired => Lifetime <= 0f;

        public Projectile(float x, float y, float vx, float vy, float lifetime)
        {
            X = x;
            Y = y;
            VelocityX = vx;
            VelocityY = vy;
            Lifetime = lifetime;
        }

        public void Update(float deltaSeconds)
        {
            X += VelocityX * deltaSeconds;
            Y += VelocityY * deltaSeconds;
            Lifetime -= deltaSeconds;
        }
    }
}