// ====================================================================================================
//  FILE: Program.cs
//  PATH: LogModernizer\Program.cs
//  PROGRAM: Program.cs
//  MODULE: Diagnostics & Engine Pipeline (Program)
//
//  ROLE:
//      Provides a deterministic execution runtime environment context.
//      Handles custom game state data transformations securely for LogModernizer.Tool.
//      Operates as a passive, zero-throw runtime loop component layer.
//
//  RESPONSIBILITIES:
//      - Provide deterministic engine pipeline handling execution logic.
//      - Maintain runtime flow and process core thread states safely.
//
//  NON-RESPONSIBILITIES:
//      - Direct rendering matrix mutations or UI canvas allocation tasks.
//      - File configurations and storage initialization hooks.
//
//  ARCHITECTURAL NOTES:
//      - Must never throw exceptions under any circumstances.
//      - Must never block engine execution; failures are silently ignored.
// ====================================================================================================

namespace LogModernizer.Tool
{
    internal static class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitInvalidArgs = 1;
        private const int ExitPathNotFound = 2;
        private const int ExitUnhandledError = 99;

        static int Main(string[] args)
        {
            Console.Title = "LogModernizer - Option B - Full Structured Rewrite";

            try
            {
                if (args.Length < 1)
                {
                    PrintUsage();
                    return ExitInvalidArgs;
                }

                string rootPath = args[0];

                if (!Directory.Exists(rootPath))
                {
                    Console.WriteLine($"[FATAL] Root path does not exist: {rootPath}");
                    return ExitPathNotFound;
                }

                bool dryRun = HasFlag(args, "--dry-run");
                bool verbose = HasFlag(args, "--verbose");

                Console.WriteLine("[INFO] LogModernizer starting...");
                Console.WriteLine($"[INFO] Root path: {rootPath}");
                Console.WriteLine($"[INFO] Dry run: {dryRun}");
                Console.WriteLine($"[INFO] Verbose: {verbose}");
                Console.WriteLine();

                var engine = new LogModernizer();

                int totalFiles = 0;
                int totalRewrittenCalls = 0;

                foreach (string file in Directory.EnumerateFiles(rootPath, "*.cs", SearchOption.AllDirectories))
                {
                    totalFiles++;

                    if (verbose)
                        Console.WriteLine($"[SCAN] {file}");

                    string originalSource = File.ReadAllText(file);
                    string rewrittenSource = engine.Rewrite(file, originalSource, out int rewrittenCount);

                    if (rewrittenCount > 0)
                    {
                        totalRewrittenCalls += rewrittenCount;

                        if (!dryRun)
                            File.WriteAllText(file, rewrittenSource);

                        Console.WriteLine($"[MODERNIZED] {file} ({rewrittenCount} calls)");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("[SUMMARY]");
                Console.WriteLine($"  Files scanned:         {totalFiles}");
                Console.WriteLine($"  Logging calls updated: {totalRewrittenCalls}");
                Console.WriteLine($"  Dry run:               {dryRun}");
                Console.WriteLine();
                Console.WriteLine("[INFO] LogModernizer completed.");

                return ExitSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FATAL] Unhandled exception in LogModernizer.");
                Console.WriteLine(ex);
                return ExitUnhandledError;
            }
        }

        private static bool HasFlag(string[] args, string flag)
        {
            foreach (var arg in args)
            {
                if (string.Equals(arg, flag, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("LogModernizer - Full Structured Rewrite (Option B)");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  LogModernizer <rootPath> [--dry-run] [--verbose]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  LogModernizer E:\\BDC\\Projects\\SASZombieAssaultTD\\Engine");
            Console.WriteLine("  LogModernizer E:\\BDC\\Projects\\SASZombieAssaultTD\\Engine --dry-run --verbose");
            Console.WriteLine();
        }
    }
}
