using SASZombieAssaultTD.Engine.Economy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SASZombieAssaultTD.Engine.Audio;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    /// <summary>
    /// Player level progression for SAS Zombie Assault TD.
    /// Manages experience points, level advancement, and rewards.
    /// </summary>
    public class PlayerLevel
    {
        // Level properties
        private int _currentLevel;
        private int _currentExperience;
        private int _experienceToNextLevel;
        private int _totalExperienceEarned;
        private bool _isInitialized;
        private static PlayerLevel _instance;

        // Level configuration
        private readonly Dictionary<int, LevelData> _levelData;
        private readonly List<PlayerReward> _levelUpRewards;
        private readonly List<PlayerUnlock> _levelUpUnlocks;

        // Events
        public event Action<int> OnLevelUp;
        public event Action<int, int> OnExperienceGained;
        public event Action<PlayerReward> OnRewardUnlocked;
        public event Action<PlayerUnlock> OnUnlockUnlocked;
        public event Action OnMaxLevelReached;

        // Properties
        public int CurrentLevel => _currentLevel;
        public int CurrentExperience => _currentExperience;
        public int ExperienceToNextLevel => _experienceToNextLevel;
        public int TotalExperienceEarned => _totalExperienceEarned;
        public float LevelProgress => (float)_currentExperience / _experienceToNextLevel;
        public bool IsMaxLevel => _currentLevel >= GetMaxLevel();
        public bool IsInitialized => _isInitialized;
        public static PlayerLevel Instance => _instance ??= new PlayerLevel();

        // Singleton
        private PlayerLevel()
        {
            _currentLevel = 1;
            _currentExperience = 0;
            _experienceToNextLevel = 100;
            _totalExperienceEarned = 0;
            _isInitialized = false;

            InitializeLevelData();
            InitializeRewards();
            InitializeUnlocks();
        }

        /// <summary>
        /// Initialize the player level system.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Player Level System");

            try
            {
                // Load saved progress
                LoadProgress();

                // Set up event subscriptions
                SetupEventSubscriptions();

                _isInitialized = true;
                Console.WriteLine($"Player Level System initialized at level {_currentLevel} with {_currentExperience} XP");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Player Level System: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add experience points.
        /// </summary>
        /// <param name="experience">Experience points to add.</param>
        /// <param name="source">Source of experience (kill, wave, etc.).</param>
        public void AddExperience(int experience, string source = "Unknown")
        {
            if (experience <= 0) return;

            try
            {
                var previousLevel = _currentLevel;
                var previousXP = _currentExperience;
                _totalExperienceEarned += experience;
                _currentExperience += experience;

                // Check for level up
                while (_currentExperience >= _experienceToNextLevel && _currentLevel < GetMaxLevel())
                {
                    _currentExperience -= _experienceToNextLevel;
                    _currentLevel++;
                    _experienceToNextLevel = CalculateExperienceForNextLevel(_currentLevel);

                    // Trigger level up events
                    OnLevelUp?.Invoke(_currentLevel);
                    OnExperienceGained?.Invoke(_currentLevel, _experienceToNextLevel);

                    // Check for unlocks
                    CheckLevelUnlocks(_currentLevel);

                    Console.WriteLine($"Level up! Now level {_currentLevel}");
                }

                // Trigger experience gained event
                OnExperienceGained?.Invoke(_currentLevel, experience);

                Console.WriteLine($"Added {experience} XP from {source}. Total: {_totalExperienceEarned}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding experience: {ex.Message}");
            }
        }

        /// <summary>
        /// Set player level directly.
        /// </summary>
        /// <param name="level">Level to set.</param>
        /// <param name="experience">Experience points for the level.</param>
        public void SetLevel(int level, int experience = 0)
        {
            if (level < 1 || level > GetMaxLevel()) return;

            try
            {
                var wasLevelUp = level > _currentLevel;
                _currentLevel = level;
                _currentExperience = experience;
                _experienceToNextLevel = CalculateExperienceForNextLevel(level);

                if (wasLevelUp)
                {
                    OnLevelUp?.Invoke(_currentLevel);
                    CheckLevelUnlocks(_currentLevel);
                }

                SaveProgress();

                Console.WriteLine($"Set level to {_currentLevel} with {_currentExperience} XP");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting level: {ex.Message}");
            }
        }

        /// <summary>
        /// Get experience required for next level.
        /// </summary>
        /// <param name="level">Current level.</param>
        /// <returns>Experience needed for next level.</returns>
        public int GetExperienceForNextLevel(int level)
        {
            return _levelData.TryGetValue(level, out var data) ? data.ExperienceRequired : 100 * level;
        }

        /// <summary>
        /// Get level data.
        /// </summary>
        /// <param name="level">Level to get data for.</param>
        /// <returns>Level data, or null if not found.</returns>
        public LevelData GetLevelData(int level)
        {
            return _levelData.TryGetValue(level, out var data) ? data : null;
        }

        /// <summary>
        /// Get all available rewards for a level.
        /// </summary>
        /// <param name="level">Level to get rewards for.</param>
        /// <returns>List of available rewards.</returns>
        public IReadOnlyList<PlayerReward> GetAvailableRewards(int level)
        {
            return _levelUpRewards.Where(r => r.RequiredLevel <= level).ToList();
        }

        /// <summary>
        /// Get all available unlocks for a level.
        /// </summary>
        /// <param name="level">Level to get unlocks for.</param>
        /// <returns>List of available unlocks.</returns>
        public IReadOnlyList<PlayerUnlock> GetAvailableUnlocks(int level)
        {
            return _levelUpUnlocks.Where(u => u.RequiredLevel <= level).ToList();
        }

        /// <summary>
        /// Get player statistics.
        /// </summary>
        /// <returns>Player level statistics.</returns>
        public PlayerLevelStatistics GetStatistics()
        {
            return new PlayerLevelStatistics
            {
                CurrentLevel = _currentLevel,
                CurrentExperience = _currentExperience,
                ExperienceToNextLevel = _experienceToNextLevel,
                TotalExperienceEarned = _totalExperienceEarned,
                LevelProgress = LevelProgress,
                LevelsGained = CalculateLevelsGained(),
                TimeToNextLevel = CalculateTimeToNextLevel(),
                UnlockedRewards = _levelUpRewards.Count(r => r.RequiredLevel <= _currentLevel),
                UnlockedUnlocks = _levelUpUnlocks.Count(u => u.RequiredLevel <= _currentLevel),
                IsMaxLevel = IsMaxLevel
            };
        }

        /// <summary>
        /// Reset player progress.
        /// </summary>
        public void ResetProgress()
        {
            try
            {
                _currentLevel = 1;
                _currentExperience = 0;
                _experienceToNextLevel = 100;
                _totalExperienceEarned = 0;

                SaveProgress();

                Console.WriteLine("Player progress reset to level 1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting player progress: {ex.Message}");
            }
        }

        /// <summary>
        /// Save player progress.
        /// </summary>
        /// <returns>True if saved successfully.</returns>
        public bool SaveProgress()
        {
            try
            {
                var saveData = new PlayerLevelSaveData
                {
                    CurrentLevel = _currentLevel,
                    CurrentExperience = _currentExperience,
                    TotalExperienceEarned = _totalExperienceEarned,
                    UnlockedRewards = _levelUpUnlocks.Where(u => u.RequiredLevel <= _currentLevel).Select(u => u.Id).ToList(),
                    UnlockedUnlocksData = _levelUpUnlocks.Where(u => u.RequiredLevel <= _currentLevel).ToDictionary(u => u.Id, u => u.Serialize()),
                    LastSaved = DateTime.Now
                };

                var json = JsonSerializer.Serialize(saveData);
                var savePath = Path.Combine("Data", "Player", "level.json");
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                File.WriteAllText(savePath, json);

                Console.WriteLine($"Player progress saved to {savePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving player progress: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load player progress.
        /// </summary>
        /// <returns>True if loaded successfully.</returns>
        public bool LoadProgress()
        {
            try
            {
                var savePath = Path.Combine("Data", "Player", "level.json");
                if (!File.Exists(savePath))
                {
                    Console.WriteLine("No saved player progress found");
                    return false;
                }

                var json = File.ReadAllText(savePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var saveData = JsonSerializer.Deserialize<PlayerLevelSaveData>(json, options);
                if (saveData == null) return false;

                _currentLevel = saveData.CurrentLevel;
                _currentExperience = saveData.CurrentExperience;
                _totalExperienceEarned = saveData.TotalExperienceEarned;

                // Restore unlocked rewards
                if (saveData.UnlockedRewardsData != null)
                {
                    foreach (var kvp in saveData.UnlockedRewardsData)
                    {
                        var unlock = _levelUpUnlocks.FirstOrDefault(u => u.Id == kvp.Key);
                        if (unlock != null)
                        {
                            unlock.IsUnlocked = (bool)kvp.Value;
                        }
                    }
                }

                Console.WriteLine($"Player progress loaded from {savePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player progress: {ex.Message}");
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// Initialize level data.
        /// </summary>
        /// 

        private void InitializeLevelData()
        {
            _levelData.Clear(); // instead of = new Dictionary<>()

            for (int level = 1; level <= GetMaxLevel(); level++)
            {
                var experienceRequired = CalculateExperienceForNextLevel(level);
                var levelData = new LevelData
                {
                    Level = level,
                    ExperienceRequired = experienceRequired,
                    Title = GetLevelTitle(level),
                    Description = GetLevelDescription(level),
                    IconPath = GetLevelIconPath(level),
                    Rewards = GetLevelRewards(level),
                    Unlocks = GetLevelUnlocks(level)
                };

                _levelData[level] = levelData;
            }
        }

        /// <summary>
        /// Initialize rewards.
        /// </summary>
        private void InitializeRewards()
        {
            _levelUpRewards.Clear(); // instead of = new List<PlayerReward>()

            // Add level up rewards
            _levelUpRewards.Add(new PlayerReward
            {
                Id = "level_up_cash",
                Name = "Cash Bonus",
                Description = "Bonus cash for reaching new level",
                Type = RewardType.Cash,
                Amount = 50,
                RequiredLevel = 2
            });

            _levelUpRewards.Add(new PlayerReward
            {
                Id = "level_up_tower_slot",
                Name = "Tower Slot",
                Description = "Additional tower slot unlocked",
                Type = RewardType.TowerSlot,
                Amount = 1,
                RequiredLevel = 5
            });

            _levelUpRewards.Add(new PlayerReward
            {
                Id = "level_up_upgrade_discount",
                Name = "Upgrade Discount",
                Description = "20% discount on tower upgrades",
                Type = RewardType.UpgradeDiscount,
                Amount = 20,
                RequiredLevel = 10
            });
        }

        /// <summary>
        /// Initialize unlocks.
        /// </summary>
        private void InitializeUnlocks()
        {
            _levelUpUnlocks.Clear(); // instead of = new List<PlayerUnlock>()

            // Add tower unlocks

            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "vickers_turret",
                Name = "Vickers Turret",
                Description = "Unlocks Vickers machine gun turret",
                Type = UnlockType.Tower,
                RequiredLevel = 1
            });

            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "mgl_turret",
                Name = "MGL Turret",
                Description = "Unlocks MGL grenade launcher turret",
                Type = UnlockType.Tower,
                RequiredLevel = 3
            });

            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "special_turret",
                Name = "Special Turret",
                Description = "Unlocks special elemental turret",
                Type = UnlockType.Tower,
                RequiredLevel = 5
            });

            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "sniper_sas",
                Name = "Sniper SAS",
                Description = "Unlocks elite sniper unit",
                Type = UnlockType.Tower,
                RequiredLevel = 7
            });

            // Add ability unlocks
            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "double_damage",
                Name = "Double Damage",
                Description = "All towers deal double damage",
                Type = UnlockType.Ability,
                RequiredLevel = 10
            });

            _levelUpUnlocks.Add(new PlayerUnlock
            {
                Id = "armor_piercing",
                Name = "Armor Piercing",
                Description = "All towers ignore enemy armor",
                Type = UnlockType.Ability,
                RequiredLevel = 15
            });
        }

        /// <summary>
        /// Calculate experience for next level.
        /// </summary>
        private int CalculateExperienceForNextLevel(int level)
        {
            // Exponential experience curve
            return (int)(100 * System.Math.Pow(1.5, level - 1));
        }

        /// <summary>
        /// Get level title.
        /// </summary>
        private string GetLevelTitle(int level)
        {
            return level switch
            {
                1 => "Recruit",
                2 => "Private",
                3 => "Corporal",
                4 => "Sergeant",
                5 => "Lieutenant",
                6 => "Captain",
                7 => "Major",
                8 => "Colonel",
                9 => "General",
                10 => "Field Marshal",
                _ => "Unknown Rank"
            };
        }

        /// <summary>
        /// Get level description.
        /// </summary>
        private string GetLevelDescription(int level)
        {
            return level switch
            {
                1 => "New to the force, learning the basics.",
                2 => "Gaining experience and trust.",
                3 => "Proven in combat, ready for leadership.",
                4 => "Veteran soldier with tactical knowledge.",
                5 => "Elite soldier, respected by the team.",
                6 => "Small squad leader material.",
                7 => "Company leadership potential.",
                8 => "Battalion command material.",
                9 => "Brigadier general material.",
                10 => "Field Marshal - Legend of the SAS.",
                _ => "Elite soldier of unknown rank."
            };
        }

        /// <summary>
        /// Get level icon path.
        /// </summary>
        private string GetLevelIconPath(int level)
        {
            return $"UI/Icons/Level_{level}.png";
        }

        /// <summary>
        /// Get maximum level.
        /// </summary>
        private int GetMaxLevel()
        {
            return 10;
        }

        /// <summary>
        /// Get rewards for level.
        /// </summary>
        private List<PlayerReward> GetLevelRewards(int level)
        {
            return _levelUpRewards.Where(r => r.RequiredLevel == level).ToList();
        }

        /// <summary>
        /// Get unlocks for level.
        /// </summary>
        private List<PlayerUnlock> GetLevelUnlocks(int level)
        {
            return _levelUpUnlocks.Where(u => u.RequiredLevel == level).ToList();
        }

        /// <summary>
        /// Check for level unlocks.
        /// </summary>
        private void CheckLevelUnlocks(int level)
        {
            var availableUnlocks = GetAvailableUnlocks(level);
            foreach (var unlock in availableUnlocks)
            {
                if (!_levelUpUnlocks.Any(u => u.Id == unlock.Id && u.IsUnlocked))
                {
                    unlock.IsUnlocked = true;
                    OnUnlockUnlocked?.Invoke(unlock);

                    // Convert unlock to reward for reward event
                    var reward = new PlayerReward
                    {
                        Type = RewardType.Ability, // or appropriate type
                        Amount = 1,
                        Description = unlock.Description ?? $"Unlocked: {unlock.Name}"
                    };
                    OnRewardUnlocked?.Invoke(reward);

                    // Play unlock sound
                    AudioSystem.PlaySound("unlock");

                    Console.WriteLine($"Unlocked: {unlock.Name}");
                }
            }
        }

        /// <summary>
        /// Set up event subscriptions.
        /// </summary>
        private void SetupEventSubscriptions()
        {
            // Economy events
            EconomyManager.OnCashChanged += OnCashChanged;
        }

        /// <summary>
        /// Handle cash changed event.
        /// </summary>
        private void OnCashChanged(int newCash)
        {
            // Award experience for cash milestones
            if (newCash > 0 && newCash % 500 == 0)
            {
                var experienceBonus = newCash / 500;
                AddExperience(experienceBonus, "Cash Milestone");
            }
        }

        /// <summary>
        /// Calculate levels gained.
        /// </summary>
        private int CalculateLevelsGained()
        {
            // This would be calculated from saved data
            // For now, return current level - 1 (starting from level 1)
            return System.Math.Max(0, _currentLevel - 1);
        }

        /// <summary>
        /// Calculate time to next level.
        /// </summary>
        private TimeSpan CalculateTimeToNextLevel()
        {
            // This would be calculated from saved data
            // For now, return a reasonable default
            return TimeSpan.FromHours(2);
        }

        #endregion
    }

    /// <summary>
    /// Level data for progression.
    /// </summary>
    public class LevelData
    {
        public int Level { get; set; }
        public int ExperienceRequired { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public List<PlayerReward> Rewards { get; set; }
        public List<PlayerUnlock> Unlocks { get; set; }
    }

    /// <summary>
    /// Player reward for level progression.
    /// </summary>
    public class PlayerReward
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RewardType Type { get; set; }
        public int Amount { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsUnlocked { get; set; }

        public object Serialize()
        {
            return new { Id, Name, Description, Type, Amount, RequiredLevel, IsUnlocked };
        }
    }

    /// <summary>
    /// Player unlock for progression.
    /// </summary>
    public class PlayerUnlock
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UnlockType Type { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsUnlocked { get; set; }

        public object Serialize()
        {
            return new { Id, Name, Description, Type, RequiredLevel, IsUnlocked };
        }
    }

    /// <summary>
    /// Player level save data.
    /// </summary>
    public class PlayerLevelSaveData
    {
        public int CurrentLevel { get; set; }
        public int CurrentExperience { get; set; }
        public int TotalExperienceEarned { get; set; }
        public List<string> UnlockedRewards { get; set; }
        public Dictionary<string, object> UnlockedRewardsData { get; set; }
        public Dictionary<string, object> UnlockedUnlocksData { get; set; }
        public DateTime LastSaved { get; set; }
    }

    /// <summary>
    /// Player level statistics.
    /// </summary>
    public class PlayerLevelStatistics
    {
        public int CurrentLevel { get; set; }
        public int CurrentExperience { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public int TotalExperienceEarned { get; set; }
        public float LevelProgress { get; set; }
        public int LevelsGained { get; set; }
        public TimeSpan TimeToNextLevel { get; set; }
        public int UnlockedRewards { get; set; }
        public int UnlockedUnlocks { get; set; }
        public bool IsMaxLevel { get; set; }

        public override string ToString()
        {
            return $"Player Level Statistics:\n" +
                   $"Current Level: {CurrentLevel}\n" +
                   $"Current Experience: {CurrentExperience}/{ExperienceToNextLevel}\n" +
                   $"Progress: {LevelProgress:P1}\n" +
                   $"Levels Gained: {LevelsGained}\n" +
                   $"Unlocked Rewards: {UnlockedRewards}\n" +
                   $"Unlocked Unlocks: {UnlockedUnlocks}\n" +
                   $"Max Level: {IsMaxLevel}";
        }
    }

    /// <summary>
    /// Reward type enumeration.
    /// </summary>
    public enum RewardType
    {
        Cash,
        TowerSlot,
        UpgradeDiscount,
        Experience,
        Ability
    }

    /// <summary>
    /// Unlock type enumeration.
    /// </summary>
    public enum UnlockType
    {
        Tower,
        Ability
    }
}
