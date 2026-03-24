using SASZombieAssaultTD.Engine.Animation.Events;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Integration
{
    public interface IAnimationEventECSHandler
    {
        string HandlerName { get; }
        void HandleEvent(uint entityId, AnimationEvent animationEvent);
    }

    public class AnimationEventECSStatistics
    {
        public int TotalEventsProcessed { get; set; }
        public int TotalHandlersRegistered { get; set; }
        public int TotalEntitiesWithHandlers { get; set; }
        public int TotalGlobalHandlers { get; set; }
        public Dictionary<uint, int> EntityHandlerCounts { get; set; } = new();
        public List<string> GlobalHandlerNames { get; set; } = new();
    }
}
