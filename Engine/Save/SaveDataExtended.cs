/*
Program Name: SASZombieAssaultTD
File Path: Engine\Save\SaveDataExtended.cs
Purpose: Extended SaveData with P120/P100 integration.
Features: Battlefield progress, wave progress, difficulty persistence, inherits from SaveData.
*/

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

//
using SASZombieAssaultTD.Engine.GameRoot.GamePlay;
using SASZombieAssaultTD.Engine.Waves;

namespace SASZombieAssaultTD.Engine.Save
{
    /// <summary>
    /// Extended SaveData with P120/P100 integration. P140-04: Extends SaveData to add battlefield and wave progress
    /// tracking.
    /// </summary>
    public class SaveDataExtended : SaveData
    {
        private Dictionary<BattlefieldType, BattlefieldProgress> _battlefieldProgress;
        private Dictionary<BattlefieldType, bool> _unlockedBattlefields;
        private DifficultyScaling.DifficultyLevel _currentDifficulty;
        private int _currentWave;
        private Dictionary<int, WaveProgress> _waveProgress;

        /// <summary>
        /// Gets the battlefield progress dictionary. P140-04: P120 Integration for battlefield progress tracking.
        /// </summary>
        public Dictionary<BattlefieldType, BattlefieldProgress> BattlefieldProgress
            => _battlefieldProgress ?? (_battlefieldProgress = new Dictionary<BattlefieldType, BattlefieldProgress>());

        /// <summary>
        /// Gets the unlocked battlefields dictionary. P140-04: P120 Integration for battlefield unlock tracking.
        /// </summary>
        public Dictionary<BattlefieldType, bool> UnlockedBattlefields
            => _unlockedBattlefields ?? (_unlockedBattlefields = new Dictionary<BattlefieldType, bool>());

        /// <summary>
        /// Gets or sets the current difficulty level. P140-04: P100 Integration for difficulty setting persistence.
        /// </summary>
        public DifficultyScaling.DifficultyLevel CurrentDifficulty
        {
            get => _currentDifficulty;
            set => _currentDifficulty = value;
        }

        /// <summary>
        /// Gets or sets the current wave number. P140-04: P100 Integration for wave progression persistence.
        /// </summary>
        public int CurrentWave
        {
            get => _currentWave;
            set => _currentWave = System.Math.Max(0, value);
        }

        /// <summary>
        /// Gets the wave progress dictionary. P140-04: P100 Integration for wave progress tracking.
        /// </summary>
        public Dictionary<int, WaveProgress> WaveProgress
            => _waveProgress ?? (_waveProgress = new Dictionary<int, WaveProgress>());

        /// <summary>
        /// Initializes a new SaveDataExtended instance.
        /// </summary>
        public SaveDataExtended() : base()
        {
            _battlefieldProgress = new Dictionary<BattlefieldType, BattlefieldProgress>();
            _unlockedBattlefields = new Dictionary<BattlefieldType, bool>();
            _currentDifficulty = DifficultyScaling.DifficultyLevel.Normal;
            _currentWave = 0;
            _waveProgress = new Dictionary<int, WaveProgress>();

            //Unlock Mean Street by default (first battlefield)
            _unlockedBattlefields[BattlefieldType.MeanStreet] = true;

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "INFO", "SaveDataExtended: Created with P120/P100 integration");
        }

        /// <summary>
        /// Gets progress for a specific battlefield. P140-04: P120 Integration for battlefield progress.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>The battlefield progress, or null if not found.</returns>
        public BattlefieldProgress GetBattlefieldProgress(BattlefieldType battlefield)
        {
            return _battlefieldProgress.TryGetValue(battlefield, out var progress) ? progress : null;
        }

        /// <summary>
        /// Updates progress for a specific battlefield. P140-04: P120 Integration for battlefield progress.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <param name="completed">Whether the battlefield was completed.</param>
        /// <param name="highestWave">The highest wave reached.</param>
        /// <param name="highScore">The high score achieved.</param>
        public void UpdateBattlefieldProgress(BattlefieldType battlefield, bool completed, int highestWave, int highScore)
        {
            var progress = GetBattlefieldProgress(battlefield);
            if (progress == null)
            {
                progress = new BattlefieldProgress(battlefield);
                _battlefieldProgress[battlefield] = progress;
            }

            progress.UpdateProgress(completed, highestWave, highScore);
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "INFO", $"SaveDataExtended: Updated {battlefield.GetDisplayName()} progress");
        }

        /// <summary>
        /// Checks if a battlefield is unlocked. P140-04: P120 Integration for battlefield unlock status.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        /// <returns>True if the battlefield is unlocked.</returns>
        public bool IsBattlefieldUnlocked(BattlefieldType battlefield)
        {
            return _unlockedBattlefields.TryGetValue(battlefield, out var unlocked) && unlocked;
        }

        /// <summary>
        /// Unlocks a battlefield. P140-04: P120 Integration for battlefield unlocking.
        /// </summary>
        /// <param name="battlefield">The battlefield type.</param>
        public void UnlockBattlefield(BattlefieldType battlefield)
        {
            _unlockedBattlefields[battlefield] = true;
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "INFO", $"SaveDataExtended: Unlocked battlefield {battlefield.GetDisplayName()}");
        }

        /// <summary>
        /// Gets progress for a specific wave. P140-04: P100 Integration for wave progress.
        /// </summary>
        /// <param name="waveNumber">The wave number.</param>
        /// <returns>The wave progress, or null if not found.</returns>
        public WaveProgress GetWaveProgress(int waveNumber)
        {
            return _waveProgress.TryGetValue(waveNumber, out var progress) ? progress : null;
        }

        /// <summary>
        /// Updates progress for a specific wave. P140-04: P100 Integration for wave progress.
        /// </summary>
        /// <param name="waveNumber">The wave number.</param>
        /// <param name="completed">Whether the wave was completed.</param>
        /// <param name="enemiesKilled">Number of enemies killed.</param>
        /// <param name="championsDefeated">Number of champions defeated.</param>
        /// <param name="timeTaken">Time taken to complete the wave.</param>
        public void UpdateWaveProgress(int waveNumber, bool completed, int enemiesKilled, int championsDefeated, float timeTaken)
        {
            var progress = GetWaveProgress(waveNumber);
            if (progress == null)
            {
                progress = new WaveProgress(waveNumber);
                _waveProgress[waveNumber] = progress;
            }

            progress.UpdateProgress(completed, enemiesKilled, championsDefeated, timeTaken);
            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "INFO", $"SaveDataExtended: Updated wave {waveNumber} progress");
        }

        /// <summary>
        /// Resets all P120/P100 progress to default values.
        /// </summary>
        public void ResetExtendedProgress()
        {
            _battlefieldProgress.Clear();
            _unlockedBattlefields.Clear();
            _currentDifficulty = DifficultyScaling.DifficultyLevel.Normal;
            _currentWave = 0;
            _waveProgress.Clear();

            //Unlock Mean Street by default
            _unlockedBattlefields[BattlefieldType.MeanStreet] = true;

            DLogger.Log(LogSubsystems.Unknown, LogEnums.LogLevel.Info, "INFO", "SaveDataExtended: Reset extended progress to defaults");
        }

        /// <summary>
        /// Creates a clone of this extended save data.
        /// </summary>
        /// <returns>A new SaveDataExtended instance with the same values.</returns>
        public new SaveDataExtended Clone()
        {
            var clone = new SaveDataExtended();

            //Clone battlefield progress
            foreach (var kvp in _battlefieldProgress)
            {
                clone._battlefieldProgress[kvp.Key] = kvp.Value.Clone();
            }

            //Clone unlocked battlefields
            foreach (var kvp in _unlockedBattlefields)
            {
                clone._unlockedBattlefields[kvp.Key] = kvp.Value;
            }

            //Clone wave progress
            foreach (var kvp in _waveProgress)
            {
                clone._waveProgress[kvp.Key] = kvp.Value.Clone();
            }

            //Clone P100 integration fields
            clone._currentDifficulty = _currentDifficulty;
            clone._currentWave = _currentWave;

            return clone;
        }

        /// <summary>
        /// Gets extended save data information as a string.
        /// </summary>
        public override string ToString()
        {
            return $"SaveDataExtended: {base.ToString()}, Battlefields={_battlefieldProgress.Count}, " +
                   $"Unlocked={_unlockedBattlefields.Count}, Difficulty={_currentDifficulty}, " +
                   $"CurrentWave={_currentWave}, WaveProgress={_waveProgress.Count}";
        }
    }
}
