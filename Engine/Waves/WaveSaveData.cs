// =====================================================================================================
//  FILE: WaveSaveData.cs
//  PATH: Engine/Save/GameSave/WaveSaveData.cs
//  SUBSYSTEM: GameSave Data Models
//
//  ROLE:
//      GSDataModels defines all data‑only structures used by GSCore and the GS subsystem. These classes
//      contain no game logic, no capture logic, no apply logic, and no serialization logic. They are
//      pure containers for save‑state information.
//
//  RESPONSIBILITIES:
//      - Provide strongly‑typed containers for all save data categories.
//      - Support cloning for backup creation.
//      - Remain stable and predictable across engine versions.
//      - Maintain strict separation from GSCore, GSCapture, GSApply, GSValidation, and GSSerial.
//
//  NON-RESPONSIBILITIES:
//      - Capturing game state (handled by GSCapture).
//      - Applying save data (handled by GSApply).
//      - Validating save data (handled by GSValidation).
//      - Serializing save data (handled by GSSerial).
//      - Managing GSCore lifecycle or metadata.
//
//  ARCHITECTURAL NOTES:
//      - All classes are POCOs (Plain Old CLR Objects).
//      - All classes support cloning for backup creation.
//      - GSDataModels replaces the nested classes previously embedded in SASGameSave.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves
{
    // =================================================================================================
    // WAVE SAVE DATA
    // =================================================================================================
    public class WaveSaveData
    {
        public int CurrentWave { get; set; }
        public int TotalWaves { get; set; }
        public float WaveProgress { get; set; }
        public float OverallProgress { get; set; }
        public bool IsWaveActive { get; set; }

        public List<int> CompletedWaves { get; set; } = new();

        public WaveSaveData Clone()
        {
            return new WaveSaveData
            {
                CurrentWave = CurrentWave,
                TotalWaves = TotalWaves,
                WaveProgress = WaveProgress,
                OverallProgress = OverallProgress,
                IsWaveActive = IsWaveActive,
                CompletedWaves = new List<int>(CompletedWaves)
            };
        }
    }
}
