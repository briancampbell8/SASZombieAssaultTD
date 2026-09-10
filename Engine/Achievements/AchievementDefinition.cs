// ====================================================================================================
//  FILE: AchievementDefinition.cs
//  PATH: ./Engine/Achievements/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AchievementDefinition module.
//
//  RESPONSIBILITIES:
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide MatchesCriteria() behavior for the Core subsystem.
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
        ///<param name="ECSEntityCoreType">Optional ECSEntityCore type filter.</param>
        ///<param name="deathType">Optional death type filter.</param>
        ///<returns>True if this achievement matches the criteria.</returns>
        public bool MatchesCriteria(AchievementRequirementType requirementType, string? ECSEntityCoreType = null, string? deathType = null)
        {
            //Now you can safely check for null
            if (ECSEntityCoreType != null)
            {
                //Do something with ECSEntityCoreType
            }

            //Or use null-conditional operators
            var length = ECSEntityCoreType?.Length;

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





