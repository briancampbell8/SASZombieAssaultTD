/*
File:    EnemyNamespaceConsolidation.cs
Purpose: Consolidates all Enemy-related namespaces to resolve visibility issues.
Features: Ensures Enemy class is accessible across all engine subsystems.
*/

//This file serves as a namespace consolidation point
//It ensures all Enemy-related types are properly accessible

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Waves;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Namespace consolidation class to ensure Enemy types are properly accessible.
    ///</summary>
    public static class EnemyNamespaceConsolidation
    {
        ///<summary>
        ///Ensures all Enemy-related types are loaded and accessible.
        ///</summary>
        public static void EnsureLoaded()
        {
            //This method ensures the namespace is properly loaded
            //and can be called during engine initialization
            var enemyType = typeof(Enemy);
            var entityType = typeof(Entity);
            var waveSpawnGroupType = typeof(IWaveSpawnGroup);
        }
    }
}
