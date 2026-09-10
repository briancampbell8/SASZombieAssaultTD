// =====================================================================================================
//  FILE: ECSRuntimeUpd.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeUpd.cs
//  SUBSYSTEM: ECS ECSRuntime
//
//  ROLE:
//      Deterministic update‑pipeline executor for ECSRuntimeCore.
//      Calls Update(), FixedUpdate(), LateUpdate(), and Render() on all registered systems
//      in strict priority order.
//
//  RESPONSIBILITIES:
//      - Execute Update(), FixedUpdate(), LateUpdate(), Render() across all IECSSystem instances.
//      - Maintain deterministic ordering guarantees using system.Priority.
//      - Provide diagnostic tracing for frame‑level behavior.
//
//  NON-RESPONSIBILITIES:
//      - Managing ECSEntityCore lifecycle or component storage.
//      - Performing spatial or collision queries.
//      - Allocating or mutating system collections.
//      - Executing physics or rendering logic directly.
// =====================================================================================================
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
{
    internal sealed class ECSRuntimeUpd
    {
        private readonly ECSRuntimeSys _systems;

        public ECSRuntimeUpd(ECSRuntimeSys systems)
        {
            _systems = systems;
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 1, "Init", "ECSRuntimeUpd subsystem initialized.");
        }

        // ---------------------------------------------------------------------------------------------
        // Update
        // ---------------------------------------------------------------------------------------------
        public void Update(ECSRuntimeCore runtime, float dt)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 2, "Update",
                $"Beginning Update() pass. DeltaTime={dt}");

            foreach (var system in _systems.ByPriority)
            {
                system.Update(dt);
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 3, "Complete", "Update() pass completed.");
        }

        // ---------------------------------------------------------------------------------------------
        // FixedUpdate
        // ---------------------------------------------------------------------------------------------
        public void FixedUpdate(ECSRuntimeCore runtime, float dt)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 4, "FixedUpdate",
                $"Beginning FixedUpdate() pass. FixedDeltaTime={dt}");

            foreach (var system in _systems.ByPriority)
            {
                system.FixedUpdate(dt);
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 5, "Complete", "FixedUpdate() pass completed.");
        }

        // ---------------------------------------------------------------------------------------------
        // LateUpdate
        // ---------------------------------------------------------------------------------------------
        public void LateUpdate(ECSRuntimeCore runtime, float dt)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 6, "LateUpdate",
                $"Beginning LateUpdate() pass. DeltaTime={dt}");

            foreach (var system in _systems.ByPriority)
            {
                system.LateUpdate(dt);
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 7, "Complete", "LateUpdate() pass completed.");
        }

        // ---------------------------------------------------------------------------------------------
        // Render
        // ---------------------------------------------------------------------------------------------
        public void Render(ECSRuntimeCore runtime)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 8, "Render",
                "Beginning Render() pass.");

            foreach (var system in _systems.ByPriority)
            {
                system.Render();
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Updates", 9, "Complete", "Render() pass completed.");
        }
    }
}
