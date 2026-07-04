/* ====================================================================================================
 *  FILE: ProgressionDefinitions.cs
 *  PATH: Engine/LevelUpControl/Definitions/ProgressionDefinitions.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Static content definition provider for milestones, achievements, and progression events.
 *
 *  RESPONSIBILITIES:
 *      - Define all built-in milestones for the progression system.
 *      - Define all built-in achievements for the progression system.
 *      - Define all built-in progression events (e.g., bonuses, recurring events).
 *      - Populate runtime collections owned by LevelProgression at startup.
 *
 *  NON-RESPONSIBILITIES:
 *      - Runtime logic or rule evaluation (handled by ProgressionLogic).
 *      - JSON serialization (handled by ProgressionSerializer).
 *      - DTO definitions (handled by ProgressionDTOs).
 *      - File I/O or engine orchestration (handled by LevelProgression).
 *
 *  DEPENDENCIES:
 *      - ProgressionModels.cs (runtime models and enums)
 *
 *  CALLED BY:
 *      - LevelProgression constructor (during subsystem initialization)
 *
 *  CALLS INTO:
 *      - Nothing beyond populating provided collections.
 *
 *  ARCHITECTURAL NOTES:
 *      - This file is pure data definition; no business logic belongs here.
 *      - All definitions are centralized to avoid scattering content across logic files.
 *      - Safe to extend with new milestones/achievements/events as the game grows.
 *
 * ==================================================================================================== */

using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    ///<summary>
    ///Static content definitions for milestones, achievements, and events.
    ///</summary>
    public static class ProgressionDefinitions
    {
        //METHOD: InitializeMilestones()
        //PURPOSE: Populate the milestone list with all built-in milestones.
        //CALLED BY: LevelProgression constructor
        //CALLS INTO: None
        public static void InitializeMilestones(List<ProgressionMilestone> milestones)
        {
            milestones.Add(new ProgressionMilestone
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

            milestones.Add(new ProgressionMilestone
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

            milestones.Add(new ProgressionMilestone
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

            milestones.Add(new ProgressionMilestone
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

            milestones.Add(new ProgressionMilestone
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

        //METHOD: InitializeAchievements()
        //PURPOSE: Populate the achievement dictionary with all built-in achievements.
        //CALLED BY: LevelProgression constructor
        public static void InitializeAchievements(Dictionary<string, ProgressionAchievement> achievements)
        {
            achievements["sharpshooter"] = new ProgressionAchievement
            {
                Id = "sharpshooter",
                Name = "Sharpshooter",
                Description = "Achieve 80% accuracy for 100 shots",
                Category = AchievementCategory.Combat,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 100f
            };

            achievements["economist"] = new ProgressionAchievement
            {
                Id = "economist",
                Name = "Economist",
                Description = "Accumulate $5000 in a single session",
                Category = AchievementCategory.Economy,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 5000f
            };

            achievements["perfect_defense"] = new ProgressionAchievement
            {
                Id = "perfect_defense",
                Name = "Perfect Defense",
                Description = "Complete a wave without taking any damage",
                Category = AchievementCategory.Defense,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 1f
            };

            achievements["speed_runner"] = new ProgressionAchievement
            {
                Id = "speed_runner",
                Name = "Speed Runner",
                Description = "Complete a wave in under 5 minutes",
                Category = AchievementCategory.Speed,
                IsUnlocked = false,
                Progress = 0f,
                MaxProgress = 1f
            };
        }

        //METHOD: InitializeEvents()
        //PURPOSE: Populate the event list with all built-in progression events.
        //CALLED BY: LevelProgression constructor
        public static void InitializeEvents(List<ProgressionEvent> events)
        {
            events.Add(new ProgressionEvent
            {
                Id = "daily_bonus",
                Name = "Daily Bonus",
                Description = "Login bonus for consecutive days",
                Category = EventCategory.Bonus,
                IsRecurring = true,
                Timestamp = System.DateTime.Now,
                Reward = new ProgressionReward { Type = RewardType.Cash, Amount = 50 }
            });

            events.Add(new ProgressionEvent
            {
                Id = "weekend_bonus",
                Name = "Weekend Bonus",
                Description = "Double experience on weekends",
                Category = EventCategory.Bonus,
                IsRecurring = true,
                Timestamp = System.DateTime.Now,
                Reward = new ProgressionReward { Type = RewardType.Experience, Amount = 100 }
            });
        }
    }
}
