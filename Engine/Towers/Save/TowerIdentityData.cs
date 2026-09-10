// =====================================================================================================
//  FILE: TowerIdECSEntityCoreData.cs
//  PATH: Engine/Towers/Save/TowerIdECSEntityCoreData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing idECSEntityCore values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store tower idECSEntityCore fields deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing idECSEntityCore logic or tower initialization rules.
//      - Managing runtime tower activation or placement systems.
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
    internal sealed class TowerIdECSEntityCoreData
    {
        public int TowerID { get; set; }
        public int TowerType { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsPlaced { get; set; }

        public TowerIdECSEntityCoreData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerIdECSEntityCoreData::CTOR - Initialized");
        }
    }
}
