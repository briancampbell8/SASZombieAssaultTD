//============================================================================
// FILE: Engine/Diagnostics/DiagnosticsManager.cs
// FILE PATH: Engine\Diagnostics\DiagnosticsManager.cs
// PURPOSE: Manages the path to the diagnostics database.
//
// VERSION: 1.0
// DATE: 2026-06-26
//============================================================================

using System;
using System.IO;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    internal static class DiagnosticsManager
    {
        public static string DatabasePath =>
            Path.Combine(AppContext.BaseDirectory, "Engine", "Diagnostics", "Diagnostics.db");
    }
}
