// ROLE: Enumeration of achievement categories.
// RESPONSIBILITY: Define categories for organizing and filtering achievements into logical 
//                  gameplay groupings.
// TRIGGERS: Referenced by AchievementDefinition and AchievementManager during setup.
// INPUTS: None (enum definition).
// OUTPUTS: Category values (General, Combat, Exploration, Collection, Progression, Special).
// DEPENDENCIES: None.
// CONTENTS: AchievementCategory enum with six category values.

namespace SASZombieAssaultTD.Engine.Achievements
{
    /// <summary>
    /// Categories for organizing achievements in the game.
    /// Used for UI filtering and player progression organization.
    /// </summary>
    public enum AchievementCategory
    {
        /// <summary>General achievements that don't fit other categories.</summary>
        General,
        /// <summary>Combat-related achievements (kills, damage, etc.).</summary>
        Combat,
        /// <summary>Exploration and discovery achievements.</summary>
        Exploration,
        /// <summary>Collection-based achievements (items, unlocks).</summary>
        Collection,
        /// <summary>Survival and endurance achievements.</summary>
        Survival,
        /// <summary>Special event or limited-time achievements.</summary>
        Special
    }
}
