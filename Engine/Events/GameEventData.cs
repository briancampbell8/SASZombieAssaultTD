// ====================================================================================================
//  FILE: GameEventData.cs
//  PATH: ./Engine/Events/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the GameEventData module.
//
//  RESPONSIBILITIES:
//      - Provide Clone() behavior for the Core subsystem.
//      - Provide ToString() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    GameEventData.cs
Purpose: Base event data structure for game events.
Features: Timestamp, EntityId, and common event properties.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Events
{
    ///<summary>
    ///Base event data structure for game events.
    ///Provides common properties for all game events.
    ///</summary>
    public class GameEventData
    {
        ///<summary>
        ///Timestamp when the event occurred.
        ///</summary>
        public DateTime Timestamp { get; set; }

        ///<summary>
        ///ID of the ECSEntityCore associated with this event.
        ///</summary>
        public int EntityId { get; set; }

        ///<summary>
        ///Type of the event.
        ///</summary>
        public string EventType { get; set; } = string.Empty;

        ///<summary>
        ///Additional event data.
        ///</summary>
        public object? Data { get; set; }
        public object EventName { get; internal set; }

        ///<summary>
        ///Initializes a new GameEventData instance.
        ///</summary>
        public GameEventData() => Timestamp = DateTime.Now;

        ///<summary>
        ///Initializes a new GameEventData with specified parameters.
        ///</summary>
        ///<param name="ECSEntityCoreId">ID of the ECSEntityCore</param>
        ///<param name="eventType">Type of the event</param>
        ///<param name="data">Additional event data</param>
        public GameEventData(int ECSEntityCoreId, string eventType, object? data = null)
        {
            Timestamp = DateTime.Now;
            EntityId = ECSEntityCoreId;
            EventType = eventType ?? string.Empty;
            Data = data;
        }

        ///<summary>
        ///Creates a copy of this event data.
        ///</summary>
        ///<returns>A new GameEventData with the same properties</returns>
        public GameEventData Clone()
        {
            return new GameEventData(EntityId, EventType, Data)
            {
                Timestamp = this.Timestamp
            };
        }

        ///<summary>
        ///Returns a string representation of the event data.
        ///</summary>
        ///<returns>Formatted string representation</returns>
        public override string ToString()
        {
            return $"GameEventData(Type: {EventType}, Entity: {EntityId}, Time: {Timestamp:HH:mm:ss})";
        }
    }
}

