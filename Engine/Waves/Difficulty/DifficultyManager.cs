// =====================================================================================================
//  FILE: DifficultyManager.cs
//  PATH: Engine/Waves/Difficulty/DifficultyManager.cs
//  SUBSYSTEM: Waves/Difficulty Subsystem
//
//  ROLE:
//      Acts as the authoritative runtime holder of the active difficulty tier.
//      Provides deterministic access to DifficultySettings for all wave-related subsystems.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Waves.Difficulty.DifficultyConfig;
using static SASZombieAssaultTD.Engine.Waves.Difficulty.DifficultyEnums;

namespace SASZombieAssaultTD.Engine.Waves.Difficulty
{


    public interface IDifficultyProvider
    {
        DifficultySettings DifficultySettings { get; set; }

        DifficultyLevel GetCurrentDifficultyLevel();
        DifficultyConfig GetDifficultySettings();

        void SetDifficulty(DifficultyLevel newDifficulty);
    }

    public class Instance
    {
        public static object CurrentDifficulty;
        public DifficultyLevel _currentDifficulty;

        public Instance(DifficultyLevel initialDifficulty) => _currentDifficulty = initialDifficulty;

        public Instance() => _currentDifficulty = DifficultyLevel.Normal;

        public DifficultyLevel GetCurrentDifficultyLevel()
        {
            return _currentDifficulty;
        }


        public void SetDifficulty(DifficultyLevel newDifficulty)
        {
            _currentDifficulty = newDifficulty;
        }

        public DifficultySettings DifficultySettings =>
            // Maps cleanly to your standalone DifficultyConfig file using the correct enum types
            _currentDifficulty switch
            {
                DifficultyLevel.Easy => DifficultyConfig.Get(DifficultyMode.Easy),
                DifficultyLevel.Normal => DifficultyConfig.Get(DifficultyMode.Normal),
                DifficultyLevel.Hard => DifficultyConfig.Get(DifficultyMode.Hard),
                DifficultyLevel.Insane => DifficultyConfig.Get(DifficultyMode.Insane),
                _ => DifficultyConfig.Get(DifficultyMode.Normal)
            };

        //private static DifficultyManager? _instance;
        //public static DifficultyManager Instance => _instance ??= new DifficultyManager();

        private DifficultyMode currentDifficulty = DifficultyMode.Normal;

        public DifficultyMode GetCurrentDifficulty()
        {
            return currentDifficulty;
        }

        public void SetCurrentDifficulty(DifficultyMode value)
        {
            currentDifficulty = value;
        }

        public Instance(DifficultyMode currentDifficulty)
        {
            _currentDifficulty = DifficultyLevel.Normal;
            // _CurrentDifficulty = currentDifficulty;

            DLogger.Log(LogSubsystems.Difficulty,
                   LogEnums.LogLevel.Info, LogCategory.Serializing,
                   "DifficultyManager: Instantiated");
        }
    }



}
