// =====================================================================================================
//  FILE: FactoryCreate.cs
//  PATH: Engine/GameRoot/GamePlay/Factory/FactoryCreate.cs
//  SUBSYSTEM: GameRoot GamePlay Factory
//
//  ROLE:
//      Provides the default no-op implementation of the IFactoryCreate lifecycle contract for
//      GamePlay-level hosts. This class exists to satisfy deterministic engine expectations when
//      a GamePlay host does not provide its own lifecycle module.
//
//  RESPONSIBILITIES:
//      - Supply a minimal, stable fallback implementation of the lifecycle contract.
//      - Ensure the GamePlay layer always has a valid lifecycle module available.
//      - Maintain deterministic sequencing: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing real initialization, execution, or shutdown logic.
//      - Managing systems, assets, or game states.
//      - Acting as a real program host or orchestrator.
//      - Creating enemies, projectiles, or other gameplay entities.
//
//  ARCHITECTURAL NOTES:
//      - GamePlay hosts should implement IFactoryCreate directly.
//      - This class is intentionally sealed and empty.
//      - The interface definition resides in IFactoryCreate.cs within the same Factory subsystem.
// =====================================================================================================

using SASZombieAssaultTD.Engine.ECS.ECSEntity.Factory;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay.Factory
{
    /// <summary>
    /// Default no-op implementation used when a GamePlay host does not provide its own lifecycle module.
    /// This class exists only to satisfy deterministic engine expectations.
    /// </summary>
    internal sealed class FactoryCreate : IFactoryCreate
    {
        public void Initialize()
        { }

        public void Execute()
        { }

        public void Shutdown()
        { }
    }
}
