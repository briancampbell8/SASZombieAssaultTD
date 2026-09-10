// ====================================================================================================
//  FILE: Debug.cs
//  PATH: Engine/Diagnostics/
//  PROGRAM: Debug.cs
//  MODULE: Diagnostics Pipeline
//
//  ROLE:
//      Provides lightweight development-time debugging helpers used by the engine.
//      Acts as a convenience layer for emitting quick diagnostic messages during development.
//
//  RESPONSIBILITIES:
//      - Offer simple, low-overhead debug output helpers.
//      - Forward debug messages into the unified diagnostics pipeline when appropriate.
//      - Support developers during engine bring-up, testing, and troubleshooting.
//
//  NON-RESPONSIBILITIES:
//      - Performing structured logging (handled by DLogger and Writer).
//      - Managing engine resources, assets, or gameplay logic.
//      - Handling rendering, audio, or subsystem operations.
//      - Generating reports or analytics.
//
//  ARCHITECTURAL NOTES:
//      - Debug helpers are optional and may be compiled out in release builds.
//      - All production diagnostics must flow through DLogger → Writer.
//      - Debug.cs should remain minimal and free of side effects outside diagnostics.
// ====================================================================================================
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class EngineDebug
    {
        ///<summary>
        ///Writes a simple message to the engine diagnostic output.
        ///Mirrors DLogger.Log(LogSubsystems.Diagnostics, LogLevel.Info, LogCategory.General, "string).         ///</summary>         public static void WriteLine(string message)         {             DLogger.Log(message");
        //}

        ///<summary>
        ///Writes a leveled message to the engine diagnostic output.
        ///Mirrors the shape of Debug.WriteLine but adds a level prefix.
        ///</summary>
        public static void WriteLine(string level, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics, LogEnums.LogLevel.Info, LogCategory.Diagnostics, $"[{level}] {message}");
        }

        internal static void Log(string v1, string v2)
        {
            NI.Hit();
        }
    }
}
