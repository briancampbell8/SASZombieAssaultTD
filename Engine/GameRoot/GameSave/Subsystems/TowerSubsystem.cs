// =====================================================================================================
//  FILE: TowerSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/TowerSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped logic for managing and updating tower-related
//      runtime values during GameRoot execution. TowerSubsystem participates in the GameRootMain
//      lifecycle but does not define or replace the engine-hosted sequencing contract. It exposes
//      controlled, frame-level operations for tower placement, upgrades, and combat behavior.
//
//  RESPONSIBILITIES:
//      - Maintain and update tower-specific runtime values during active gameplay.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide deterministic, subsystem-contained logic without external side effects.
//      - Serve as the foundation for future tower-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing global engine assets, registration pools, or cross-system orchestration.
//      - Handling rendering, input processing, or hardware device boundaries.
//      - Performing save/load serialization or persistent state management.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - TowerSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================

using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class TowerSubsystem
    {
        private int _towerId;
        private string _towerType;
        private int _level;
        private int _damage;
        private float _range;
        private int _shotsFired;
        private int _currentFrame;

        public void Initialize(int towerId, string towerType, int startingLevel, int baseDamage, float baseRange)
        {
            _towerId = towerId;
            _towerType = towerType;
            _level = startingLevel;
            _damage = baseDamage;
            _range = baseRange;
            _shotsFired = 0;
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic tower logic goes here.
            // Example: auto-fire, cooldown tracking, upgrade triggers, etc.
        }

        public void Upgrade(int damageIncrease, float rangeIncrease)
        {
            _level++;
            _damage += damageIncrease;
            _range += rangeIncrease;
        }

        public void RegisterShot()
        {
            _shotsFired++;
        }

        public int GetLevel() => _level;
        public int GetDamage() => _damage;
        public float GetRange() => _range;
        public int GetShotsFired() => _shotsFired;
        public int GetCurrentFrame() => _currentFrame;

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public TowerRuntimeState ExportRuntimeState()
        {
            return new TowerRuntimeState(
                _towerId,
                _towerType,
                _level,
                _damage,
                _range,
                _shotsFired,
                _currentFrame
            );
        }

        public TowerRuntimeSummary ExportRuntimeSummary()
        {
            return new TowerRuntimeSummary(
                _towerId,
                _towerType,
                _level,
                _damage,
                _range,
                _shotsFired,
                _currentFrame
            );
        }
    }
}
