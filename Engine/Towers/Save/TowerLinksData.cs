// =====================================================================================================
//  FILE: TowerLinksData.cs
//  PATH: Engine/Towers/Save/TowerLinksData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing tower link and
//      network‑association values required for tower save/load operations.
//      Provides a minimal, stable, serialization‑ready data surface with no
//      behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store linked tower identifiers deterministically.
//      - Store network group identifiers for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing link logic or network propagation behavior.
//      - Managing runtime link resolution or network grouping systems.
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
    internal sealed class TowerLinksData
    {
        public List<int> LinkedTowerIDs { get; set; } = new List<int>();
        public int NetworkGroupID { get; set; }

        public TowerLinksData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerLinksData::CTOR - Initialized");
        }
    }
}
