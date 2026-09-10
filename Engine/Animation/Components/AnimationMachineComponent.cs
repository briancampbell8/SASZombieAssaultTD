// =====================================================================================================
//  FILE: AnimationMachineComponent.cs
//  PATH: Engine/Animation/Components/AnimationMachineComponent.cs
//  SUBSYSTEM: Animation Components
//
//  ROLE:
//      Animation subsystem system responsible for deterministic animation playback, state-machine
//      updates, animation event dispatch, and renderable synchronization. Although it participates
//      in the ECS update loop through SystemCore, it is NOT an ECS subsystem — it is an Animation
//      subsystem module that operates on ECS entities.
//
//  RESPONSIBILITIES:
//      - Update AnimationControllerComponent instances deterministically.
//      - Update AnimationMachineComponent state machines.
//      - Dispatch animation events (start, complete, custom events).
//      - Synchronize SpriteComponent renderables with animation playback.
//      - Maintain per-frame animation statistics.
//
//  NON-RESPONSIBILITIES:
//      - Owning ECS ECSEntityCore lifecycle or ECS subsystem responsibilities.
//      - Managing animation clip libraries.
//      - Performing rendering operations directly.
//      - Executing gameplay logic (callbacks are delegated).
//
//  ARCHITECTURAL NOTES:
//      - This system is part of the Animation subsystem, not the ECS subsystem.
//      - It runs inside the ECS lifecycle via SystemCore but does not register as an ECS system.
//      - Includes deterministic event helper methods (EventCrossed, HasEventFired, MarkEventFired,
//        HandleGameplayCallbacks) required for animation event dispatch.
//      - Fully subsystem-aligned under Option‑B deterministic architecture.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    internal class AnimationMachineComponent
    {
        public object? UserData { get; internal set; }
        public AnimationControllerComponent? Controller { get; internal set; }
        public object? Context { get; internal set; }
        public object? CustomData { get; internal set; }


        public string? CurrentAnimation { get; internal set; }
        public string? CurrentEntity { get; internal set; }
        public string? CurrentLibrary { get; internal set; }
        public string? CurrentState { get; internal set; }
        public string? CurrentStateName { get; internal set; }
        public string? CurrentClip { get; internal set; }
        public bool IsRunning { get; internal set; }
        public bool IsLooping { get; internal set; }
        public bool IsPlaying { get; internal set; }
        public bool IsComplete { get; internal set; }
        public bool IsFirstFrame { get; internal set; }
        public bool IsLastFrame { get; internal set; }
        public bool IsFirstFrameAfterPause { get; internal set; }
        public bool IsPaused { get; internal set; }
        public bool StepFrame { get; internal set; }
        public float TimeScale { get; internal set; }
        public float PlaybackTime { get; internal set; }
        public float PreviousPlaybackTime { get; internal set; }
        public float DeltaTime { get; internal set; }
        public float TimeInState { get; internal set; }
        public float PreviousTimeInState { get; internal set; }
        public float PreviousDeltaTime { get; internal set; }
        public int Frame { get; internal set; }
        public int PreviousFrame { get; internal set; }
        public int FrameCount { get; internal set; }
        public int PreviousFrameCount { get; internal set; }
        public int FrameRate { get; internal set; }
        public int PreviousFrameRate { get; internal set; }
        public int FrameRateCount { get; internal set; }
        public int PreviousFrameRateCount { get; internal set; }
        public int SpriteIndex { get; internal set; }
        public int PreviousSpriteIndex { get; internal set; }
        public int SpriteIndexCount { get; internal set; }
        public int PreviousSpriteIndexCount { get; internal set; }
        public int SpriteIndexRate { get; internal set; }
        public int PreviousSpriteIndexRate { get; internal set; }
        public int SpriteIndexRateCount { get; internal set; }
        public int PreviousSpriteIndexRateCount { get; internal set; }
        public object Transitions { get; internal set; }

        public void Update() { }
        public void Update(float deltaTime) { }
        public void Update(float deltaTime, float timeInState) { }
        public void Update(float deltaTime, float timeInState, int frameInState) { }
    }
}
