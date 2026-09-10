// =====================================================================================================
//  FILE: IFactoryCreate.cs
//  PATH: Engine/ECS/ECSEntity/Factory/IFactoryCreate.cs
//  SUBSYSTEM: ECS ECSEntity Factory
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      Implemented by engine hosts (e.g., GameRootMain) to provide a clean, engine-facing API
//      for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Execute → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update logic or rendering commands.
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

namespace SASZombieAssaultTD.Engine.ECS.ECSEntity.Factory
{
    /// <summary>
    /// Minimal deterministic lifecycle contract for engine-hosted programs.
    /// </summary>
    internal interface IFactoryCreate
    {
        /// <summary>
        /// Deterministic startup entry point.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Deterministic execution entry point.
        /// </summary>
        void Execute();

        /// <summary>
        /// Deterministic shutdown entry point.
        /// </summary>
        void Shutdown();
    }
}
