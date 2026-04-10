// ROLE: Defines achievement rarity levels.
// RESPONSIBILITY: Indicate difficulty and prestige levels for achievements affecting 
//                  point values and visual presentation.
// TRIGGERS: Referenced by AchievementDefinition and AchievementManager during setup.
// INPUTS: None (enum definition).
// OUTPUTS: Rarity values (Common, Rare, Epic, Legendary).
// DEPENDENCIES: None.
// CONTENTS: AchievementRarity enum with four rarity values.

namespace SASZombieAssaultTD.Engine.Achievements
{
    /// <summary>
    /// Rarity levels indicating achievement difficulty and prestige.
    /// Affects point values and visual presentation.
    /// </summary>
    public enum AchievementRarity
    {
        /// <summary>Standard achievements, easily obtainable.</summary>
        Common,
        /// <summary>Moderate difficulty achievements.</summary>
        Rare,
        /// <summary>Difficult achievements requiring skill or time.</summary>
        Epic,
        /// <summary>Extremely challenging or rare achievements.</summary>
        Legendary
    }
}




