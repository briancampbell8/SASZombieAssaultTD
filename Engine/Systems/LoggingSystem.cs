using System;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Minimal logging system for engine bring-up and debugging.
    /// Can later be redirected to files, overlays, or external sinks.
    /// </summary>
    public static class LoggingSystem
    {
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}";
            Debug.WriteLine(line);
            Console.WriteLine(line);
        }
    }
}
