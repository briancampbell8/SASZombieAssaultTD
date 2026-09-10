/* ====================================================================================================
 *  FILE: ProgressionLogic.cs
 *  PATH: Engine/LevelUpControl/Logic/ProgressionLogic.cs
 *  SUBSYSTEM: LevelUpControl
 *  ROLE: Central rule engine for milestone and achievement progression.
 *
 *  RESPONSIBILITIES:
 *      - Evaluate milestone completion conditions.
 *      - Evaluate achievement progression and unlock conditions.
 *      - Dispatch rewards through LevelProgression.
 *      - Generate progression events for logging and UI.
 *      - Serve as the deterministic rule layer for all progression logic.
 *
 *  NON-RESPONSIBILITIES:
 *      - JSON serialization (handled by ProgressionSerializer).
 *      - Data modeling (handled by ProgressionModels).
 *      - DTO definitions (handled by ProgressionDTOs).
 *      - Static content definitions (handled by ProgressionDefinitions).
 *      - File I/O or engine orchestration (handled by LevelProgression).
 *
 *  DEPENDENCIES:
 *      - LevelProgression.cs (façade)
 *      - ProgressionModels.cs (runtime models)
 *      - PlayerLevel.cs (for XP/level data)
 *      - ModernPlayerStateSystem.cs (for reward dispatch)
 *
 *  CALLED BY:
 *      - LevelProgression.OnPlayerLevelUp()
 *      - LevelProgression.OnExperienceGained()
 *      - LevelProgression.OnMaxLevelReached()
 *
 *  CALLS INTO:
 *      - LevelProgression internal event invokers
 *      - PlayerLevel (for XP/level thresholds)
 *      - ModernPlayerStateSystem (for reward application)
 *
 *  ARCHITECTURAL NOTES:
 *      - This file contains all business rules for progression.
 *      - No serialization, no DTOs, no static definitions.
 *      - All logic must be deterministic and side-effect free except for reward dispatch.
 *      - This file must remain stable and isolated from UI or engine orchestration.
 *
 * ==================================================================================================== */

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.GameRoot.GamePlay;

namespace SASZombieAssaultTD.Engine.LevelUpControl
{
    ///<summary>
    ///Deterministic rule engine for milestone and achievement progression.
    ///</summary>
    public static class ProgressionLogic
    {
        //===============================================================================================
        // LEVEL-UP HANDLING
        //===============================================================================================

        //METHOD: HandleLevelUp()
        //PURPOSE: Evaluate milestone completion when the player levels up.
        //CALLED BY: LevelProgression.OnPlayerLevelUp()
        //CALLS INTO: LevelProgression.InvokeMilestoneReached(), reward dispatch
        public static void HandleLevelUp(LevelProgression progression, int newLevel)
        {
            var milestone = progression._milestones
                .FirstOrDefault(m => m.RequiredLevel == newLevel && !m.IsCompleted);

            if (milestone == null)
                return;

            milestone.IsCompleted = true;
            milestone.CompletionDate = DateTime.Now;

            //Dispatch rewards
            foreach (var reward in milestone.Rewards)
                ApplyReward(reward);

            //Notify listeners (legal via invoker)
            progression.InvokeMilestoneReached(milestone);

            //Log event (legal via invoker)
            progression.InvokeProgressionEvent(new ProgressionEvent
            {
                Id = $"milestone_{milestone.Id}",
                Name = milestone.Name,
                Description = milestone.Description,
                Category = EventCategory.Milestone,
                Timestamp = DateTime.Now
            });
        }

        //===============================================================================================
        // EXPERIENCE HANDLING
        //===============================================================================================

        //METHOD: HandleExperienceGained()
        //PURPOSE: Update achievement progress based on XP gain.
        //CALLED BY: LevelProgression.OnExperienceGained()
        //CALLS INTO: LevelProgression.InvokeAchievementUnlocked(), InvokeProgressionEvent()
        public static void HandleExperienceGained(LevelProgression progression, int level, int experience)
        {
            foreach (var achievement in progression._achievements)
            {
                if (achievement.IsUnlocked)
                    continue;

                float xpForNext = PlayerLevel.Instance.GetExperienceForNextLevel(level);
                float delta = (float)experience / xpForNext;

                achievement.Progress = System.Math.Min(achievement.MaxProgress, achievement.Progress + delta);

                if (achievement.Progress >= achievement.MaxProgress)
                {
                    achievement.IsUnlocked = true;
                    achievement.UnlockedDate = DateTime.Now;

                    //Notify listeners (legal via invoker)
                    progression.InvokeAchievementUnlocked(achievement);

                    //Log event (legal via invoker)
                    progression.InvokeProgressionEvent(new ProgressionEvent
                    {
                        Id = $"achievement_{achievement.Id}",
                        Name = achievement.Name,
                        Description = achievement.Description,
                        Category = EventCategory.Achievement,
                        Timestamp = DateTime.Now
                    });
                }
            }

            //Log XP event (legal via invoker)
            progression.InvokeProgressionEvent(new ProgressionEvent
            {
                Id = $"xp_gain_{level}",
                Name = $"Gained {experience} XP",
                Description = $"Player gained {experience} XP at level {level}",
                Category = EventCategory.Progress,
                Timestamp = DateTime.Now
            });
        }

        //===============================================================================================
        // MAX LEVEL HANDLING
        //===============================================================================================

        //METHOD: HandleMaxLevelReached()
        //PURPOSE: Log a special event when the player reaches max level.
        //CALLED BY: LevelProgression.OnMaxLevelReached()
        public static void HandleMaxLevelReached(LevelProgression progression)
        {
            progression.InvokeProgressionEvent(new ProgressionEvent
            {
                Id = "max_level_reached",
                Name = "Maximum Level Achieved",
                Description = "The player has reached the maximum possible level.",
                Category = EventCategory.Progression,
                Timestamp = DateTime.Now
            });
        }

        //===============================================================================================
        // REWARD APPLICATION
        //===============================================================================================

        //METHOD: ApplyReward()
        //PURPOSE: Apply a reward to the player.
        //CALLED BY: HandleLevelUp(), achievement unlock logic
        //CALLS INTO: ModernPlayerStateSystem, PlayerLevel
        private static void ApplyReward(ProgressionReward reward)
        {
            switch (reward.Type)
            {
                case RewardType.Cash:
                    ModernPlayerStateSystem.Instance?.AddCash(reward.Amount);
                    break;

                case RewardType.Experience:
                    PlayerLevel.Instance?.AddExperience(reward.Amount, "Reward");
                    break;

                case RewardType.TowerSlot:
                    //Future expansion: tower slot unlock system
                    break;

                case RewardType.Ability:
                    //Future expansion: ability unlock system
                    break;

                case RewardType.UpgradeDiscount:
                    //Future expansion: upgrade discount system
                    break;
            }
        }
    }
}
