/* ====================================================================================================
 *  FILE: ProgressionModels.cs
 *  PATH: Engine/LevelUpControl/Models/ProgressionModels.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Runtime data model layer for the progression system.
 *
 *  RESPONSIBILITIES:
 *      - Define all runtime data structures used by the progression system.
 *      - Provide strongly typed containers for milestones, achievements, events, rewards, and statistics.
 *      - Serve as the authoritative in-memory representation of progression state.
 *      - Remain stable and free of business logic, serialization logic, or engine orchestration.
 *
 *  NON-RESPONSIBILITIES:
 *      - JSON serialization (handled by ProgressionSerializer).
 *      - DTO definitions (handled by ProgressionDTOs).
 *      - Progression logic (handled by ProgressionLogic).
 *      - Static content definitions (handled by ProgressionDefinitions).
 *      - Event routing or engine integration (handled by LevelProgression).
 *
 *  DEPENDENCIES:
 *      - None (pure data layer).
 *
 *  CALLED BY:
 *      - LevelProgression (runtime state management)
 *      - ProgressionSerializer (mapping to/from DTOs)
 *      - ProgressionLogic (rule evaluation)
 *      - ProgressionDefinitions (initialization)
 *
 *  CALLS INTO:
 *      - Nothing. This file must remain dependency-free.
 *
 *  ARCHITECTURAL NOTES:
 *      - This file must remain pure and deterministic.
 *      - No methods beyond simple constructors or property containers.
 *      - No engine references, no PlayerLevel, no ModernPlayerStateSystem.
 *      - This is the safest layer in the subsystem and should never accumulate logic.
 *
 * ==================================================================================================== */

//
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    //ENUM: MilestoneCategory
    //PURPOSE: Categorize milestones for filtering, UI grouping, and analytics.
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

    //ENUM: AchievementCategory
    //PURPOSE: Categorize achievements for UI grouping and analytics.
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

    //ENUM: EventCategory
    //PURPOSE: Categorize progression events for logging and UI.
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

    //CLASS: ProgressionMilestone
    //PURPOSE: Represents a milestone the player can complete.
    //NOTES: Pure data container; logic lives in ProgressionLogic.
    public class ProgressionMilestone
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MilestoneCategory Category { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsUnlocked { get; set; }

        public DateTime? CompletionDate { get; set; }
        public List<ProgressionReward> Rewards { get; set; } = new();
    }

    //CLASS: ProgressionAchievement
    //PURPOSE: Represents an achievement the player can unlock.
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
    }

    //CLASS: ProgressionEvent
    //PURPOSE: Represents a logged event in the progression system.
    public class ProgressionEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EventCategory Category { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime Timestamp { get; set; }
        public ProgressionReward Reward { get; set; }
    }

    //CLASS: ProgressionReward
    //PURPOSE: Represents a reward granted by milestones or achievements.
    public class ProgressionReward
    {
        private static object TheType;
        private static object TheMember;

        public RewardType Type { get; set; }
        public int Amount { get; set; }

        internal static object Deserialize(object r)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }

    //CLASS: ProgressionStatistics
    //PURPOSE: Snapshot of progression metrics for UI and analytics.
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
