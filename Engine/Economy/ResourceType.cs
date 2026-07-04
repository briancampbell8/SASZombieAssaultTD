/*
File:    ResourceType.cs
Purpose: Defines different resource types for the economy system.
Features: Resource enumeration, metadata, and conversion rates.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Economy
{
    ///<summary>
    ///Enumeration of different resource types in the game economy.
    ///</summary>
    public enum ResourceType
    {
        ///<summary>
        ///Primary currency for purchasing towers and upgrades.
        ///</summary>
        Cash = 0,

        ///<summary>
        ///Alternative currency, often used for premium content.
        ///</summary>
        Gold = 1,

        ///<summary>
        ///Credits earned from achievements and special events.
        ///</summary>
        Credits = 2,

        ///<summary>
        ///Experience points for player progression.
        ///</summary>
        Experience = 3,

        ///<summary>
        ///Special currency for limited-time offers.
        ///</summary>
        Tokens = 4
    }

    ///<summary>
    ///Provides metadata and utilities for resource types.
    ///</summary>
    public static class ResourceTypeExtensions
    {
        ///<summary>
        ///Gets the display name for a resource type.
        ///</summary>
        ///<param name="resourceType">The resource type</param>
        ///<returns>Display name</returns>
        public static string GetDisplayName(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Cash => "Cash",
                ResourceType.Gold => "Gold",
                ResourceType.Credits => "Credits",
                ResourceType.Experience => "Experience",
                ResourceType.Tokens => "Tokens",
                _ => "Unknown"
            };
        }

        ///<summary>
        ///Gets the description for a resource type.
        ///</summary>
        ///<param name="resourceType">The resource type</param>
        ///<returns>Description</returns>
        public static string GetDescription(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Cash => "Primary currency for purchasing towers and upgrades",
                ResourceType.Gold => "Premium currency for special purchases",
                ResourceType.Credits => "Earned from achievements and special events",
                ResourceType.Experience => "Gained from gameplay for player progression",
                ResourceType.Tokens => "Limited-time event currency",
                _ => "Unknown resource type"
            };
        }

        ///<summary>
        ///Gets the icon path for a resource type.
        ///</summary>
        ///<param name="resourceType">The resource type</param>
        ///<returns>Icon path</returns>
        public static string GetIconPath(this ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Cash => "ui/icons/cash.png",
                ResourceType.Gold => "ui/icons/gold.png",
                ResourceType.Credits => "ui/icons/credits.png",
                ResourceType.Experience => "ui/icons/experience.png",
                ResourceType.Tokens => "ui/icons/tokens.png",
                _ => "ui/icons/unknown.png"
            };
        }

        ///<summary>
        ///Checks if a resource type is a premium currency.
        ///</summary>
        ///<param name="resourceType">The resource type</param>
        ///<returns>True if premium</returns>
        public static bool IsPremium(this ResourceType resourceType)
        {
            return resourceType == ResourceType.Gold || resourceType == ResourceType.Tokens;
        }

        ///<summary>
        ///Checks if a resource type is a primary currency.
        ///</summary>
        ///<param name="resourceType">The resource type</param>
        ///<returns>True if primary</returns>
        public static bool IsPrimary(this ResourceType resourceType)
        {
            return resourceType == ResourceType.Cash;
        }
    }
}
