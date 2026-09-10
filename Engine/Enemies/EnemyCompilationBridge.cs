// =====================================================================================================
//  FILE: EnemyCompilationBridge.cs
//  PATH: Engine/CompilationBridge/EnemyCompilationBridge.cs
//  SUBSYSTEM: CompilationBridge Core
//
//  ROLE:
//      Ensures the Enemy class and related ECS types are compiled and visible to the engine.
//      Provides deterministic type-loading behavior for compilation dependency resolution.
//
//  RESPONSIBILITIES:
//      - Force compilation of Enemy and ECSEntityCore types.
//      - Provide CreateEnemyDependency() for controlled type instantiation.
//      - Ensure namespace visibility for Enemy-related modules.
//
//  NON-RESPONSIBILITIES:
//      - Creating gameplay enemies or spawning runtime entities.
//      - Managing ECS component storage or world state.
//      - Performing any runtime logic beyond compilation dependency enforcement.
//
//  ARCHITECTURAL NOTES:
//      - Used strictly as a compilation bridge; not part of gameplay systems.
//      - Must rely on ECSRuntimeCore.Instance for valid ECSEntityCore creation.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Compilation bridge to ensure Enemy class is properly accessible.
    /// </summary>
    public static class EnemyCompilationBridge
    {
        private static readonly Type EnemyType = typeof(Enemy);
        private static readonly Type EntityType = typeof(ECSEntityCore);

        /// <summary>
        /// Creates an explicit dependency on Enemy class.
        /// This does NOT create a real runtime entity.
        /// It only forces the compiler to instantiate the type.
        /// </summary>
        public static Enemy CreateEnemyDependency()
        {
            // Use the ECSRuntimeCore instance to create a proper entity
            var runtime = ECSRuntimeCore.Instance;

            if (runtime == null)
                throw new InvalidOperationException(
                    "ECSRuntimeCore.Instance is null. EnemyCompilationBridge requires an initialized runtime.");

            // Create a real entity using the runtime
            var entity = runtime.CreateEntity();

            // Create the Enemy using the proper ECS entity
            return Enemy.Create(entity);
        }

        /// <summary>
        /// Ensures Enemy namespace is properly loaded.
        /// </summary>
        public static void EnsureEnemyNamespaceLoaded()
        {
            var enemy = CreateEnemyDependency();
            var entity = enemy.ECSEntityCore;
            var id = enemy.Id;

            // No-op: this method only forces type loading
        }
    }
}
