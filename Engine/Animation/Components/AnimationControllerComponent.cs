// =====================================================================================================
//  FILE: AnimationControllerComponent.cs
//  PATH: Engine/Animation/Components/AnimationControllerComponent.cs
//  MODULE: Animation Components
//
//  ROLE:
//      ECS-facing façade for the Animation subsystem. Provides deterministic playback control,
//      timing surfaces, sprite index surfaces, tint surfaces, and transform offset surfaces required
//      by AnimationSystem.cs.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Core.Controller;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    public sealed class AnimationControllerComponent
    {
        // --------------------------------------------------------------------------------------------
        //  REQUIRED BY AnimationSystem.cs & UnifiedSystemCorePass.cs
        // --------------------------------------------------------------------------------------------

        // Deterministic animation playback time
        public float PlaybackTime { get; private set; } = 0f;

        // Deterministic transform offset surface
        public (float X, float Y) CurrentTransformOffset { get; set; }

        // Deterministic color tint surface
        public (float R, float G, float B, float A) CurrentColorTint { get; set; }

        // Deterministic sprite index surface
        public int GetCurrentSpriteIndex => (int)_controller.CurrentSpriteIndex;

        // Deterministic transform surface (optional)
        public (float X, float Y) CurrentTransform { get; set; }

        // Deterministic clip surface
        public string CurrentClip => _controller.CurrentClip;

        // ECS entity reference
        public ECSEntityCore Entity { get; internal set; }

        // Internal animation controller
        private readonly AnimationController _controller;

        // --------------------------------------------------------------------------------------------
        //  CONSTRUCTORS
        // --------------------------------------------------------------------------------------------

        public AnimationControllerComponent(ECSEntityCore entity)
        {
            Entity = entity;
            _controller = new AnimationController();
        }

        public AnimationControllerComponent()
        {
            _controller = new AnimationController();
        }

        // --------------------------------------------------------------------------------------------
        //  PLAYBACK CONTROL
        // --------------------------------------------------------------------------------------------

        public void UpdatePlaybackTime(float deltaTime)
        {
            PlaybackTime += deltaTime;
        }

        public void Stop()
        {
            _controller.Stop();
            PlaybackTime = 0f;
        }

        // --------------------------------------------------------------------------------------------
        //  PARAMETER CONTROL
        // --------------------------------------------------------------------------------------------

        public void SetParameter(string name, float value)
        {
            _controller.Parameters.SetValue(name, value);
        }

        // --------------------------------------------------------------------------------------------
        //  UPDATE (CALLED BY AnimationSystem.cs)
        // --------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            PlaybackTime += deltaTime;
            _controller.Update(deltaTime);
        }

        // --------------------------------------------------------------------------------------------
        //  ECS-FACING HELPERS
        // --------------------------------------------------------------------------------------------

        public string? GetCurrentClip()
        {
            return _controller.CurrentClip;
        }

        public (float X, float Y) GetCurrentTransformOffset()
        {
            return _controller.CurrentOffset;
        }
    }
}
