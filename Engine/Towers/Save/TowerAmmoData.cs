// =====================================================================================================
//  FILE: TowerAmmoData.cs
//  PATH: Engine/Towers/Save/TowerAmmoData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing ammo state values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store ammo counts and reload timing deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing reload logic or ammo consumption rules.
//      - Managing runtime firing or projectile systems.
//      - Performing validation, mutation, or lifecycle operations.
//
//  ARCHITECTURAL NOTES:
//      - Part of the TowerSaveData breakup into micro‑class POCO models.
//      - Contains no engine‑level logic; pure data container.
//      - All diagnostic trace statements are applied deterministically.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Towers.Save
{
    internal sealed class TowerAmmoData
    {
        public int AmmoCount { get; set; }
        public int AmmoMax { get; set; }
        public int ReloadTime { get; set; }

        public TowerAmmoData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerAmmoData::CTOR - Initialized");
        }
    }
}
