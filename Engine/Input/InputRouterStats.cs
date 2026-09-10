// =====================================================================================================
//  FILE: InputRouterStats.cs
//  PATH: Engine/Input/InputRouterStats.cs
//  SUBSYSTEM: Core Input System
//
//  ROLE:
//      Immutable telemetry snapshot produced by the UIInputRouter each frame. Captures diagnostic
//      metrics related to keyboard tracking coverage, mouse event throughput, and router activity
//      state. Used by engine dashboards, developer tooling, and runtime performance monitors.
//
//  RESPONSIBILITIES:
//      - Report the number of unique key identifiers currently tracked in the router’s keyboard state.
//      - Report the cumulative count of mouse button events intercepted since engine startup.
//      - Report whether the routing subsystem is actively processing input traffic.
//
//  NON-RESPONSIBILITIES:
//      - Modifying input states or influencing device buffers.
//      - Managing configuration or lifecycle behavior of the input routing subsystem.
//      - Performing any form of input mapping, debouncing, or transitional state analysis.
//
//  NOTES:
//      This structure is intentionally minimal and purely informational. It is designed to be
//      lightweight, immutable, and safe to pass across subsystem boundaries without side effects.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Input
{
    /// <summary>
    /// Immutable telemetry snapshot describing input routing metrics for the current engine context.
    /// </summary>
    public sealed class InputRouterStats
    {
        /// <summary>
        /// Total number of unique key identifiers tracked inside the router’s keyboard state matrix.
        /// </summary>
        public int KeysTracked { get; init; }

        /// <summary>
        /// Total number of mouse button events (down/up) intercepted since engine startup.
        /// </summary>
        public int MouseEventsProcessed { get; init; }

        /// <summary>
        /// Indicates whether the input routing subsystem is currently enabled and processing traffic.
        /// </summary>
        public bool IsEnabled { get; init; }
    }
}
