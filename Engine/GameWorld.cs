/*
File:    GameWorld.cs
Purpose: Main game world container for entities.
*/

using System.Collections.Generic;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine
//
{
    ///<summary>
    ///Main game world container for entities.
    ///</summary>
    public sealed class GameWorld
    {
        private static GameWorld? _instance;
        public static GameWorld Instance => _instance ??= new GameWorld();

        private readonly List<object> _entities = new();

        private GameWorld()
        {
            DLogger.Log(LogSubsystems.Root, LogLevel.Info, "INFO", "GameWorld: Initialized");
        }

        ///<summary>
        ///Adds an entity to the game world.
        ///</summary>
        public void AddEntity(object entity)
        {
            _entities.Add(entity);
            DLogger.Log(LogSubsystems.Root, LogLevel.Info, "INFO", $"GameWorld: Added entity {entity.GetType().Name}");
        }

        ///<summary>
        ///Removes an entity from the game world.
        ///</summary>
        public void RemoveEntity(object entity)
        {
            _entities.Remove(entity);
            DLogger.Log(LogSubsystems.Root, LogLevel.Info, "INFO", $"GameWorld: Removed entity {entity.GetType().Name}");
        }
    }
}
