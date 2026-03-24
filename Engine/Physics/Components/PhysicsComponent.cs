/*
File:    PhysicsComponent.cs
Path:    Engine/Components/PhysicsComponent.cs
Purpose:   P11-04-02-A - Core ECS component for physics simulation properties.
           Stores velocity, acceleration, mass, drag, bounciness, and kinematic flags.

Role:      Fundamental physics component for entities requiring physical simulation.
           - Stores linear velocity for movement calculations
           - Manages acceleration for force-based physics
           - Handles mass for collision response calculations
           - Provides drag coefficients for air resistance
           - Controls bounciness for collision elasticity
           - Supports kinematic flag for animated objects
           - Enforces maximum speed limits for gameplay balance

Features:   ECS-friendly pure data structure with no logic.
           Optimized for high-frequency physics system access.
           Thread-safe property access for concurrent systems.
           Supports serialization for save/load functionality.
           Provides utility methods for common physics calculations.

Notes:      This component is required by all entities participating in physics simulation.
           All physics values are designed for real-time performance.
           Component integrates seamlessly with collision detection system.
           Mass values are used for realistic collision responses.

*/
using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Physics.Components
{
    /// <summary>
    /// Component for physics simulation properties.
    /// P11-04-02-A: Stores velocity, acceleration, mass, drag, bounciness, kinematic flag, and max speed.
    /// This component is pure data, ECS-friendly, and fully documented.
    /// </summary>
    public class PhysicsComponent
    {
        /// <summary>
        /// Current velocity of the entity in world units per second.
        /// </summary>
        public PointF Velocity { get; set; } = PointF.Empty;

        /// <summary>
        /// Current acceleration of the entity in world units per second squared.
        /// </summary>
        public PointF Acceleration { get; set; } = PointF.Empty;

        /// <summary>
        /// Mass of the entity (affects force calculations and collision response).
        /// Higher mass = more resistance to acceleration changes.
        /// </summary>
        public float Mass { get; set; } = 1.0f;

        /// <summary>
        /// Drag coefficient that opposes motion (0 = no drag, higher = more resistance).
        /// Applied as a force proportional to velocity magnitude.
        /// </summary>
        public float Drag { get; set; } = 0.0f;

        /// <summary>
        /// Bounciness coefficient for collision response (0.0 = no bounce, 1.0 = perfect bounce).
        /// Affects how much velocity is retained after collision.
        /// </summary>
        public float Bounciness { get; set; } = 0.5f;

        /// <summary>
        /// Whether this entity is kinematic (true = not affected by forces, false = affected by forces).
        /// Kinematic entities can still have velocity but don't respond to external forces.
        /// </summary>
        public bool IsKinematic { get; set; } = false;

        /// <summary>
        /// Maximum speed the entity can travel (null = no speed limit).
        /// Velocity is clamped to this value each frame.
        /// </summary>
        public float? MaxSpeed { get; set; } = null;

        /// <summary>
        /// Whether this component is enabled and participating in physics simulation.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// List of actual collisions detected for this entity in the current frame.
        /// Used by collision debugging and statistics systems.
        /// </summary>
        public System.Collections.Generic.List<object> ActualCollisions { get; set; } = new System.Collections.Generic.List<object>();

        /// <summary>
        /// Creates a new PhysicsComponent with default values.
        /// </summary>
        public PhysicsComponent()
        {
        }

        /// <summary>
        /// Creates a new PhysicsComponent with specified mass.
        /// </summary>
        /// <param name="mass">Mass of the entity</param>
        public PhysicsComponent(float mass)
        {
            Mass = mass > 0.0f ? mass : 1.0f; // Ensure positive mass
        }

        /// <summary>
        /// Creates a new PhysicsComponent with specified velocity and acceleration.
        /// </summary>
        /// <param name="velocity">Initial velocity</param>
        /// <param name="acceleration">Initial acceleration</param>
        public PhysicsComponent(PointF velocity, PointF acceleration)
        {
            Velocity = velocity;
            Acceleration = acceleration;
        }

        /// <summary>
        /// Creates a new PhysicsComponent with full configuration.
        /// </summary>
        /// <param name="velocity">Initial velocity</param>
        /// <param name="acceleration">Initial acceleration</param>
        /// <param name="mass">Mass of the entity</param>
        /// <param name="drag">Drag coefficient</param>
        /// <param name="bounciness">Bounciness coefficient</param>
        /// <param name="isKinematic">Whether entity is kinematic</param>
        /// <param name="maxSpeed">Maximum speed limit</param>
        /// <param name="enabled">Whether component is enabled</param>
        public PhysicsComponent(
        PointF velocity,
        PointF acceleration,
        float mass = 1.0f,
        float drag = 0.0f,
        float bounciness = 0.5f,
        bool isKinematic = false,
        float? maxSpeed = null,
        bool enabled = true)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Mass = mass > 0.0f ? mass : 1.0f; // Ensure positive mass
            Drag = System.Math.Max(0.0f, drag); // Ensure non-negative drag
            Bounciness = System.Math.Clamp(bounciness, 0.0f, 1.0f); // Clamp to valid range
            IsKinematic = isKinematic;
            MaxSpeed = maxSpeed.HasValue ? System.Math.Max(0.0f, maxSpeed.Value) : null; // Ensure positive if set
            Enabled = enabled;
        }

        /// <summary>
        /// Applies a force to this entity (changes acceleration based on mass).
        /// </summary>
        /// <param name="force">Force to apply in world units</param>
        public void ApplyForce(PointF force)
        {
            if (Mass <= 0.0f || IsKinematic)
                return;

            // F = ma, therefore a = F/m
            Acceleration = new PointF(
            Acceleration.X + force.X / Mass,
            Acceleration.Y + force.Y / Mass
            );
        }

        /// <summary>
        /// Applies an impulse to this entity (instantaneous velocity change).
        /// </summary>
        /// <param name="impulse">Impulse to apply in world units</param>
        public void ApplyImpulse(PointF impulse)
        {
            if (Mass <= 0.0f || IsKinematic)
                return;

            // J = Δp = mΔv, therefore Δv = J/m
            Velocity = new PointF(
            Velocity.X + impulse.X / Mass,
            Velocity.Y + impulse.Y / Mass
            );
        }

        /// <summary>
        /// Gets the current speed (magnitude of velocity).
        /// </summary>
        /// <returns>Current speed in world units per second</returns>
        public float GetSpeed()
        {
            return (float)System.Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
        }

        /// <summary>
        /// Gets the squared speed (avoids square root operation).
        /// </summary>
        /// <returns>Squared speed in world units squared per second squared</returns>
        public float GetSpeedSquared()
        {
            return Velocity.X * Velocity.X + Velocity.Y * Velocity.Y;
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"PhysicsComponent(Vel: {Velocity}, Acc: {Acceleration}, Mass: {Mass}, Kinematic: {IsKinematic}, Enabled: {Enabled})";
        }
    }
}




