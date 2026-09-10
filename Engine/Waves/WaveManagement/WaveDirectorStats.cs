// ====================================================================================================
//  FILE: WaveDirectorStats.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorStats.cs
//  SUBSYSTEM: Waves
//
//  ROLE:
//      Handles stats tracking, progress mapping calculations, and wave validation checks.
//
//  RESPONSIBILITIES:
//      - Compute remaining and progress values for active game states safely.
//      - Evaluate overall enemy script size scaling calculations cleanly.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Waves.Difficulty;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public sealed class WaveDirectorStats
    {
        // Reference the central core tracking object cleanly
        private readonly WaveDirectorCore _core = WaveDirectorCore.Instance;

        //===============================================================================================
        // CURRENT WAVE EXTRACTORS
        //===============================================================================================
        public WaveScript GetActiveWaveScript()
        {
            // CRITICAL FIX (CS0103): Accessing internal core variable securely via instance context
            return _core._currentWave;
        }

        public int GetCurrentWaveDisplayProgress()
        {
            // CRITICAL FIX (CS0103): Map core counter tracking scopes safely
            if (_core._currentWave == null)
                return 0;

            return _core._currentWaveNumber;
        }

        //===============================================================================================
        // PROGRESSION CALCULATIONS
        //===============================================================================================
        public float GetTotalWavePercentage()
        {
            // CRITICAL FIX (CS0103): Route state check calculations using core reference mappings
            if (_core._totalWaves <= 0)
                return 0f;

            return (float)_core._currentWaveNumber / _core._totalWaves;
        }

        public bool IsActiveSessionFinished()
        {
            // CRITICAL FIX (CS0103): Core field state checking evaluation loops
            if (_core._currentState == WaveState.NotStarted)
                return false;

            return _core._currentWaveNumber >= _core._totalWaves && _core._currentState == WaveState.Complete;
        }

        //===============================================================================================
        // ENEMY COUNT MATH OPERATIONS
        //===============================================================================================
        public int GetTotalEnemiesInWave(WaveScript script)
        {
            if (script == null)
                return 0;

            int aggregateCount = 0;

            foreach (var group in script.SpawnGroups)
            {
                // CRITICAL FIX (CS0103): Replaced missing ApplyDifficultyMultiplier_Internal local method call
                // with our robust, fixed DifficultyProgression static indexer utility.
                int baseCount = group.Count;
                float progressionMultiplier = DifficultyProgression.GetMultiplier[script.WaveNumber];

                aggregateCount += (int)(baseCount * progressionMultiplier);
            }

            return aggregateCount;
        }
    }
}
