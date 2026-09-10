// =====================================================================================================
//  FILE: TowerCooldownData.cs
//  PATH: Engine/Towers/Save/TowerCooldownData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing cooldown state values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store cooldown timing values deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing cooldown logic or timing progression.
//      - Managing runtime ability triggers or cooldown resets.
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
    internal sealed class TowerCooldownData
    {
        public int CooldownRemaining { get; set; }
        public int CooldownMax { get; set; }

        public TowerCooldownData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerCooldownData::CTOR - Initialized");
        }
    }
}
