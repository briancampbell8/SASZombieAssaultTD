// =====================================================================================================
//  FILE: AnimationControllerComponent.cs
//  PATH: Engine/Animation/Components/AnimationControllerComponent.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Holds the animation playback state for an ECSEntityCore and provides methods to trigger
//      and update state transitions within the animation pipeline.
// =====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Controller;
using SASZombieAssaultTD.Engine.Animation.Core.Time;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Animation
{
    public class AnimationControllerComponent
    {
        // ---------------------------------------------------------------------------------------------
        // CORE STATE
        // ---------------------------------------------------------------------------------------------
        public ECSEntityCore Entity { get; internal set; }
        public ECSEntityCore EntityCore { get; internal set; }

        public AnimationEnums.DeathType CurrentDeathVisual { get; private set; }
        public string CurrentAnimationKey { get; private set; } = "Idle";

        public bool IsPlaying { get; private set; }
        public bool IsActive => IsPlaying;

        public float PlaybackTime { get; private set; }
        public int CurrentFrameIndex { get; private set; }
        public int Speed { get; private set; } = 0;

        public float GetCurrentSpriteIndex { get; internal set; }
        public string CurrentClip { get; internal set; }
        public string GetCurrentClip() => CurrentClip;
        public static event Action OnAnimationStarted;

        public object CurrentTransform { get; internal set; }
        public object CurrentTransformOffset { get; internal set; }
        public (float R, float G, float B, float A) CurrentColorTint { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        // PARAMETERS & EVENTS
        // ---------------------------------------------------------------------------------------------
        private readonly Dictionary<string, object> _parameters = new();
        private readonly List<object> _pendingEvents = new();

        // ---------------------------------------------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------------------------------------------
        public AnimationControllerComponent(object currentTransformOffset)
        {
            CurrentTransformOffset = currentTransformOffset;
        }

        // ---------------------------------------------------------------------------------------------
        // PLAYBACK CONTROL
        // ---------------------------------------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (IsPlaying)
            {
                PlaybackTime += Time.DeltaTime;
            }
        }

        public void UpdatePlaybackTime(float deltaTime)
        {
            PlaybackTime += deltaTime;
        }

        public void Play((AnimationEnums.DeathType VisualType, string AssetKey) animationName)
        {
            CurrentDeathVisual = animationName.VisualType;
            CurrentAnimationKey = animationName.AssetKey;
            IsPlaying = true;

            PlaybackTime = 0f;
            CurrentFrameIndex = 0;

            AnimationPlaybackState.RequestBind = true;
            AnimationPlaybackState.RequestedAssetKey = animationName.AssetKey;
            AnimationPlaybackState.RequestedVisualType = animationName.VisualType;
        }

        public void Stop()
        {
            IsPlaying = false;
        }



        // ---------------------------------------------------------------------------------------------
        // PARAMETERS
        // ---------------------------------------------------------------------------------------------
        public void SetParameter(string key, object value)
        {
            _parameters[key] = value;
        }

        // ---------------------------------------------------------------------------------------------
        // EVENTS
        // ---------------------------------------------------------------------------------------------
        public List<object> GetPendingEvents()
        {
            return _pendingEvents;
        }

        public void ClearPendingEvents()
        {
            _pendingEvents.Clear();
        }

        public void AddPendingEvent(object evt)
        {
            if (evt != null)
                _pendingEvents.Add(evt);
        }
    }
}
