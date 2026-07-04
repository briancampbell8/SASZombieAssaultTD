/*
File:    NotImplementedTracker.cs
Folder:  Engine/Diagnostics/
Purpose: Global tracking + interception of ALL NotImplementedException calls.
Author:  BDC + Copilot
Date:    P11-Phase-Zero

Role:
    - Centralized NI tracking system
    - Logs file, line, member, subsystem
    - Integrates with DebugLogger + EngineDiagnostics
    - Provides deterministic forensic visibility
    - Replaces all "throw new NotImplementedException()" engine-wide

Usage:
    Replace:
        throw new NotImplementedException();

    With:
        NI.Hit();

    For return types:
        return NI.Hit<T>();
*/

using System;
using System.Runtime.CompilerServices;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Global interceptor for all NotImplementedException stubs.
    /// Automatically logs file, line, and member using Caller Info attributes.
    /// </summary>
    public static class NI
    {
        /// <summary>
        /// Tracks a void-returning NotImplemented stub.
        /// Logs full context and throws a descriptive exception.
        /// </summary>
        public static void Hit(
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            // Log to engine diagnostics
            DLogger.Log(
                "NOT_IMPLEMENTED",
                $"❌ NOT IMPLEMENTED → {file}:{line} → {member}"
            );

            // Throw with full context
            throw new NotImplementedException($"{file}:{line} → {member}");
        }

        /// <summary>
        /// Tracks a value-returning NotImplemented stub.
        /// Logs full context and throws a descriptive exception.
        /// </summary>
        public static T Hit<T>(
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            // Log to engine diagnostics
            DLogger.Log(
                "NOT_IMPLEMENTED",
                $"❌ NOT IMPLEMENTED → {file}:{line} → {member}"
            );

            // Throw with full context
            throw new NotImplementedException($"{file}:{line} → {member}");
        }
    }
}
