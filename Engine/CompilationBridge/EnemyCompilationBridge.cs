/*
File:    EnemyCompilationBridge.cs
Purpose: Forces compilation of Enemy class and ensures namespace visibility.
Features: Creates explicit compilation dependencies to resolve CS0246 errors.
*/

//This file creates explicit compilation dependencies
//to ensure the Enemy class is properly compiled and accessible

using System;
using SASZombieAssaultTD.Engine.ECS;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.CompilationBridge
{
    ///<summary>
    ///Compilation bridge to ensure Enemy class is properly accessible.
    ///</summary>
    public static class EnemyCompilationBridge
    {
        ///<summary>
        ///Explicit reference to Enemy class to force compilation.
        ///</summary>
        private static readonly Type EnemyType = typeof(Enemy);
        
        ///<summary>
        ///Explicit reference to Entity class to force compilation.
        ///</summary>
        private static readonly Type EntityType = typeof(Entity);
        
        ///<summary>
        ///Creates an explicit dependency on Enemy class.
        ///</summary>
        public static Enemy CreateEnemyDependency()
        {
            //This creates an explicit compilation dependency
            //on the Enemy class to ensure it's properly compiled
            return Enemy.Create(new Entity(1));
        }
        
        ///<summary>
        ///Ensures Enemy namespace is properly loaded.
        ///</summary>
        public static void EnsureEnemyNamespaceLoaded()
        {
            //Force the compiler to load the Enemy namespace
            var enemy = CreateEnemyDependency();
            var entity = enemy.Entity;
            var id = enemy.Id;
        }
    }
}
