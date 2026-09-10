// ====================================================================================================
//  FILE: NavAgentComponent.cs
//  PATH: ./Engine/Navigation/
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides the navigation agent component used by the Navigation subsystem. This component stores
//      deterministic navigation parameters, runtime path state, and target information required by
//      NavigationSystem, AISystem, and migration utilities.
//
//  RESPONSIBILITIES:
//      - Store navigation configuration values such as speed, stopping distance, and repath behavior.
//      - Maintain deterministic runtime navigation state including current path, target position,
//        and path validity.
//      - Serve as the core component enabling NavAgent-based movement within the Navigation subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Executing navigation logic, pathfinding algorithms, or movement updates.
//      - Managing ECSEntityCore lifecycle, attachment sequencing, or subsystem orchestration.
//      - Performing migration, validation, or analysis operations.
//
//  NOTES:
//      This component was extracted from legacy NavigationMigrationHelper.cs and now resides in its
//      proper subsystem-aligned module. Contains no logic beyond deterministic navigation state storage.
// ====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
{
    public sealed class NavAgentComponent : BaseComponent
    {
        public float Speed;
        public float StoppingDistance;
        public int MaxRepathAttempts;
        public bool RepathOnBlock;
        public bool UseFlowField;

        // Navigation state properties
        public bool PathValid { get; set; }

        public List<Vector3> CurrentPath { get; set; } = new List<Vector3>();
        public Vector3 TargetPosition { get; set; }

        public string GetStateSummary()
        {
            return $"Speed: {Speed}, PathValid: {PathValid}, PathLength: {CurrentPath.Count}, Target: {TargetPosition}";
        }
    }
}
