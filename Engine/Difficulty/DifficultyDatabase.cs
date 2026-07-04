/*
File:    DifficultyDatabase.cs
Purpose: Database for difficulty-related data.
*/

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Difficulty;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Difficulty
{
    ///<summary>
    ///Database for difficulty-related data.
    ///</summary>
    public static class DifficultyDatabase
    {
        ///<summary>
        ///Gets the difficulty multiplier for the specified difficulty mode.
        ///</summary>
        public static float GetMultiplier(DifficultyMode difficulty)
        {
            return difficulty switch
            {
                DifficultyMode.Easy => 0.8f,
                DifficultyMode.Normal => 1.0f,
                DifficultyMode.Hard => 1.5f,
                DifficultyMode.Expert => 2.0f,
                _ => 1.0f
            };
        }
    }
}
