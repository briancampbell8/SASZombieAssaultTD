// =====================================================================================================
//  FILE: ProjectileComponent.cs
//  PATH: Engine/Components/ProjectileComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing projectile state. Stores positional, directional,
//      velocity, and source/target metadata used by gameplay, combat, and projectile-processing
//      systems.
//
//  RESPONSIBILITIES:
//      - Store projectile position, direction, and velocity
//      - Store projectile metadata (type, source, target)
//      - Provide a stable data surface for gameplay systems that process projectile behavior
//
//  NON-RESPONSIBILITIES:
//      - Executing projectile movement or collision logic
//      - Managing world-level ECSEntityCore lifecycle or ECS attachment
//      - Allocating entities or performing world-level operations
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with no behavioral logic
//      - Integrates with gameplay, combat, and projectile systems but does not implement them
// =====================================================================================================


using System.Numerics;

namespace SASZombieAssaultTD.Engine.Components
{
    public class ProjectileComponent
    {
        public object Projectile { get; internal set; }
        public object ProjectileType { get; internal set; }
        public object ProjectileSource { get; internal set; }
        public int TargetId { get; internal set; }
        public int SourceId { get; internal set; }
        public Vector2 Position { get; internal set; }
        public Vector2 Direction { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        public ProjectileComponent(
            object projectile,
            object projectileType,
            object projectileSource,
            int targetId,
            int sourceId,
            Vector2 position,
            Vector2 direction,
            Vector2 velocity)
        {
            Projectile = projectile;
            ProjectileType = projectileType;
            ProjectileSource = projectileSource;
            TargetId = targetId;
            SourceId = sourceId;
            Position = position;
            Direction = direction;
            Velocity = velocity;
        }

        public ProjectileComponent()
        {
            Projectile = null;
            ProjectileType = null;
            ProjectileSource = null;
        }
    }
}
