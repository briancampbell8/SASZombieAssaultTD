// =====================================================================================================
//  FILE: HazardAnalytics.cs
//  PATH: Engine/HazardsControl/HazardAnalytics.cs
//  SUBSYSTEM: Hazards Module
//
//  ROLE:
//      Provides deterministic, read-only analytics over active hazards managed by HazardsMain.
//
//  RESPONSIBILITIES:
//      - Compute total hazard count.
//      - Compute active hazard count.
//      - Compute hazard type breakdown.
//      - Compute average hazard lifetime.
//      - Compute average effectiveness score.
//      - Integrate with HazardsMain as an IHazardSubsystem.
//
//  NON-RESPONSIBILITIES:
//      - Mutating hazards.
//      - Spawning hazards.
//      - Performing simulation updates.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    public sealed class HazardAnalytics : IHazardSubsystem
    {
        private readonly List<Hazard> _hazards;

        public HazardAnalytics(List<Hazard> hazards) => _hazards = hazards ?? throw new ArgumentNullException(nameof(hazards));

        public HazardAnalytics() => _hazards = new List<Hazard>();

        // IHazardSubsystem
        public void Init() { }
        public void Cleanup() { }

        public int GetTotalHazardCount()
        {
            return _hazards.Count;
        }
        public struct HazardTypeAnalytics
        {
            public string HazardType { get; set; }
            public int TotalCreated { get; set; }
            public int CurrentlyActive { get; set; }
            public float AverageLifetime { get; set; }
            public float AverageEffectiveness { get; set; }
            public float TotalDamageDealt { get; set; }
            public int TotalEnemiesAffected { get; set; }


        }
        public int GetActiveHazardCount()
        {
            return _hazards.Count(static h => h.IsActive);
        }

        public int GetHazardCountByState(HazardState state)
        {
            Func<Hazard, bool> predicate = hazard => hazard.State == state;
            return _hazards.Count(predicate);
        }

        public Dictionary<string, int> GetHazardTypeBreakdown()
        {
            var breakdown = new Dictionary<string, int>();

            foreach (var hazard in _hazards)
            {
                var key = hazard.Type.ToString();
                if (!breakdown.ContainsKey(key))
                    breakdown[key] = 0;

                breakdown[key]++;
            }

            return breakdown;
        }

        public float GetAverageHazardLifetime()
        {
            if (_hazards.Count == 0)
                return 0f;

            return _hazards.Average(h => h.LifetimeSeconds);
        }

        public float GetAverageEffectivenessScore()
        {
            if (_hazards.Count == 0)
                return 0f;

            return _hazards.Average(h => h.EffectivenessScore);
        }
    }
}
