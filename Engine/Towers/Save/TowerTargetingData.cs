// =====================================================================================================
//  FILE: TowerAmmoData.cs
//  PATH: Engine/Towers/Save/TowerAmmoData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing targeting mode,
//      priority, and last‑target identifiers required for tower save/load
//      operations. Provides a minimal, stable, serialization‑ready data surface
//      with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store targeting mode and priority deterministically.
//      - Store last‑target identifiers for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing targeting logic or selecting runtime targets.
//      - Managing aim resolution, targeting heuristics, or threat evaluation.
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
    internal sealed class TowerTargetingData
    {
        public int TargetMode { get; set; }
        public int Priority { get; set; }
        public int LastTargetID { get; set; }

        public TowerTargetingData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerTargetingData::CTOR - Initialized");
        }
    }
}
