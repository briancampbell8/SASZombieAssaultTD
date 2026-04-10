// ROLE: Tracks per-player achievement progress.
// RESPONSIBILITY: Maintain achievement completion state with progress tracking and 
//                  serialization support.
// TRIGGERS: Instantiated by AchievementManager when player starts tracking an achievement.
// INPUTS: Receives progress updates from gameplay systems.
// OUTPUTS: Provides completion status and progress data for UI and save systems.
// DEPENDENCIES: References AchievementDefinition for requirements.
// CONTENTS: AchievementInstance class with AchievementId, CurrentProgress, TargetProgress, 
//           IsCompleted, CompletedAt, IsNotificationSuppressed properties.

using SASZombieAssaultTD.Engine;
using SASZombieAssaultTD.Engine.Math;
using System;
using System.Runtime.Serialization;

namespace SASZombieAssaultTD.Engine.Achievements
{
    /// <summary>
    /// Tracks per-player achievement progress and completion state.
    /// Provides ECS-friendly design with progress tracking and serialization support.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AchievementInstance
    {
        /// <summary>
        /// Reference to the achievement definition
        /// </summary>
        [DataMember]
        public string AchievementId { get; set; }

        /// <summary>
        /// Current progress toward completion (0 to RequirementTarget)
        /// </summary>
        [DataMember]
        public int CurrentProgress { get; set; }

        /// <summary>
        /// Whether the achievement has been completed
        /// </summary>
        [DataMember]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Timestamp when the achievement was completed
        /// </summary>
        [DataMember]
        public DateTime CompletionTime { get; set; }

        /// <summary>
        /// Timestamp when progress was last updated
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// Whether the achievement has been viewed by the player
        /// </summary>
        [DataMember]
        public bool IsViewed { get; set; }

        /// <summary>
        /// Number of times this achievement's progress has been updated
        /// </summary>
        [DataMember]
        public int UpdateCount { get; set; }

        /// <summary>
        /// Additional tracking data for complex achievements
        /// </summary>
        [DataMember]
        public string CustomProgressData { get; set; }

        /// <summary>
        /// Initializes a new AchievementInstance for the given achievement ID
        /// </summary>
        /// <param name="achievementId">ID of the achievement definition</param>
        public AchievementInstance(string achievementId)
        {
            AchievementId = achievementId ?? throw new ArgumentNullException(nameof(achievementId));
            CurrentProgress = 0;
            IsCompleted = false;
            CompletionTime = DateTime.MinValue;
            LastUpdatedTime = DateTime.UtcNow;
            IsViewed = false;
            UpdateCount = 0;
            CustomProgressData = string.Empty;
        }

        /// <summary>
        /// Updates the progress for this achievement
        /// </summary>
        /// <param name="progressAmount">Amount to add to current progress</param>
        /// <param name="requirementTarget">Target value for completion</param>
        /// <returns>True if the achievement was newly completed by this update</returns>
        public bool UpdateProgress(int progressAmount, int requirementTarget)
        {
            if (IsCompleted)
                return false;

            if (progressAmount <= 0)
                return false;

            CurrentProgress = Math.Math.Min(CurrentProgress + progressAmount,
                                            requirementTarget);
            LastUpdatedTime = DateTime.UtcNow;
            UpdateCount++;

            bool newlyCompleted = CurrentProgress >= requirementTarget;
            if (newlyCompleted)
            {
                MarkCompleted();
            }

            return newlyCompleted;
        }

        /// <summary>
        /// Sets the progress to a specific value
        /// </summary>
        /// <param name="newProgress">New progress value</param>
        /// <param name="requirementTarget">Target value for completion</param>
        /// <returns>True if the achievement was newly completed by this update</returns>
        public bool SetProgress(int newProgress, int requirementTarget)
        {
            if (IsCompleted)
                return false;

            CurrentProgress = Math.Math.Max(0, Math.Math.Min(newProgress, requirementTarget));
            LastUpdatedTime = DateTime.UtcNow;
            UpdateCount++;

            bool newlyCompleted = CurrentProgress >= requirementTarget;
            if (newlyCompleted)
            {
                MarkCompleted();
            }

            return newlyCompleted;
        }

        /// <summary>
        /// Marks the achievement as completed
        /// </summary>
        public void MarkCompleted()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                CompletionTime = DateTime.UtcNow;
                LastUpdatedTime = CompletionTime;
            }
        }

        /// <summary>
        /// Resets all progress for this achievement
        /// </summary>
        public void ResetProgress()
        {
            CurrentProgress = 0;
            IsCompleted = false;
            CompletionTime = DateTime.MinValue;
            LastUpdatedTime = DateTime.UtcNow;
            IsViewed = false;
            UpdateCount = 0;
            CustomProgressData = string.Empty;
        }

        /// <summary>
        /// Marks the achievement as viewed by the player
        /// </summary>
        public void MarkViewed()
        {
            IsViewed = true;
        }

        /// <summary>
        /// Gets the completion percentage (0.0 to 1.0)
        /// </summary>
        /// <param name="requirementTarget">Target value for completion</param>
        /// <returns>Completion percentage</returns>
        public float GetCompletionPercentage(int requirementTarget)
        {
            if (requirementTarget <= 0)
                return 0f;

            return System.Math.Min(1f, (float)CurrentProgress / requirementTarget);
        }

        /// <summary>
        /// Gets the remaining progress needed for completion
        /// </summary>
        /// <param name="requirementTarget">Target value for completion</param>
        /// <returns>Remaining progress amount</returns>
        public int GetRemainingProgress(int requirementTarget)
        {
            if (IsCompleted)
                return 0;

            return System.Math.Max(0, requirementTarget - CurrentProgress);
        }

        /// <summary>
        /// Sets custom progress data for complex achievement tracking
        /// </summary>
        /// <param name="data">Custom data string</param>
        public void SetCustomProgressData(string data)
        {
            CustomProgressData = data ?? string.Empty;
            LastUpdatedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Validates that the achievement instance is in a valid state
        /// </summary>
        /// <returns>True if the instance is valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AchievementId) &&
            CurrentProgress >= 0 &&
            UpdateCount >= 0;
        }

        /// <summary>
        /// Creates a copy of this achievement instance
        /// </summary>
        /// <returns>A new AchievementInstance with the same data</returns>
        public AchievementInstance Clone()
        {
            return new AchievementInstance(AchievementId)
            {
                CurrentProgress = this.CurrentProgress,
                IsCompleted = this.IsCompleted,
                CompletionTime = this.CompletionTime,
                LastUpdatedTime = this.LastUpdatedTime,
                IsViewed = this.IsViewed,
                UpdateCount = this.UpdateCount,
                CustomProgressData = this.CustomProgressData
            };
        }
    }
}







