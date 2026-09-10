// ====================================================================================================
//  FILE: AnimationPlayer.cs
//  PATH: ./Engine/Animation/Core/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationPlayer module.
//
//  RESPONSIBILITIES:
//      - Provide Play() behavior for the Core subsystem.
//      - Provide Stop() behavior for the Core subsystem.
//      - Provide Update() behavior for the Core subsystem.
//      - Provide deterministic frame advancement for the Core subsystem.
//      - Provide deterministic cross‑fade blending for the Core subsystem.
//      - Provide GetBlendWeights() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    ///<summary>
    ///Plays an AnimationClip by advancing frames over time.
    ///Rendering code is expected to query the CurrentFrameIndex.
    ///Includes deterministic cross‑fade support for blending between clips.
    ///</summary>
    public sealed class AnimationPlayer
    {
        // ----------------------------------------------------------------------------------------------
        // EXISTING PLAYBACK FIELDS
        // ----------------------------------------------------------------------------------------------
        public AnimationClip? Clip { get; private set; }
        public int CurrentFrameIndex { get; private set; }
        public bool IsPlaying { get; private set; }
        private float _frameTime;

        // ----------------------------------------------------------------------------------------------
        // CROSS‑FADE SUPPORT
        // ----------------------------------------------------------------------------------------------
        private AnimationClip? _fadeTargetClip;
        private float _fadeTime;
        private float _fadeDuration;
        private bool _fadeActive;

        ///<summary>
        ///Starts playing the specified animation clip.
        ///</summary>
        ///<param name="clip">The animation clip to play.</param>
        public void Play(AnimationClip clip)
        {
            Clip = clip ?? throw new ArgumentNullException(nameof(clip));
            Reset();
            IsPlaying = true;

            // Cancel any active fade
            _fadeActive = false;
            _fadeTargetClip = null;
            _fadeTime = 0f;
            _fadeDuration = 0f;
        }

        ///<summary>
        ///Stops the animation playback.
        ///</summary>
        public void Stop()
        {
            Reset();
            IsPlaying = false;

            // Cancel any active fade
            _fadeActive = false;
            _fadeTargetClip = null;
            _fadeTime = 0f;
            _fadeDuration = 0f;
        }

        ///<summary>
        ///Begins a deterministic cross‑fade to another animation clip.
        ///</summary>
        ///<param name="targetClip">The clip to fade into.</param>
        ///<param name="duration">Fade duration in seconds.</param>
        public void StartCrossFade(AnimationClip targetClip, float duration)
        {
            if (targetClip == null)
                throw new ArgumentNullException(nameof(targetClip));

            if (Clip == null)
            {
                // If no current clip, just play the target immediately
                Play(targetClip);
                return;
            }

            _fadeTargetClip = targetClip;
            _fadeDuration = MathF.Max(0.01f, duration);
            _fadeTime = 0f;
            _fadeActive = true;
        }

        ///<summary>
        ///Advances the animation by the specified delta time (in seconds).
        ///</summary>
        ///<param name="deltaSeconds">Time elapsed since the last update.</param>
        public void Update(float deltaSeconds)
        {
            if (!IsPlaying || Clip is null || Clip.FrameCount == 0)
                return;

            // Advance base clip frame time
            _frameTime += deltaSeconds;

            while (_frameTime >= Clip.GetFrameDuration(CurrentFrameIndex))
            {
                _frameTime -= Clip.GetFrameDuration(CurrentFrameIndex);
                AdvanceFrame();
            }

            // Handle cross‑fade if active
            if (_fadeActive)
                UpdateCrossFade(deltaSeconds);
        }

        ///<summary>
        ///Resets the animation player to its initial state.
        ///</summary>
        private void Reset()
        {
            _frameTime = 0f;
            CurrentFrameIndex = 0;
        }

        ///<summary>
        ///Advances to the next frame, handling looping or stopping as needed.
        ///</summary>
        private void AdvanceFrame()
        {
            if (Clip is null) return;

            CurrentFrameIndex++;

            if (CurrentFrameIndex >= Clip.FrameCount)
            {
                if (Clip.Loops)
                {
                    CurrentFrameIndex = 0;
                }
                else
                {
                    CurrentFrameIndex = Clip.FrameCount - 1;
                    IsPlaying = false;
                }
            }
        }

        // ----------------------------------------------------------------------------------------------
        // CROSS‑FADE MATH
        // ----------------------------------------------------------------------------------------------
        ///<summary>
        ///Updates cross‑fade timing and completes the fade when finished.
        ///</summary>
        private void UpdateCrossFade(float deltaSeconds)
        {
            if (_fadeTargetClip == null)
            {
                _fadeActive = false;
                return;
            }

            _fadeTime += deltaSeconds;
            float t = System.Math.Clamp(_fadeTime / _fadeDuration, 0f, 1f);

            if (t >= 1f)
            {
                // Fade complete — switch to target clip
                Clip = _fadeTargetClip;
                Reset();

                _fadeActive = false;
                _fadeTargetClip = null;
                _fadeTime = 0f;
                _fadeDuration = 0f;
            }
        }

        ///<summary>
        ///Gets deterministic blend weights for rendering or debugging.
        ///Returns (currentWeight, targetWeight).
        ///</summary>
        public (float currentWeight, float targetWeight) GetBlendWeights()
        {
            if (!_fadeActive || _fadeTargetClip == null)
                return (1f, 0f);

            float t = System.Math.Clamp(
                _fadeTime / _fadeDuration,
                0f,
                1f);
            return (1f - t, t);
        }
    }
}
