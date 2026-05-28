// ============================================================================
// File: ProgressionCore.cs
// FilePath: Engine/Player/ProgressionCore.cs
// Purpose: Defines deterministic rules for player progression. Provides all
// mathematical operations, validation rules, level clamping, XP clamping, and
// tower unlock thresholds. Contains no state and performs no event dispatch.
// Integration:
//   - PlayerProgressionController calls all methods in this type.
//   - Save/load systems do not interact with this type directly.
//   - PlayerSystem receives events triggered by the controller, not here.
// Data Flow:
//   - Input: level values, XP values, XP additions.
//   - Output: validated XP, clamped levels, next-level XP requirements,
//             tower unlock thresholds.
// ============================================================================

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Player
{
    internal static class ProgressionCore
    {
        // --------------------------------------------------------------------
        // Constants
        // --------------------------------------------------------------------

        public const int MAX_LEVEL = 100;                 // Hard cap for level progression
        public const int MAX_EXPERIENCE_ADDITION = 1_000_000; // Upper bound for XP increments

        // --------------------------------------------------------------------
        // Tower Unlock Table
        // Purpose: Defines required levels for each tower unlock.
        // Integration: PlayerProgressionController queries this table during
        // CheckTowerUnlocks().
        // --------------------------------------------------------------------

        public static readonly Dictionary<string, int> TowerUnlockLevels =
            new Dictionary<string, int>
            {
                { "BasicGun", 1 },
                { "Sniper",   3 },
                { "Flame",    5 },
                { "Rocket",   7 }
            };

        // --------------------------------------------------------------------
        // XP Curve
        // Purpose: Computes XP required for the next level.
        // Data Flow: Controller calls this during initialization, level-up,
        // and restore operations.
        // --------------------------------------------------------------------

        public static int CalculateExperienceForNextLevel(int level)
        {
            if (level >= MAX_LEVEL)
                return int.MaxValue;                      // Max-level sentinel

            long value = 100L * level * level;            // Quadratic XP curve
            return value > int.MaxValue ? int.MaxValue : (int)value;
        }

        // --------------------------------------------------------------------
        // XP Validation
        // Purpose: Ensures XP increments fall within allowed bounds.
        // Integration: Called by AddExperience() before applying XP.
        // --------------------------------------------------------------------

        public static bool IsValidExperienceAddition(int amount)
        {
            return amount > 0 && amount <= MAX_EXPERIENCE_ADDITION;
        }

        // --------------------------------------------------------------------
        // Level Clamping
        // Purpose: Ensures level values remain within valid bounds.
        // Integration: Used during level-up and restore operations.
        // --------------------------------------------------------------------

        public static int ClampLevel(int level)
        {
            if (level < 1) return 1;
            if (level > MAX_LEVEL) return MAX_LEVEL;
            return level;
        }

        // --------------------------------------------------------------------
        // XP Clamping
        // Purpose: Ensures XP values remain within valid bounds for the level.
        // Integration: Used during level-up and restore operations.
        // --------------------------------------------------------------------

        public static int ClampExperience(int experience, int experienceToNextLevel)
        {
            if (experience < 0) return 0;
            if (experienceToNextLevel <= 0) return experience;
            if (experience > experienceToNextLevel) return experienceToNextLevel;
            return experience;
        }
    }
}
