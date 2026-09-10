// ====================================================================================================
// FILE: HazardAnalyticsSummary.cs
// PATH: Engine/HazardsControl/HazardAnalyticsSummary.cs
// MODULE: HazardsControl
//
// ROLE:
//     Central orchestrator for all hazard subsystems.
//     Provides the unified API surface for registering, updating, analyzing,
//     and visualizing hazards.
//
// RESPONSIBILITIES:
//     - Maintain the active hazard list.
//     - Route operations to lifecycle, registration, cleanup, occupancy,
//       density, kill attribution, lane interaction, analytics, and visuals.
//     - Manage subsystem initialization and cleanup.
//     - Provide deterministic hazard analytics and zone/density queries.
//     - Expose a clean external API with no embedded hazard logic.
//
// NON-RESPONSIBILITIES:
//     - Performing hazard logic (delegated to subsystems).
//     - Rendering, simulation, or persistence.
//     - Managing hazard types or definitions.
//
// NOTES:
//     Pure orchestration layer. All complex behavior is delegated.
//     Single responsibility: coordination and routing.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    public class HazardAnalyticsSummary
    {
        internal Dictionary<string, int> TypeBreakdown;
        private int totalCount;
        private int activeCount;

        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }

        public int Active { get; set; }
        public int Inactive { get; set; }
        public int Dead { get; set; }
        public int Revived { get; set; }
        public int Killed { get; set; }
        public int Reviving { get; set; }
        public int ReviveTime { get; set; }
        public int KillTime { get; set; }

        public HazardAnalyticsSummary()
        {
            TotalCount = totalCount;
            ActiveCount = activeCount;
        }
        public float AverageLifetime { get; set; }
        public float EffectivenessScore { get; set; }

        public HazardAnalyticsSummary(
            int totalCount,
            int activeCount,
            float averageLifetime,
            float effectivenessScore
            )
        {

        }



        public enum HazardType
        {
            Fire,
            Ice,
            Lightning,
            Chemical,
            Radiation,
            Nuke,
            Poison

        }


    }
}
