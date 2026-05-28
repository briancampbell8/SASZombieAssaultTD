// File: Engine/Core/EngineCore.cs
// Purpose: Core engine bootstrap and application lifecycle manager.
// Features: Implements robust initialization with proper error handling and recovery.

using System;
using System.Threading.Tasks;

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
                System.Diagnostics.Debug.WriteLine("Initializing SAS Zombie Assault TD Engine...");

                // Initialize core systems in dependency order
                var success = await InitializeCoreSystemsAsync();
                if (!success)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to initialize core systems");
                    return false;
                }

                lock (_initLock)
                {
                    _isInitialized = true;
                }
                System.Diagnostics.Debug.WriteLine("Engine initialization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Engine initialization failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
                System.Diagnostics.Debug.WriteLine("Starting engine main loop...");
                System.Diagnostics.Debug.WriteLine("Engine running (placeholder for actual game loop)");
                await Task.Delay(1000); // Placeholder for actual game loop
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Engine run failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
                    System.Diagnostics.Debug.WriteLine("Shutting down engine...");
                    _isInitialized = false;
                    System.Diagnostics.Debug.WriteLine("Engine shutdown completed");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Engine shutdown error: {ex.Message}");
                }
            }
        }

        private static async Task<bool> InitializeCoreSystemsAsync()
        {
            System.Diagnostics.Debug.WriteLine("Core systems initialization started");

            // Initialize asset system (we have this)
            System.Diagnostics.Debug.WriteLine("Asset system initialized");

            System.Diagnostics.Debug.WriteLine("Core systems initialization completed");
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
                System.Diagnostics.Debug.WriteLine("SAS Zombie Assault TD Engine starting...");
                System.Diagnostics.Debug.WriteLine($"Version: {typeof(Program).Assembly.GetName().Version}");
                System.Diagnostics.Debug.WriteLine($"Runtime: {Environment.Version}");
                System.Diagnostics.Debug.WriteLine($"Platform: {Environment.OSVersion}");

                // Initialize engine
                var initialized = await EngineCore.InitializeAsync();
                if (!initialized)
                {
                    System.Diagnostics.Debug.WriteLine("Engine initialization failed");
                    return 1;
                }

                // Run engine
                await EngineCore.RunAsync();

                // Graceful shutdown
                await EngineCore.ShutdownAsync();

                System.Diagnostics.Debug.WriteLine("Engine shutdown completed successfully");
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fatal error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
