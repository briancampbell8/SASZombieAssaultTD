//====================================================================================================
//  FILE: DiagnosticsManager.cs
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
//====================================================================================================



using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    internal static class DiagnosticsManager
    {
        public static string DatabasePath =>
            Path.Combine(AppContext.BaseDirectory, "Engine", "Diagnostics", "Diagnostics.db");
    }
}

