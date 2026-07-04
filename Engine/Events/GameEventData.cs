/*
File:    GameEventData.cs
Purpose: Base event data structure for game events.
Features: Timestamp, EntityId, and common event properties.
*/

using System;

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
        ///ID of the entity associated with this event.
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

        ///<summary>
        ///Initializes a new GameEventData instance.
        ///</summary>
        public GameEventData()
        {
            Timestamp = DateTime.Now;
        }

        ///<summary>
        ///Initializes a new GameEventData with specified parameters.
        ///</summary>
        ///<param name="entityId">ID of the entity</param>
        ///<param name="eventType">Type of the event</param>
        ///<param name="data">Additional event data</param>
        public GameEventData(int entityId, string eventType, object? data = null)
        {
            Timestamp = DateTime.Now;
            EntityId = entityId;
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
