/*
File:    EnemySpawnedEvent.cs
Purpose: Event fired when an enemy is spawned in the game.
Features: Enemy reference, spawn position, spawn timestamp.

P11-04-07-B: Enemy spawn event for ECS integration and event system.
*/

using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.ECS;
using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Events
{
    ///<summary>
    ///Event fired when an enemy is spawned in the game.
    ///Used for tracking enemy spawns and triggering related game events.
    ///</summary>
    public class EnemySpawnedEvent
    {
        ///<summary>
        ///The enemy that was spawned.
        ///</summary>
        public Enemy Enemy { get; set; }

        ///<summary>
        ///Type identifier of the spawned enemy.
        ///</summary>
        public string EnemyType { get; set; }

        ///<summary>
        ///Position where the enemy was spawned.
        ///</summary>
        public Vector3 Position { get; set; }

        ///<summary>
        ///Timestamp when the enemy was spawned.
        ///</summary>
        public DateTime SpawnTime { get; set; }

        ///<summary>
        ///Entity ID of the spawned enemy.
        ///</summary>
        public uint EntityId { get; set; }

        ///<summary>
        ///Wave ID that spawned this enemy (if applicable).
        ///</summary>
        public string? WaveId { get; set; }

        ///<summary>
        ///Initializes a new enemy spawned event.
        ///</summary>
        public EnemySpawnedEvent()
        {
            SpawnTime = DateTime.UtcNow;
        }

        ///<summary>
        ///Initializes a new enemy spawned event with specified enemy.
        ///</summary>
        ///<param name="enemy">The enemy that was spawned.</param>
        public EnemySpawnedEvent(Enemy enemy)
        {
            Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            EnemyType = enemy.Type.ToString();
            Position = enemy.Position;
            EntityId = enemy.Entity.Id;
            SpawnTime = DateTime.UtcNow;
        }
    }
}
