// =====================================================================================================
//  FILE: TowerStatsData.cs
//  PATH: Engine/Towers/Save/TowerStatsData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing core tower stat
//      values required for tower save/load operations. Provides a minimal,
//      stable, serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store tower stat values deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing stat logic or applying runtime modifiers.
//      - Managing damage, range, or fire‑rate calculations.
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
    internal sealed class TowerStatsData
    {
        public int Damage { get; set; }
        public float Range { get; set; }
        public float FireRate { get; set; }
        public float SplashRadius { get; set; }
        public int StatusEffectFlags { get; set; }

        public TowerStatsData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerStatsData::CTOR - Initialized");
        }
    }
}
