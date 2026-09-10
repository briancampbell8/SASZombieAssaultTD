// =====================================================================================================
//  FILE: TowerModData.cs
//  PATH: Engine/Towers/Save/TowerModData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing tower modification
//      identifiers, levels, and states required for tower save/load operations.
//      Provides a minimal, stable, serialization‑ready data surface with no
//      behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store mod identifiers deterministically.
//      - Store mod levels and mod states for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing mod logic or applying runtime modifiers.
//      - Managing mod activation, deactivation, or progression systems.
//      - Performing validation, mutation, or lifecycle operations.
//
//  ARCHITECTURAL NOTES:
//      - Part of the TowerSaveData breakup into micro‑class POCO models.
//      - Contains no engine‑level logic; pure data container.
//      - All diagnostic trace statements are applied deterministically.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers.Save
{
    internal sealed class TowerModData
    {
        public List<int> ModIDs { get; set; } = new List<int>();
        public List<int> ModLevels { get; set; } = new List<int>();
        public List<int> ModStates { get; set; } = new List<int>();

        public TowerModData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerModData::CTOR - Initialized");
        }
    }
}
