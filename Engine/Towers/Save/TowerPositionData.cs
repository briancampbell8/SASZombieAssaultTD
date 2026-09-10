// =====================================================================================================
//  FILE: TowerPositionData.cs
//  PATH: Engine/Towers/Save/TowerPositionData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing positional values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store tower world/grid position deterministically.
//      - Store rotation values for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing movement, placement, or rotation logic.
//      - Managing runtime spatial systems or grid occupancy rules.
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
    internal sealed class TowerPositionData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float Rotation { get; set; }
        public int GridCell { get; set; }

        public TowerPositionData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerPositionData::CTOR - Initialized");
        }
    }
}
