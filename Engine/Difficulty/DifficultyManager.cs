//============================================================================
// Name        : DifficultyManager.cs
// File Path   : Engine\Difficulty\DifficultyManager.cs
// Author      : SASZombieAssaultTD
// Description : Manages game difficulty settings.
//============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Difficulty
{
    ///<summary>
    ///Manages game difficulty settings.
    ///</summary>
    public sealed class DifficultyManager
    {
        private static DifficultyManager? _instance;
        public static DifficultyManager Instance => _instance ??= new DifficultyManager();

        public DifficultyMode CurrentDifficulty { get; set; } = DifficultyMode.Normal;

        private DifficultyManager()
        {
            DLogger.Log(LogSubsystems.Difficulty,
                LogLevel.Info, LogCategory.Serializing,
                "DifficultyManager: Instantiated");

            // Additional initialization logic can be added here if needed.
        }
    }

    ///<summary>
    ///Difficulty modes for the game.
    ///</summary>
    public enum DifficultyMode
    {
        Easy,
        Normal,
        Hard,
        Expert
    }
}
