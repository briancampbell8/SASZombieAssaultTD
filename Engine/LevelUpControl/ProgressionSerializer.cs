/* ====================================================================================================
 *  FILE: ProgressionSerializer.cs
 *  PATH: Engine/LevelUpControl/Serialization/ProgressionSerializer.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Serialization and mapping engine for the progression subsystem.
 *
 *  RESPONSIBILITIES:
 *      - Convert runtime models (ProgressionModels.cs) into DTOs (ProgressionDTOs.cs).
 *      - Convert DTOs back into runtime models.
 *      - Construct the ProgressionSaveData root object for JSON serialization.
 *      - Restore LevelProgression state from ProgressionSaveData.
 *      - Maintain version-safe, deterministic mapping logic.
 *
 *  NON-RESPONSIBILITIES:
 *      - File I/O (handled by LevelProgression).
 *      - Business logic (handled by ProgressionLogic).
 *      - Static content definitions (handled by ProgressionDefinitions).
 *      - Event routing or engine integration (handled by LevelProgression).
 *
 *  DEPENDENCIES:
 *      - ProgressionModels.cs (runtime models)
 *      - ProgressionDTOs.cs (DTOs)
 *      - LevelProgression.cs (façade)
 *
 *  CALLED BY:
 *      - LevelProgression.SaveProgression()
 *      - LevelProgression.LoadProgression()
 *
 *  CALLS INTO:
 *      - No engine systems; pure mapping layer.
 *
 *  ARCHITECTURAL NOTES:
 *      - This file must remain deterministic and side-effect free.
 *      - DTOs must remain stable across versions; serializer must handle evolution safely.
 *      - No engine references allowed; this is a pure transformation layer.
 *      - All mapping logic must be explicit—no reflection, no dynamic typing.
 *
 * ==================================================================================================== */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    /// <summary>
    /// Static mapping engine for converting runtime models ↔ DTOs.
    /// </summary>
    public static class ProgressionSerializer
    {
        // METHOD: CreateSaveData()
        // PURPOSE: Build a complete ProgressionSaveData object from the current LevelProgression state.
        // CALLED BY: LevelProgression.SaveProgression()
        // CALLS INTO: ToDTO() methods
        public static ProgressionSaveData CreateSaveData(
            LevelProgression progression,
            Dictionary<string, object> achievementDataBlob)
        {
            // Completed milestone IDs
            var completedMilestones = progression._milestones
                .Where(m => m.IsCompleted)
                .Select(m => m.Id)
                .ToList();

            // Unlocked achievement IDs
            var unlockedAchievements = progression._achievements.Values
                .Where(a => a.IsUnlocked)
                .Select(a => a.Id)
                .ToList();

            // Full milestone DTO map
            var milestoneData = progression._milestones
                .ToDictionary(m => m.Id, m => ToDTO(m));

            // Full achievement DTO map
            var achievementData = progression._achievements
                .ToDictionary(kvp => kvp.Key, kvp => ToDTO(kvp.Value));

            // Event DTO list
            var eventData = progression._events
                .Select(ToDTO)
                .ToList();

            return new ProgressionSaveData
            {
                CompletedMilestones = completedMilestones,
                UnlockedAchievements = unlockedAchievements,
                MilestoneData = milestoneData,
                AchievementData = achievementData,
                EventData = eventData,
                LastSaved = DateTime.Now
            };
        }

        // METHOD: RestoreFromSaveData()
        // PURPOSE: Restore LevelProgression state from a ProgressionSaveData object.
        // CALLED BY: LevelProgression.LoadProgression()
        // CALLS INTO: FromDTO() methods
        public static void RestoreFromSaveData(LevelProgression progression, ProgressionSaveData saveData)
        {
            // Restore milestones
            foreach (var kvp in saveData.MilestoneData)
            {
                var milestone = progression._milestones.FirstOrDefault(m => m.Id == kvp.Key);
                if (milestone != null)
                {
                    FromDTO(kvp.Value, milestone);
                }
            }

            // Restore achievements
            foreach (var kvp in saveData.AchievementData)
            {
                if (progression._achievements.TryGetValue(kvp.Key, out var achievement))
                {
                    FromDTO(kvp.Value, achievement);
                }
            }

            // Restore events
            progression._events.Clear();
            if (saveData.EventData != null)
            {
                foreach (var dto in saveData.EventData)
                {
                    var evt = FromDTO(dto);
                    if (evt != null)
                        progression._events.Add(evt);
                }
            }
        }

        // ===============================================================================================
        //  MILESTONE MAPPING
        // ===============================================================================================

        // METHOD: ToDTO(ProgressionMilestone)
        // PURPOSE: Convert runtime milestone → DTO.
        private static ProgressionMilestoneDTO ToDTO(ProgressionMilestone m)
        {
            return new ProgressionMilestoneDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Category = m.Category,
                RequiredLevel = m.RequiredLevel,
                IsCompleted = m.IsCompleted,
                CompletionDate = m.CompletionDate,
                Rewards = m.Rewards?.Select(ToDTO).ToList() ?? new List<ProgressionRewardDTO>()
            };
        }

        // METHOD: FromDTO(ProgressionMilestoneDTO)
        // PURPOSE: Apply DTO → runtime milestone.
        private static void FromDTO(ProgressionMilestoneDTO dto, ProgressionMilestone m)
        {
            m.Id = dto.Id;
            m.Name = dto.Name;
            m.Description = dto.Description;
            m.Category = dto.Category;
            m.RequiredLevel = dto.RequiredLevel;
            m.IsCompleted = dto.IsCompleted;
            m.CompletionDate = dto.CompletionDate;
            m.Rewards = dto.Rewards?.Select(FromDTO).ToList() ?? new List<ProgressionReward>();
        }

        // ===============================================================================================
        //  ACHIEVEMENT MAPPING
        // ===============================================================================================

        private static ProgressionAchievementDTO ToDTO(ProgressionAchievement a)
        {
            return new ProgressionAchievementDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Category = a.Category,
                IsUnlocked = a.IsUnlocked,
                UnlockedDate = a.UnlockedDate,
                Progress = a.Progress,
                MaxProgress = a.MaxProgress
            };
        }

        private static void FromDTO(ProgressionAchievementDTO dto, ProgressionAchievement a)
        {
            a.Id = dto.Id;
            a.Name = dto.Name;
            a.Description = dto.Description;
            a.Category = dto.Category;
            a.IsUnlocked = dto.IsUnlocked;
            a.UnlockedDate = dto.UnlockedDate;
            a.Progress = dto.Progress;
            a.MaxProgress = dto.MaxProgress;
        }

        // ===============================================================================================
        //  EVENT MAPPING
        // ===============================================================================================

        private static ProgressionEventDTO ToDTO(ProgressionEvent e)
        {
            return new ProgressionEventDTO
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Category = e.Category,
                IsRecurring = e.IsRecurring,
                Timestamp = e.Timestamp,
                Reward = e.Reward != null ? ToDTO(e.Reward) : null
            };
        }

        private static ProgressionEvent FromDTO(ProgressionEventDTO dto)
        {
            if (dto == null) return null;

            return new ProgressionEvent
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                IsRecurring = dto.IsRecurring,
                Timestamp = dto.Timestamp,
                Reward = dto.Reward != null ? FromDTO(dto.Reward) : null
            };
        }

        // ===============================================================================================
        //  REWARD MAPPING
        // ===============================================================================================

        private static ProgressionRewardDTO ToDTO(ProgressionReward r)
        {
            return new ProgressionRewardDTO
            {
                Type = r.Type,
                Amount = r.Amount
            };
        }

        private static ProgressionReward FromDTO(ProgressionRewardDTO dto)
        {
            return new ProgressionReward
            {
                Type = dto.Type,
                Amount = dto.Amount
            };
        }
    }
}
