// =====================================================================================================
//  FILE: PlayerSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/PlayerSubsystem.cs
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

using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummeries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class PlayerSubsystem
    {
        private int _playerId;
        private string _playerName;
        private int _maxHealth;
        private int _currentHealth;
        private int _score;
        private int _currentFrame;

        public void Initialize(int playerId, string playerName, int maxHealth)
        {
            _playerId = playerId;
            _playerName = playerName;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _score = 0;
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic player logic goes here.
        }

        public void AddScore(int amount)
        {
            _score += amount;
        }

        public void ApplyDamage(int amount)
        {
            _currentHealth -= amount;
            if (_currentHealth < 0)
                _currentHealth = 0;
        }

        public bool IsDead()
        {
            return _currentHealth <= 0;
        }

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public PlayerRuntimeState ExportRuntimeState()
        {
            return new PlayerRuntimeState(
                _playerId,
                _playerName,
                _maxHealth
            )
            {
                Health = _currentHealth,
                Score = _score,
                CurrentFrame = _currentFrame
            };
        }

        public PlayerRuntimeSummary ExportRuntimeSummary()
        {
            return new PlayerRuntimeSummary(
                _playerId,
                _playerName,
                _score,
                _currentFrame
            );
        }
    }
}
