/*
    File:    HazardLaneInteraction.cs
    Author:  BDC
    Created: 2026-03-02

    Purpose:
        Manages hazard effects on game lanes and pathing.
        Centralizes all lane interaction logic in one place.

    Notes:
        - Applies lane slowdowns and manages lane blocking.
        - Tracks hazard intensity per lane.
        - Processes lane-specific hazard effects each frame.
        - Lifecycle: Initialize → Update(deltaSeconds) → Shutdown.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Holds the active hazard state for a single lane.
    /// </summary>
    public sealed class LaneHazardState
    {
        /// <summary>Gets the lane identifier.</summary>
        public int LaneId { get; }

        /// <summary>
        /// Slowdown multiplier applied to entities in this lane.
        /// 1.0 = full speed; 0.0 = fully stopped.
        /// Clamped to [0, 1].
        /// </summary>
        public float SlowFactor { get; set; }

        /// <summary>
        /// Whether this lane is fully blocked (no movement allowed).
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Accumulated hazard intensity for this lane.
        /// Higher values indicate more severe hazard conditions.
        /// </summary>
        public float HazardIntensity { get; set; }

        public LaneHazardState(int laneId)
        {
            LaneId = laneId;
            SlowFactor = 1f;
            IsBlocked = false;
            HazardIntensity = 0f;
        }
    }

    /// <summary>
    /// A read-only summary of the hazard conditions for a single lane.
    /// </summary>
    public sealed class LaneInteractionSummary
    {
        public int LaneId { get; }
        public float SlowFactor { get; }
        public bool IsBlocked { get; }
        public float HazardIntensity { get; }

        internal LaneInteractionSummary(LaneHazardState state)
        {
            LaneId = state.LaneId;
            SlowFactor = state.SlowFactor;
            IsBlocked = state.IsBlocked;
            HazardIntensity = state.HazardIntensity;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Lane {LaneId}: SlowFactor={SlowFactor:F2}, " +
                   $"Blocked={IsBlocked}, Intensity={HazardIntensity:F2}";
        }
    }

    /// <summary>
    /// Manages hazard effects on game lanes and pathing.
    /// Applies lane slowdowns, manages lane blocking, tracks hazard intensity,
    /// and processes lane-specific hazard effects each frame.
    /// Follows the standard subsystem lifecycle: Initialize, Update, Shutdown.
    /// </summary>
    public sealed class HazardLaneInteraction
    {
        private readonly Dictionary<int, LaneHazardState> _lanes = new();
        private bool _initialized;

        /// <summary>
        /// Initializes the hazard lane interaction system.
        /// Called once during engine startup.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _lanes.Clear();
            _initialized = true;

            DebugLogger.Log(DebugLogger.Phase5,
                "[HazardLaneInteraction] Initialized.");
        }

        /// <summary>
        /// Registers a lane so that hazard state can be tracked for it.
        /// Calling this for an already-registered lane is a no-op.
        /// </summary>
        public void RegisterLane(int laneId)
        {
            if (_lanes.ContainsKey(laneId))
                return;

            _lanes[laneId] = new LaneHazardState(laneId);
        }

        /// <summary>
        /// Applies a slowdown multiplier to the specified lane.
        /// The effective slow factor is the minimum of the current value and
        /// <paramref name="slowFactor"/>, so multiple overlapping hazards
        /// compound correctly.
        /// </summary>
        /// <param name="laneId">Target lane identifier.</param>
        /// <param name="slowFactor">
        /// Speed multiplier in [0, 1].  1.0 = full speed; 0.0 = fully stopped.
        /// </param>
        public void ApplySlowdown(int laneId, float slowFactor)
        {
            if (slowFactor < 0f) slowFactor = 0f;
            if (slowFactor > 1f) slowFactor = 1f;

            LaneHazardState state = GetOrCreate(laneId);
            if (slowFactor < state.SlowFactor)
                state.SlowFactor = slowFactor;
        }

        /// <summary>
        /// Sets whether the specified lane is fully blocked.
        /// A blocked lane prevents all entity movement through it.
        /// </summary>
        public void SetBlocked(int laneId, bool blocked)
        {
            GetOrCreate(laneId).IsBlocked = blocked;
        }

        /// <summary>
        /// Adds to the hazard intensity for the specified lane.
        /// Intensity is cumulative and must not be negative.
        /// </summary>
        /// <param name="laneId">Target lane identifier.</param>
        /// <param name="intensity">Amount to add (clamped to ≥ 0).</param>
        public void AddHazardIntensity(int laneId, float intensity)
        {
            if (intensity < 0f) intensity = 0f;

            GetOrCreate(laneId).HazardIntensity += intensity;
        }

        /// <summary>
        /// Processes hazard effects for the specified lane.
        /// When hazard intensity exceeds the blocking threshold the lane is
        /// automatically marked as blocked.
        /// </summary>
        /// <param name="laneId">Target lane identifier.</param>
        public void ProcessLane(int laneId)
        {
            if (!_lanes.TryGetValue(laneId, out LaneHazardState? state))
                return;

            const float BlockingIntensityThreshold = 10f;

            if (state.HazardIntensity >= BlockingIntensityThreshold)
                state.IsBlocked = true;
        }

        /// <summary>
        /// Advances all lane hazard states by one frame.
        /// Hazard intensity decays naturally over time; slow factors are reset
        /// each frame so they must be reapplied by active hazard sources.
        /// Called once per frame by GameRoot.
        /// </summary>
        public void Update(float deltaSeconds)
        {
            if (!_initialized)
                return;

            const float IntensityDecayRate = 1f;

            foreach (var kvp in _lanes)
            {
                LaneHazardState state = kvp.Value;

                // Decay hazard intensity toward zero
                state.HazardIntensity -= IntensityDecayRate * deltaSeconds;
                if (state.HazardIntensity < 0f)
                    state.HazardIntensity = 0f;

                // Reset per-frame slow factor; sources reapply each frame
                state.SlowFactor = 1f;

                // Unblock lanes whose intensity has fully decayed
                if (state.HazardIntensity <= 0f)
                    state.IsBlocked = false;

                ProcessLane(state.LaneId);
            }
        }

        /// <summary>
        /// Returns a read-only summary of the current hazard conditions for a lane.
        /// Returns <see langword="null"/> if the lane is not registered.
        /// </summary>
        public LaneInteractionSummary? GetLaneSummary(int laneId)
        {
            return _lanes.TryGetValue(laneId, out LaneHazardState? state)
                ? new LaneInteractionSummary(state)
                : null;
        }

        /// <summary>
        /// Returns summaries for all registered lanes.
        /// </summary>
        public IEnumerable<LaneInteractionSummary> GetAllLaneSummaries()
        {
            foreach (var kvp in _lanes)
                yield return new LaneInteractionSummary(kvp.Value);
        }

        /// <summary>
        /// Tears down the hazard lane interaction system.
        /// Called during engine shutdown.
        /// </summary>
        public void Shutdown()
        {
            _lanes.Clear();
            _initialized = false;

            DebugLogger.Log(DebugLogger.Phase5,
                "[HazardLaneInteraction] Shutdown.");
        }

        // ------------------------------------------------------------------ //
        // Private helpers
        // ------------------------------------------------------------------ //

        private LaneHazardState GetOrCreate(int laneId)
        {
            if (!_lanes.TryGetValue(laneId, out LaneHazardState? state))
            {
                state = new LaneHazardState(laneId);
                _lanes[laneId] = state;
            }

            return state;
        }
    }
}
