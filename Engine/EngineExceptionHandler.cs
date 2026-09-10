// ====================================================================================================
//  FILE: EngineExceptionHandler.cs
//  PATH: ./Engine/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EngineExceptionHandler module.
//
//  RESPONSIBILITIES:
//      - Provide Initialize() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
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

