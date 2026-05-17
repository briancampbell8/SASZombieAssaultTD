using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.AI.Behaviors
{
    using SASZombieAssaultTD.Engine.AI;

    public class BasicChaseBehavior : IAIBehavior
    {
        public float Speed { get; set; } = 50f;

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

    public interface IMovable
    {
        PointF Position { get; set; }
    }

    public interface IPositionProvider
    {
        PointF Position { get; }
    }
}




