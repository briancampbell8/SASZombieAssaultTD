// ====================================================================================================
//  FILE: DLoggerLog.cs
//  PATH: ./Engine/Diagnostics/
//  MODULE: Diagnostics
//
//  ROLE:
//      Provide logging, profiling, or diagnostic instrumentation.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Diagnostics subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
// ============================================================================
//  FILE: DLoggerLog.cs
//  MODULE: Diagnostics Compatibility Bridge
//  PARTIAL: DLogger
//  PURPOSE:
//      Provides compatibility forwarding for legacy DLogger.Log(...) calls
//      without defining duplicate overloads. Prevents ambiguity errors by
//      exposing uniquely named forwarding methods.
// ============================================================================
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        internal static void ForwardLog(string category, string message)
        {
            DLogger.Log(LogSubsystems.Diagnostics,
                category, message);
        }

        internal static void ForwardLog(object anything)
        {
            DLogger.Log(anything);
        }

        internal static void ForwardLog(params object[] items)
        {
            DLogger.Log(items);
        }

        internal static void ForwardLog(string format, params object[] args)
        {
            DLogger.Log(format, args);
        }
    }
}

