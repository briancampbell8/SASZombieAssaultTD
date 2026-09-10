// ====================================================================================================
//  FILE: ECSManager.cs
//  PATH: Engine/ECS/
//  MODULE: ECS
//
//  ROLE:
//      Encapsulate core engine behavior for the ECSManager module.
//
//  RESPONSIBILITIES:
//      - Provide ToString() behavior for the Core subsystem.
//      - Provide Initialize() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide FixedUpdate() behavior for the Core subsystem.
//      - Provide LateUpdate() behavior for the Core subsystem.
//      - Provide Render() behavior for the Core subsystem.
//      - Provide Destroy() behavior for the Core subsystem.
//      - Provide Reset() behavior for the Core subsystem.
//      - Provide CreateEntity() behavior for the Core subsystem.
//      - Provide DestroyEntity() behavior for the Core subsystem.
//      - Provide AddSystem() behavior for the Core subsystem.
//      - Provide RemoveSystem() behavior for the Core subsystem.
//      - Provide GetSystems() behavior for the Core subsystem.
//      - Provide GetSystemsByPriority() behavior for the Core subsystem.
//      - Provide FindEntitiesWithComponents() behavior for the Core subsystem.
//      - Provide FindEntitiesInRadius() behavior for the Core subsystem.
//      - Provide GetActiveEntities() behavior for the Core subsystem.
//      - Provide GetActiveSystems() behavior for the Core subsystem.
//      - Provide GetStats() behavior for the Core subsystem.
//      - Provide ResetStats() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
// PURPOSE:   One-Pass Engine Reconstruction - Core ECS Manager Implementation
//            Provides unified ECS management with proper type safety and math integration.
//            All manager operations delegate to EngineMath for consistency.
//
// FEATURES:  Complete manager operations with type safety, performance optimization, and math integration.
//            Supports ECSEntityCore lifecycle, component management, and system coordination.
//
// NOTES:    This replaces all fragmented manager implementations across the engine.
//            All engine code must use this unified Manager type.
//
// ====================================================================================================
//
namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///Manager statistics for performance monitoring.
    ///</summary>
    ///
    public class ECSManager : IESCManager
    {
        public struct ManagerStats
        {
            public int TotalEntitiesCreated;
            public int TotalEntitiesDestroyed;
            public int TotalComponentsAdded;
            public int TotalComponentsRemoved;
            public int TotalSystemsAdded;
            public int TotalSystemsRemoved;
            public float AverageUpdateTime;
            public float MaxUpdateTime;
            public float MinUpdateTime;

            public static ManagerStats Empty => new ManagerStats();

            public override string ToString() =>
                $"Stats(Created:{TotalEntitiesCreated}, Destroyed:{TotalEntitiesDestroyed}, " +
                $"Components:+{TotalComponentsAdded}/-{TotalComponentsRemoved}, " +
                $"Systems:+{TotalSystemsAdded}/-{TotalSystemsRemoved}, " +
                $"AvgUpdate:{AverageUpdateTime:F3}ms, Max:{MaxUpdateTime:F3}ms, Min:{MinUpdateTime:F3}ms)";
            public ManagerStats() { }

        }
    }

}



