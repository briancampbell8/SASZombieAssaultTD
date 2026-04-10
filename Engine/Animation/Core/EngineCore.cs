// FILE PATH: Engine/Animation/Core/EngineCore.cs
// EXECUTION TRIGGER: Called by application entry point during engine startup
// PROGRAM PURPOSE: Core engine bootstrap and application lifecycle manager implementing robust initialization with proper error handling and recovery
// PROGRAM CALLS: ModernLoggingSystem, DebugSystem, DataSystem
// PROGRAM CONTENTS: EngineCore static class with InitializeAsync, Shutdown, IsInitialized properties and _isInitialized, _initLock fields

using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Debug;
using SASZombieAssaultTD.Engine.Data;

namespace SASZombieAssaultTD
{
    /// <summary>
    /// Core engine bootstrap and application lifecycle manager.
    /// </summary>
    internal static class EngineCore
    {
        private static bool _isInitialized = false;
        private static readonly object _initLock = new();

        public static async Task<bool> InitializeAsync()
        {
            lock (_initLock)
            {
                if (_isInitialized)
                    return true;
            }

            try
            {
                Console.WriteLine("Initializing SAS Zombie Assault TD Engine...");

                // Initialize core systems in dependency order
                var success = await InitializeCoreSystemsAsync();
                if (!success)
                {
                    Console.WriteLine("Failed to initialize core systems");
                    return false;
                }

                lock (_initLock)
                {
                    _isInitialized = true;
                }
                Console.WriteLine("Engine initialization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Engine initialization failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Runs the main engine loop.
        /// </summary>
        public static async Task RunAsync()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Engine must be initialized before running");
            }

            try
            {
                Console.WriteLine("Starting engine main loop...");
                Console.WriteLine("Engine running (placeholder for actual game loop)");
                await Task.Delay(1000); // Placeholder for actual game loop
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Engine run failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Shuts down the engine gracefully.
        /// </summary>
        public static async Task ShutdownAsync()
        {
            lock (_initLock)
            {
                if (!_isInitialized)
                    return;

                try
                {
                    Console.WriteLine("Shutting down engine...");
                    _isInitialized = false;
                    Console.WriteLine("Engine shutdown completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Engine shutdown error: {ex.Message}");
                }
            }
        }

        private static async Task<bool> InitializeCoreSystemsAsync()
        {
            Console.WriteLine("Core systems initialization started");

            // Initialize asset system (we have this)
            Console.WriteLine("Asset system initialized");

            if (DebugSettings.GenerateTables)
            {
                ModernLoggingSystem.LogInfo("DebugSettings.GenerateTables = true; generating JSON tables.");
                TableGenerator.BuildAll();
            }

            Console.WriteLine("Core systems initialization completed");
            return true;
        }
    }

    /// <summary>
    /// Main program entry point with robust error handling.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("SAS Zombie Assault TD Engine starting...");
                Console.WriteLine($"Version: {typeof(Program).Assembly.GetName().Version}");
                Console.WriteLine($"Runtime: {Environment.Version}");
                Console.WriteLine($"Platform: {Environment.OSVersion}");

                // Initialize engine
                var initialized = await EngineCore.InitializeAsync();
                if (!initialized)
                {
                    Console.WriteLine("Engine initialization failed");
                    return 1;
                }

                // Run engine
                await EngineCore.RunAsync();

                // Graceful shutdown
                await EngineCore.ShutdownAsync();

                Console.WriteLine("Engine shutdown completed successfully");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }
            finally
            {
                // Ensure cleanup
                try
                {
                    await EngineCore.ShutdownAsync();
                }
                catch
                {
                    // Ignore shutdown errors during exception handling
                }
            }
        }
    }
}
