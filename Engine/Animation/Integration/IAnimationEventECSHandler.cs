// ====================================================================================================
//  FILE: IAnimationEventECSHandler.cs
//  PATH: Engine/Animation/Integration/IAnimationEventECSHandler.cs
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationEventTypes module.
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
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.Integration
{
    public interface IAnimationEventECSHandler
    {
        string HandlerName { get; }
        void HandleEvent(uint ECSEntityCoreId, AnimationEvent animationEvent, Core.AnimationEventContext context);
        //void HandleEvent(uint ECSEntityCoreId, AnimationEvent animationEvent, AnimationEventContext context);
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

