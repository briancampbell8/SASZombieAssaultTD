// ====================================================================================================
//  FILE: ChallengeInstance.cs
//  PATH: ./Engine/Achievements/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ChallengeInstance module.
//
//  RESPONSIBILITIES:
//      - Provide UpdateProgress() behavior for the Core subsystem.
//      - Provide SetProgress() behavior for the Core subsystem.
//      - Provide MarkCompleted() behavior for the Core subsystem.
//      - Provide ClaimRewards() behavior for the Core subsystem.
//      - Provide ResetProgress() behavior for the Core subsystem.
//      - Provide MarkViewed() behavior for the Core subsystem.
//      - Provide IsExpired() behavior for the Core subsystem.
//      - Provide GetTimeRemaining() behavior for the Core subsystem.
//      - Provide GetCompletionPercentage() behavior for the Core subsystem.
//      - Provide GetRemainingProgress() behavior for the Core subsystem.
//      - Provide SetCustomProgressData() behavior for the Core subsystem.
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide Clone() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Runtime.Serialization;

namespace SASZombieAssaultTD.Engine.Achievements
{
    ///<summary>
    ///Tracks per-player challenge progress and completion state.
    ///Provides time-based expiration tracking and serialization support.
    ///</summary>
    [Serializable]
    [DataContract]
    public class ChallengeInstance
    {
        [DataMember] public string ChallengeId { get; private set; }
        [DataMember] public int CurrentProgress { get; private set; }
        [DataMember] public bool IsCompleted { get; private set; }
        [DataMember] public bool IsRewardsClaimed { get; private set; }
        [DataMember] public DateTime CompletionTime { get; private set; }
        [DataMember] public DateTime LastUpdatedTime { get; private set; }
        [DataMember] public DateTime ExpirationTime { get; private set; }
        [DataMember] public DateTime StartTime { get; private set; }
        [DataMember] public bool IsViewed { get; private set; }
        [DataMember] public int UpdateCount { get; private set; }
        [DataMember] public string CustomProgressData { get; private set; }

        public ChallengeInstance(string challengeId, DateTime expirationTime)
        {
            if (string.IsNullOrWhiteSpace(challengeId))
                throw new ArgumentException("Challenge ID cannot be null or empty.", nameof(challengeId));

            ChallengeId = challengeId;
            ResetProgress(expirationTime);
        }

        public bool UpdateProgress(int progressAmount, int requirementTarget)
        {
            if (IsCompleted || IsExpired() || progressAmount <= 0) return false;

            CurrentProgress = Math.Math.Min(CurrentProgress + progressAmount, requirementTarget);
            LastUpdatedTime = DateTime.UtcNow;
            UpdateCount++;

            if (CurrentProgress >= requirementTarget) MarkCompleted();
            return IsCompleted;
        }

        public bool SetProgress(int newProgress, int requirementTarget)
        {
            if (IsCompleted || IsExpired()) return false;

            CurrentProgress = System.Math.Clamp(newProgress, 0, requirementTarget);
            LastUpdatedTime = DateTime.UtcNow;
            UpdateCount++;

            if (CurrentProgress >= requirementTarget) MarkCompleted();
            return IsCompleted;
        }

        public void MarkCompleted()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            CompletionTime = LastUpdatedTime = DateTime.UtcNow;
        }

        public bool ClaimRewards()
        {
            if (!IsCompleted || IsRewardsClaimed) return false;

            IsRewardsClaimed = true;
            LastUpdatedTime = DateTime.UtcNow;
            return true;
        }

        public void ResetProgress(DateTime newExpirationTime)
        {
            if (newExpirationTime <= DateTime.UtcNow)
                throw new ArgumentException("Expiration time must be in the future.", nameof(newExpirationTime));

            CurrentProgress = 0;
            IsCompleted = false;
            IsRewardsClaimed = false;
            CompletionTime = DateTime.MinValue;
            LastUpdatedTime = StartTime = DateTime.UtcNow;
            ExpirationTime = newExpirationTime;
            IsViewed = false;
            UpdateCount = 0;
            CustomProgressData = string.Empty;
        }

        public void MarkViewed() => IsViewed = true;

        public bool IsExpired() => DateTime.UtcNow > ExpirationTime;

        public TimeSpan GetTimeRemaining() => IsExpired() ? TimeSpan.Zero : ExpirationTime - DateTime.UtcNow;

        public float GetCompletionPercentage(int requirementTarget) =>
            requirementTarget > 0 ? System.Math.Min(1f, (float)CurrentProgress / requirementTarget) : 0f;

        public int GetRemainingProgress(int requirementTarget) =>
            IsCompleted ? 0 : System.Math.Max(0, requirementTarget - CurrentProgress);

        public void SetCustomProgressData(string data)
        {
            CustomProgressData = data ?? string.Empty;
            LastUpdatedTime = DateTime.UtcNow;
        }

        public bool IsValid() =>
            !string.IsNullOrEmpty(ChallengeId) &&
            CurrentProgress >= 0 &&
            UpdateCount >= 0 &&
            ExpirationTime > StartTime;

        public ChallengeInstance Clone() =>
            new(ChallengeId, ExpirationTime)
            {
                CurrentProgress = CurrentProgress,
                IsCompleted = IsCompleted,
                IsRewardsClaimed = IsRewardsClaimed,
                CompletionTime = CompletionTime,
                LastUpdatedTime = LastUpdatedTime,
                StartTime = StartTime,
                IsViewed = IsViewed,
                UpdateCount = UpdateCount,
                CustomProgressData = CustomProgressData
            };
    }
}





