// ====================================================================================================
//  FILE: DiagnosticsBackupManager.cs
//  PATH: Engine/Diagnostics/
//  MODULE: Diagnostics Pipeline (Log Rotation & Backup Management)
//
//  ROLE:
//      Performs deterministic log rotation at the beginning of each engine run. This module moves
//      the previous run's EngineTrace.md and DiagnosticsReport.md into the ReportBackups directory,
//      renaming them with a timestamp suffix. It then creates fresh empty log files for the new run.
//      A cleanup pass removes backup files older than the retention window.
//
//  RESPONSIBILITIES:
//      - Rotate EngineTrace.md and DiagnosticsReport.md into timestamped backups.
//      - Guarantee that rotation occurs BEFORE any diagnostic entry is written.
//      - Create fresh empty log files for the new run.
//      - Clean up backup files older than the retention window (7 days).
//      - Guarantee that all operations NEVER throw and NEVER block engine flow.
//      - Maintain deterministic behavior aligned with the diagnostics pipeline.
//
//  NON-RESPONSIBILITIES:
//      - Writing diagnostic entries (EngineTraceWriter, MarkdownLogWriter).
//      - Diagnostics formatting (Writer.cs).
//      - Message counting (DiagnosticsMessageCounter.cs).
//      - Diagnostics table management (DiagnosticsTableLoader.cs).
//      - UI or report viewing (future Windows Forms modules).
//
//  ARCHITECTURAL NOTES:
//      - This module MUST run before any DLogger call.
//      - It MUST NEVER throw exceptions; failures MUST degrade gracefully.
//      - It MUST NEVER interfere with log writing or diagnostics pipeline flow.
//      - Timestamped backups allow historical run inspection without polluting active logs.
// ====================================================================================================

using System;
using System.IO;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    public static class DiagnosticsBackupManager
    {
        private const int RetentionDays = 7;

        public static void BackupAndReset()
        {
            try
            {
                // Determine runtime folder (bin/Debug/net8.0-windows/)
                string runtimeDir = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location
                );

                // Walk up 3 levels: net8.0-windows → Debug → bin → project root
                string projectRoot = Path.GetFullPath(
                    Path.Combine(runtimeDir, @"..\..\..")
                );

                string logsDir = Path.Combine(projectRoot, "Engine", "Reporting", "Logs");
                string backupDir = Path.Combine(projectRoot, "Engine", "Reporting", "ReportBackups");

                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);

                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                // Cleanup old backups first
                CleanupOldBackups(backupDir);

                // Files to rotate
                string engineTrace = Path.Combine(logsDir, "EngineTrace.md");
                string diagnosticsReport = Path.Combine(logsDir, "DiagnosticsReport.md");

                // Timestamp suffix
                string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                // Backup EngineTrace.md
                if (File.Exists(engineTrace))
                {
                    string backupName = $"EngineTrace_{stamp}.md";
                    File.Move(engineTrace, Path.Combine(backupDir, backupName));
                }

                // Backup DiagnosticsReport.md
                if (File.Exists(diagnosticsReport))
                {
                    string backupName = $"DiagnosticsReport_{stamp}.md";
                    File.Move(diagnosticsReport, Path.Combine(backupDir, backupName));
                }

                // Create fresh empty files for the new run
                File.WriteAllText(engineTrace, "");
                File.WriteAllText(diagnosticsReport, "");
            }
            catch
            {
                // Diagnostics pipeline must never throw.
            }
        }

        private static void CleanupOldBackups(string backupDir)
        {
            try
            {
                var files = Directory.GetFiles(backupDir, "*.md");

                foreach (var file in files)
                {
                    DateTime modified = File.GetLastWriteTime(file);
                    if ((DateTime.Now - modified).TotalDays > RetentionDays)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Cleanup is best-effort and must never throw.
            }
        }
    }
}
