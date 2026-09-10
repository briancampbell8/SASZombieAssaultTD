// =====================================================================================================
//  FILE: IECSSystem.cs
//  PATH: Engine/ECS/Systems/IECSSystem.cs
//  SUBSYSTEM: ECS System Contract
//
//  ROLE:
//      Defines the deterministic execution contract for all ECS systems. Every system participating
//      in the ECSRuntime update pipeline must implement this interface to guarantee stable ordering,
//      predictable update sequencing, and consistent frame-level behavior.
//
//  RESPONSIBILITIES:
//      - Expose a Priority value for deterministic system ordering.
//      - Provide Update(), FixedUpdate(), LateUpdate(), and Render() execution surfaces.
//      - Serve as the foundational contract for all ECSRuntimeSys-managed systems.
//
//  NON-RESPONSIBILITIES:
//      - Managing ECSEntityCore lifecycle or component storage.
//      - Performing world lifecycle operations.
//      - Handling event routing or subscription logic.
//      - Implementing ECSRuntime subsystem orchestration.
//
//  ARCHITECTURAL NOTES:
//      - ECSRuntimeSys sorts systems by Priority before execution.
//      - ECSRuntimeUpd invokes these methods in strict deterministic order.
//      - All ECS systems must implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.ECS
{
    public interface IECSSystem
    {
        int Priority { get; }

        void Update(float dt);
        void FixedUpdate(float dt);
        void LateUpdate(float dt);
        void Render();
    }
}
