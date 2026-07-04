// ============================================================================
//  FILE: DLogger.cs
//  MODULE: Diagnostics Manager
//  PARTIAL: DLogger
//  PURPOSE:
//      Core diagnostics manager responsible for routing all logging requests
//      from pattern modules (Arg1–Arg7, Legacy, Custom) into the unified
//      diagnostics pipeline. This file contains NO overloads and NO patterns.
//      All pattern logic is defined in modular partial classes located in
//      Diagnostics/Patterns/.
//
//  NOTES:
//      - Provides universal prefix resolver and normalization helpers.
//      - Ensures deterministic routing for all pattern modules.
//      - Converts resolved data into DiagnosticEntry objects.
//      - Writes final entries to EngineTrace for downstream processing.
//      - Must remain minimal, stable, and free of pattern definitions.
//      - All overloads MUST be implemented in pattern modules only.
//
//  LOCATION: Diagnostics/
//  VERSION: 2.1 (Diagnostics Manager)
// ============================================================================

using System;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static partial class DLogger
    {
        // ====================================================================
        // COMPATIBILITY FORWARDER (NEW)
        // ====================================================================

        internal static void ForwardLegacyLog(string category, string message)
        {
            DLogger.Log(category, message);
        }

        internal static void ForwardLegacyLog(object anything)
        {
            SASZombieAssaultTD.Engine.Diagnostics.DLogger.Log(anything);
        }

        internal static void ForwardLegacyLog(params object[] items)
        {
            SASZombieAssaultTD.Engine.Diagnostics.DLogger.Log(items);
        }

        internal static void ForwardLegacyLog(string format, params object[] args)
        {
            SASZombieAssaultTD.Engine.Diagnostics.DLogger.Log(format, args);
        }

        // ====================================================================
        // NORMALIZATION HELPERS
        // ====================================================================

        internal static LogCategory NormalizeCategory(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return LogCategory.General;

            string t = tag.Trim().ToUpperInvariant();

            return t switch
            {
                "GENERAL" => LogCategory.General,
                "ANIMATION" => LogCategory.Animation,
                "RENDERING" => LogCategory.Rendering,
                "GAMEPLAY" => LogCategory.Gameplay,
                "AUDIO" => LogCategory.Audio,
                "NETWORKING" => LogCategory.Networking,
                "PHYSICS" => LogCategory.Physics,
                "AI" => LogCategory.AI,
                "SCRIPTING" => LogCategory.Scripting,
                "ENGINE" => LogCategory.Engine,
                "CONTENT" => LogCategory.Content,
                "INPUT" => LogCategory.Input,
                "DIAGNOSTICS" => LogCategory.Diagnostics,

                "ERROR" => LogCategory.Error,
                "EXCEPTION" => LogCategory.Exception,
                "DEBUG" => LogCategory.Debug,

                "GRAPHICS" => LogCategory.Rendering,
                "NETWORK" => LogCategory.Networking,
                "UI" => LogCategory.Gameplay,
                "FACTORY" => LogCategory.Content,

                "ENTITYCREATION" => LogCategory.EntityCreation,
                "RESOURCESLOADER" => LogCategory.ResourcesLoader,
                "RESOURCESCACHING" => LogCategory.ResourcesCaching,
                "RESOURCESVALIDATION" => LogCategory.ResourcesValidation,
                "DEVELOPERTOOLS" => LogCategory.DeveloperTools,
                "NAVIGATIONMIGRATION" => LogCategory.NavigationMigration,
                "FRAMEPRESENTATION" => LogCategory.FramePresentation,
                "TEXTUREMANAGEMENT" => LogCategory.TextureManagement,
                "INPUTPROCESSING" => LogCategory.InputProcessing,
                "AUDIOPLAYBACK" => LogCategory.AudioPlayback,
                "MONITORING" => LogCategory.Monitoring,
                "SYSTEM" => LogCategory.System,
                "VIDEO" => LogCategory.Video,

                _ => LogCategory.General
            };
        }

        internal static LogLevel NormalizeLevel(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return LogLevel.Info;

            string t = tag.Trim().ToUpperInvariant();

            return t switch
            {
                "TRACE" => LogLevel.Trace,
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Info,
                "INFORMATION" => LogLevel.Info,
                "WARN" => LogLevel.Warn,
                "WARNING" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                "FATAL" => LogLevel.Fatal,
                "CRITICAL" => LogLevel.Critical,
                "EXCEPTION" => LogLevel.Exception,
                "RECOVERY" => LogLevel.Recovery,
                _ => LogLevel.Info
            };
        }

        // ====================================================================
        // UNIVERSAL PREFIX RESOLVER
        // ====================================================================

        internal static void ResolveAndLog(
            LogSubsystems subsystem,
            string raw,
            string patternTag,
            int overloadArgs)
        {
            if (string.IsNullOrWhiteSpace(raw))
                raw = "(empty message)";

            string msg = raw.Trim();
            LogCategory category = LogCategory.General;
            LogLevel level = LogLevel.Info;

            while (msg.StartsWith("["))
            {
                int end = msg.IndexOf(']');
                if (end < 0)
                {
                    patternTag += "_MalformedPrefix";
                    break;
                }

                string tag = msg.Substring(1, end - 1);
                msg = msg.Substring(end + 1).Trim();

                var cat = NormalizeCategory(tag);
                if (cat != LogCategory.General)
                {
                    category = cat;
                    patternTag += $"_Category:{cat}";
                    continue;
                }

                var lvl = NormalizeLevel(tag);
                if (lvl != LogLevel.Info)
                {
                    level = lvl;
                    patternTag += $"_Level:{lvl}";
                    continue;
                }

                patternTag += $"_UnknownTag:{tag}";
            }

            var entry = new DiagnosticEntry(
                subsystem.ToString(),
                category.ToString()
            )
            {
                Message = msg,
                Priority = "Normal",
                Timestamp = System.DateTime.Now,
                Level = level.ToString(),
                CorrelationId = Guid.NewGuid().ToString(),
                RequestId = Guid.NewGuid().ToString(),
                Operation = "",
                PatternTag = patternTag,
                OverloadArgs = overloadArgs
            };

            EngineTrace.Write(entry);
        }

        // ====================================================================
        // SECONDARY RESOLVER
        // ====================================================================

        internal static void ResolveAndLog(
            LogSubsystems subsystem,
            LogLevel level,
            LogCategory category,
            string raw,
            string patternTag,
            int overloadArgs)
        {
            string synthetic = $"[{level}][{category}] {raw}";
            ResolveAndLog(subsystem, synthetic, patternTag, overloadArgs);
        }
    }
}
