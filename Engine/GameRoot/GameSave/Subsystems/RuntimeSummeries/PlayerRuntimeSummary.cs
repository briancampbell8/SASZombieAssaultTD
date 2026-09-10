// =====================================================================================================
//  FILE: PlayerRuntimeSummary.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/RuntimeSummeries/PlayerRuntimeSummary.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped runtime logic for player-related state during
//      GameRoot execution. PlayerSubsystem participates in the GameRootMain lifecycle but does
//      not define or replace the engine-hosted sequencing contract. It exposes controlled,
//      frame-level operations and shutdown-time extraction surfaces for higher-level systems.
//
//  RESPONSIBILITIES:
//      - Maintain and update player-specific runtime values during active gameplay.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide deterministic, subsystem-contained logic without external side effects.
//      - Serve as the foundation for future player-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing global engine assets, registration pools, or cross-system orchestration.
//      - Handling rendering, input processing, or hardware device boundaries.
//      - Performing save/load serialization or persistent state management.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - PlayerSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    public class PlayerRuntimeSummary
    {
        private int playerId;
        private string playerName;
        private int score;
        private int currentFrame;

        public int PlayerId { get => playerId; set => playerId = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public int Score { get => score; set => score = value; }
        public int CurrentFrame { get => currentFrame; set => currentFrame = value; }



        public PlayerRuntimeSummary(int playerId, string playerName, int score, int currentFrame)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.score = score;
            this.currentFrame = currentFrame;
        }
    }
}
