/* ====================================================================================================
 *  FILE: IProgram.cs
 *  PATH: Engine/Platform/IProgram.cs
 *  SUBSYSTEM: Platform Abstraction Layer
 *  ROLE: Defines the deterministic lifecycle contract for any engine-hosted program.
 *
 *  RESPONSIBILITIES:
 *      - Provide a strict, minimal, engine-facing lifecycle interface.
 *      - Enforce a predictable startup → update → render → shutdown sequence.
 *      - Serve as the contract implemented by GameRoot (or any future program host).
 *
 *  NON-RESPONSIBILITIES:
 *      - Containing game logic.
 *      - Containing rendering logic.
 *      - Managing systems, assets, or state machines.
 *
 *  ARCHITECTURAL NOTES:
 *      - The engine owns the main loop and calls these methods deterministically.
 *      - GameRoot implements this interface and delegates to its partial files.
 *      - All engine-hosted programs MUST implement this interface without exception.
 * ==================================================================================================== */

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Platform
{
    ///<summary>
    ///Defines the minimal deterministic lifecycle contract for any engine-hosted program.
    ///Implemented by <see cref="GameRoot"/> to provide a clean, engine-facing API.
    ///</summary>
    public interface IProgram
    {
        ///<summary>
        ///Called exactly once at engine startup.
        ///Used to initialize all game-level systems and state.
        ///</summary>
        void Initialize();

        ///<summary>
        ///Called once per frame by the engine's host loop.
        ///Used to advance game logic, systems, and state machines.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since the previous frame.</param>
        void Update(TimeSpan deltaTime);

        ///<summary>
        ///Called once per frame after <see cref="Update"/>.
        ///Used to issue all rendering commands through the active render context.
        ///</summary>
        void Render();

        ///<summary>
        ///Called exactly once when the engine is shutting down.
        ///Used to cleanly release resources and stop all systems.
        ///</summary>
        void Shutdown();
    }
}
