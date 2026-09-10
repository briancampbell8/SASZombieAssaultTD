// =====================================================================================================
//  FILE: CollisionEnums.cs
//  PATH: Engine/Physics/CollisionEnums.cs
//  SUBSYSTEM: Physics
//
//  ROLE:
//      Defines enumerations used for collision detection and resolution.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - provides collision enumerations for collision detection and resolution
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    public class CollisionEnums
    {
        //------------------------------------------------------------
        // Collision Layer Enum
        //------------------------------------------------------------
        public enum CollisionLayer
        {
            /// <summary>
            /// No collision layer - ECSEntityCore doesn't participate in collisions.
            /// </summary>
            None = 0,

            /// <summary>
            /// Default layer for general entities.
            /// </summary>
            Default = 1 << 0,

            /// <summary>
            /// Player entities and player-owned objects.
            /// </summary>
            Player = 1 << 1,

            /// <summary>
            /// Enemy entities and enemy-owned objects.
            /// </summary>
            Enemy = 1 << 2,

            /// <summary>
            /// Projectile entities (bullets, rockets, etc.).
            /// </summary>
            Projectile = 1 << 3,

            /// <summary>
            /// Environmental objects (walls, obstacles, terrain).
            /// </summary>
            Environment = 1 << 4,

            /// <summary>
            /// Trigger volumes and zones.
            /// </summary>
            Trigger = 1 << 5,

            /// <summary>
            /// Pickup items and collectibles.
            /// </summary>
            Pickup = 1 << 6,

            /// <summary>
            /// UI and decorative elements.
            /// </summary>
            UI = 1 << 7,

            Custom1 = 1 << 5,
            Custom2 = 1 << 6,

            All = 1 << 8

            /// <summary>
            /// All collision layers (for queries that should hit everything).
            /// </summary>
        }

        //------------------------------------------------------------
        // Collision Category Enum
        //------------------------------------------------------------
        public enum CollisionCategory
        {
            /// <summary>
            /// No collision category - ECSEntityCore doesn't participate in collisions.
            /// </summary>
            None = 0,

            Sensor = 1 << 2,
            Damageable = 1 << 3,
            PhysicsBody = 1 << 4,

            /// <summary>
            /// Solid collision category - entities that can collide with other solid entities.
            /// </summary>
            Solid = 1 << 0,

            /// <summary>
            /// Trigger collision category - entities that can collide with other trigger entities.
            /// </summary>
            Trigger = 1 << 1,

            /// <summary>
            /// All collision categories (for queries that should hit everything).
            /// </summary>
            All = ~0
        }

        public enum CollisionShapeType
        {
            /// <summary>
            /// Unknown or undefined shape type.
            /// </summary>
            Unknown,
            /// <summary>
            ///  Rectangle Shape
            /// </summary>
            Rectangle,
            /// <summary>
            /// Circular collision shape.
            /// </summary>
            Circle,

            /// <summary>
            /// Axis-Aligned Bounding Box collision shape.
            /// </summary>
            AABB,

            /// <summary>
            /// Capsule collision shape (circle with height).
            /// </summary>
            Capsule
        }

        //------------------------------------------------------------
        // Collision Type Enum
        //------------------------------------------------------------
        public enum CollisionType
        {
            None,
            Solid,
            Trigger,
            Overlap,
            Sweep,
            Continuous
        }




    }
}
