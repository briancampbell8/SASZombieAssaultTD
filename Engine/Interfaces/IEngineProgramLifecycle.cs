//====================================================================================================
//   FILE: IEngineProgramLifecycle.cs
//   PATH: Engine/GameRoot/IEngineProgramLifecycle.cs
//   SUBSYSTEM: Platform Abstraction Layer
//   ROLE: Canonical deterministic lifecycle contract for any engine-hosted program.
//
//   RESPONSIBILITIES:
//       - Define the strict engine-facing lifecycle sequence.
//       - Enforce predictable startup → update → render → shutdown semantics.
//       - Serve as the authoritative contract implemented by GameRoot.
//
//   ARCHITECTURAL NOTES:
//       - The engine owns the main loop and calls these methods deterministically.
//       - GameRoot implements this interface and delegates to its partial files.
//       - All engine-hosted programs MUST implement this interface without exception.
//====================================================================================================

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Canonical deterministic lifecycle contract for any engine-hosted program.
    /// GameRoot implements this interface.
    /// </summary>
    public interface IEngineProgramLifecycle
    {
        /// <summary>
        /// Perform all deterministic engine initialization.
        /// This MUST prepare all subsystems for Update() and Render().
        /// </summary>
        void Initialize();

        /// <summary>
        /// Perform a single deterministic update tick.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Perform a single deterministic render pass.
        /// </summary>
        void Render();

        /// <summary>
        /// Perform full engine shutdown.
        /// </summary>
        void Shutdown();
    }
}
