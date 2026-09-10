// =====================================================================================================
//  FILE: ECSRuntimeWorld.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeWorld.cs
//  SUBSYSTEM: ECS ECSRuntime
//
//  ROLE:
//      Deterministic world‑lifecycle subsystem for ECSRuntimeCore.
//      Provides Reset() and Shutdown() behavior using ECSRuntimeEntities and ECSRuntimeSys.
//
//  RESPONSIBILITIES:
//      - Reset world state (entities, systems, counters)
//      - Shutdown world deterministically
//      - Provide diagnostic tracing
//
//  NON-RESPONSIBILITIES:
//      - Managing component storage directly
//      - Performing spatial or collision queries
//      - Executing update‑loop responsibilities
// =====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
{
    internal sealed class ECSRuntimeWorld
    {
        private readonly ECSRuntimeCore _runtime;

        public ECSRuntimeWorld(ECSRuntimeCore runtime)
        {
            _runtime = runtime;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 1, "Init", "ECSRuntimeWorld subsystem initialized.");
        }

        // ---------------------------------------------------------------------------------------------
        // Reset world
        // ---------------------------------------------------------------------------------------------
        public void Reset()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 2, "Reset", "Beginning ECS world reset.");

            // Destroy all entities
            _runtime.Entities.DestroyAll();

            // Clear systems
            foreach (var system in _runtime.Systems.Active)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 3, "Reset",
                    $"Removing system: {system.GetType().Name}");
            }
            _runtime.Systems.Clear();

            // Reset counters
            _runtime.Root.ResetCounters();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 4, "Complete", "ECS world reset completed.");
        }

        // ---------------------------------------------------------------------------------------------
        // Shutdown world
        // ---------------------------------------------------------------------------------------------
        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 5, "Shutdown", "Beginning ECS world shutdown.");

            // Destroy all entities
            _runtime.Entities.DestroyAll();

            // Clear systems
            _runtime.Systems.Clear();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.World", 6, "Complete", "ECS world shutdown completed.");
        }
    }
}
