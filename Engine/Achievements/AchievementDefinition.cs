using System;
using System.Runtime.Serialization;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Achievements
{
    ///<summary>
    ///Defines achievement metadata including requirements and rewards.
    ///Supports serialization for save/load operations and provides comprehensive achievement tracking.
    ///</summary>
    [Serializable]
    [DataContract]
    public class AchievementDefinition
    {
        [DataMember] public string Id { get; private set; } = string.Empty;
        [DataMember] public string Name { get; private set; } = string.Empty;
        [DataMember] public string Description { get; private set; } = string.Empty;
        [DataMember] public AchievementCategory Category { get; private set; } = AchievementCategory.General;
        [DataMember] public AchievementRarity Rarity { get; private set; } = AchievementRarity.Common;
        [DataMember] public string IconId { get; private set; } = string.Empty;
        [DataMember] public int PointValue { get; private set; } = 10;
        [DataMember] public AchievementRequirementType RequirementType { get; private set; } = AchievementRequirementType.KillCount;
        [DataMember] public int RequirementTarget { get; private set; } = 1;
        [DataMember] public string EntityTypeFilter { get; private set; } = string.Empty;
        [DataMember] public string DeathTypeFilter { get; private set; } = string.Empty;
        [DataMember] public bool IsHidden { get; private set; } = false;
        [DataMember] public bool IsActive { get; private set; } = true;
        [DataMember] public int SortOrder { get; private set; } = 0;
        [DataMember] public string CustomData { get; private set; } = string.Empty;

        ///<summary>
        ///Validates that the achievement definition has all required fields populated.
        ///</summary>
        ///<returns>True if the definition is valid, false otherwise.</returns>
        public bool IsValid() =>
            !string.IsNullOrEmpty(Id) &&
            !string.IsNullOrEmpty(Name) &&
            RequirementTarget > 0;

        ///<summary>
        ///Checks if this achievement matches the given criteria for progress evaluation.
        ///</summary>
        ///<param name="requirementType">Type of requirement to check.</param>
        ///<param name="entityType">Optional entity type filter.</param>
        ///<param name="deathType">Optional death type filter.</param>
        ///<returns>True if this achievement matches the criteria.</returns>
        public bool MatchesCriteria(AchievementRequirementType requirementType, string? entityType = null, string? deathType = null)
        {
            //Now you can safely check for null
            if (entityType != null)
            {
                //Do something with entityType
            }

            //Or use null-conditional operators
            var length = entityType?.Length;

            return false; //Your actual logic here
        }

        ///<summary>
        ///Types of requirements that can trigger achievement progress.
        ///</summary>
        public enum AchievementRequirementType
        {
            KillCount,
            EntityTypeKills,
            DeathTypeKills,
            ScoreThreshold,
            RoundReached,
            ItemCollected,
            TimeSurvived,
            ResourcesSpent,
            BuildingsPlaced,
            Custom
        }
    }
}




