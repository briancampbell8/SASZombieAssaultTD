// ============================================================================
//  FILE: DLoggerLegacy.cs
//  MODULE: Diagnostics Pattern Library
//  PARTIAL: DLogger
//  PURPOSE:
//      Provides compatibility for ALL legacy logging patterns used by older
//      engine subsystems. This module ensures that outdated overloads,
//      formatting styles, and message conventions are safely normalized and
//      routed through the modern diagnostics pipeline.
//
//  FEATURES:
//      - Implements legacy logging overloads for backward compatibility.
//      - Accepts ANY legacy string message and normalizes it into modern format.
//      - Accepts ANY object and converts it to a diagnostic-safe string.
//      - Accepts ANY number of objects (NI patterns, debug dumps, varargs).
//      - Emits unique PatternTags for legacy overloads to ensure deterministic routing.
//      - Guarantees strict isolation: only legacy overloads are defined in this module.
//      - Produces fully normalized, Manager-ready output with no legacy formatting leakage.
//      - Ensures old engine subsystems continue functioning without modification.
//
//  LOCATION: Diagnostics/Patterns/
//  VERSION: 1.0 (Legacy Pattern Module)
// ============================================================================

using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        // ====================================================================
        // LEGACY OVERLOAD: string message
        // ====================================================================

        public static void Log(string category, string message)
        {
            string msg = message ?? "(null legacy message)";

            ResolveAndLog(
                LogSubsystems.Diagnostics,
                $"[INFO][{category}] {msg}",
                "Legacy_StringCategoryMessage",
                2
            );
        }

        // ====================================================================
        // LEGACY OVERLOAD: object
        // ====================================================================

        public static void Log(object anything)
        {
            string msg = anything?.ToString() ?? "(null legacy object)";

            ResolveAndLog(
                LogSubsystems.Diagnostics,
                $"[INFO][Generic] {msg}",
                "Legacy_Object",
                1
            );
        }

        // ====================================================================
        // LEGACY OVERLOAD: params object[]
        // ====================================================================

        public static void Log(params object[] items)
        {
            string combined =
                (items == null || items.Length == 0)
                ? "(empty legacy varargs)"
                : string.Join(" | ", items);

            ResolveAndLog(
                LogSubsystems.Diagnostics,
                $"[INFO][Multi] {combined}",
                "Legacy_VarArgs",
                items?.Length ?? 0
            );
        }

        // ====================================================================
        // LEGACY OVERLOAD: formatted string
        // ====================================================================

        public static void Log(string format, params object[] args)
        {
            string msg;

            try
            {
                msg = string.Format(format ?? "", args ?? Array.Empty<object>());
            }
            catch
            {
                msg = "(legacy format error)";
            }

            ResolveAndLog(
                LogSubsystems.Diagnostics,
                $"[INFO][Formatted] {msg}",
                "Legacy_Formatted",
                args?.Length ?? 0
            );
        }
    }
}
