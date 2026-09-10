//====================================================================================================
// FILE: NotImplementedGuard.cs
// PATH: Engine/Diagnostics/NotImplementedGuard.cs
// SUBSYSTEM: Diagnostics
// ROLE: Centralized helper for making all NotImplemented paths visible, logged, and attributable.
//
// RESPONSIBILITIES:
//     - Provide a single, canonical entry point for all "not implemented" execution paths.
//     - Emit a high-severity diagnostic log entry for every hit, with clear contextual information.
//     - Throw a NotImplementedException to preserve correct control-flow semantics.
//
// NON-RESPONSIBILITIES:
//     - Deciding which features are implemented or not (owned by subsystems).
//     - Swallowing or recovering from NotImplementedException (owned by callers / Program.cs).
//
// ARCHITECTURAL NOTES:
//     - All future "not implemented" sites should call NotImplementedGuard.Hit(...) instead of
//       directly throwing NotImplementedException.
//     - This ensures every missing implementation is visible in EngineTrace.md and can be
//       prioritized and burned down systematically.
//==================================================================================================== 

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Centralized helper for logging and throwing all not-implemented execution paths.
    /// </summary>
    internal static class NotImplementedGuard
    {
        /// <summary>
        /// Records a critical diagnostic entry and throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="context">
        /// Human-readable context describing the missing implementation, typically in the form
        /// "TypeName.MethodName: short reason".
        /// </param>
        /// <remarks>
        /// This method must be used instead of directly throwing <see cref="NotImplementedException"/>
        /// to guarantee that every hit is visible in the diagnostics log.
        /// </remarks>
        public static void Hit(
           string context,
           [System.Runtime.CompilerServices.CallerFilePath] string file = "",
           [System.Runtime.CompilerServices.CallerMemberName] string member = "",
           [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            string programName = System.IO.Path.GetFileName(file);
            string className = System.IO.Path.GetFileNameWithoutExtension(file);

            string fullContext =
                $"{context} | Program={programName} | Class={className} | Member={member} | Line={line}";

            DLogger.Log(
                LogSubsystems.Diagnostics, LogEnums.LogLevel.Critical,
                "NOT_IMPLEMENTED", fullContext);

            throw new NotImplementedException(fullContext);
        }


    }
}
