// ====================================================================================================
//  FILE: DifficultyDatabase.cs
//  PATH: ./Engine/Difficulty/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the DifficultyDatabase module.
//
//  RESPONSIBILITIES:
//      - Provide GetMultiplier() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    DifficultyDatabase.cs
Purpose: Database for difficulty-related data.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
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

