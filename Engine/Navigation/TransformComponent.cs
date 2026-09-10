// ====================================================================================================
//  FILE: TransformComponent.cs
//  PATH: ./Engine/Navigation/
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides the spatial position component used by Navigation subsystem entities. This component
//      stores deterministic 2D coordinates and exposes a Vector3 position representation for
//      compatibility with navigation and movement utilities.
//
//  RESPONSIBILITIES:
//      - Store world-space X/Y coordinates for navigation-enabled entities.
//      - Provide a Vector3 position wrapper for systems requiring 3D-compatible math types.
//      - Serve as a foundational component required by NavAgentComponent and Navigation routines.
//
//  NON-RESPONSIBILITIES:
//      - Executing navigation logic or movement behavior.
//      - Managing ECSEntityCore lifecycle, attachment sequencing, or subsystem orchestration.
//      - Performing pathfinding, migration, or validation operations.
//
//  NOTES:
//      This component was extracted from legacy NavigationMigrationHelper.cs and now resides in its
//      proper subsystem-aligned module. Contains no logic beyond deterministic spatial storage.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    public sealed class TransformComponent : BaseComponent
    {
        public float X;
        public float Y;

        public Vector3 Position => new Vector3(X, Y, 0f);
    }
}
