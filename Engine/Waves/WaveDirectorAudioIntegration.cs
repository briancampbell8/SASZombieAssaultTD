// ====================================================================================================
//  FILE: WaveDirectorAudioIntegration.cs
//  PATH: Engine/Waves/
//  MODULE: Wave System Integration (Audio)
//
//  ROLE:
//  Bridges WaveDirector events to the audio subsystem for gameplay feedback.
//
//  RESPONSIBILITIES:
//  - Subscribe to WaveDirector lifecycle events and play corresponding sounds.
//  - Provide initialization guard to ensure hooks are attached once.
//
//  NON-RESPONSIBILITIES:
//  - Implementing audio playback primitives (delegated to ModernPlaySound/Audio subsystem).
//
//  ARCHITECTURAL NOTES:
//  - Lightweight static helper intended to be initialized at game startup.
// ====================================================================================================

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Enemies;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    ///<summary>
    ///Audio integration for WaveDirector.
    ///P100-04: WaveDirector audio integration with WaveAudioIntegration
    ///</summary>
    public static class WaveDirectorAudioIntegration
    {
        private static bool _isInitialized = false;

        ///<summary>
        ///Initialize audio integration with WaveDirector.
        ///</summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            var director = WaveDirector.Instance;
            if (director == null) return;

            //Hook into wave events
            director.OnWaveStarted += OnWaveStarted;
            director.OnWaveCompleted += OnWaveCompleted;
            director.OnEnemySpawned += OnEnemySpawned;
            director.OnAllWavesCompleted += OnAllWavesCompleted;
            director.OnGameComplete += OnGameComplete;

            _isInitialized = true;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveDirectorAudioIntegration: Initialized");
        }

        ///<summary>
        ///Shutdown audio integration.
        ///</summary>
        public static void Shutdown()
        {
            if (!_isInitialized) return;

            var director = WaveDirector.Instance;
            if (director == null) return;

            director.OnWaveStarted -= OnWaveStarted;
            director.OnWaveCompleted -= OnWaveCompleted;
            director.OnEnemySpawned -= OnEnemySpawned;
            director.OnAllWavesCompleted -= OnAllWavesCompleted;
            director.OnGameComplete -= OnGameComplete;

            _isInitialized = false;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveDirectorAudioIntegration: Shutdown");
        }

        private static void OnWaveStarted(int waveNumber)
        {
            WaveAudioIntegration.PlayWaveStart();
            WaveAudioIntegration.PlayWaveAnnouncement(waveNumber);
            WaveAudioIntegration.PlayWaveMusic();
            DLogger.Log($"WaveDirectorAudioIntegration: Wave {waveNumber} started");
        }

        private static void OnWaveCompleted(int waveNumber)
        {
            WaveAudioIntegration.PlayWaveComplete();
            WaveAudioIntegration.StopWaveMusic();
            DLogger.Log($"WaveDirectorAudioIntegration: Wave {waveNumber} completed");
        }

        private static void OnEnemySpawned(Enemy enemy)
        {
            if (enemy == null) return;

            EnemyAudioIntegration.PlayEnemySpawn(enemy.Position);

            //Play special sound for champion enemies
            if (enemy.IsChampion)
            {
                ModernPlaySound.Play("success_level_up", 1.2f);
                //TODO: Enemy.ChampionLevel doesn't exist - need to add this property or use different approach
                DLogger.Log($"WaveDirectorAudioIntegration: Champion enemy spawned");
            }
        }

        private static void OnAllWavesCompleted()
        {
            WaveAudioIntegration.StopWaveMusic();
            ModernPlaySound.Play("success_game_complete", 1.0f);
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveDirectorAudioIntegration: All waves completed");
        }

        private static void OnGameComplete()
        {
            WaveAudioIntegration.StopWaveMusic();
            ModernPlaySound.Play("success_game_complete", 1.0f);
            DLogger.Log(LogSubsystems.ResourcesPipeline, "WaveDirectorAudioIntegration: Game complete");
        }
    }
}
