// ====================================================================================================
// FILE: Hazard.cs
// PATH: Engine/HazardsControl/Hazard.cs
// MODULE: HazardsControl
//
// ROLE:
//     Core hazard data model. Represents a single hazard instance and its deterministic state.
//     Used directly by lifecycle, occupancy, density, kill attribution, analytics, and visuals.
//
// RESPONSIBILITIES:
//     - Store all hazard properties (idECSEntityCore, type, position, radius, timing, intensity).
//     - Maintain lifecycle transitions (Pending → Active → Decaying → Expired).
//     - Track lifetime, decay progression, activation timing.
//     - Expose analytics fields consumed by HazardAnalytics and HazardsMain.
//
// NON-RESPONSIBILITIES:
//     - Subsystem logic (handled externally).
//     - Managing hazard collections (HazardsMain).
//
// NOTES:
//     Minimal state machine. No subsystem behavior. Pure data model.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    public class Hazard
    {
        // ====================================================================================
        // CORE PROPERTIES
        // ====================================================================================

        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public HazardState State { get; set; } = HazardState.Pending;

        public Vector3 Position { get; set; }
        public float Radius { get; set; }

        public float MaxLifetime { get; set; }
        public float CurrentIntensity { get; set; } = 1f;
        public float InitialIntensity { get; set; } = 1f;

        public bool HasDecay { get; set; } = false;

        public float ActivationDelay { get; set; }
        public float ActivationDuration { get; set; } = 1f;
        public float ActivationProgress { get; set; }
        public bool HasActivated { get; set; } = false;
        public DateTime ActivationTime { get; set; }

        public float Lifetime { get; set; }
        public float DecayDuration { get; set; } = 5f;
        public float DecayProgress { get; set; }
        public float IntensityDecayRate { get; set; }

        public float EffectPeriod { get; set; }
        public float ElapsedTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ====================================================================================
        // ANALYTICS FIELDS
        // ====================================================================================

        public bool IsActive { get; set; } = true;
        public float LifetimeSeconds { get; set; } = 0f;
        public float EffectivenessScore { get; set; } = 0f;

        // ====================================================================================
        // LIFECYCLE METHODS
        // ====================================================================================

        public virtual void Update(float deltaTime)
        {
            Lifetime += deltaTime;
            LifetimeSeconds = Lifetime;

            if (State == HazardState.Pending && Lifetime >= ActivationDelay)
                Activate();

            if (State == HazardState.Active && HasDecay)
            {
                DecayProgress += deltaTime / DecayDuration;
                CurrentIntensity = System.Math.Max(0, InitialIntensity - IntensityDecayRate * Lifetime);

                if (DecayProgress >= 1f)
                    Expire();
            }
        }

        public virtual void Activate()
        {
            State = HazardState.Active;
            HasActivated = true;
            ActivationTime = DateTime.Now;
            IsActive = true;
        }

        public virtual void Expire()
        {
            State = HazardState.Expired;
            CurrentIntensity = 0f;
            IsActive = false;
        }
    }

    public enum HazardState
    {
        Pending,
        Activating,
        Active,
        Decaying,
        Expired,
        Inactive,
        Dead,
        Revived,
        Killed,
        Reviving
    }

    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z = 0f)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Zero => new Vector3(0f, 0f);
        public static Vector3 One => new Vector3(1f, 1f);

        public static Vector3 operator +(Vector3 a, Vector3 b) =>
            new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3 operator -(Vector3 a, Vector3 b) =>
            new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3 operator *(Vector3 a, float scalar) =>
            new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);

        public float LengthSquared => X * X + Y * Y + Z * Z;
        public float Length => (float)System.Math.Sqrt(LengthSquared);
    }

    public struct Rectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Vector3 Center => new Vector3(X + Width / 2f, Y + Height / 2f, 0f);
    }
}
