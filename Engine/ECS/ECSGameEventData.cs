// ====================================================================================================
//  FILE: ECSGameEventData.cs
//  PATH: Engine/ECS/
//  MODULE: ECS
//
//
//  ROLE:
//      Encapsulate core engine behavior for the GameEventData module.
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
namespace SASZombieAssaultTD.Engine.ECS
{
    //Lightweight game event payload used by animation and other systems.
    public sealed class ECSGameEventData
    {

        public ECSEntityCore Target { get; set; }

        public ECSGameEventData(string eventName, ECSEntityCore target)
        {
            EventName = eventName;
            Target = target;
        }
        public uint EntityId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public float Timestamp { get; set; }
        public object? Payload { get; set; }
    }
}

