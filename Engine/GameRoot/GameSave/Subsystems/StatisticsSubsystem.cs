// =====================================================================================================
//  FILE: StatisticsSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/StatisticsSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped logic for tracking and updating gameplay
//      statistics during GameRoot execution. StatisticsSubsystem participates in the
//      GameRootMain lifecycle but does not define or replace the engine-hosted sequencing
//      contract. It exposes controlled, frame-level statistical aggregation surfaces for
//      higher-level systems.
//
//  RESPONSIBILITIES:
//      - Maintain and update gameplay statistics in a deterministic and isolated manner.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide controlled access to aggregated statistical values used by other subsystems.
//      - Serve as the foundation for future statistics-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing rendering, input, hardware, or cross-system orchestration.
//      - Performing save/load serialization or persistent state management.
//      - Implementing deep frame-level update logic for unrelated subsystems.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - StatisticsSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================

using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class StatisticsSubsystem
    {
        private int _totalEnemiesKilled;
        private int _totalDamageDealt;
        private int _totalDamageTaken;
        private int _totalShotsFired;
        private int _currentFrame;

        public void Initialize()
        {
            _totalEnemiesKilled = 0;
            _totalDamageDealt = 0;
            _totalDamageTaken = 0;
            _totalShotsFired = 0;
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic statistics logic goes here.
        }

        public void RegisterEnemyKill()
        {
            _totalEnemiesKilled++;
        }

        public void RegisterDamageDealt(int amount)
        {
            _totalDamageDealt += amount;
        }

        public void RegisterDamageTaken(int amount)
        {
            _totalDamageTaken += amount;
        }

        public void RegisterShotFired()
        {
            _totalShotsFired++;
        }

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public StatisticsRuntimeState ExportRuntimeState()
        {
            return new StatisticsRuntimeState(
                _totalEnemiesKilled,
                _totalDamageDealt,
                _totalDamageTaken,
                _totalShotsFired,
                _currentFrame
            );
        }

        public StatisticsRuntimeSummary ExportRuntimeSummary()
        {
            return new StatisticsRuntimeSummary(
                _totalEnemiesKilled,
                _totalDamageDealt,
                _totalDamageTaken,
                _totalShotsFired,
                _currentFrame
            );
        }
    }
}
