// =====================================================================================================
//  FILE: AchievementSubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/AchievementSubsystem.cs
//  SUBSYSTEM: GameRoot GameSave Subsystems
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    public class AchievementSubsystem
    {
        // Internal runtime-only achievement state.
        private readonly Dictionary<string, AchievementRuntimeState> _states
            = new Dictionary<string, AchievementRuntimeState>();

        // Called during GameRootMain.Initialize()
        public void Initialize(IEnumerable<string> achievementIds)
        {
            _states.Clear();

            foreach (var id in achievementIds)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    _states[id] = new AchievementRuntimeState(id);
                }
            }
        }

        // Called during GameRootMain.Update()
        public void ApplyProgress(string achievementId, int delta)
        {
            if (!_states.TryGetValue(achievementId, out var state))
                return;

            state.Progress += delta;

            if (state.Progress >= state.Required && !state.IsUnlocked)
            {
                state.IsUnlocked = true;
                state.UnlockFrame = state.CurrentFrame;
            }
        }

        // Called every frame by GameRootMain.Update()
        public void Tick(int frame)
        {
            foreach (var state in _states.Values)
            {
                state.CurrentFrame = frame;
            }
        }

        // Called during GameRootMain.Shutdown()
        public IReadOnlyDictionary<string, AchievementRuntimeState> GetRuntimeState()
        {
            return _states;
        }
    }
}
