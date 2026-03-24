/*
File:    GameWorld.cs
Purpose: Main game world container for entities.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Main game world container for entities.
    /// </summary>
    public sealed class GameWorld
    {
        private static GameWorld? _instance;
        public static GameWorld Instance => _instance ??= new GameWorld();
        
        private readonly List<object> _entities = new();
        
        private GameWorld()
        {
            ModernLoggingSystem.Log("INFO", "GameWorld: Initialized");
        }
        
        /// <summary>
        /// Adds an entity to the game world.
        /// </summary>
        public void AddEntity(object entity)
        {
            _entities.Add(entity);
            ModernLoggingSystem.Log("INFO", $"GameWorld: Added entity {entity.GetType().Name}");
        }
        
        /// <summary>
        /// Removes an entity from the game world.
        /// </summary>
        public void RemoveEntity(object entity)
        {
            _entities.Remove(entity);
            ModernLoggingSystem.Log("INFO", $"GameWorld: Removed entity {entity.GetType().Name}");
        }
    }
}
