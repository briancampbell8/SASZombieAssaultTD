// ============================================================================
// PROGRAM: MarkdownLogWriter.cs
// FILE PATH E:\SASZombieAssaultTD\Engine\Diagnostics
// PURPOSE: Writes engine diagnostic output to a Markdown file for forensic
//          analysis, debugging, and post-run review.
// FEATURES:
//   - Creates timestamped Markdown log files.
//   - Buffers log entries in memory before flushing to disk.
//   - Writes structured sections for each diagnostic level.
//   - Ensures the Logs directory exists before writing.
// RESPONSIBILITIES:
//   - Provide a deterministic Markdown sink for DebugLogger.
//   - Maintain a stable file format for external tools and analysis.
//   - Avoid formatting drift or uncontrolled file growth.
// INTERACTIONS:
//   - Called exclusively by DebugLogger.
//   - Does not perform any filtering or routing logic.
// NOTES:
//   - File is flushed on every write to prevent data loss on crash.
//   - Safe for multi-run usage; each run generates a new file.
// ============================================================================

using System;
using System.IO;
using System.Text;

namespace SASZombieAssaultTD.Engine.Diagnostics
{
    /// <summary>
    /// Writes structured Markdown log output to a timestamped file.
    /// </summary>
    public sealed class MarkdownLogWriter
    {
        // Stores the full path to the log file created for this run.
        private readonly string _filePath;

        // In-memory buffer used to accumulate Markdown content before flushing.
        private readonly StringBuilder _buffer = new();

        /// <summary>
        /// Initializes the writer and prepares the log directory and file.
        /// </summary>
        public MarkdownLogWriter(string directory = "Logs")
        {
            // Ensure the log directory exists before writing.
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Generate a timestamped filename for this run.
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            _filePath = Path.Combine(directory, $"log_{timestamp}.md");

            // Write the initial header block to the buffer.
            WriteHeader();
        }

        /// <summary>
        /// Writes the top-of-file Markdown header containing run metadata.
        /// </summary>
        private void WriteHeader()
        {
            _buffer.AppendLine("# Engine Diagnostic Log");
            _buffer.AppendLine($"**Run Timestamp:** {DateTime.Now}");
            _buffer.AppendLine($"**Session ID:** {Guid.NewGuid()}");
            _buffer.AppendLine("\n---\n");
        }

        /// <summary>
        /// Appends a structured log entry to the Markdown buffer.
        /// </summary>
        public void Write(string level, string message)
        {
            // Write the diagnostic level section header.
            _buffer.AppendLine($"## {level}");

            // Write the timestamped message entry.
            _buffer.AppendLine($"- [{DateTime.Now:HH:mm:ss}] {message}");
            _buffer.AppendLine();

            // Flush the updated buffer to disk.
            Flush();
        }

        /// <summary>
        /// Writes the current buffer contents to the log file.
        /// </summary>
        private void Flush()
        {
            File.WriteAllText(_filePath, _buffer.ToString());
        }
    }
}
