// ====================================================================================================
//  FILE: DeathType.cs
//  PATH: ./Engine/Gameplay/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the DeathType module.
//
//  RESPONSIBILITIES:
//      - Provide GetDisplayName() behavior for the Core subsystem.
//      - Provide GetScoreMultiplier() behavior for the Core subsystem.
//      - Provide HasSpecialEffects() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    DeathType.cs
Purpose: Enumeration for different types of death causes in SAS Zombie Assault TD.
Features: Complete death type classification for combat system, kill feed, and statistics.
Used by: KillFeedSystem, combat calculations, death animations, scoring system.
*/

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay
{
    ///<summary>
    ///Enumeration representing different types of death causes for entities.
    ///Used by the combat system to classify kills, trigger appropriate death animations,
    ///calculate scores, and populate the kill feed with appropriate messages.
    ///</summary>
    public enum DeathType
    {
        ///<summary>
        ///Death caused by bullet/projectile weapons (towers, firearms).
        ///Most common death type for standard tower defense gameplay.
        ///</summary>
        Bullet = 0,

        ///<summary>
        ///Death caused by fire-based weapons (flamethrowers, fire towers).
        ///Triggers burning death animations and fire-related sound effects.
        ///</summary>
        Fire = 1,

        ///<summary>
        ///Death caused by melee/close combat attacks.
        ///Used for special melee towers or close-range enemies.
        ///</summary>
        Melee = 2,

        ///<summary>
        ///Death caused by explosive weapons (mortars, rocket launchers).
        ///Triggers explosion effects and area damage calculations.
        ///</summary>
        Explosion = 3,

        ///<summary>
        ///Death caused by electrical weapons (tesla coils, lightning towers).
        ///Triggers electrocution effects and chain lightning.
        ///</summary>
        Electric = 4,

        ///<summary>
        ///Death caused by poison/toxic damage over time.
        ///Used for poison towers and environmental hazards.
        ///</summary>
        Poison = 5,

        ///<summary>
        ///Death caused by ice/freeze weapons.
        ///Triggers shattering effects and frozen death animations.
        ///</summary>
        Freeze = 6,

        ///<summary>
        ///Death caused by falling from height or environmental hazards.
        ///Used for map-specific death scenarios.
        ///</summary>
        Fall = 7,
        
        ///<summary>
        ///Death caused by drowning in water or other liquids.
        ///Used for water-related death scenarios.
        ///</summary>
        Drowning = 8,

        ///<summary>
        ///Death caused by player actions or abilities.
        ///Special case for player-triggered deaths.
        ///</summary>
        Player = 8,

        ///<summary>
        ///Death caused by suicide or self-destruction.
        ///Used for enemies with explosive death mechanics.
        ///</summary>
        Suicide = 9,

        ///<summary>
        ///Death caused by unknown or unspecified reasons.
        ///Default fallback death type.
        ///</summary>
        Unknown = 10,
        
        ///<summary>
        ///Death caused by other miscellaneous reasons.
        ///Used for death types that don't fit other categories.
        ///</summary>
        Other = 11,
        
        Environmental = 12
    }

    ///<summary>
    ///Extension methods for DeathType enum providing additional functionality.
    ///</summary>
    public static class DeathTypeExtensions
    {
        ///<summary>
        ///Gets a human-readable display name for the death type.
        ///</summary>
        ///<param name="deathType">The death type.</param>
        ///<returns>Formatted display string.</returns>
        public static string GetDisplayName(this DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Bullet => "Shot",
                DeathType.Fire => "Burned",
                DeathType.Melee => "Bludgeoned",
                DeathType.Explosion => "Exploded",
                DeathType.Electric => "Electrocuted",
                DeathType.Poison => "Poisoned",
                DeathType.Freeze => "Frozen",
                DeathType.Environmental => "Killed",
                DeathType.Player => "Eliminated",
                DeathType.Suicide => "Self-Destructed",
                DeathType.Unknown => "Died",
                _ => "Unknown"
            };
        }

        ///<summary>
        ///Gets the score multiplier for this death type.
        ///</summary>
        ///<param name="deathType">The death type.</param>
        ///<returns>Score multiplier (1.0 = normal, higher = bonus).</returns>
        public static float GetScoreMultiplier(this DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Bullet => 1.0f,
                DeathType.Fire => 1.2f,
                DeathType.Melee => 1.5f,
                DeathType.Explosion => 1.3f,
                DeathType.Electric => 1.4f,
                DeathType.Poison => 1.1f,
                DeathType.Freeze => 1.3f,
                DeathType.Environmental => 0.8f,
                DeathType.Player => 2.0f,
                DeathType.Suicide => 0.5f,
                DeathType.Unknown => 1.0f,
                _ => 1.0f
            };
        }

        ///<summary>
        ///Determines if this death type should trigger special effects.
        ///</summary>
        ///<param name="deathType">The death type.</param>
        ///<returns>True if special effects should be triggered.</returns>
        public static bool HasSpecialEffects(this DeathType deathType)
        {
            return deathType is DeathType.Explosion or DeathType.Electric or DeathType.Fire or DeathType.Freeze;
        }
    }
}

