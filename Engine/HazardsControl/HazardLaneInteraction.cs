/*
File:    HazardLaneInteraction.cs
Path:    Engine/HazardsControl/HazardLaneInteraction.cs
Purpose:  Interaction with lanes, paths, or movement for hazard systems.
          Manages hazard effects on game lanes and pathing.

Role:     Lane interaction manager for hazard systems.
          - Applies lane slowdown effects
          - Manages lane blocking
          - Tracks lane hazard intensity
          - Processes lane-specific hazard effects

Notes:    This file keeps lane/path logic isolated.
          All lane interaction logic is centralized here.
          Single responsibility: lane interaction management.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Lane interaction manager for hazard systems.
    ///Manages hazard effects on lanes and pathing.
    ///</summary>
    public class HazardLaneInteraction
    {
        private readonly Dictionary<int, LaneInteractionData> _laneData = new();
        private readonly Dictionary<int, List<Hazard>> _laneHazards = new();
        private const float DefaultLaneWidth = 200f;
        private float _laneWidth = DefaultLaneWidth;
        private bool _isInitialized;

        ///<summary>
        ///Initializes the hazard lane interaction system.
        ///</summary>
        public void Init()
        {
            _laneData.Clear();
            _laneHazards.Clear();
            _laneWidth = DefaultLaneWidth;
            _isInitialized = true;
        }

        ///<summary>
        ///Applies slowdown to a lane.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<param name="amount">Slowdown amount (0-1, where 1 is full stop).</param>
        public void ApplyLaneSlowdown(int laneId, float amount)
        {
            if (!_isInitialized) return;

            var lane = GetOrCreateLaneData(laneId);
            lane.SlowdownAmount = System.Math.Clamp(amount, 0f, 1f);

            OnLaneSlowdownApplied?.Invoke(laneId, amount);
        }

        ///<summary>
        ///Determines if a lane is blocked.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<returns>True if lane is blocked.</returns>
        public bool IsLaneBlocked(int laneId)
        {
            return _isInitialized && _laneData.TryGetValue(laneId, out var lane) && lane.IsBlocked;
        }

        ///<summary>
        ///Gets hazard intensity for a lane.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<returns>Intensity value (0-1).</returns>
        public float GetLaneHazardIntensity(int laneId)
        {
            return _isInitialized && _laneData.TryGetValue(laneId, out var lane) ? lane.HazardIntensity : 0f;
        }

        ///<summary>
        ///Processes lane hazard effects.
        ///</summary>
        ///<param name="hazard">The hazard to process.</param>
        public void ProcessLaneHazardEffects(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return;

            foreach (var laneId in GetAffectedLanes(hazard))
            {
                UpdateLaneWithHazard(laneId, hazard);
            }
        }

        ///<summary>
        ///Updates lane data with hazard information.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<param name="hazard">The hazard affecting the lane.</param>
        private void UpdateLaneWithHazard(int laneId, Hazard hazard)
        {
            var lane = GetOrCreateLaneData(laneId);
            var hazardsInLane = GetOrCreateLaneHazards(laneId);

            if (!hazardsInLane.Contains(hazard))
            {
                hazardsInLane.Add(hazard);
                lane.AffectedHazards.Add(hazard);
            }

            RecalculateLaneEffects(laneId);
        }

        ///<summary>
        ///Recalculates lane effects based on hazards.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        private void RecalculateLaneEffects(int laneId)
        {
            if (!_laneData.TryGetValue(laneId, out var lane) || !_laneHazards.TryGetValue(laneId, out var hazardsInLane))
                return;

            hazardsInLane.RemoveAll(h => h.State == HazardState.Expired);
            lane.AffectedHazards.RemoveAll(h => h.State == HazardState.Expired);

            if (hazardsInLane.Count == 0)
            {
                ResetLaneEffects(lane);
                return;
            }

            float totalSlowdown = 0f, totalIntensity = 0f;
            bool isBlocked = false;

            foreach (var hazard in hazardsInLane)
            {
                if (hazard == null) continue;

                var hazardEffect = CalculateHazardLaneEffect(hazard);
                totalSlowdown += hazardEffect.SlowdownAmount;
                totalIntensity += hazardEffect.Intensity;
                isBlocked |= hazardEffect.BlocksLane;
            }

            lane.SlowdownAmount = System.Math.Min(1f, totalSlowdown);
            lane.HazardIntensity = System.Math.Min(1f, totalIntensity);
            lane.IsBlocked = isBlocked;
        }

        private static void ResetLaneEffects(LaneInteractionData lane)
        {
            lane.SlowdownAmount = 0f;
            lane.IsBlocked = false;
            lane.HazardIntensity = 0f;
        }

        ///<summary>
        ///Calculates hazard effect on lanes.
        ///</summary>
        ///<param name="hazard">The hazard to calculate effects for.</param>
        ///<returns>Lane effect data.</returns>
        private HazardLaneEffect CalculateHazardLaneEffect(Hazard hazard)
        {
            var effect = hazard.Type.ToLowerInvariant() switch
            {
                "nuke" => new HazardLaneEffect(0.8f, 1.0f, true, 30f),
                "radiation" => new HazardLaneEffect(0.3f, 0.6f, false, 15f),
                "fire" => new HazardLaneEffect(0.5f, 0.8f, false, 10f),
                "chemical" => new HazardLaneEffect(0.4f, 0.7f, false, 20f),
                _ => new HazardLaneEffect(0.2f, 0.5f, false, 5f),
            };

            effect.ApplyIntensityModifier(hazard.CurrentIntensity);
            return effect;
        }

        ///<summary>
        ///Gets lanes affected by a hazard.
        ///</summary>
        ///<param name="hazard">The hazard to check.</param>
        ///<returns>List of affected lane IDs.</returns>
        private List<int> GetAffectedLanes(Hazard hazard)
        {
            var affectedLanes = new List<int>();
            int leftLane = (int)((hazard.Position.X - hazard.Radius) / _laneWidth);
            int rightLane = (int)((hazard.Position.X + hazard.Radius) / _laneWidth);

            for (int laneId = System.Math.Max(0, leftLane); laneId <= rightLane; laneId++)
            {
                affectedLanes.Add(laneId);
            }

            return affectedLanes;
        }

        ///<summary>
        ///Gets lane interaction data.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<returns>Lane interaction data.</returns>
        public LaneInteractionData GetLaneData(int laneId)
        {
            return _isInitialized && _laneData.TryGetValue(laneId, out var data) ? data : new LaneInteractionData { LaneId = laneId };
        }

        ///<summary>
        ///Gets all lane interaction data.
        ///</summary>
        ///<returns>Dictionary of all lane data.</returns>
        public Dictionary<int, LaneInteractionData> GetAllLaneData()
        {
            return _isInitialized ? new Dictionary<int, LaneInteractionData>(_laneData) : new();
        }

        ///<summary>
        ///Updates all lane effects.
        ///</summary>
        ///<param name="hazards">List of active hazards.</param>
        public void UpdateAllLaneEffects(List<Hazard> hazards)
        {
            if (!_isInitialized || hazards == null) return;

            foreach (var laneHazards in _laneHazards.Values)
            {
                laneHazards.Clear();
            }

            foreach (var hazard in hazards)
            {
                if (hazard != null) ProcessLaneHazardEffects(hazard);
            }

            OnLaneEffectsUpdated?.Invoke();
        }

        ///<summary>
        ///Gets lanes with high hazard intensity.
        ///</summary>
        ///<param name="threshold">Intensity threshold (0-1).</param>
        ///<returns>List of lane IDs with high intensity.</returns>
        public List<int> GetHighIntensityLanes(float threshold = 0.7f)
        {
            return _isInitialized
                ? _laneData.Where(kvp => kvp.Value.HazardIntensity >= threshold).Select(kvp => kvp.Key).ToList()
                : new List<int>();
        }

        ///<summary>
        ///Gets blocked lanes.
        ///</summary>
        ///<returns>List of blocked lane IDs.</returns>
        public List<int> GetBlockedLanes()
        {
            return _isInitialized
                ? _laneData.Where(kvp => kvp.Value.IsBlocked).Select(kvp => kvp.Key).ToList()
                : new List<int>();
        }

        ///<summary>
        ///Gets lane interaction summary.
        ///</summary>
        ///<returns>Complete lane interaction summary.</returns>
        public LaneInteractionSummary GetLaneInteractionSummary()
        {
            if (!_isInitialized) return new LaneInteractionSummary();

            return new LaneInteractionSummary
            {
                TotalLanes = _laneData.Count,
                BlockedLanes = _laneData.Values.Count(l => l.IsBlocked),
                HighIntensityLanes = _laneData.Values.Count(l => l.HazardIntensity >= 0.7f),
                AverageSlowdown = _laneData.Values.Average(l => l.SlowdownAmount),
                AverageIntensity = _laneData.Values.Average(l => l.HazardIntensity),
                MostAffectedLane = _laneData.Values.OrderByDescending(l => l.HazardIntensity).FirstOrDefault()?.LaneId ?? -1
            };
        }

        ///<summary>
        ///Cleans up the hazard lane interaction system.
        ///</summary>
        public void Cleanup()
        {
            _laneData.Clear();
            _laneHazards.Clear();
            _isInitialized = false;
        }

        private LaneInteractionData GetOrCreateLaneData(int laneId)
        {
            return _laneData.TryGetValue(laneId, out var lane)
                ? lane
                : _laneData[laneId] = new LaneInteractionData { LaneId = laneId };
        }

        private List<Hazard> GetOrCreateLaneHazards(int laneId)
        {
            return _laneHazards.TryGetValue(laneId, out var hazards)
                ? hazards
                : _laneHazards[laneId] = new List<Hazard>();
        }

        ///<summary>
        ///Event triggered when lane slowdown is applied.
        ///</summary>
        public event Action<int, float> OnLaneSlowdownApplied;

        ///<summary>
        ///Event triggered when lane effects are updated.
        ///</summary>
        public event Action OnLaneEffectsUpdated;
    }

    ///<summary>
    ///Interaction data for a lane.
    ///</summary>
    public class LaneInteractionData
    {
        public int LaneId { get; set; }
        public float SlowdownAmount { get; set; }
        public bool IsBlocked { get; set; }
        public float HazardIntensity { get; set; }
        public List<Hazard> AffectedHazards { get; set; } = new();
    }

    ///<summary>
    ///Effect of a hazard on lanes.
    ///</summary>
    internal class HazardLaneEffect
    {
        public float SlowdownAmount { get; set; }
        public float Intensity { get; set; }
        public bool BlocksLane { get; }
        public float Duration { get; }

        public HazardLaneEffect(float slowdownAmount, float intensity, bool blocksLane, float duration)
        {
            SlowdownAmount = slowdownAmount;
            Intensity = intensity;
            BlocksLane = blocksLane;
            Duration = duration;
        }

        public void ApplyIntensityModifier(float intensityModifier)
        {
            SlowdownAmount *= intensityModifier;
            Intensity *= intensityModifier;
        }
    }

    ///<summary>
    ///Summary of lane interaction data.
    ///</summary>
    public class LaneInteractionSummary
    {
        public int TotalLanes { get; set; }
        public int BlockedLanes { get; set; }
        public int HighIntensityLanes { get; set; }
        public float AverageSlowdown { get; set; }
        public float AverageIntensity { get; set; }
        public int MostAffectedLane { get; set; }
    }
}
