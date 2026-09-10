// =====================================================================================================
//  FILE: EnemySubsystem.cs
//  PATH: Engine/GameRoot/GameSave/Subsystems/EnemySubsystem.cs
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
using System.Linq;
using SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems.RuntimeSummeries;

namespace SASZombieAssaultTD.Engine.GameRoot.GameSave.Subsystems
{
    internal partial class EnemySubsystem
    {
        // Runtime-only enemy shadow state.
        private readonly Dictionary<int, EnemyRuntimeState> _activeEnemies
            = new Dictionary<int, EnemyRuntimeState>();

        private int _nextId;
        private int _totalSpawned;
        private int _totalKilled;

        // Called during GameRootMain.Initialize()
        public void Initialize()
        {
            _activeEnemies.Clear();
            _nextId = 1;
            _totalSpawned = 0;
            _totalKilled = 0;
        }

        // Called during GameRootMain.Update()
        public void Tick(int frame)
        {
            foreach (var enemy in _activeEnemies.Values)
            {
                enemy.CurrentFrame = frame;

                // Deterministic placeholder for future runtime behavior.
                // No movement, AI, or damage logic here.
            }
        }

        // Runtime operations
        public int SpawnEnemy(string enemyType, int maxHealth)
        {
            var id = _nextId++;

            var state = new EnemyRuntimeState(id, enemyType, maxHealth);
            _activeEnemies[id] = state;

            _totalSpawned++;
            return id;
        }

        public void ApplyDamage(int enemyId, int amount)
        {
            if (!_activeEnemies.TryGetValue(enemyId, out var enemy))
                return;

            enemy.Health -= amount;

            if (enemy.Health <= 0)
            {
                enemy.Health = 0;
                enemy.IsDead = true;
                _activeEnemies.Remove(enemyId);
                _totalKilled++;
            }
        }

        public bool IsAlive(int enemyId)
        {
            return _activeEnemies.ContainsKey(enemyId);
        }

        public IReadOnlyDictionary<int, EnemyRuntimeState> GetActiveEnemies()
        {
            return _activeEnemies;
        }

        // Called during GameRootMain.Shutdown()
        public EnemyRuntimeSummary ExportRuntimeState()
        {
            return new EnemyRuntimeSummary(
                _totalSpawned,
                _totalKilled,
                _activeEnemies.Values.ToList()
            );
        }

        // Internal runtime-only enemy model.

    }
}
