/*
File:    Hazard.cs
Path:    Engine/HazardsControl/Hazard.cs
Purpose:  Base hazard class for HazardsControl subsystem.
          Defines core hazard properties and state.

Role:     Base class for all hazard types.
          - Provides common hazard interface
          - Defines hazard state enumeration
          - Supports hazard lifecycle
          - Enables hazard categorization

Notes:    This is the base class that all hazards inherit from.
          All hazard subsystems work with this base class.
          Single responsibility: core hazard definition.
*/

using System;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Base class for all hazard types.
    /// Defines core properties and state for hazards.
    /// </summary>
    public class Hazard
    {
        #region Properties

        /// <summary>Unique identifier for the hazard.</summary>
        public int Id { get; set; }

        /// <summary>Type of hazard (e.g., nuke, radiation, fire, chemical).</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Current state of the hazard.</summary>
        public HazardState State { get; set; } = HazardState.Pending;

        /// <summary>Position of the hazard in world space.</summary>
        public Vector3 Position { get; set; }

        /// <summary>Radius of the effect area.</summary>
        public float Radius { get; set; }

        /// <summary>Maximum lifetime in seconds.</summary>
        public float MaxLifetime { get; set; }

        /// <summary>Current intensity (0-1).</summary>
        public float CurrentIntensity { get; set; } = 1f;

        /// <summary>Initial intensity value.</summary>
        public float InitialIntensity { get; set; } = 1f;

        /// <summary>Indicates whether the hazard decays over time.</summary>
        public bool HasDecay { get; set; } = false;

        /// <summary>Time until activation.</summary>
        public float ActivationDelay { get; set; }

        /// <summary>Duration of the activation animation.</summary>
        public float ActivationDuration { get; set; } = 1f;

        /// <summary>Progress of activation (0-1).</summary>
        public float ActivationProgress { get; set; }

        /// <summary>Indicates whether the hazard has been activated.</summary>
        public bool HasActivated { get; set; } = false;

        /// <summary>Time when the hazard was activated.</summary>
        public DateTime ActivationTime { get; set; }

        /// <summary>Current elapsed lifetime.</summary>
        public float Lifetime { get; set; }

        /// <summary>Duration of the decay process.</summary>
        public float DecayDuration { get; set; } = 5f;

        /// <summary>Progress of decay (0-1).</summary>
        public float DecayProgress { get; set; }

        /// <summary>Rate of intensity decay per second.</summary>
        public float IntensityDecayRate { get; set; }

        /// <summary>Period for effects (0 = no periodic effects).</summary>
        public float EffectPeriod { get; set; }

        /// <summary>Elapsed time since the last effect.</summary>
        public float ElapsedTime { get; set; }

        /// <summary>Time when the hazard was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        #endregion

        #region Methods

        /// <summary>
        /// Updates the hazard's state based on elapsed time.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update.</param>
        public virtual void Update(float deltaTime)
        {
            Lifetime += deltaTime;

            if (State == HazardState.Pending && Lifetime >= ActivationDelay)
            {
                Activate();
            }

            if (State == HazardState.Active && HasDecay)
            {
                DecayProgress += deltaTime / DecayDuration;
                CurrentIntensity = System.Math.Max(0, InitialIntensity - IntensityDecayRate * Lifetime);

                if (DecayProgress >= 1f)
                {
                    Expire();
                }
            }
        }

        /// <summary>
        /// Activates the hazard.
        /// </summary>
        public virtual void Activate()
        {
            State = HazardState.Active;
            HasActivated = true;
            ActivationTime = DateTime.Now;
        }

        /// <summary>
        /// Expires the hazard, transitioning it to the expired state.
        /// </summary>
        public virtual void Expire()
        {
            State = HazardState.Expired;
            CurrentIntensity = 0f;
        }

        #endregion
    }

    /// <summary>
    /// States that a hazard can be in.
    /// </summary>
    public enum HazardState
    {
        Pending,    // Waiting to be activated.
        Activating, // In the process of activation.
        Active,     // Fully active.
        Decaying,   // In the process of decaying.
        Expired,    // Fully expired.
        Inactive    // Not currently active.
    }

    /// <summary>
    /// Simple 3D vector for positions.
    /// </summary>
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

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, float scalar) => new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);

        public float LengthSquared => X * X + Y * Y + Z * Z;
        public float Length => (float)System.Math.Sqrt(LengthSquared);
    }

    /// <summary>
    /// Simple rectangle for areas.
    /// </summary>
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
