// =====================================================================================================
//  FILE: ColliosionEvents.cs
//  PATH: Engine/Physics/Collision/ColliosionEvents.cs
//  SUBSYSTEM: Physics Collision Subsystem
//
//  ROLE:
//      Handles collision event classification and state transitions.
//      Processes trigger collisions, physical collisions, and collision‑exit events.
//      Provides deterministic event sequencing for the collision subsystem.
//
//  RESPONSIBILITIES:
//      - Emit debug/event notifications for trigger and physical collisions.
//      - Detect collision exit conditions by comparing current and previous frames.
//      - Maintain deterministic ordering of event processing.
//      - Serve as the final stage of the collision pipeline after resolution.
//
//  NON-RESPONSIBILITIES:
//      - Performing broad‑phase or narrow‑phase collision checks.
//      - Resolving physical collisions or applying forces.
//      - Managing ECS ECSEntityCore iteration or component retrieval.
//      - Generating collision results (handled by CollisionCore).
//
//  ARCHITECTURAL NOTES:
//      - Invoked by CollisionCore after collision resolution.
//      - Operates only on CollisionResult objects produced earlier in the pipeline.
//      - Ensures clean separation between detection, resolution, and event signaling.
// =====================================================================================================

using System.Collections.Concurrent;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Physics.Collision
{
    internal class CollisionEvents
    {
        internal void ProcessCollisionEvents(
            ConcurrentBag<CollisionResult> currentCollisions,
            ConcurrentBag<CollisionResult> previousCollisions)
        {
            // Trigger vs Physical collision notifications
            foreach (var collision in currentCollisions)
            {
                if (collision.IsTrigger)
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Trigger collision detected");
                else
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Physical collision detected");
            }

            // Collision exit detection
            foreach (var previous in previousCollisions)
            {
                if (!currentCollisions.Any(c => c.Equals(previous)))
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "Collision exit detected");
            }
        }
    }
}
