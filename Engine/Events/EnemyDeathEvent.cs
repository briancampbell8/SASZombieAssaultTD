// ====================================================================================================
//  FILE: EnemyDeathEvent.cs
//  PATH: ./Engine/Events/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyDeathEvent module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EnemyDeathEvent.cs
Purpose: Event fired when an enemy dies in the game.
Features: Enemy reference, death position, death timestamp, killer information.

P11-04-07-B: Enemy death event for ECS integration and event system.
*/

using SASZombieAssaultTD.Engine.VectorMath;
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Events
{
    ///<summary>
    ///Event fired when an enemy dies in the game.
    ///Used for tracking enemy deaths and triggering related game events.
    ///</summary>
    public class EnemyDeathEvent
    {
        ///<summary>
        ///The enemy that died.
        ///</summary>
        public Enemy Enemy { get; set; }

        ///<summary>
        ///Position where the enemy died.
        ///</summary>
        public Vector3 Position { get; set; }

        ///<summary>
        ///Timestamp when the enemy died.
        ///</summary>
        public DateTime DeathTime { get; set; }

        ///<summary>
        ///Entity ID of the dead enemy.
        ///</summary>
        public uint EntityId { get; set; }

        ///<summary>
        ///Type of the enemy that died.
        ///</summary>
        public string EnemyType { get; set; }

        ///<summary>
        ///Wave ID that spawned this enemy (if applicable).
        ///</summary>
        public string? WaveId { get; set; }

        ///<summary>
        ///Initializes a new enemy death event.
        ///</summary>
        public EnemyDeathEvent() => DeathTime = DateTime.UtcNow;

        ///<summary>
        ///Initializes a new enemy death event with specified enemy.
        ///</summary>
        ///<param name="enemy">The enemy that died.</param>
        public EnemyDeathEvent(Enemy enemy)
        {
            Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            Position = enemy.Position;
            EntityId = enemy.ECSEntityCore.Id;
            EnemyType = enemy.Type.ToString();
            DeathTime = DateTime.UtcNow;
        }
    }
}

