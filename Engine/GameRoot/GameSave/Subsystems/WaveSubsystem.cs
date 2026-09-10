// =====================================================================================================
//  FILE: WaveSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/WaveSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped logic for managing and updating wave-related
//      runtime values during GameRoot execution. WaveSubsystem participates in the GameRootMain
//      lifecycle but does not define or replace the engine-hosted sequencing contract. It exposes
//      controlled, frame-level operations for wave progression, enemy counts, and timing.
//
//  RESPONSIBILITIES:
//      - Maintain and update wave-specific runtime values during active gameplay.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide deterministic, subsystem-contained logic without external side effects.
//      - Serve as the foundation for future wave-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing global engine assets, registration pools, or cross-system orchestration.
//      - Handling rendering, input processing, or hardware device boundaries.
//      - Performing save/load serialization or persistent state management.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - WaveSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================

using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class WaveSubsystem
    {
        private int _currentWave;
        private int _enemiesRemaining;
        private int _enemiesSpawned;
        private int _waveStartFrame;
        private int _currentFrame;

        public void Initialize(int startingWave)
        {
            _currentWave = startingWave;
            _enemiesRemaining = 0;
            _enemiesSpawned = 0;
            _waveStartFrame = 0;
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic wave logic goes here.
            // Example: spawn timing, wave completion checks, etc.
        }

        public void StartWave(int enemyCount)
        {
            _currentWave++;
            _enemiesRemaining = enemyCount;
            _enemiesSpawned = 0;
            _waveStartFrame = _currentFrame;
        }

        public void RegisterEnemySpawn()
        {
            _enemiesSpawned++;
        }

        public void RegisterEnemyDeath()
        {
            if (_enemiesRemaining > 0)
                _enemiesRemaining--;
        }

        public bool IsWaveComplete()
        {
            return _enemiesRemaining <= 0;
        }

        public int GetCurrentWave() => _currentWave;
        public int GetEnemiesRemaining() => _enemiesRemaining;
        public int GetEnemiesSpawned() => _enemiesSpawned;
        public int GetWaveStartFrame() => _waveStartFrame;
        public int GetCurrentFrame() => _currentFrame;

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public WaveRuntimeState ExportRuntimeState()
        {
            return new WaveRuntimeState(
                _currentWave,
                _enemiesRemaining,
                _enemiesSpawned,
                _waveStartFrame,
                _currentFrame
            );
        }

        public WaveRuntimeSummary ExportRuntimeSummary()
        {
            return new WaveRuntimeSummary(
                _currentWave,
                _enemiesRemaining,
                _enemiesSpawned,
                _waveStartFrame,
                _currentFrame
            );
        }
    }
}
