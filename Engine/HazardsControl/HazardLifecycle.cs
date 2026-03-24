/*
File:    HazardLifecycle.cs
Path:    Engine/HazardsControl/HazardLifecycle.cs
Purpose:  Lifecycle behavior for all hazards.
          Handles state transitions, timers, and activation logic.

Role:     Lifecycle manager for hazard systems.
          - Manages hazard state transitions
          - Handles activation and decay processes
          - Processes hazard timers and duration
          - Controls hazard heartbeat

Notes:    This file handles the temporal aspects of hazards.
          All time-based hazard behavior is managed here.
          Single responsibility: lifecycle management.
*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    /// <summary>
    /// Lifecycle manager for hazard systems.
    /// Handles state transitions, timers, and activation logic.
    /// </summary>
    public class HazardLifecycle
    {
        private readonly Dictionary<int, HazardTimer> _hazardTimers = new();
        private bool _isInitialized;

        /// <summary>
        /// Initializes the hazard lifecycle system.
        /// </summary>
        public void Init()
        {
            _hazardTimers.Clear();
            _isInitialized = true;
        }

        /// <summary>
        /// Updates all hazards with lifecycle processing.
        /// </summary>
        /// <param name="hazards">List of active hazards.</param>
        /// <param name="deltaTime">Time since last update.</param>
        public void UpdateAllHazards(List<Hazard> hazards, float deltaTime)
        {
            if (!_isInitialized || hazards == null) return;

            foreach (var hazard in hazards)
            {
                if (hazard == null) continue;

                AdvanceHazardState(hazard, deltaTime);
                ApplyHazardTimers(hazard, deltaTime);
                ProcessHazardDecay(hazard, deltaTime);
                ProcessHazardActivation(hazard);
            }

            CleanupCompletedTimers();
        }

        /// <summary>
        /// Advances the state of a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to update.</param>
        /// <param name="deltaTime">Time since last update.</param>
        public void AdvanceHazardState(Hazard hazard, float deltaTime)
        {
            if (!_isInitialized || hazard == null) return;

            switch (hazard.State)
            {
                case HazardState.Pending:
                    hazard.ActivationDelay = System.Math.Max(0, hazard.ActivationDelay - deltaTime);
                    if (hazard.ActivationDelay <= 0)
                        hazard.State = HazardState.Activating;
                    break;

                case HazardState.Activating:
                    hazard.ActivationProgress = System.Math.Min(1.0f, hazard.ActivationProgress + deltaTime / hazard.ActivationDuration);
                    if (hazard.ActivationProgress >= 1.0f)
                    {
                        hazard.State = HazardState.Active;
                        StartHazardTimer(hazard);
                    }
                    break;

                case HazardState.Decaying:
                    hazard.DecayProgress = System.Math.Min(1.0f, hazard.DecayProgress + deltaTime / hazard.DecayDuration);
                    if (hazard.DecayProgress >= 1.0f)
                    {
                        hazard.State = HazardState.Expired;
                        StopHazardTimer(hazard);
                    }
                    break;
            }
        }

        /// <summary>
        /// Applies timers to a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to process.</param>
        /// <param name="deltaTime">Time since last update.</param>
        public void ApplyHazardTimers(Hazard hazard, float deltaTime)
        {
            if (!_isInitialized || hazard == null || !_hazardTimers.TryGetValue(hazard.Id, out var timer)) return;

            timer.ElapsedTime += deltaTime;

            // Process periodic effects
            if (timer.ElapsedTime >= timer.Period)
            {
                ProcessPeriodicEffect(hazard);
                timer.ElapsedTime = 0;
            }

            // Update hazard's internal timer
            hazard.ElapsedTime = timer.ElapsedTime;
        }

        /// <summary>
        /// Processes decay for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to process.</param>
        /// <param name="deltaTime">Time since last update.</param>
        public void ProcessHazardDecay(Hazard hazard, float deltaTime)
        {
            if (!_isInitialized || hazard == null || hazard.State != HazardState.Active || !hazard.HasDecay) return;

            hazard.Lifetime += deltaTime;

            if (hazard.Lifetime >= hazard.MaxLifetime)
            {
                hazard.State = HazardState.Decaying;
                hazard.DecayProgress = 0f;
            }

            // Apply intensity decay if applicable
            if (hazard.IntensityDecayRate > 0)
            {
                hazard.CurrentIntensity = System.Math.Max(0, hazard.CurrentIntensity - hazard.IntensityDecayRate * deltaTime);
            }
        }

        /// <summary>
        /// Processes activation for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to process.</param>
        public void ProcessHazardActivation(Hazard hazard)
        {
            if (!_isInitialized || hazard == null || hazard.State != HazardState.Active || hazard.HasActivated) return;

            hazard.HasActivated = true;
            hazard.ActivationTime = DateTime.Now;

            // Trigger activation effects
            OnHazardActivated?.Invoke(hazard);
        }

        /// <summary>
        /// Starts a timer for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to start timer for.</param>
        private void StartHazardTimer(Hazard hazard)
        {
            if (hazard == null) return;

            _hazardTimers[hazard.Id] = new HazardTimer
            {
                HazardId = hazard.Id,
                Period = hazard.EffectPeriod,
                ElapsedTime = 0f
            };
        }

        /// <summary>
        /// Stops a timer for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to stop timer for.</param>
        private void StopHazardTimer(Hazard hazard)
        {
            if (hazard == null) return;

            _hazardTimers.Remove(hazard.Id);
        }

        /// <summary>
        /// Processes periodic effects for a hazard.
        /// </summary>
        /// <param name="hazard">The hazard to process effects for.</param>
        private void ProcessPeriodicEffect(Hazard hazard)
        {
            if (hazard == null) return;

            // Trigger periodic effect event
            OnHazardPeriodicEffect?.Invoke(hazard);
        }

        /// <summary>
        /// Cleans up completed timers.
        /// </summary>
        private void CleanupCompletedTimers()
        {
            foreach (var timerId in _hazardTimers.Keys)
            {
                if (_hazardTimers[timerId].IsCompleted)
                {
                    _hazardTimers.Remove(timerId);
                }
            }
        }

        /// <summary>
        /// Cleans up the hazard lifecycle system.
        /// </summary>
        public void Cleanup()
        {
            _hazardTimers.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// Event triggered when a hazard is activated.
        /// </summary>
        public event Action<Hazard> OnHazardActivated;

        /// <summary>
        /// Event triggered when a hazard processes a periodic effect.
        /// </summary>
        public event Action<Hazard> OnHazardPeriodicEffect;
    }

    /// <summary>
    /// Timer data for hazard periodic effects.
    /// </summary>
    internal class HazardTimer
    {
        public int HazardId { get; set; }
        public float Period { get; set; }
        public float ElapsedTime { get; set; }
        public bool IsCompleted => false; // Placeholder for future logic
    }
}
