// =====================================================================================================
//  FILE: StateSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/StateSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped logic for managing and exposing high-level
//      game state values during GameRoot execution. StateSubsystem participates in the
//      GameRootMain lifecycle but does not define or replace the engine-hosted sequencing
//      contract. It acts as a centralized state authority for other subsystems.
//
//  RESPONSIBILITIES:
//      - Maintain global game state values in a deterministic and isolated manner.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide controlled access to shared state used by other GameSave subsystems.
//      - Serve as the foundation for future game-state-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing rendering, input, hardware, or cross-system orchestration.
//      - Performing save/load serialization or persistent state management.
//      - Implementing deep frame-level update logic for unrelated subsystems.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - StateSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummeries;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates;
namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class StateSubsystem
    {
        private int _currentLevel;
        private bool _isPaused;
        private int _currentFrame;

        public void Initialize(int startingLevel)
        {
            _currentLevel = startingLevel;
            _isPaused = false;
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic global state logic goes here.
            // Example: pause toggles, level transitions, etc.
        }

        public void SetPaused(bool paused)
        {
            _isPaused = paused;
        }

        public bool IsPaused()
        {
            return _isPaused;
        }

        public void AdvanceLevel()
        {
            _currentLevel++;
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }

        public int GetCurrentFrame()
        {
            return _currentFrame;
        }

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public StateRuntimeState ExportRuntimeState()
        {
            return new StateRuntimeState(
                _currentLevel,
                _isPaused,
                _currentFrame
            );
        }

        public StateRuntimeSummary ExportRuntimeSummary()
        {
            return new StateRuntimeSummary(
                _currentLevel,
                _isPaused,
                _currentFrame
            );
        }
    }
}
