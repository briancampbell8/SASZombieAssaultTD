// ROLE: Simple chase behavior for AI entities.
// RESPONSIBILITY: Move an entity toward a target position using vector-based pursuit 
//                  with configurable speed.
// TRIGGERS: Invoked by AIController during behavior Update cycle.
// INPUTS: Receives AIContext with target position and delta time.
// OUTPUTS: Updates entity position through IMovable interface.
// DEPENDENCIES: Requires AIContext, IMovable, and IPositionProvider interfaces.
// CONTENTS: BasicChaseBehavior class implementing IAIBehavior with Speed property and Tick method.

using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.AI.Behaviors
{
    using SASZombieAssaultTD.Engine.AI;

    /// <summary>
    /// Simple chase behavior that moves an entity toward a target.
    /// Implements vector-based pursuit with normalized direction and deltaTime scaling.
    /// </summary>
    public class BasicChaseBehavior : IAIBehavior
    {
        /// <summary>
        /// Movement speed in units per second.
        /// Configurable per behavior instance.
        /// </summary>
        public float Speed { get; set; } = 50f;

        /// <summary>
        /// Executes one chase tick.
        /// Calculates direction vector, normalizes it, and moves owner toward target.
        /// </summary>
        /// <param name="context">AI context containing owner (IMovable) and target (IPositionProvider).</param>
        public void Tick(AIContext context)
        {
            if (context.Owner is IMovable movable && context.Target is IPositionProvider target)
            {
                var pos = movable.Position;
                var targetPos = target.Position;

                var dx = targetPos.X - pos.X;
                var dy = targetPos.Y - pos.Y;
                var len = MathF.Sqrt(dx * dx + dy * dy);
                if (len > 0.001f)
                {
                    dx /= len;
                    dy /= len;
                    pos = new PointF(
                    pos.X + dx * Speed * context.DeltaTime,
                    pos.Y + dy * Speed * context.DeltaTime
                    );
                    movable.Position = pos;
                }
            }
        }
    }

    /// <summary>
    /// Interface for entities that can move (have a settable position).
    /// Implemented by enemies, NPCs, and other mobile game objects.
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Current position of the entity. Can be read and modified.
        /// </summary>
        PointF Position { get; set; }
    }

    /// <summary>
    /// Interface for entities that provide a position (read-only).
    /// Implemented by targets, waypoints, and stationary reference points.
    /// </summary>
    public interface IPositionProvider
    {
        /// <summary>
        /// Position of the entity. Read-only access for targeting.
        /// </summary>
        PointF Position { get; }
    }
}




