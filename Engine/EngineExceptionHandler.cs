using System;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Global exception routing for the engine.
    /// Ensures all unhandled exceptions are forwarded to DLogger.
    /// </summary>
    public static class EngineExceptionHandler
    {
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            // AppDomain-level unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    DLogger.Log(LogSubsystems.System, ex);
                }
            };

            // TaskScheduler unobserved task exceptions
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                DLogger.Log(LogSubsystems.System, args.Exception);
                args.SetObserved();
            };
        }
    }
}
