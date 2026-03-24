/*
File:    GetUpgradeInfoText.cs
Purpose: Generates descriptive text for upgrade tooltips.
*/

using System;
using System.Text;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Generates descriptive text for upgrade tooltips.
    /// </summary>
    public static class GetUpgradeInfoText
    {
        /// <summary>
        /// Generates basic upgrade info text.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <returns>Formatted upgrade info text.</returns>
        public static string GetBasicInfo(string upgradeName, string description, int cost)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine(description);
            sb.AppendLine($"<color=yellow>Cost: ${cost}</color>");
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates detailed upgrade info text with stats.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <param name="damageIncrease">Damage increase value.</param>
        /// <param name="rangeIncrease">Range increase value.</param>
        /// <param name="fireRateIncrease">Fire rate increase value.</param>
        /// <returns>Formatted upgrade info text with stats.</returns>
        public static string GetDetailedInfo(string upgradeName, string description, int cost, 
            float damageIncrease = 0, float rangeIncrease = 0, float fireRateIncrease = 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine(description);
            sb.AppendLine();
            
            if (damageIncrease > 0)
                sb.AppendLine($"<color=green>Damage: +{damageIncrease:F1}</color>");
            if (rangeIncrease > 0)
                sb.AppendLine($"<color=green>Range: +{rangeIncrease:F1}</color>");
            if (fireRateIncrease > 0)
                sb.AppendLine($"<color=green>Fire Rate: +{fireRateIncrease:F1}</color>");
                
            sb.AppendLine($"<color=yellow>Cost: ${cost}</color>");
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates upgrade info text for locked upgrades.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="requiredLevel">Required level to unlock.</param>
        /// <param name="prerequisite">Prerequisite upgrade name.</param>
        /// <returns>Formatted locked upgrade info text.</returns>
        public static string GetLockedInfo(string upgradeName, int requiredLevel, string prerequisite = "")
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine("<color=gray>LOCKED</color>");
            sb.AppendLine();
            sb.AppendLine($"<color=orange>Requires Level {requiredLevel}</color>");
            
            if (!string.IsNullOrEmpty(prerequisite))
            {
                sb.AppendLine($"<color=orange>Requires: {prerequisite}</color>");
            }
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates upgrade info text for unaffordable upgrades.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <param name="playerCash">Player's current cash.</param>
        /// <returns>Formatted unaffordable upgrade info text.</returns>
        public static string GetUnaffordableInfo(string upgradeName, string description, int cost, int playerCash)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine(description);
            sb.AppendLine();
            sb.AppendLine($"<color=red>Cost: ${cost}</color>");
            sb.AppendLine($"<color=red>You have: ${playerCash}</color>");
            sb.AppendLine($"<color=red>Need: ${cost - playerCash} more</color>");
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates upgrade info text for maxed upgrades.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <returns>Formatted maxed upgrade info text.</returns>
        public static string GetMaxedInfo(string upgradeName, string description)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine(description);
            sb.AppendLine();
            sb.AppendLine("<color=gold>MAXED OUT</color>");
            sb.AppendLine("<color=gold>Already at maximum level</color>");
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates upgrade info text with special abilities.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <param name="specialAbilities">List of special abilities.</param>
        /// <returns>Formatted upgrade info text with abilities.</returns>
        public static string GetInfoWithAbilities(string upgradeName, string description, int cost, string[] specialAbilities)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{upgradeName}</b>");
            sb.AppendLine(description);
            sb.AppendLine();
            
            if (specialAbilities != null && specialAbilities.Length > 0)
            {
                sb.AppendLine("<color=cyan>Special Abilities:</color>");
                foreach (var ability in specialAbilities)
                {
                    sb.AppendLine($"  • {ability}");
                }
                sb.AppendLine();
            }
            
            sb.AppendLine($"<color=yellow>Cost: ${cost}</color>");
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates upgrade info text based on status.
        /// </summary>
        /// <param name="upgradeName">Name of the upgrade.</param>
        /// <param name="description">Upgrade description.</param>
        /// <param name="cost">Upgrade cost.</param>
        /// <param name="status">Upgrade status.</param>
        /// <param name="playerCash">Player's current cash (for unaffordable status).</param>
        /// <param name="requiredLevel">Required level (for locked status).</param>
        /// <param name="prerequisite">Prerequisite upgrade (for locked status).</param>
        /// <returns>Appropriate formatted upgrade info text.</returns>
        public static string GetInfoByStatus(string upgradeName, string description, int cost, 
            UpgradeStatus status, int playerCash = 0, int requiredLevel = 0, string prerequisite = "")
        {
            return status switch
            {
                UpgradeStatus.Available => GetBasicInfo(upgradeName, description, cost),
                UpgradeStatus.Locked => GetLockedInfo(upgradeName, requiredLevel, prerequisite),
                UpgradeStatus.Maxed => GetMaxedInfo(upgradeName, description),
                UpgradeStatus.Unaffordable => GetUnaffordableInfo(upgradeName, description, cost, playerCash),
                _ => GetBasicInfo(upgradeName, description, cost)
            };
        }
    }
}