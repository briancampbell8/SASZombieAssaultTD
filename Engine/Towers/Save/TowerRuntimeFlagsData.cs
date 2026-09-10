// =====================================================================================================
//  FILE: TowerRuntimeFlagsData.cs
//  PATH: Engine/Towers/Save/TowerRuntimeFlagsData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing runtime flag values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store runtime boolean flags deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing runtime flag logic or state transitions.
//      - Managing boost, freeze, or disable mechanics.
//      - Performing validation, mutation, or lifecycle operations.
//
//  ARCHITECTURAL NOTES:
//      - Part of the TowerSaveData breakup into micro‑class POCO models.
//      - Contains no engine‑level logic; pure data container.
//      - All diagnostic trace statements are applied deterministically.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Save
{
    internal sealed class TowerRuntimeFlagsData
    {
        public bool IsOverclocked { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsBoosted { get; set; }

        public TowerRuntimeFlagsData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerRuntimeFlagsData::CTOR - Initialized");
        }
    }
}
