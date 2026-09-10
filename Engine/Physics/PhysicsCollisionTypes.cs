// ====================================================================================================
//  FILE: PhysicsCollisionTypes.cs
//  PATH: ./Engine/Physics/
//  MODULE: Physics
//  AUTHOR: BDC
//
//  ROLE:
//      Encapsulate core engine behavior for the PhysicsCollisionTypes module.
//
//  RESPONSIBILITIES:
//      - Provide collision-layer definitions for the Physics subsystem.
//      - Provide collision-category definitions for the Physics subsystem.
//      - Provide collision-mask utilities for the Physics subsystem.
//      - Provide DLogger diagnostics for collision-type usage.
//
//  NON-RESPONSIBILITIES:
//      - ECS component lifecycle management.
//      - Spatial grid indexing.
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      This file was migrated from Engine/ECS/CollisionTypes.cs during ECSRuntimeCore breakup.
//      All physics shapes and collision events now live in the Physics subsystem.
//      Auto-generated structure verified locally via file state scripts.
//
//  CHANGE LOG:
//      [2026-08-13] Created PhysicsCollisionTypes.cs from migrated ECS file. (BDC)
// ====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    /// <summary>
    /// Defines collision layers used by the Physics subsystem.
    /// </summary>

    /// <summary>
    /// Provides collision-mask utilities for determining interaction rules.
    /// </summary>
    public static class CollisionMask
    {
        /// <summary>
        /// Computes whether two collision layers should interact.
        /// </summary>
        public static bool LayersInteract(CollisionLayer a, CollisionLayer b)
        {
            bool result = ((uint)a & (uint)b) != 0;

            DLogger.Log($"[PhysicsCollisionTypes] LayersInteract(a={a}, b={b}) => {result}");

            return result;
        }

        /// <summary>
        /// Computes whether two collision categories should interact.
        /// </summary>
        public static bool CategoriesInteract(CollisionCategory a, CollisionCategory b)
        {
            bool result = ((uint)a & (uint)b) != 0;

            DLogger.Log($"[PhysicsCollisionTypes] CategoriesInteract(a={a}, b={b}) => {result}");

            return result;
        }
    }
}
