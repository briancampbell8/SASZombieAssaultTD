/*
    File:    DebugOverlay.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Placeholder debug overlay for showing FPS and diagnostics.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System.Diagnostics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Simple debug overlay for showing FPS and diagnostic info.
    /// </summary>
    public sealed class DebugOverlay
    {
        private readonly FrameStats _stats;

        public DebugOverlay(FrameStats stats)
        {
            _stats = stats;
        }

        public void Render()
        {
            DebugLogger.Log("BREAKPOINT", "Execution reached here");
            DebugLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DebugLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, " +
                $"Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
            // Placeholder: real rendering backend would draw text here.
            // This keeps the engine compiling and ready for DebugSession-0001.
            DebugLogger.Log("BREAKPOINT", "End of Breakpoint");
        }
    }
}