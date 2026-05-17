using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using SASZombieAssaultTD.Engine.Gameplay;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    /// <summary>
    /// Milestone category for level progression.
    /// </summary>
    public enum MilestoneCategory
    {
        Level,
        Combat,
        Wave,
        Defense,
        Speed,
        FirstTower,
        Wave10,
        Wave25,
        Wave50,
        Wave100,
        BossDefeated,
        PerfectWave,
        SpeedRun,
        SurvivalExpert,
        TowerMaster,
        ZombieSlayer,
        ResourceCollector,
        AchievementHunter
    }

    /// <summary>
    /// Achievement category for level progression.
    /// </summary>
    public enum AchievementCategory
    {
        Combat,
        Survival,
        Economy,
        Tower,
        Wave,
        Defense,
        Speed,
        Special,
        Hidden
    }

    /// <summary>
    /// Event category for level progression events.
    /// </summary>
    public enum EventCategory
    {
        Milestone,
        Achievement,
        LevelUp,
        Progress,
        Bonus,
        Progression,
        Notification,
        System
    }

    /// <summary>
    /// Level progression manager for SAS Zombie Assault TD.
    /// Manages player advancement, milestone tracking, and progression rewards.
    /// </summary>
    public class LevelProgression
    {
        private readonly List<ProgressionMilestone> _milestones;
        private readonly Dictionary<string, ProgressionAchievement> _achievements;
        private readonly List<ProgressionEvent> _events;
        private bool _isInitialized;
        private static LevelProgression _instance;

        // Events
        public event Action<ProgressionMilestone> OnMilestoneReached;
        public event Action<ProgressionAchievement> OnAchievementUnlocked;
        public event Action<ProgressionEvent> OnProgressionEvent;
        public event Action OnAllMilestonesCompleted;

        // Properties
        public bool IsInitialized => _isInitialized;
        public int TotalMilestones => _milestones.Count;
        public int TotalAchievements => _achievements.Count;
        public int CompletedMilestones => _milestones.Count(m => m.IsCompleted);
        public int UnlockedAchievements => _achievements.Count(a => a.IsUnlocked);
        public IReadOnlyList<ProgressionMilestone> AllMilestones => _milestones;
        public IReadOnlyDictionary<string, ProgressionAchievement> AllAchievements => _achievements;
        public static LevelProgression Instance => _instance ??= new LevelProgression();

        // Singleton
        private LevelProgression()
        {
            _milestones = new List<ProgressionMilestone>();
            _achievements = new Dictionary<string, ProgressionAchievement>();
            _events = new List<ProgressionEvent>();
            _isInitialized = false;

            InitializeMilestones();
            InitializeAchievements();
            InitializeEvents();
        }

        /// <summary>
        /// Initialize the level progression system.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            Console.WriteLine("Initializing Level Progression System");

            try
            {
                // Subscribe to player level events
                if (PlayerLevel.Instance != null)
                {
                    PlayerLevel.Instance.OnLevelUp += OnPlayerLevelUp;
                    PlayerLevel.Instance.OnExperienceGained += OnExperienceGained;
                    PlayerLevel.Instance.OnRewardUnlocked += OnRewardUnlocked;
                    PlayerLevel.Instance.OnUnlockUnlocked += OnUnlockUnlocked;
                    PlayerLevel.Instance.OnMaxLevelReached += OnMaxLevelReached;
                }

                _isInitialized = true;
                OnAllMilestonesCompleted?.Invoke();

                Console.WriteLine($"Level Progression System initialized with {_milestones.Count} milestones and {_achievements.Count} achievements");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Level Progression System: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add a custom milestone.
        /// </summary>
        /// <param name="milestone">Milestone to add.</param>
        /// <returns>True if milestone was added.</returns>
        public bool AddMilestone(ProgressionMilestone milestone)
        {
            if (milestone == null) return false;
            if (_milestones.Any(m => m.Id == milestone.Id)) return false;

            try
            {
                _milestones.Add(milestone);
                Console.WriteLine($"Added milestone: {milestone.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding milestone: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Add a custom achievement.
        /// </summary>
        /// <param name="achievement">Achievement to add.</param>
        /// <returns>True if achievement was added.</returns>
        public bool AddAchievement(ProgressionAchievement achievement)
        {
            if (achievement == null) return false;
            if (_achievements.ContainsKey(achievement.Id)) return false;

            try
            {
                _achievements[achievement.Id] = achievement;
                Console.WriteLine($"Added achievement: {achievement.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding achievement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Add a progression event.
        /// </summary>
        /// <param name="progressionEvent">Event to add.</param>
        /// <returns>True if event was added.</returns>
        public bool AddProgressionEvent(ProgressionEvent progressionEvent)
        {
            if (progressionEvent == null) return false;

            try
            {
                _events.Add(progressionEvent);
                Console.WriteLine($"Added progression event: {progressionEvent.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding progression event: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if milestone is completed.
        /// </summary>
        /// <param name="milestoneId">Milestone ID to check.</param>
        /// <returns>True if milestone is completed.</returns>
        public bool IsMilestoneCompleted(string milestoneId)
        {
            var milestone = _milestones.FirstOrDefault(m => m.Id == milestoneId);
            return milestone?.IsCompleted ?? false;
        }

        /// <summary>
        /// Check if achievement is unlocked.
        /// </summary>
        /// <param name="achievementId">Achievement ID to check.</param>
        /// <returns>True if achievement is unlocked.</returns>
        public bool IsAchievementUnlocked(string achievementId)
        {
            return _achievements.TryGetValue(achievementId, out var achievement) && achievement.IsUnlocked;
        }

        /// <summary>
        /// Get completed milestones.
        /// </summary>
        /// <returns>List of completed milestones.</returns>
        public IReadOnlyList<ProgressionMilestone> GetCompletedMilestones()
        {
            return _milestones.Where(m => m.IsCompleted).ToList();
        }

        /// <summary>
        /// Get available achievements.
        /// </summary>
        /// <returns>List of available achievements.</returns>
        public IReadOnlyList<ProgressionAchievement> GetAvailableAchievements()
        {
            return _achievements.Values.Where(a => a.IsUnlocked).ToList();
        }

        /// <summary>
        /// Get progression statistics.
        /// </summary>
        /// <returns>Progression statistics.</returns>
        public ProgressionStatistics GetStatistics()
        {
            return new ProgressionStatistics
            {
                TotalMilestones = _milestones.Count,
                CompletedMilestones = CompletedMilestones,
                TotalAchievements = _achievements.Count,
                UnlockedAchievements = UnlockedAchievements,
                CompletionPercentage = _milestones.Count > 0 ? (float)CompletedMilestones / _milestones.Count : 0f,
                RecentMilestones = _milestones.Where(m => m.IsCompleted).Take(5).ToList(),
                RecentAchievements = _achievements.Values.Where(a => a.IsUnlocked && a.UnlockedDate.HasValue).OrderByDescending(a => a.UnlockedDate).Take(10).ToList(),
                TotalEvents = _events.Count,
                AverageMilestonesPerSession = CalculateAverageMilestonesPerSession()
            };
        }

        /// <summary>
        /// Get milestones by category.
        /// </summary>
        /// <param name="category">Milestone category.</param>
        /// <returns>List of milestones in category.</returns>
        public IReadOnlyList<ProgressionMilestone> GetMilestonesByCategory(MilestoneCategory category)
        {
            return _milestones.Where(m => m.Category == category).ToList();
        }

        /// <summary>
        /// Get achievements by category.
        /// </summary>
        /// <param name="category">Achievement category.</param>
        /// <returns>List of achievements in category.</returns>
        public IReadOnlyList<ProgressionAchievement> GetAchievementsByCategory(AchievementCategory category)
        {
            return _achievements.Values.Where(a => a.Category == category).ToList();
        }

        /// <summary>
        /// Reset all progression.
        /// </summary>
        public void ResetProgression()
        {
            try
            {
                // Reset milestones
                foreach (var milestone in _milestones)
                {
                    milestone.IsCompleted = false;
                    milestone.CompletionDate = null;
                }

                // Reset achievements
                foreach (var achievement in _achievements.Values)
                {
                    achievement.IsUnlocked = false;
                    achievement.UnlockedDate = null;
                    achievement.Progress = 0f;
                }

                // Clear events
                _events.Clear();

                Console.WriteLine("Level progression reset");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting progression: {ex.Message}");
            }
        }

        /// <summary>
        /// Save progression data.
        /// </summary>
        /// <returns>True if saved successfully.</returns>
        public bool SaveProgression()
        {
            try
            {
                var saveData = new ProgressionSaveData
                {
                    CompletedMilestones = _milestones.Where(m => m.IsCompleted).Select(m => m.Id).ToList(),
                    UnlockedAchievements = _achievements.Values.Where(a => a.IsUnlocked).Select(a => a.Id).ToList(),
                    MilestoneData = _milestones.ToDictionary<string, string>(m => m.Id, m => m.Serialize()),
                    AchievementData = _achievements.ToDictionary<string, string>(a => a.Id, a => a.Serialize()),
                    EventData = _events.Select(e => e.Serialize()).ToList(),
                    LastSaved = DateTime.Now
                };

                var json = JsonSerializer.Serialize(saveData);
                var savePath = Path.Combine("Data", "Progression", "progression.json");
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                File.WriteAllText(savePath, json);

                Console.WriteLine($"Progression data saved to {savePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving progression: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load progression data.
        /// </summary>
        /// <returns>True if loaded successfully.</returns>
        public bool LoadProgression()
        {
            try
            {
                var savePath = Path.Combine("Data", "Progression", "progression.json");
                if (!File.Exists(savePath))
                {
                    Console.WriteLine("No saved progression data found");
                    return false;
                }

                var json = File.ReadAllText(savePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var saveData = JsonSerializer.Deserialize<ProgressionSaveData>(json, options);
                if (saveData == null) return false;

                // Restore milestones
                foreach (var kvp in saveData.MilestoneData)
                {
                    if (_milestones.Any(m => m.Id == kvp.Key))
                    {
                        var milestone = _milestones.First(m => m.Id == kvp.Key);
                        milestone.Deserialize(kvp.Value);
                    }
                }

                // Restore achievements
                foreach (var kvp in saveData.AchievementData)
                {
                    if (_achievements.ContainsKey(kvp.Key))
                    {
                        var achievement = _achievements[kvp.Key];
                        achievement.Deserialize(kvp.Value);
                    }
                }

                // Restore events
                _events.Clear();
                foreach (var eventData in saveData.EventData)
                {
                    _events.Add(ProgressionEvent.Deserialize(eventData));
                }

                Console.WriteLine($"Progression data loaded from {savePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading progression: {ex.Message}");
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// Initialize milestones.
        /// </summary>
        private void InitializeMilestones()
        {
            // Level milestones
            _milestones.Add(new ProgressionMilestone
            {
                Id = "level_5",
                Name = "Reach Level 5",
                Description = "Achieve the rank of Sergeant",
                Category = MilestoneCategory.Level,
                RequiredLevel = 5,
                IsCompleted = false,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 100 },
                    new ProgressionReward { Type = RewardType.TowerSlot, Amount = 1 }
                }
            });

            _milestones.Add(new ProgressionMilestone
            {
                Id = "level_10",
                Name = "Reach Level 10",
                Description = "Achieve the rank of Captain",
                Category = MilestoneCategory.Level,
                RequiredLevel = 10,
                IsCompleted = false,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 500 },
                    new ProgressionReward { Type = RewardType.UpgradeDiscount, Amount = 25 },
                    new ProgressionReward { Type = RewardType.TowerSlot, Amount = 2 }
                }
            });

            // Combat milestones
            _milestones.Add(new ProgressionMilestone
            {
                Id = "first_blood",
                Name = "First Blood",
                Description = "Kill your first zombie",
                Category = MilestoneCategory.Combat,
                RequiredLevel = 1,
                IsCompleted = false,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Experience, Amount = 50 }
                }
            });

            _milestones.Add(new ProgressionMilestone
            {
                Id = "zombie_slayer_100",
                Name = "Zombie Slayer",
                Description = "Kill 100 zombies",
                Category = MilestoneCategory.Combat,
                RequiredLevel = 1,
                IsCompleted = false,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 200 },
                    new ProgressionReward { Type = RewardType.Ability, Amount = 1 }
                }
            });

            // Wave milestones
            _milestones.Add(new ProgressionMilestone
            {
                Id = "wave_survivor_10",
                Name = "Survive 10 Waves",
                Description = "Complete 10 waves without losing a life",
                Category = MilestoneCategory.Wave,
                RequiredLevel = 1,
                IsCompleted = false,
                Rewards = new List<ProgressionReward>
                {
                    new ProgressionReward { Type = RewardType.Cash, Amount = 300 },
                    new ProgressionReward { Type = RewardType.TowerSlot, Amount = 1 }
                }
            });
        }

        /// <summary>
        /// Initialize achievements.
        /// </summary>
        private void InitializeAchievements()
        {
            // Combat achievements
            _achievements.Add("sharpshooter", new ProgressionAchievement
            {
                Id = "sharpshooter",
                Name = "Sharpshooter",
                Description = "Achieve 80% accuracy for 100 shots",
                Category = AchievementCategory.Combat,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 100f
            });

            _achievements.Add("economist", new ProgressionAchievement
            {
                Id = "economist",
                Name = "Economist",
                Description = "Accumulate $5000 in a single session",
                Category = AchievementCategory.Economy,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 5000f
            });

            _achievements.Add("perfect_defense", new ProgressionAchievement
            {
                Id = "perfect_defense",
                Name = "Perfect Defense",
                Description = "Complete a wave without taking any damage",
                Category = AchievementCategory.Defense,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 1f
            });

            _achievements.Add("speed_runner", new ProgressionAchievement
            {
                Id = "speed_runner",
                Name = "Speed Runner",
                Description = "Complete a wave in under 5 minutes",
                Category = AchievementCategory.Speed,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 1f
            });
        }

        /// <summary>
        /// Initialize events.
        /// </summary>
        private void InitializeEvents()
        {
            _events.Add(new ProgressionEvent
            {
                Id = "daily_bonus",
                Name = "Daily Bonus",
                Description = "Login bonus for consecutive days",
                Category = EventCategory.Bonus,
                IsRecurring = true,
                Reward = new ProgressionReward { Type = RewardType.Cash, Amount = 50 }
            });

            _events.Add(new ProgressionEvent
            {
                Id = "weekend_bonus",
                Name = "Weekend Bonus",
                Description = "Double experience on weekends",
                Category = EventCategory.Bonus,
                IsRecurring = true,
                Reward = new ProgressionReward { Type = RewardType.Experience, Amount = 100 }
            });
        }

        /// <summary>
        /// Calculate average milestones per session.
        /// </summary>
        private float CalculateAverageMilestonesPerSession()
        {
            // This would be calculated from saved data
            // For now, return a reasonable default
            return 2.5f; // Average 2.5 milestones per session
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle player level up event.
        /// </summary>
        private void OnPlayerLevelUp(int newLevel)
        {
            try
            {
                // Check for milestone completion
                var milestone = _milestones.FirstOrDefault(m => m.RequiredLevel == newLevel);
                if (milestone != null && !milestone.IsCompleted)
                {
                    milestone.IsCompleted = true;
                    milestone.CompletionDate = DateTime.Now;

                    // Grant rewards
                    foreach (var reward in milestone.Rewards)
                    {
                        GrantReward(reward);
                    }

                    OnMilestoneReached?.Invoke(milestone);
                    Console.WriteLine($"Milestone reached: {milestone.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling level up: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle experience gained event.
        /// </summary>
        private void OnExperienceGained(int level, int experience)
        {
            try
            {
                // Check for achievements
                CheckAchievements(level, experience);

                // Trigger progression events
                OnProgressionEvent?.Invoke(new ProgressionEvent
                {
                    Id = $"level_{level}_xp",
                    Name = $"Level {level} Experience",
                    Description = $"Gained {experience} experience",
                    Category = EventCategory.Progression,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling experience gained: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle reward unlocked event.
        /// </summary>
        private void OnRewardUnlocked(ProgressionReward reward)
        {
            try
            {
                // Grant reward based on type
                switch (reward.Type)
                {
                    case RewardType.Cash:
                        if (ModernPlayerStateSystem.Instance != null)
                        {
                            ModernPlayerStateSystem.Instance.AddCash(reward.Amount);
                        }
                        break;
                    case RewardType.TowerSlot:
                        // Grant tower slot
                        break;
                    case RewardType.UpgradeDiscount:
                        // Apply upgrade discount
                        break;
                    case RewardType.Ability:
                        // Grant ability
                        break;
                }

                Console.WriteLine($"Reward unlocked: {reward.Type} - {reward.Amount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling reward: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle unlock event.
        /// </summary>
        private void OnUnlockUnlocked(ProgressionAchievement unlock)
        {
            try
            {
                Console.WriteLine($"Unlock achieved: {unlock.Name}");

                // Trigger progression events
                OnProgressionEvent?.Invoke(new ProgressionEvent
                {
                    Id = $"unlock_{unlock.Id}",
                    Name = unlock.Name,
                    Description = unlock.Description,
                    Category = EventCategory.Achievement,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling unlock: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle max level reached event.
        /// </summary>
        private void OnMaxLevelReached()
        {
            try
            {
                Console.WriteLine("Maximum level reached!");

                // Trigger special event
                OnProgressionEvent?.Invoke(new ProgressionEvent
                {
                    Id = "max_level",
                    Name = "Maximum Level",
                    Description = "Reached the maximum player level",
                    Category = EventCategory.Progression,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling max level: {ex.Message}");
            }
        }

        /// <summary>
        /// Check for achievements.
        /// </summary>
        private void CheckAchievements(int level, int experience)
        {
            foreach (var achievement in _achievements.Values)
            {
                if (achievement.IsUnlocked) continue;

                // Update progress
                achievement.Progress = System.Math.Min(achievement.MaxProgress,
                    achievement.Progress + (float)experience / PlayerLevel.Instance.GetExperienceForNextLevel(level));

                // Check for completion
                if (achievement.Progress >= achievement.MaxProgress && !achievement.IsUnlocked)
                {
                    achievement.IsUnlocked = true;
                    achievement.UnlockedDate = DateTime.Now;
                    OnAchievementUnlocked?.Invoke(achievement);
                    Console.WriteLine($"Achievement unlocked: {achievement.Name}");
                }
            }
        }

        /// <summary>
        /// Grant a reward.
        /// </summary>
        private void GrantReward(ProgressionReward reward)
        {
            // This would integrate with appropriate systems
            switch (reward.Type)
            {
                case RewardType.Cash:
                    if (ModernPlayerStateSystem.Instance != null)
                    {
                        ModernPlayerStateSystem.Instance.AddCash(reward.Amount);
                    }
                    break;
                case RewardType.Experience:
                    if (PlayerLevel.Instance != null)
                    {
                        PlayerLevel.Instance.AddExperience(reward.Amount, "Reward");
                    }
                    break;
                case RewardType.TowerSlot:
                    // Grant tower slot through tower system
                    break;
                case RewardType.Ability:
                    // Grant ability through player system
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Progression milestone for player advancement.
    /// </summary>
    public class ProgressionMilestone
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MilestoneCategory Category { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public List<ProgressionReward> Rewards { get; set; }

        public object Serialize()
        {
            return new { Id, Name, Description, Category, RequiredLevel, IsCompleted, Rewards };
        }

        public void Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return;

            var dict = (Dictionary<string, object>)data;

            if (dict.TryGetValue("Id", out var id)) Id = (string)id;
            if (dict.TryGetValue("Name", out var name)) Name = (string)name;
            if (dict.TryGetValue("Description", out var description)) Description = (string)description;
            if (dict.TryGetValue("Category", out var category)) Category = (MilestoneCategory)Enum.Parse<MilestoneCategory>(category.ToString());
            if (dict.TryGetValue("RequiredLevel", out var requiredLevel)) RequiredLevel = (int)requiredLevel;
            if (dict.TryGetValue("IsCompleted", out var isCompleted)) IsCompleted = (bool)isCompleted;
            if (dict.TryGetValue("CompletionDate", out var completionDate)) CompletionDate = completionDate != null ? (DateTime?)completionDate : null;

            if (dict.TryGetValue("Rewards", out var rewardsData) && rewardsData is List<object>)
            {
                Rewards = ((List<object>)rewardsData).Select(r => ProgressionReward.Deserialize(r)).ToList();
            }
        }
    }

    /// <summary>
    /// Progression achievement for player accomplishments.
    /// </summary>
    public class ProgressionAchievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AchievementCategory Category { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedDate { get; set; }
        public float Progress { get; set; }
        public float MaxProgress { get; set; }

        public object Serialize()
        {
            return new { Id, Name, Description, Category, IsUnlocked, Progress, MaxProgress, UnlockedDate };
        }

        public void Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return;

            var dict = (Dictionary<string, object>)data;

            if (dict.TryGetValue("Id", out var id)) Id = (string)id;
            if (dict.TryGetValue("Name", out var name)) Name = (string)name;
            if (dict.TryGetValue("Description", out var description)) Description = (string)description;
            if (dict.TryGetValue("Category", out var category)) Category = (AchievementCategory)Enum.Parse<AchievementCategory>(category.ToString());
            if (dict.TryGetValue("IsUnlocked", out var isUnlocked)) IsUnlocked = (bool)isUnlocked;
            if (dict.TryGetValue("Progress", out var progress)) Progress = progress != null ? (float)progress : 0f;
            if (dict.TryGetValue("MaxProgress", out var maxProgress)) MaxProgress = maxProgress != null ? (float)maxProgress : 0f;
            if (dict.TryGetValue("UnlockedDate", out var unlockedDate)) UnlockedDate = unlockedDate != null ? (DateTime?)unlockedDate : null;
        }
    }

    /// <summary>
    /// Progression event for tracking player actions.
    /// </summary>
    public class ProgressionEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EventCategory Category { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime Timestamp { get; set; }
        public ProgressionReward Reward { get; set; }

        public object Serialize()
        {
            return new { Id, Name, Description, Category, IsRecurring, Reward, Timestamp };
        }

        public void Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return;

            var dict = (Dictionary<string, object>)data;

            if (dict.TryGetValue("Id", out var id)) Id = (string)id;
            if (dict.TryGetValue("Name", out var name)) Name = (string)name;
            if (dict.TryGetValue("Description", out var description)) Description = (string)description;
            if (dict.TryGetValue("Category", out var category)) Category = (EventCategory)Enum.Parse<EventCategory>(category.ToString());
            if (dict.TryGetValue("IsRecurring", out var isRecurring)) IsRecurring = (bool)isRecurring;
            if (dict.TryGetValue("Reward", out var rewardData)) Reward = ProgressionReward.Deserialize(rewardData);

            if (dict.TryGetValue("Timestamp", out var timestamp)) Timestamp = timestamp != null ? (DateTime)timestamp : DateTime.Now;
        }
    }

    /// <summary>
    /// Progression reward for milestones and achievements.
    /// </summary>
    public class ProgressionReward
    {
        public RewardType Type { get; set; }
        public int Amount { get; set; }

        public static ProgressionReward Deserialize(object data)
        {
            if (data is not Dictionary<string, object> dict) return null;

            var rewardDict = (Dictionary<string, object>)data;

            if (rewardDict.TryGetValue("Type", out var type) && Enum.TryParse<RewardType>(type.ToString(), out var rewardType))
            {
                Type = rewardType;
            }

            if (rewardDict.TryGetValue("Amount", out var amount) && int.TryParse(amount.ToString(), out var rewardAmount))
            {
                Amount = rewardAmount;
            }

            return new ProgressionReward { Type, Amount };
        }
    }

    /// <summary>
    /// Progression save data.
    /// </summary>
    public class ProgressionSaveData
    {
        public List<string> CompletedMilestones { get; set; }
        public List<string> UnlockedAchievements { get; set; }
        public Dictionary<string, object> MilestoneData { get; set; }
        public Dictionary<string, object> AchievementData { get; set; }
        public List<object> EventData { get; set; }
        public DateTime LastSaved { get; set; }
    }

    /// <summary>
    /// Progression statistics.
    /// </summary>
    public class ProgressionStatistics
    {
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public int TotalAchievements { get; set; }
        public int UnlockedAchievements { get; set; }
        public float CompletionPercentage { get; set; }
        public List<ProgressionMilestone> RecentMilestones { get; set; }
        public List<ProgressionAchievement> RecentAchievements { get; set; }
        public int TotalEvents { get; set; }
        public float AverageMilestonesPerSession { get; set; }
    }
}
