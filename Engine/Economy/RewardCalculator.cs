/*
File:    RewardCalculator.cs
Purpose: Calculates rewards from kills, waves, and achievements.
Features: Kill rewards, wave bonuses, achievement rewards, difficulty scaling.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Enemies;
using EnemyType = SASZombieAssaultTD.Engine.Dictionary.EnemyType;

namespace SASZombieAssaultTD.Engine.Economy
{
    /// <summary>
    /// Represents reward calculation parameters.
    /// </summary>
    public class RewardParameters
    {
        public float DifficultyMultiplier { get; set; } = 1.0f;
        public float WaveMultiplier { get; set; } = 1.0f;
        public float ComboMultiplier { get; set; } = 1.0f;
        public float PerformanceBonus { get; set; } = 1.0f;
        public int BaseKillReward { get; set; } = 10;
        public int BaseWaveBonus { get; set; } = 50;
        public int BaseAchievementReward { get; set; } = 100;
    }

    /// <summary>
    /// Calculates various types of rewards for the player.
    /// Handles kill rewards, wave completion bonuses, and achievement rewards.
    /// </summary>
    public class RewardCalculator
    {
        private static RewardCalculator _instance;
        public static RewardCalculator Instance => _instance ??= new RewardCalculator();

        private readonly Dictionary<EnemyType, int> _enemyKillRewards;
        private readonly Dictionary<string, int> _waveCompletionBonuses;
        private RewardParameters _parameters;

        private RewardCalculator()
        {
            _parameters = new RewardParameters();
            _enemyKillRewards = new Dictionary<EnemyType, int>();
            _waveCompletionBonuses = new Dictionary<string, int>();
            InitializeRewardTables();
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "RewardCalculator: Initialized with reward tables");
        }

        /// <summary>
        /// Gets current reward parameters.
        /// </summary>
        public RewardParameters Parameters => _parameters;

        /// <summary>
        /// Event fired when reward parameters change.
        /// </summary>
        public event Action<RewardParameters> OnParametersChanged;

        /// <summary>
        /// Initializes the default reward tables.
        /// </summary>
        private void InitializeRewardTables()
        {
            // Enemy kill rewards by type
            _enemyKillRewards[EnemyType.Basic] = 10;
            _enemyKillRewards[EnemyType.Fast] = 15;
            _enemyKillRewards[EnemyType.Tank] = 25;
            _enemyKillRewards[EnemyType.Swift] = 20;
            _enemyKillRewards[EnemyType.Armored] = 30;
            _enemyKillRewards[EnemyType.Flying] = 35;
            _enemyKillRewards[EnemyType.Boss] = 100;

            // Wave completion bonuses
            for (int i = 1; i <= 50; i++)
            {
                var bonus = _parameters.BaseWaveBonus + (i * 5);
                _waveCompletionBonuses[$"wave_{i}"] = bonus;
            }

            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"RewardCalculator: Initialized {_enemyKillRewards.Count} enemy rewards and {_waveCompletionBonuses.Count} wave bonuses");
        }

        /// <summary>
        /// Calculates reward for killing an enemy.
        /// </summary>
        public int CalculateKillReward(EnemyType enemyType, int waveNumber = 1, float performanceScore = 1.0f)
        {
            if (!_enemyKillRewards.TryGetValue(enemyType, out var baseReward))
            {
                baseReward = _parameters.BaseKillReward;
            }

            var waveMultiplier = 1.0f + (waveNumber * 0.1f);
            var finalReward = (int)(baseReward * _parameters.DifficultyMultiplier * waveMultiplier * performanceScore);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"RewardCalculator: Kill reward for {enemyType} = {finalReward} (base: {baseReward}, wave: {waveMultiplier:F2}, performance: {performanceScore:F2})");
            return finalReward;
        }

        /// <summary>
        /// Calculates wave completion bonus.
        /// </summary>
        public int CalculateWaveBonus(int waveNumber, float performanceScore = 1.0f, int enemiesKilled = 0, int totalEnemies = 0)
        {
            var waveKey = $"wave_{waveNumber}";
            if (!_waveCompletionBonuses.TryGetValue(waveKey, out var baseBonus))
            {
                baseBonus = _parameters.BaseWaveBonus + (waveNumber * 5);
            }

            // Performance bonus based on completion time and lives lost
            var completionBonus = System.Math.Min(2.0f, performanceScore);
            var survivalBonus = totalEnemies > 0 ? (float)enemiesKilled / totalEnemies : 0.0f;
            var totalMultiplier = _parameters.DifficultyMultiplier * completionBonus * (1.0f + survivalBonus * 0.5f);

            var finalBonus = (int)(baseBonus * totalMultiplier);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"RewardCalculator: Wave bonus for wave {waveNumber} = {finalBonus} (base: {baseBonus}, performance: {completionBonus:F2}, survival: {survivalBonus:F2})");
            return finalBonus;
        }

        /// <summary>
        /// Calculates achievement reward.
        /// </summary>
        public int CalculateAchievementReward(string achievementId, int difficultyLevel = 1)
        {
            var baseReward = _parameters.BaseAchievementReward;
            var difficultyBonus = 1.0f + (difficultyLevel * 0.2f);
            var finalReward = (int)(baseReward * difficultyBonus * _parameters.DifficultyMultiplier);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"RewardCalculator: Achievement reward for {achievementId} = {finalReward} (base: {baseReward}, difficulty: {difficultyLevel})");
            return finalReward;
        }

        /// <summary>
        /// Calculates combo reward for multiple kills in quick succession.
        /// </summary>
        public int CalculateComboReward(int comboCount, int baseKillReward)
        {
            if (comboCount <= 1)
                return 0;

            var comboMultiplier = System.Math.Min(5.0f, 1.0f + (comboCount * 0.25f));
            var comboReward = (int)(baseKillReward * comboMultiplier * _parameters.ComboMultiplier);

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"RewardCalculator: Combo reward for {comboCount}x combo = {comboReward}");
            return comboReward;
        }

        /// <summary>
        /// Calculates performance bonus based on gameplay metrics.
        /// </summary>
        public float CalculatePerformanceBonus(float completionTime, float targetTime, int livesLost, int startingLives)
        {
            var timeBonus = completionTime <= targetTime ? 1.5f : 1.0f;
            var livesBonus = livesLost == 0 ? 1.2f : (1.0f - (float)livesLost / startingLives * 0.3f);
            var performanceBonus = timeBonus * livesBonus * _parameters.PerformanceBonus;

            Engine.Diagnostics.DebugLogger.LogDebug("DEBUG", $"RewardCalculator: Performance bonus = {performanceBonus:F2} (time: {timeBonus:F2}, lives: {livesBonus:F2})");
            return performanceBonus;
        }

        /// <summary>
        /// Updates reward parameters.
        /// </summary>
        public void UpdateParameters(RewardParameters parameters)
        {
            _parameters = parameters;
            OnParametersChanged?.Invoke(_parameters);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"RewardCalculator: Updated parameters - Difficulty: {parameters.DifficultyMultiplier:F2}, Wave: {parameters.WaveMultiplier:F2}");
        }

        /// <summary>
        /// Sets difficulty multiplier.
        /// </summary>
        public void SetDifficultyMultiplier(float multiplier)
        {
            _parameters.DifficultyMultiplier = System.Math.Max(0.1f, multiplier);
            OnParametersChanged?.Invoke(_parameters);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"RewardCalculator: Set difficulty multiplier to {_parameters.DifficultyMultiplier:F2}");
        }

        /// <summary>
        /// Gets enemy kill reward for specific enemy type.
        /// </summary>
        public int GetEnemyKillReward(EnemyType enemyType)
        {
            return _enemyKillRewards.TryGetValue(enemyType, out var reward) ? reward : _parameters.BaseKillReward;
        }

        /// <summary>
        /// Sets custom kill reward for enemy type.
        /// </summary>
        public void SetEnemyKillReward(EnemyType enemyType, int reward)
        {
            _enemyKillRewards[enemyType] = reward;
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"RewardCalculator: Set {enemyType} kill reward to {reward}");
        }

        /// <summary>
        /// Gets wave completion bonus for specific wave.
        /// </summary>
        public int GetWaveBonus(int waveNumber)
        {
            var waveKey = $"wave_{waveNumber}";
            return _waveCompletionBonuses.TryGetValue(waveKey, out var bonus) ? bonus : (_parameters.BaseWaveBonus + waveNumber * 5);
        }

        /// <summary>
        /// Resets all reward parameters to defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            _parameters = new RewardParameters();
            InitializeRewardTables();
            OnParametersChanged?.Invoke(_parameters);
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "RewardCalculator: Reset to default parameters");
        }
    }
}
