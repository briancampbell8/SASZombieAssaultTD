using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Handles movement logic for a zombie entity.
    /// </summary>
    public class ZombieMovement
    {
        /// <summary>
        /// Gets the current position of the zombie.
        /// </summary>
        public PointF Position { get; private set; }

        /// <summary>
        /// Gets or sets the movement speed of the zombie.
        /// </summary>
        public float Speed { get; set; } = 40f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZombieMovement"/> class.
        /// </summary>
        /// <param name="start">The starting position of the zombie.</param>
        public ZombieMovement(PointF start)
        {
            Position = start;
        }

        /// <summary>
        /// Moves the zombie towards the specified target position.
        /// </summary>
        /// <param name="target">The target position to move towards.</param>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public void MoveTowards(PointF target, float deltaTime)
        {
            float dx = target.X - Position.X;
            float dy = target.Y - Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            if (distance < 0.001f)
                return;

            dx /= distance;
            dy /= distance;

            Position = new PointF(
                Position.X + dx * Speed * deltaTime,
                Position.Y + dy * Speed * deltaTime
            );
        }

        /// <summary>
        /// Calculates the distance between the zombie's current position and the target position.
        /// </summary>
        /// <param name="target">The target position.</param>
        /// <returns>The distance to the target.</returns>
        public float DistanceTo(PointF target)
        {
            float dx = target.X - Position.X;
            float dy = target.Y - Position.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
    }
}




