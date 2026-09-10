// =====================================================================================================
//  FILE: StateRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummeries/StateRuntimeSummary.cs
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

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummeries
{
    public class StateRuntimeSummary
    {
        private int currentLevel;
        private bool isPaused;
        private int currentFrame;

        public StateRuntimeSummary(int currentLevel, bool isPaused, int currentFrame)
        {
            this.currentLevel = currentLevel;
            this.isPaused = isPaused;
            this.currentFrame = currentFrame;
        }
    }
}
