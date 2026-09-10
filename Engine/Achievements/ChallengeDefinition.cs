// ====================================================================================================
//  FILE: ChallengeDefinition.cs
//  PATH: ./Engine/Achievements/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the ChallengeDefinition module.
//
//  RESPONSIBILITIES:
//      - Provide IsValid() behavior for the Core subsystem.
//      - Provide MatchesCriteria() behavior for the Core subsystem.
//      - Provide GetTotalRewardValue() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Runtime.Serialization;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Achievements
{
    ///<summary>
    ///Defines daily/weekly challenge metadata including objectives and rewards.
    ///Supports rotating challenges with time-based expiration and serialization.
    ///</summary>
    [Serializable]
    [DataContract]
    public class ChallengeDefinition
    {
        [DataMember] public string Id { get; private set; } = string.Empty;
        [DataMember] public string Name { get; private set; } = string.Empty;
        [DataMember] public string Description { get; private set; } = string.Empty;
        [DataMember] public ChallengeType ChallengeType { get; private set; } = ChallengeType.Daily;
        [DataMember] public ChallengeCategory Category { get; private set; } = ChallengeCategory.General;
        [DataMember] public ChallengeRequirementType RequirementType { get; private set; } = ChallengeRequirementType.KillCount;
        [DataMember] public int RequirementTarget { get; private set; } = 1;
        [DataMember] public string EntityTypeFilter { get; private set; } = string.Empty;
        [DataMember] public string DeathTypeFilter { get; private set; } = string.Empty;
        [DataMember] public int XPReward { get; private set; } = 100;
        [DataMember] public int CurrencyReward { get; private set; } = 50;
        [DataMember] public int SortOrder { get; private set; } = 0;
        [DataMember] public ChallengeDifficulty Difficulty { get; private set; } = ChallengeDifficulty.Normal;
        [DataMember] public bool IsActive { get; private set; } = true;
        [DataMember] public int MinimumLevel { get; private set; } = 1;
        [DataMember] public string CustomData { get; private set; } = string.Empty;

        ///<summary>
        ///Validates that the challenge definition has all required fields populated.
        ///</summary>
        ///<returns>True if the definition is valid, false otherwise.</returns>
        public bool IsValid() =>
            !string.IsNullOrEmpty(Id) &&
            !string.IsNullOrEmpty(Name) &&
            RequirementTarget > 0 &&
            XPReward >= 0 &&
            CurrencyReward >= 0;

        ///<summary>
        ///Checks if this challenge matches the given criteria for progress evaluation.
        ///</summary>
        ///<param name="requirementType">Type of requirement to check.</param>
        ///<param name="ECSEntityCoreType">Optional ECSEntityCore type filter.</param>
        ///<param name="deathType">Optional death type filter.</param>
        ///<returns>True if this challenge matches the criteria.</returns>
        public bool MatchesCriteria(ChallengeRequirementType requirementType, string ECSEntityCoreType = "", string deathType = "")
        {
            if (RequirementType != requirementType) return false;
            if (!string.IsNullOrEmpty(EntityTypeFilter) && EntityTypeFilter != ECSEntityCoreType) return false;
            if (!string.IsNullOrEmpty(DeathTypeFilter) && DeathTypeFilter != deathType) return false;

            return true;
        }

        ///<summary>
        ///Gets the total reward value for this challenge.
        ///</summary>
        ///<returns>Total reward value (XP + currency).</returns>
        public int GetTotalRewardValue() => XPReward + CurrencyReward;
    }

    ///<summary>
    ///Types of challenges based on rotation schedule.
    ///</summary>
    public enum ChallengeType
    {
        Daily,
        Weekly,
        Special
    }

    ///<summary>
    ///Categories for grouping challenges in the UI.
    ///</summary>
    public enum ChallengeCategory
    {
        General,
        Combat,
        Survival,
        Collection,
        Economy,
        Special
    }

    ///<summary>
    ///Types of requirements that can trigger challenge progress.
    ///</summary>
    public enum ChallengeRequirementType
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

    ///<summary>
    ///Difficulty tiers affecting challenge rewards and requirements.
    ///</summary>
    public enum ChallengeDifficulty
    {
        Easy,
        Normal,
        Hard,
        Extreme
    }
}





