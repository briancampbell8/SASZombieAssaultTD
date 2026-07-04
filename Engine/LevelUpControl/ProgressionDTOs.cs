/* ====================================================================================================
 *  FILE: ProgressionDTOs.cs
 *  PATH: Engine/LevelUpControl/Serialization/ProgressionDTOs.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Strongly typed JSON Data Transfer Objects (DTOs) for the progression system.
 *
 *  RESPONSIBILITIES:
 *      - Define the exact JSON-serializable shapes used for saving and loading progression data.
 *      - Provide stable, version-safe structures for System.Text.Json.
 *      - Serve as the intermediary between runtime models and serialized data.
 *      - Remain immutable to runtime logic; DTOs must not contain behavior.
 *
 *  NON-RESPONSIBILITIES:
 *      - Runtime logic (handled by ProgressionLogic).
 *      - Data modeling (handled by ProgressionModels).
 *      - Save/load orchestration (handled by ProgressionSerializer).
 *      - Engine integration or event routing (handled by LevelProgression).
 *
 *  DEPENDENCIES:
 *      - ProgressionModels.cs (for enum types only)
 *
 *  CALLED BY:
 *      - ProgressionSerializer (mapping to/from runtime models)
 *      - LevelProgression (indirectly through serializer)
 *
 *  CALLS INTO:
 *      - Nothing. DTOs must remain dependency-free.
 *
 *  ARCHITECTURAL NOTES:
 *      - DTOs must remain stable across versions to avoid breaking save files.
 *      - DTOs must not contain methods, logic, or references to engine systems.
 *      - DTOs must mirror runtime models but remain serialization-focused.
 *      - Adding fields requires versioning consideration in ProgressionSerializer.
 *
 * ==================================================================================================== */

using System;
using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    //DTO: ProgressionMilestoneDTO
    //PURPOSE: JSON-safe representation of ProgressionMilestone.
    public class ProgressionMilestoneDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MilestoneCategory Category { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public List<ProgressionRewardDTO> Rewards { get; set; }
    }

    //DTO: ProgressionAchievementDTO
    //PURPOSE: JSON-safe representation of ProgressionAchievement.
    public class ProgressionAchievementDTO
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

    //DTO: ProgressionEventDTO
    //PURPOSE: JSON-safe representation of ProgressionEvent.
    public class ProgressionEventDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EventCategory Category { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime Timestamp { get; set; }
        public ProgressionRewardDTO Reward { get; set; }
    }

    //DTO: ProgressionRewardDTO
    //PURPOSE: JSON-safe representation of ProgressionReward.
    public class ProgressionRewardDTO
    {
        public RewardType Type { get; set; }
        public int Amount { get; set; }
    }

    //DTO: ProgressionSaveData
    //PURPOSE: Root save container for the entire progression system.
    public class ProgressionSaveData
    {
        public List<string> CompletedMilestones { get; set; }
        public List<string> UnlockedAchievements { get; set; }

        public Dictionary<string, ProgressionMilestoneDTO> MilestoneData { get; set; }
        public Dictionary<string, ProgressionAchievementDTO> AchievementData { get; set; }
        public List<ProgressionEventDTO> EventData { get; set; }

        public DateTime LastSaved { get; set; }
    }
}
