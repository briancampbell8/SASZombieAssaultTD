// =====================================================================================================
//  FILE: TowerEconomyData.cs
//  PATH: Engine/Towers/Save/TowerEconomyData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing economy values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store tower economic values deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing sell/buy logic or refund calculations.
//      - Managing runtime economy systems or tower pricing rules.
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
    internal sealed class TowerEconomyData
    {
        public int SellValue { get; set; }
        public int PurchaseCost { get; set; }
        public int RefundRate { get; set; }

        public TowerEconomyData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerEconomyData::CTOR - Initialized");
        }
    }
}
