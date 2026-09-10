// =====================================================================================================
//  FILE: NavigationResult.cs
//  PATH: Engine/Navigation/NavigationResult.cs
//  SUBSYSTEM: Navigation
//
//  ROLE:
//      Provides deterministic result structures for Navigation subsystem operations. This file
//      contains result models used by navigation routines, migration validators, and path
//      consistency checks.
//
//  RESPONSIBILITIES:
//      - Represent pure data outcomes from navigation or migration validation processes.
//      - Store issue collections, counts, and success/failure indicators in a deterministic form.
//      - Provide analysis summaries for migration progress and subsystem recommendations.
//      - Serve as the result contract for higher-level Navigation subsystem operations.
//
//  NON-RESPONSIBILITIES:
//      - Executing navigation logic, pathfinding algorithms, or migration routines.
//      - Managing engine state, runtime sequencing, or subsystem orchestration.
//      - Performing validation logic directly; this file only stores results.
//
//  ARCHITECTURAL NOTES:
//      - Navigation result classes are lightweight containers used by Navigation subsystem programs.
//      - These classes contain no logic beyond simple field/property storage.
//      - This header is a template updated to match the purpose of this Navigation result file.
//
//  CHANGE LOG:
//      - 2026-08-12: Initial creation. Created by: BDC - Breakup of NavigationMigrationHelper.cs
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Navigation
{
    // ======================================================================
    // MIGRATION VALIDATION RESULT
    // ======================================================================

    public sealed class NavigationResult
    {
        public int MigratedCount { get; set; }
        public Dictionary<int, List<string>> Issues { get; } = new();

        public void AddIssue(int ECSEntityCoreId, string issue)
        {
            if (!Issues.ContainsKey(ECSEntityCoreId))
                Issues[ECSEntityCoreId] = new List<string>();

            Issues[ECSEntityCoreId].Add(issue);
        }

        public bool Success => Issues.Count == 0;
    }

    // ======================================================================
    // MIGRATION ANALYSIS RESULT
    // ======================================================================



}
