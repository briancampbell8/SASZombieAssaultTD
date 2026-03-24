/*
File:    HazardVisuals.cs
Path:    Engine/HazardsControl/HazardVisuals.cs
Purpose:  Visual triggers and effects for hazard systems.
          Manages hazard visual feedback and effects.

Role:     Visual manager for hazard systems.
          - Triggers hazard visual effects
          - Manages visual state transitions
          - Handles hazard animations
          - Provides visual feedback

Notes:    This file keeps visuals out of gameplay logic.
          All visual effect logic is centralized here.
          Single responsibility: visual management.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Visual manager for hazard systems.
    /// Triggers visual effects and manages hazard visual state.
    /// </summary>
    public class HazardVisuals
    {
        private Dictionary<int, HazardVisualStateData> _hazardVisualStates;
        private Queue<VisualEffectRequest> _effectQueue;
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes the hazard visuals system.
        /// </summary>
        public void Init()
        {
            _hazardVisualStates = new Dictionary<int, HazardVisualStateData>();
            _effectQueue = new Queue<VisualEffectRequest>();
            _isInitialized = true;
        }

        /// <summary>
        /// Triggers a visual effect for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to trigger the effect for.</param>
        /// <param name="effectType">The type of visual effect.</param>
        public void TriggerVisualEffect(Hazard hazard, HazardVisualEffectType effectType)
        {
            if (!_isInitialized || hazard == null) return;

            var effect = new VisualEffectRequest
            {
                HazardId = hazard.Id,
                EffectType = effectType,
                Position = hazard.Position,
                Intensity = hazard.CurrentIntensity,
                Duration = GetEffectDuration(effectType, hazard.Type),
                Color = GetEffectColor(effectType, hazard.Type),
                Radius = effectType == HazardVisualEffectType.Warning ? hazard.Radius * 1.2f : hazard.Radius,
                StartTime = DateTime.Now
            };

            _effectQueue.Enqueue(effect);
            UpdateHazardVisualState(hazard, GetVisualStateFromEffectType(effectType));

            OnVisualEffectTriggered?.Invoke(hazard, effectType);
        }

        /// <summary>
        /// Updates visual state for hazards.
        /// </summary>
        /// <param name="hazards">List of hazards to update.</param>
        public void UpdateHazardVisualState(List<Hazard> hazards)
        {
            if (!_isInitialized || hazards == null) return;

            foreach (var hazard in hazards)
            {
                if (hazard == null) continue;

                var visualState = DetermineVisualState(hazard);
                UpdateHazardVisualState(hazard, visualState);
            }

            ProcessEffectQueue();
        }

        /// <summary>
        /// Updates visual state for a specific hazard.
        /// </summary>
        /// <param name="hazard">The hazard to update.</param>
        /// <param name="visualState">The new visual state.</param>
        private void UpdateHazardVisualState(Hazard hazard, HazardVisualState visualState)
        {
            if (!_hazardVisualStates.ContainsKey(hazard.Id))
            {
                _hazardVisualStates[hazard.Id] = new HazardVisualStateData
                {
                    HazardId = hazard.Id,
                    CurrentState = visualState,
                    StateStartTime = DateTime.Now,
                    AnimationProgress = 0f,
                    LastEffectTime = DateTime.Now
                };
            }
            else
            {
                var state = _hazardVisualStates[hazard.Id];
                state.CurrentState = visualState;
                state.StateStartTime = DateTime.Now;
                state.AnimationProgress = 0f;
            }

            OnHazardVisualStateChanged?.Invoke(hazard, visualState);
        }

        /// <summary>
        /// Processes the visual effect queue.
        /// </summary>
        private void ProcessEffectQueue()
        {
            while (_effectQueue.Count > 0)
            {
                var effect = _effectQueue.Dequeue();
                ProcessVisualEffect(effect);
            }
        }

        /// <summary>
        /// Processes a single visual effect.
        /// </summary>
        /// <param name="effect">The effect to process.</param>
        private void ProcessVisualEffect(VisualEffectRequest effect)
        {
            // This would integrate with the actual rendering system
            // For now, we just trigger events
            OnVisualEffectProcessed?.Invoke(effect);
        }

        /// <summary>
        /// Determines the visual state for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to determine state for.</param>
        /// <returns>Visual state for the hazard.</returns>
        private HazardVisualState DetermineVisualState(Hazard hazard)
        {
            return hazard.State switch
            {
                HazardState.Pending => HazardVisualState.Pending,
                HazardState.Activating => HazardVisualState.Activating,
                HazardState.Active => HazardVisualState.Active,
                HazardState.Decaying => HazardVisualState.Decaying,
                HazardState.Expired => HazardVisualState.Expired,
                _ => HazardVisualState.Inactive
            };
        }

        /// <summary>
        /// Gets the visual state corresponding to a visual effect type.
        /// </summary>
        /// <param name="effectType">The visual effect type.</param>
        /// <returns>The corresponding visual state.</returns>
        private HazardVisualState GetVisualStateFromEffectType(HazardVisualEffectType effectType)
        {
            return effectType switch
            {
                HazardVisualEffectType.Flash => HazardVisualState.Flash,
                HazardVisualEffectType.Explosion => HazardVisualState.Exploding,
                HazardVisualEffectType.Warning => HazardVisualState.Warning,
                _ => HazardVisualState.Inactive
            };
        }

        /// <summary>
        /// Gets the duration of a visual effect based on its type and hazard type.
        /// </summary>
        /// <param name="effectType">The visual effect type.</param>
        /// <param name="hazardType">The hazard type.</param>
        /// <returns>The duration of the effect.</returns>
        private float GetEffectDuration(HazardVisualEffectType effectType, string hazardType)
        {
            return effectType switch
            {
                HazardVisualEffectType.Flash => 0.5f,
                HazardVisualEffectType.Explosion => hazardType.ToLowerInvariant() switch
                {
                    "nuke" => 5f,
                    "radiation" => 2f,
                    "fire" => 1.5f,
                    "chemical" => 3f,
                    _ => 2f
                },
                HazardVisualEffectType.Warning => 2f,
                _ => 0f
            };
        }

        /// <summary>
        /// Gets the color of a visual effect based on its type and hazard type.
        /// </summary>
        /// <param name="effectType">The visual effect type.</param>
        /// <param name="hazardType">The hazard type.</param>
        /// <returns>The color of the effect.</returns>
        private string GetEffectColor(HazardVisualEffectType effectType, string hazardType)
        {
            return effectType switch
            {
                HazardVisualEffectType.Flash => hazardType.ToLowerInvariant() switch
                {
                    "nuke" => "#FFFFFF",
                    "radiation" => "#00FF00",
                    "fire" => "#FF6600",
                    "chemical" => "#FF00FF",
                    _ => "#FFFF00"
                },
                HazardVisualEffectType.Explosion => hazardType.ToLowerInvariant() switch
                {
                    "nuke" => "#FF4500",
                    "radiation" => "#00FF00",
                    "fire" => "#FF0000",
                    "chemical" => "#9400D3",
                    _ => "#808080"
                },
                HazardVisualEffectType.Warning => hazardType.ToLowerInvariant() switch
                {
                    "nuke" => "#FF0000",
                    "radiation" => "#FFFF00",
                    "fire" => "#FF6600",
                    "chemical" => "#FF00FF",
                    _ => "#FFFFFF"
                },
                _ => "#000000"
            };
        }

        /// <summary>
        /// Cleans up the hazard visuals system.
        /// </summary>
        public void Cleanup()
        {
            _hazardVisualStates?.Clear();
            _effectQueue?.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// Event triggered when a visual effect is triggered.
        /// </summary>
        public event Action<Hazard, HazardVisualEffectType> OnVisualEffectTriggered;

        /// <summary>
        /// Event triggered when hazard visual state changes.
        /// </summary>
        public event Action<Hazard, HazardVisualState> OnHazardVisualStateChanged;

        /// <summary>
        /// Event triggered when a visual effect is processed.
        /// </summary>
        public event Action<VisualEffectRequest> OnVisualEffectProcessed;
    }

    /// <summary>
    /// Request for a visual effect.
    /// </summary>
    public class VisualEffectRequest
    {
        public int HazardId { get; set; }
        public HazardVisualEffectType EffectType { get; set; }
        public Vector3 Position { get; set; }
        public float Intensity { get; set; }
        public float Duration { get; set; }
        public string Color { get; set; }
        public float Radius { get; set; }
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// Visual state for a hazard.
    /// </summary>
    public class HazardVisualStateData
    {
        public int HazardId { get; set; }
        public HazardVisualState CurrentState { get; set; }
        public DateTime StateStartTime { get; set; }
        public float AnimationProgress { get; set; }
        public DateTime LastEffectTime { get; set; }
    }

    /// <summary>
    /// Statistics for visual operations.
    /// </summary>
    public class VisualStatistics
    {
        public int TotalHazardsTracked { get; set; }
        public int ActiveEffects { get; set; }
        public Dictionary<HazardVisualState, int> StatesByType { get; set; }
        public HazardVisualState MostCommonState { get; set; }
    }

    /// <summary>
    /// Hazard visual states.
    /// </summary>
    public enum HazardVisualState
    {
        Inactive,
        Pending,
        Activating,
        Active,
        Flash,
        Exploding,
        Warning,
        Decaying,
        Expired
    }

    /// <summary>
    /// Types of visual effects.
    /// </summary>
    public enum HazardVisualEffectType
    {
        None,
        Flash,
        Explosion,
        Warning
    }
}
