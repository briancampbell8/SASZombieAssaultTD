using System;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Towers;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Enemies;

namespace SASZombieAssaultTD.Engine.Projectiles
{
    /// <summary>
    /// Lightweight projectile implementation used by the projectile system and pools.
    /// This class is intentionally small and contains only the members required by
    /// ProjectileSystem and ProjectilePool. Replace or extend with the game's full
    /// implementation as needed.
    /// </summary>
    public class Projectile
    {
        public ProjectileType Type { get; set; }
        public Vector3 Position { get; set; }
        public Tower Source { get; set; }
        public Enemy Target { get; set; }

        public int Damage { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public bool HasSplash { get; set; }
        public float SplashRadius { get; set; }
        public ElementDamageType Element { get; set; }
        public TrailEffect Trail { get; set; }

        public bool IsActive { get; private set; }
        public float Lifetime { get; private set; }
        public float MaxLifetime { get; set; } = 5f;

        // Gameplay modifiers (kept simple)
        public bool IsHoming { get; set; }
        public float HomingStrength { get; set; }
        public float HomingMaxTurnRate { get; set; }

        public bool IsBouncing { get; set; }
        public int MaxBounces { get; set; }
        public int CurrentBounces { get; set; }
        public float BounceDamping { get; set; }

        public bool IsPiercing { get; set; }
        public int MaxPierces { get; set; }
        public int CurrentPierces { get; set; }
        public float PierceDamageReduction { get; set; }
        public bool HasGravity { get; internal set; } // NEW

        public float FuseTime { get; internal set; } // NEW

        public bool HasFuse { get; internal set; } // NEW
        public float GravityStrength { get; internal set; } // NEW
        public bool HasExploded { get; internal set; }

        public Projectile() { }

        public void Activate()
        {
            IsActive = true;
            Lifetime = 0f;
        }

        public void Activate(Vector3 position, Vector3 direction, Tower source = null, Enemy target = null)
        {
            Position = position;
            Source = source;
            Target = target;
            IsActive = true;
            Lifetime = 0f;
        }

        public void Reset()
        {
            IsActive = false;
            Source = null;
            Target = null;
            Lifetime = 0f;
            CurrentBounces = 0;
            CurrentPierces = 0;
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            Lifetime += deltaTime;
            if (Lifetime > MaxLifetime)
            {
                Deactivate();
            }
        }

        public void Render()
        {
            // Rendering is handled elsewhere. Keep stub for compatibility.
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetFromTowerData(Towers.TowerData data)
        {
            if (data == null) return;
            Damage = (int)data.Damage;
            Speed = data.AttackSpeed;
            Range = data.Range;
        }
    }
}

