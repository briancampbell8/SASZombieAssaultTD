// ============================================================================
// File:    Debug.cs
// Purpose: Engine-level diagnostic logging wrapper for unified logging.
//          Mirrors System.Diagnostics.Debug.WriteLine while keeping engine logging unified.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using System;

namespace Engine.Diagnostics
{
    public static class EngineDebug
    {
        /// <summary>
        /// Writes a simple message to the engine diagnostic output.
        /// Mirrors System.Diagnostics.Debug.WriteLine(string).
        /// </summary>
        public static void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes a leveled message to the engine diagnostic output.
        /// Mirrors the shape of Debug.WriteLine but adds a level prefix.
        /// </summary>
        public static void WriteLine(string level, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{level}] {message}");
        }

        internal static void Log(string v1, string v2)
        {
            NI.Hit();
        }
    }
}
