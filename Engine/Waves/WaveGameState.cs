// FILE PATH: Engine/Waves/WaveGameState.cs
// EXECUTION TRIGGER: Instantiated by WaveManager during wave condition evaluation
// PROGRAM PURPOSE: Holds game state data for wave spawn condition checking including player level, towers, game speed, and pause state
// PROGRAM CALLS: None (data container)
// PROGRAM CONTENTS: WaveGameState class with PlayerLevel, BuiltTowers, GameSpeed, IsPaused properties and default constructor

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Game state for wave system.
    /// Contains all relevant state information for spawn condition evaluation.
    /// </summary>
    public class WaveGameState
    {
        /// <summary>
        /// Current player level.
        /// </summary>
        public int PlayerLevel { get; set; }

        /// <summary>
        /// Number of towers built by the player.
        /// </summary>
        public int BuiltTowers { get; set; }

        /// <summary>
        /// Current game speed multiplier.
        /// </summary>
        public float GameSpeed { get; set; }

        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Creates a new WaveGameState with default values.
        /// </summary>
        public WaveGameState()
        {
            PlayerLevel = 1;
            BuiltTowers = 0;
            GameSpeed = 1f;
            IsPaused = false;
        }
    }
}
