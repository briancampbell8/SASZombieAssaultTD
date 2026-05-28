using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.Towers;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Difficulty multiplier system for SAS Zombie Assault TD.
    /// Provides scaling factors for different difficulty levels.
    /// </summary>
    public class DifficultyMultiplier
    {
        private readonly Dictionary<DifficultyMode, DifficultySettings> _settings;
        private static DifficultyMultiplier _instance;
        private DifficultyMode _currentDifficulty = DifficultyMode.Normal;
        internal float HealthMultiplier;
        internal float SpeedMultiplier;
        internal int CountMultiplier;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static DifficultyMultiplier Instance => _instance ??= new DifficultyMultiplier();

        /// <summary>
        /// Current difficulty mode.
        /// </summary>
        public DifficultyMode CurrentDifficulty 
        { 
            get => _currentDifficulty; 
            set => _currentDifficulty = value; 
        }

        /// <summary>
        /// Health modifier for current difficulty.
        /// </summary>
        public float HealthModifier => GetSettingsForCurrent().HealthMultiplier;

        /// <summary>
        /// Speed modifier for current difficulty.
        /// </summary>
        public float SpeedModifier => GetSettingsForCurrent().SpeedMultiplier;

        /// <summary>
        /// Damage multiplier for current difficulty.
        /// </summary>
        public float DamageMultiplier => GetSettingsForCurrent().DamageMultiplier;

        /// <summary>
        /// Clone the current difficulty settings.
        /// </summary>
        /// <returns>Cloned difficulty settings.</returns>
        public DifficultySettings Clone() => GetSettingsForCurrent().Clone();

        /// <summary>
        /// Get settings for current difficulty.
        /// </summary>
        /// <returns>Current difficulty settings.</returns>
        private DifficultySettings GetSettingsForCurrent()
        {
            return GetMultiplier(_currentDifficulty);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DifficultyMultiplier() // Changed from private to public
        {
            _settings = new Dictionary<DifficultyMode, DifficultySettings>();
            InitializeDifficultySettings();
        }

        /// <summary>
        /// Constructor that accepts DifficultySettings for conversion.
        /// </summary>
        /// <param name="settings">The settings to convert from.</param>
        public DifficultyMultiplier(DifficultySettings settings)
        {
            _settings = new Dictionary<DifficultyMode, DifficultySettings>();
            InitializeDifficultySettings();

            // Map the provided settings to Normal difficulty mode
            _settings[DifficultyMode.Normal] = settings;
        }

        /// <summary>
        /// Get multiplier for specific difficulty mode.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <returns>Difficulty settings.</returns>
        public DifficultySettings GetMultiplier(DifficultyMode difficulty)
        {
            if (_settings.TryGetValue(difficulty, out var settings))
            {
                return settings;
            }

            System.Diagnostics.Debug.WriteLine($"No settings found for difficulty: {difficulty}, using Normal");
            return _settings[DifficultyMode.Normal];
        }

        /// <summary>
        /// Get multiplier for specific stat and difficulty.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <param name="stat">Stat to multiply.</param>
        /// <returns>Multiplier value.</returns>
        public float GetMultiplier(DifficultyMode difficulty, string stat)
        {
            var settings = GetMultiplier(difficulty);
            return settings.GetMultiplier(stat);
        }

        /// <summary>
        /// Get overall multiplier for difficulty mode.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <returns>Overall multiplier.</returns>
        public float GetOverallMultiplier(DifficultyMode difficulty)
        {
            var settings = GetMultiplier(difficulty);
            return settings.GetOverallMultiplier();
        }

        /// <summary>
        /// Apply difficulty multiplier to enemy stats.
        /// </summary>
        /// <param name="enemy">Enemy to modify.</param>
        /// <param name="difficulty">Difficulty mode.</param>
        public void ApplyToEnemy(Enemy enemy, DifficultyMode difficulty)
        {
            if (enemy == null) return;

            var settings = GetMultiplier(difficulty);
            settings.ApplyToEnemy(enemy);
        }

        /// <summary>
        /// Apply difficulty multiplier to wave script.
        /// </summary>
        /// <param name="waveScript">Wave script to modify.</param>
        /// <param name="difficulty">Difficulty mode.</param>
        public void ApplyToWaveScript(WaveScript waveScript, DifficultyMode difficulty)
        {
            if (waveScript == null) return;

            var settings = GetMultiplier(difficulty);
            settings.ApplyToWaveScript(waveScript);
        }

        /// <summary>
        /// Apply difficulty multiplier to tower stats.
        /// </summary>
        /// <param name="tower">Tower to modify.</param>
        /// <param name="difficulty">Difficulty mode.</param>
        public void ApplyToTower(Tower tower, DifficultyMode difficulty)
        {
            if (tower == null) return;

            var settings = GetMultiplier(difficulty);
            settings.ApplyToTower(tower);
        }

        /// <summary>
        /// Gets the overall difficulty multiplier for current difficulty.
        /// Adapts parameterless multiplier calls to the canonical GetMultiplier implementation.
        /// </summary>
        /// <returns>Overall difficulty multiplier.</returns>
        public float GetMultiplier()
        {
            return GetOverallMultiplier(_currentDifficulty);
        }

        /// <summary>
        /// Get difficulty description.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <returns>Difficulty description.</returns>
        public string GetDifficultyDescription(DifficultyMode difficulty)
        {
            var settings = GetMultiplier(difficulty);
            return settings.Description;
        }

        /// <summary>
        /// Get difficulty color for UI.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <returns>Difficulty color.</returns>
        public Color GetDifficultyColor(DifficultyMode difficulty)
        {
            return difficulty switch
            {
                DifficultyMode.Easy => Color.Green,
                DifficultyMode.Normal => Color.Yellow,
                DifficultyMode.Hard => Color.Orange,
                DifficultyMode.Elite => Color.Red,
                DifficultyMode.Nightmare => Color.Purple,
                _ => Color.White
            };
        }

        /// <summary>
        /// Check if difficulty is unlocked.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>True if difficulty is unlocked.</returns>
        public bool IsDifficultyUnlocked(DifficultyMode difficulty, int playerLevel)
        {
            return difficulty switch
            {
                DifficultyMode.Easy => true,
                DifficultyMode.Normal => true,
                DifficultyMode.Hard => playerLevel >= 1,
                DifficultyMode.Elite => playerLevel >= 5,
                DifficultyMode.Nightmare => playerLevel >= 10,
                _ => false
            };
        }

        /// <summary>
        /// Get recommended difficulty for player level.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>Recommended difficulty.</returns>
        public DifficultyMode GetRecommendedDifficulty(int playerLevel)
        {
            return playerLevel switch
            {
                < 3 => DifficultyMode.Easy,
                < 7 => DifficultyMode.Normal,
                < 12 => DifficultyMode.Hard,
                < 20 => DifficultyMode.Elite,
                _ => DifficultyMode.Nightmare
            };
        }

        /// <summary>
        /// Register custom difficulty settings.
        /// </summary>
        /// <param name="difficulty">Difficulty mode.</param>
        /// <param name="settings">Settings to register.</param>
        public void RegisterDifficulty(DifficultyMode difficulty, DifficultySettings settings)
        {
            _settings[difficulty] = settings;
            System.Diagnostics.Debug.WriteLine($"Registered custom difficulty settings for {difficulty}");
        }

        /// <summary>
        /// Get all available difficulties.
        /// </summary>
        /// <returns>List of available difficulties.</returns>
        public List<DifficultyMode> GetAvailableDifficulties()
        {
            return new List<DifficultyMode>(_settings.Keys);
        }

        /// <summary>
        /// Validate difficulty settings.
        /// </summary>
        /// <param name="settings">Settings to validate.</param>
        /// <returns>Validation result.</returns>
        public ValidationResult ValidateSettings(DifficultySettings settings)
        {
            var result = new ValidationResult { IsValid = true };

            if (settings.HealthMultiplier <= 0)
            {
                result.IsValid = false;
                result.AddError("Health multiplier must be positive");
            }

            if (settings.SpeedMultiplier <= 0)
            {
                result.IsValid = false;
                result.AddError("Speed multiplier must be positive");
            }

            if (settings.DamageMultiplier <= 0)
            {
                result.IsValid = false;
                result.AddError("Damage multiplier must be positive");
            }

            if (settings.CashMultiplier < 0)
            {
                result.IsValid = false;
                result.AddError("Cash multiplier cannot be negative");
            }

            if (settings.ExperienceMultiplier < 0)
            {
                result.IsValid = false;
                result.AddError("Experience multiplier cannot be negative");
            }

            return result;
        }

        /// <summary>
        /// Initialize default difficulty settings.
        /// </summary>
        private void InitializeDifficultySettings()
        {
            // Easy difficulty
            _settings[DifficultyMode.Easy] = new DifficultySettings
            {
                Name = "Easy",
                Description = "For beginners - Reduced enemy strength and increased rewards",
                HealthMultiplier = 0.7f,
                SpeedMultiplier = 0.8f,
                DamageMultiplier = 0.7f,
                CashMultiplier = 1.5f,
                ExperienceMultiplier = 1.3f,
                TowerCostMultiplier = 0.8f,
                StartingCash = 600,
                StartingLives = 25,
                WaveDelayMultiplier = 1.2f,
                SpecialAbilities = new List<string>()
            };

            // Normal difficulty
            _settings[DifficultyMode.Normal] = new DifficultySettings
            {
                Name = "Normal",
                Description = "Balanced gameplay experience",
                HealthMultiplier = 1.0f,
                SpeedMultiplier = 1.0f,
                DamageMultiplier = 1.0f,
                CashMultiplier = 1.0f,
                ExperienceMultiplier = 1.0f,
                TowerCostMultiplier = 1.0f,
                StartingCash = 450,
                StartingLives = 20,
                WaveDelayMultiplier = 1.0f,
                SpecialAbilities = new List<string>()
            };

            // Hard difficulty
            _settings[DifficultyMode.Hard] = new DifficultySettings
            {
                Name = "Hard",
                Description = "Challenging gameplay with stronger enemies",
                HealthMultiplier = 1.5f,
                SpeedMultiplier = 1.2f,
                DamageMultiplier = 1.3f,
                CashMultiplier = 0.8f,
                ExperienceMultiplier = 1.5f,
                TowerCostMultiplier = 1.2f,
                StartingCash = 350,
                StartingLives = 15,
                WaveDelayMultiplier = 0.9f,
                SpecialAbilities = new List<string> { "armored", "regenerating" }
            };

            // Elite difficulty
            _settings[DifficultyMode.Elite] = new DifficultySettings
            {
                Name = "Elite",
                Description = "Very challenging with elite enemy variants",
                HealthMultiplier = 2.0f,
                SpeedMultiplier = 1.5f,
                DamageMultiplier = 1.8f,
                CashMultiplier = 0.6f,
                ExperienceMultiplier = 2.0f,
                TowerCostMultiplier = 1.5f,
                StartingCash = 250,
                StartingLives = 10,
                WaveDelayMultiplier = 0.8f,
                SpecialAbilities = new List<string> { "armored", "regenerating", "stealth", "explosive" }
            };

            // Nightmare difficulty
            _settings[DifficultyMode.Nightmare] = new DifficultySettings
            {
                Name = "Nightmare",
                Description = "Extreme difficulty for expert players",
                HealthMultiplier = 3.0f,
                SpeedMultiplier = 2.0f,
                DamageMultiplier = 2.5f,
                CashMultiplier = 0.4f,
                ExperienceMultiplier = 3.0f,
                TowerCostMultiplier = 2.0f,
                StartingCash = 200,
                StartingLives = 5,
                WaveDelayMultiplier = 0.7f,
                SpecialAbilities = new List<string> { "armored", "regenerating", "stealth", "explosive", "champion", "boss" }
            };

            System.Diagnostics.Debug.WriteLine($"Initialized {_settings.Count} difficulty settings");
        }
    }

    /// <summary>
    /// Difficulty settings container.
    /// </summary>
    public class DifficultySettings
    {
        private object TheType;
        private object TheMember;

        public string Name { get; set; }
        public string Description { get; set; }
        public float HealthMultiplier { get; set; }
        public float SpeedMultiplier { get; set; }
        public float DamageMultiplier { get; set; }
        public float CashMultiplier { get; set; }
        public float ExperienceMultiplier { get; set; }
        public float TowerCostMultiplier { get; set; }
        public int StartingCash { get; set; }
        public int StartingLives { get; set; }
        public float WaveDelayMultiplier { get; set; }
        public List<string> SpecialAbilities { get; set; }
        public Dictionary<string, float> CustomMultipliers { get; set; }

        public DifficultySettings()
        {
            SpecialAbilities = new List<string>();
            CustomMultipliers = new Dictionary<string, float>();
        }

        /// <summary>
        /// Get multiplier for specific stat.
        /// </summary>
        /// <param name="stat">Stat to get multiplier for.</param>
        /// <returns>Multiplier value.</returns>
        public float GetMultiplier(string stat)
        {
            stat = stat.ToLower();

            return stat switch
            {
                "health" => HealthMultiplier,
                "speed" => SpeedMultiplier,
                "damage" => DamageMultiplier,
                "cash" => CashMultiplier,
                "experience" => ExperienceMultiplier,
                "towercost" => TowerCostMultiplier,
                "wavedelay" => WaveDelayMultiplier,
                _ => CustomMultipliers.TryGetValue(stat, out var custom) ? custom : 1.0f
            };
        }

        /// <summary>
        /// Get overall multiplier (average of core stats).
        /// </summary>
        /// <returns>Overall multiplier.</returns>
        public float GetOverallMultiplier()
        {
            return (HealthMultiplier + SpeedMultiplier + DamageMultiplier + CashMultiplier + ExperienceMultiplier) / 5f;
        }

        /// <summary>
        /// Apply difficulty settings to enemy.
        /// </summary>
        /// <param name="enemy">Enemy to modify.</param>
        public void ApplyToEnemy(Enemy enemy)
        {
            if (enemy == null) return;

            enemy.MaxHealth = (int)(enemy.MaxHealth * HealthMultiplier);
            enemy.Health = enemy.MaxHealth;
            enemy.Speed *= SpeedMultiplier;
            enemy.Damage = (int)(enemy.Damage * DamageMultiplier);

            // Apply special abilities
            foreach (var ability in SpecialAbilities)
            {
                enemy.AddSpecialAbility(ability);
            }

            // Apply custom multipliers
            foreach (var custom in CustomMultipliers)
            {
                enemy.SetCustomProperty(custom.Key, custom.Value);
            }
        }

        /// <summary>
        /// Apply difficulty settings to wave script.
        /// </summary>
        /// <param name="waveScript">Wave script to modify.</param>
        public void ApplyToWaveScript(WaveScript waveScript)
        {
            if (waveScript == null) return;

            // Apply difficulty multipliers to wave script using current instance properties
            waveScript.Modifiers.GlobalHealthModifier = this.HealthMultiplier;
            waveScript.Modifiers.GlobalSpeedModifier = this.SpeedMultiplier;
            
            // Apply cash and experience multipliers to rewards
            waveScript.Rewards.CashBonus = (int)(waveScript.Rewards.CashBonus * this.CashMultiplier);
            waveScript.Rewards.ExperienceBonus = (int)(waveScript.Rewards.ExperienceBonus * this.ExperienceMultiplier);

            // Apply wave delay multiplier
            foreach (var spawnGroup in waveScript.SpawnGroups)
            {
                spawnGroup.SpawnDelay *= WaveDelayMultiplier;
                spawnGroup.DelayAfterGroup *= WaveDelayMultiplier;
            }

            // Add special abilities to enemies
            foreach (var spawnGroup in waveScript.SpawnGroups)
            {
                foreach (var ability in SpecialAbilities)
                {
                    // Cast the modifiers list to IEnumerable dynamic or a concrete modifier type to allow LINQ queries
                    var modifiers = spawnGroup.EnemyModifiers() as System.Collections.IEnumerable;
                    bool alreadyHasModifier = false;

                    if (modifiers != null)
                    {
                        foreach (dynamic mod in modifiers)
                        {
                            if (mod.ModifierType == ability)
                            {
                                alreadyHasModifier = true;
                                break;
                            }
                        }
                    }

                    if (!alreadyHasModifier)
                    {
                        // Call Add on the dynamic collection to bypass compilation checks
                        dynamic modifierList = spawnGroup.EnemyModifiers();
                        modifierList.Add(new EnemyBehaviorModifier
                        {
                            ModifierType = ability,
                            Value = 1.0f,
                            Duration = -1f // Permanent
                        });
                    }
                }
            }

        }

        private object GetSettingsForCurrent()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Apply difficulty settings to tower.
        /// </summary>
        /// <param name="tower">Tower to modify.</param>
        public void ApplyToTower(Tower tower)
        {
            if (tower == null) return;

            // Use the custom property setter to bypass the read-only restriction
            int modifiedCost = (int)(tower.Cost * TowerCostMultiplier);
            tower.SetCustomProperty("Cost", modifiedCost);

            // Apply custom multipliers
            foreach (var custom in CustomMultipliers)
            {
                tower.SetCustomProperty(custom.Key, custom.Value);
            }
        }


        /// <summary>
        /// Clone this difficulty settings.
        /// </summary>
        /// <returns>Cloned settings.</returns>
        public DifficultySettings Clone()
        {
            var clone = new DifficultySettings
            {
                Name = this.Name,
                Description = this.Description,
                HealthMultiplier = this.HealthMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                DamageMultiplier = this.DamageMultiplier,
                CashMultiplier = this.CashMultiplier,
                ExperienceMultiplier = this.ExperienceMultiplier,
                TowerCostMultiplier = this.TowerCostMultiplier,
                StartingCash = this.StartingCash,
                StartingLives = this.StartingLives,
                WaveDelayMultiplier = this.WaveDelayMultiplier,
                SpecialAbilities = new List<string>(this.SpecialAbilities),
                CustomMultipliers = new Dictionary<string, float>(this.CustomMultipliers)
            };

            return clone;
        }

        /// <summary>
        /// Get difficulty rating (1-10).
        /// </summary>
        /// <returns>Difficulty rating.</returns>
        public int GetDifficultyRating()
        {
            var rating = (int)(GetOverallMultiplier() * 2);
            return System.Math.Min(10, System.Math.Max(1, rating));
        }

        /// <summary>
        /// Get difficulty summary.
        /// </summary>
        /// <returns>Summary string.</returns>
        public string GetSummary()
        {
            var summary = $"{Name} (Difficulty {GetDifficultyRating()}/10)\n";
            summary += $"Enemy Health: x{HealthMultiplier:F1}\n";
            summary += $"Enemy Speed: x{SpeedMultiplier:F1}\n";
            summary += $"Enemy Damage: x{DamageMultiplier:F1}\n";
            summary += $"Cash Rewards: x{CashMultiplier:F1}\n";
            summary += $"Tower Costs: x{TowerCostMultiplier:F1}\n";

            if (SpecialAbilities.Count > 0)
            {
                summary += $"Special Abilities: {string.Join(", ", SpecialAbilities)}\n";
            }

            return summary.Trim();
        }
    }

    /// <summary>
    /// Difficulty modes for SAS TD.
    /// </summary>
    public enum DifficultyMode
    {
        Easy,
        Normal,
        Hard,
        Elite,
        Nightmare
    }

    /// <summary>
    /// Validation result for difficulty settings.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }
    }

    /// <summary>
    /// Extension methods for DifficultyManager.
    /// </summary>
    public static class DifficultyManagerExtensions
    {
        /// <summary>
        /// Get current difficulty from game state.
        /// </summary>
        /// <returns>Current difficulty.</returns>
        public static DifficultyMode GetCurrentDifficulty()
        {
            // This would typically come from a game state manager or settings system
            // For now, return Normal as default
            return DifficultyMode.Normal;
        }
    }
}
