// =====================================================================================================
//  FILE: TowerLevelData.cs
//  PATH: Engine/Towers/Save/TowerLevelData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing level and progression
//      values required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store tower level and XP values deterministically.
//      - Store upgrade path identifiers for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing level‑up logic or XP progression rules.
//      - Managing runtime upgrade systems or tower evolution behavior.
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
    internal sealed class TowerLevelData
    {
        public int Level { get; set; }
        public int XP { get; set; }
        public int UpgradePathID { get; set; }

        public TowerLevelData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerLevelData::CTOR - Initialized");
        }
    }
}
