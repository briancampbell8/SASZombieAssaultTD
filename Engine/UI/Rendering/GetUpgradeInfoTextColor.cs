/*
File:    GetUpgradeInfoTextColor.cs
Purpose: Returns color coding for upgrade tooltip text.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Returns color coding for upgrade tooltip text.
    /// </summary>
    public static class GetUpgradeInfoTextColor
    {
        /// <summary>
        /// Gets color for title text.
        /// </summary>
        /// <returns>White color for titles.</returns>
        public static Color Title()
        {
            return Color.White;
        }
        
        /// <summary>
        /// Gets color for description text.
        /// </summary>
        /// <returns>Light gray color for descriptions.</returns>
        public static Color Description()
        {
            return Color.LightGray;
        }
        
        /// <summary>
        /// Gets color for cost text.
        /// </summary>
        /// <returns>Yellow color for cost information.</returns>
        public static Color Cost()
        {
            return Color.Yellow;
        }
        
        /// <summary>
        /// Gets color for positive stat text.
        /// </summary>
        /// <returns>Green color for positive stats.</returns>
        public static Color PositiveStat()
        {
            return Color.Green;
        }
        
        /// <summary>
        /// Gets color for negative stat text.
        /// </summary>
        /// <returns>Red color for negative stats.</returns>
        public static Color NegativeStat()
        {
            return Color.Red;
        }
        
        /// <summary>
        /// Gets color for requirement text.
        /// </summary>
        /// <returns>Orange color for requirements.</returns>
        public static Color Requirement()
        {
            return Color.Orange;
        }
        
        /// <summary>
        /// Gets color for locked text.
        /// </summary>
        /// <returns>Gray color for locked items.</returns>
        public static Color Locked()
        {
            return Color.Gray;
        }
        
        /// <summary>
        /// Gets color for maxed text.
        /// </summary>
        /// <returns>Gold color for maxed items.</returns>
        public static Color Maxed()
        {
            return Color.Gold;
        }
        
        /// <summary>
        /// Gets color for special ability text.
        /// </summary>
        /// <returns>Cyan color for special abilities.</returns>
        public static Color SpecialAbility()
        {
            return Color.Cyan;
        }
        
        /// <summary>
        /// Gets color for warning text.
        /// </summary>
        /// <returns>Red color for warnings.</returns>
        public static Color Warning()
        {
            return Color.Red;
        }
        
        /// <summary>
        /// Gets color for info text.
        /// </summary>
        /// <returns>Light blue color for info.</returns>
        public static Color Info()
        {
            return Color.LightBlue;
        }
        
        /// <summary>
        /// Gets color based on text type.
        /// </summary>
        /// <param name="textType">Type of text.</param>
        /// <returns>Appropriate color for the text type.</returns>
        public static Color GetColor(UpgradeTextType textType)
        {
            return textType switch
            {
                UpgradeTextType.Title => Title(),
                UpgradeTextType.Description => Description(),
                UpgradeTextType.Cost => Cost(),
                UpgradeTextType.PositiveStat => PositiveStat(),
                UpgradeTextType.NegativeStat => NegativeStat(),
                UpgradeTextType.Requirement => Requirement(),
                UpgradeTextType.Locked => Locked(),
                UpgradeTextType.Maxed => Maxed(),
                UpgradeTextType.SpecialAbility => SpecialAbility(),
                UpgradeTextType.Warning => Warning(),
                UpgradeTextType.Info => Info(),
                _ => Description()
            };
        }
        
        /// <summary>
        /// Gets color based on upgrade status.
        /// </summary>
        /// <param name="status">Upgrade status.</param>
        /// <returns>Appropriate color for the upgrade status.</returns>
        public static Color GetStatusColor(UpgradeStatus status)
        {
            return status switch
            {
                UpgradeStatus.Available => PositiveStat(),
                UpgradeStatus.Locked => Locked(),
                UpgradeStatus.Maxed => Maxed(),
                UpgradeStatus.Unaffordable => Warning(),
                UpgradeStatus.InProgress => SpecialAbility(),
                UpgradeStatus.Selected => Cost(),
                UpgradeStatus.PrerequisitesNotMet => Requirement(),
                _ => Description()
            };
        }
    }
    
    /// <summary>
    /// Enumeration for upgrade text types.
    /// </summary>
    public enum UpgradeTextType
    {
        Title,
        Description,
        Cost,
        PositiveStat,
        NegativeStat,
        Requirement,
        Locked,
        Maxed,
        SpecialAbility,
        Warning,
        Info
    }
}