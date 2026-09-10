// =====================================================================================================
//  FILE: TowerEffectsData.cs
//  PATH: Engine/Towers/Save/TowerEffectsData.cs
//  SUBSYSTEM: Towers Save
//
//  ROLE:
//      Defines the deterministic micro‑class model containing active tower effect
//      identifiers and their associated durations and stack counts. Provides a minimal,
//      stable, serialization‑ready data surface with no behavioral logic.
//
//  RESPONSIBILITIES:
//      - Store active effect IDs deterministically.
//      - Store effect durations and stack counts for save/load operations.
//      - Support diagnostic tracing for all field assignments.
//
//  NON-RESPONSIBILITIES:
//      - Executing effect logic or applying runtime modifiers.
//      - Managing effect expiration, stacking rules, or propagation.
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
    internal sealed class TowerEffectsData
    {
        public List<int> ActiveEffectIDs { get; set; } = new List<int>();
        public List<int> EffectDurations { get; set; } = new List<int>();
        public List<int> EffectStacks { get; set; } = new List<int>();

        public TowerEffectsData()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "TowerEffectsData::CTOR - Initialized");
        }
    }
}
