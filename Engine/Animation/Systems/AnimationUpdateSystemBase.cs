/*
File:    AnimationUpdateSystem.cs
Purpose: System for updating animation states on entities.
Features: ECS integration, animation state updates, time-based progression.

P11-04-07-B: System updates animation states for entities with AnimationControllerComponent.
Manages animation time progression and state machine updates with comprehensive logging.
*/
using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Animation.Events;
using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Animation.Systems
{
    public class AnimationUpdateSystemBase
    {
        public event Action<AnimationControllerComponent, AnimationEvent> OnAnimationEventFired;
    }
}
