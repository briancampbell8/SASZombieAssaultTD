/*
File: IProgram.cs
Author: BDC
Created: 2026-02-08

Purpose:
    Defines the minimal contract for a runnable engine program.
    GameRoot implements this interface to provide a deterministic
    startup → run → shutdown lifecycle.

Notes:
    This interface intentionally contains only the core lifecycle
    methods required by the engine. No rendering or asset logic
    belongs here.
*/

namespace SASZombieAssaultTD.Engine.Core
{
    public interface IProgram
    {
        /// <summary>
        /// Called once at application startup.
        /// Use this to initialize engine systems.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called once per frame by the host loop.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        void Update(System.TimeSpan deltaTime);

        /// <summary>
        /// Called once per frame after Update().
        /// Rendering is performed through the active IRenderContext.
        /// </summary>
        void Render();

        /// <summary>
        /// Called once when the application is shutting down.
        /// </summary>
        void Shutdown();
    }
}