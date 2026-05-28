/*
Program Name: SASZombieAssaultTD
File Path: Engine\Diagnostics\DiagnosticCategory.cs
Purpose: Core engine systems for diagnostics, difficulty, economy, and ECS management.
Features: Diagnostic logging, difficulty scaling, economic transactions, and entity component system debugging.
*/

/*
// File:    DiagnosticCategory.cs
// Path:    Engine / Diagnostics / DiagnosticCategory.cs
//
// Purpose: Defines standardized diagnostic categories for engine-wide logging.
//          Used by Engine.Diagnostics.DebugLogger to ensure consistent, 
//          deterministic diagnostic output across all subsystems.
//
// Role:    - Provides canonical enumeration for diagnostic message classification.
//          - Eliminates string drift and inconsistent prefixes.
//          - Enables filtering, routing, and structured log analysis.
//
// Integration: Used by DebugLogger.Log(DiagnosticCategory, string)
//              and all new or refactored engine subsystems.
//
// Standards: Engine-level infrastructure; fully qualified usage required.
//             Example:
//             Engine.Diagnostics.DebugLogger.Log(DiagnosticCategory.Debug, "Renderer initialized.");
//
// Notes:   This enum is forward-compatible and may be extended with additional
//          categories as new subsystems are introduced.
using SASZombieAssaultTD.Engine.Diagnostics;

*/

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Defines canonical diagnostic categories for engine-wide logging.
    /// Each category corresponds to a standardized prefix used by DebugLogger.
    /// </summary>
    /// <remarks>
    /// Diagnostic categories provide a consistent taxonomy for all engine logs.
    /// They are used to classify messages by severity and purpose, ensuring
    /// deterministic output and simplifying filtering and analysis.
    ///
    /// Usage Example:
    /// <code>
    /// Engine.Diagnostics.DebugLogger.Log(DiagnosticCategory.Debug, "Renderer initialized.");
    /// Engine.Diagnostics.DebugLogger.Log(DiagnosticCategory.Exception, ex.ToString());
    /// </code>
    /// </remarks>
    public enum DiagnosticCategory
    {
        /// <summary>
        /// Indicates a pass-through diagnostic where no processing or state change occurs.
        /// Used for methods that perform no work but must emit a trace for audit purposes.
        /// </summary>
        PassThru = 0,

        /// <summary>
        /// Indicates a trace-level diagnostic used for fine-grained execution flow tracking.
        /// Typically used for lifecycle boundaries and state transitions.
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Indicates a debug-level diagnostic used for general engine debugging output.
        /// Commonly used for initialization, configuration, and runtime state reporting.
        /// </summary>
        Debug = 2,

        /// <summary>
        /// Indicates a warning-level diagnostic used for recoverable issues or anomalies.
        /// These messages highlight potential problems but do not halt execution.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Indicates an error-level diagnostic used for non-recoverable failures.
        /// These messages typically precede exception handling or subsystem shutdown.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Indicates an exception-level diagnostic used to log caught exceptions.
        /// Always includes exception details and stack trace information.
        /// </summary>
        Exception = 5,
        Info = 6
    }
}
