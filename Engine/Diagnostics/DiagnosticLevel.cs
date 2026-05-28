/*
File:    DiagnosticLevel.cs
File Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\Diagnostics
Purpose: Canonical diagnostic severity levels for engine-wide logging and tracing.
*/

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Represents the severity level of a diagnostic event.
    /// Used across engine systems for logging, tracing, and NI instrumentation.
    /// </summary>
    public enum DiagnosticLevel
    {
        /// <summary>
        /// No diagnostics emitted.
        /// </summary>
        None = 0,

        /// <summary>
        /// Informational messages about normal operation.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Debug-level messages for troubleshooting.
        /// </summary>
        Debug = 2,

        /// <summary>
        /// Non-fatal issues that may require attention.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Errors indicating failures or broken invariants.
        /// </summary>
        Error = 4,

        /// <summary>
        /// High-volume, detailed diagnostic output.
        /// </summary>
        Verbose = 5
    }
}
