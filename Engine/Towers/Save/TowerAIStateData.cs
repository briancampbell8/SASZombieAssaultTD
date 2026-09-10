// =====================================================================================================
//  FILE: TowerAIStateData.cs
//  PATH: Engine/Towers/Save/TowerAIStateData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing AI state values
//      required for tower save/load operations. Provides a minimal, stable,
//      serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store AI state identifiers deterministically.
//      - Provide stable fields for save/load serialization.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing AI logic or behavior trees.
//      - Managing runtime AI transitions or decision processes.
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
    internal sealed class TowerAIStateData
    {
        public int AIState { get; set; }
        public int BehaviorTreeNodeID { get; set; }
        public int LastActionTick { get; set; }

        public TowerAIStateData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerAIStateData::CTOR - Initialized");
        }
    }
}
