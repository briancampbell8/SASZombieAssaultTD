/*
    File:    GameLoop.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Placeholder game loop wrapper that steps the GameRoot update/render.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Minimal placeholder game loop wrapper.
    /// </summary>
    public sealed class GameLoop
    {
        private readonly GameRoot _root;

        public GameLoop(GameRoot root)
        {
            _root = root;
        }

        public void Run()
        {
            // Single-frame step: GameRoot.RunMainLoop() owns the real loop.
            // This exists as a test harness for stepping one update/render cycle.
            const float deltaSeconds = 1f / 60f;

            _root.Update(deltaSeconds);
            _root.Render();
        }
    }
}