/*
File:    DifficultyManager.cs
Purpose: Manages game difficulty settings.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Difficulty
{
    /// <summary>
    /// Manages game difficulty settings.
    /// </summary>
    public sealed class DifficultyManager
    {
        private static DifficultyManager? _instance;
        public static DifficultyManager Instance => _instance ??= new DifficultyManager();
        
        public DifficultyMode CurrentDifficulty { get; set; } = DifficultyMode.Normal;
        
        private DifficultyManager()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", "DifficultyManager: Initialized");
        }
    }
    
    /// <summary>
    /// Difficulty modes for the game.
    /// </summary>
    public enum DifficultyMode
    {
        Easy,
        Normal,
        Hard,
        Expert
    }
}
