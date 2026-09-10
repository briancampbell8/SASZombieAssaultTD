// =====================================================================================================
//  FILE: UnlockSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/UnlockSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Provides deterministic, subsystem-scoped logic for managing unlockable game content
//      during GameRoot execution. UnlockSubsystem participates in the GameRootMain lifecycle
//      but does not define or replace the engine-hosted sequencing contract. It exposes
//      controlled, frame-level operations for unlocking towers, abilities, levels, or rewards.
//
//  RESPONSIBILITIES:
//      - Maintain and update unlockable content flags in a deterministic and isolated manner.
//      - Participate in the GameRootMain lifecycle: Initialize → Tick → Shutdown.
//      - Provide controlled access to unlock states used by other subsystems.
//      - Serve as the foundation for future unlock-related runtime expansions.
//
//  NON-RESPONSIBILITIES:
//      - Defining the engine-hosted lifecycle contract or sequencing rules.
//      - Managing rendering, input, hardware, or cross-system orchestration.
//      - Performing save/load serialization or persistent state management.
//      - Implementing deep frame-level update logic for unrelated subsystems.
//
//  ARCHITECTURAL NOTES:
//      - Subsystems are orchestrated by GameRootMain and must remain single-class files.
//      - UnlockSubsystem will reference RuntimeState and RuntimeSummary programs, which you
//        will create and move manually into RuntimeStates/ or RuntimeSummaries/.
//      - Subsystems must not contain nested types or multi-class definitions.
//      - All subsystem logic MUST remain deterministic and isolated within this class.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeStates;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummaries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal sealed class UnlockSubsystem
    {
        private readonly HashSet<string> _unlockedItems = new HashSet<string>();
        private int _currentFrame;

        public void Initialize()
        {
            _unlockedItems.Clear();
            _currentFrame = 0;
        }

        public void Tick(int frame)
        {
            _currentFrame = frame;

            // Deterministic unlock logic goes here.
            // Example: unlocks triggered by score, level, or achievements.
        }

        public void Unlock(string itemId)
        {
            if (!string.IsNullOrWhiteSpace(itemId))
                _unlockedItems.Add(itemId);
        }

        public bool IsUnlocked(string itemId)
        {
            return _unlockedItems.Contains(itemId);
        }

        public IEnumerable<string> GetUnlockedItems()
        {
            return _unlockedItems;
        }

        // =====================================================================
        //  SHUTDOWN EXPORTS
        //  These references will resolve once you create the Runtime programs.
        // =====================================================================

        public UnlockRuntimeState ExportRuntimeState()
        {
            return new UnlockRuntimeState(
                _unlockedItems.ToList(),
                _currentFrame
            );
        }

        public UnlockRuntimeSummary ExportRuntimeSummary()
        {
            return new UnlockRuntimeSummary(
                _unlockedItems.ToList(),
                _currentFrame
            );
        }
    }
}
