/*
File:    HazardsMain.cs
Path:    Engine/HazardsControl/HazardsMain.cs
Purpose: Orchestrator and entry point for Hazards Control system.
         Provides unified API for all hazard-related functionality.

Role:    Central coordinator for hazard management systems.
         - Provides clean API surface for external systems
         - Routes calls to appropriate subsystems
         - Manages subsystem lifecycle
         - No hazard logic - pure orchestration

Notes:   This is the main entry point that external systems interact with.
         All complex operations are delegated to specialized subsystems.
         Single responsibility: coordination and routing.
*/

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Types of visual effects for hazards.
    ///</summary>
    public enum HazardVisualEffect
    {
        None,
        Explosion,
        Fire,
        Smoke,
        Acid,
        Electric,
        Poison,
        Ice
    }

    ///<summary>
    ///Main orchestrator for Hazards Control system.
    ///Provides unified API for all hazard-related functionality.
    ///</summary>
    public class HazardsMain
    {
        private readonly HazardLifecycle _lifecycle = new();
        private readonly HazardRegistration _registration = new();
        private readonly HazardCleanup _cleanup = new();
        private readonly HazardAnalytics _analytics = new();
        private readonly HazardOccupancy _occupancy = new();
        private readonly HazardKillAttribution _killAttribution = new();
        private readonly HazardDensity _density = new();
        private readonly HazardLaneInteraction _laneInteraction = new();
        private readonly HazardVisuals _visuals = new();

        private readonly List<Hazard> _activeHazards = new();
        private bool _isInitialized;

        ///<summary>
        ///Initializes all hazard subsystems.
        ///</summary>
        public void InitHazards()
        {
            if (_isInitialized) return;

            foreach (var subsystem in GetSubsystems())
            {
                subsystem?.Init();
            }

            _isInitialized = true;
        }

        ///<summary>
        ///Updates all hazard subsystems.
        ///</summary>
        ///<param name="deltaTime">Time since last update.</param>
        public void UpdateHazards(float deltaTime)
        {
            if (!_isInitialized) return;

            _lifecycle?.UpdateAllHazards(_activeHazards, deltaTime);
            _cleanup?.PerformCleanup(_activeHazards);
            _visuals?.UpdateHazardVisualState(_activeHazards);
        }

        ///<summary>
        ///Cleans up all hazard subsystems.
        ///</summary>
        public void CleanupHazards()
        {
            if (!_isInitialized) return;

            foreach (var subsystem in GetSubsystems())
            {
                subsystem?.Cleanup();
            }

            _activeHazards.Clear();
            _isInitialized = false;
        }

        ///<summary>
        ///Registers a new hazard with the system.
        ///</summary>
        ///<param name="hazard">The hazard to register.</param>
        ///<returns>True if successfully registered.</returns>
        public bool RegisterHazard(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return false;

            if (!_registration?.ValidateHazard(hazard) ?? false) return false;

            var hazardId = _registration?.AssignHazardId();
            if (hazardId == null) return false;

            hazard.Id = hazardId.Value;
            _registration?.RegisterHazardType(hazard.Type);

            if (_registration?.AddHazard(hazard) ?? false)
            {
                _activeHazards.Add(hazard);
                _occupancy?.TrackHazardZoneEntry(hazard, GetCurrentZone(hazard));
                _density?.UpdateDensityForNewHazard(hazard);
                return true;
            }

            return false;
        }

        ///<summary>
        ///Gets hazard analytics data.
        ///</summary>
        ///<returns>Hazard analytics summary.</returns>
        public HazardAnalyticsSummary GetHazardAnalytics()
        {
            if (!_isInitialized) return new HazardAnalyticsSummary();

            return new HazardAnalyticsSummary
            {
                TotalCount = _analytics?.GetTotalHazardCount() ?? 0,
                ActiveCount = _analytics?.GetActiveHazardCount() ?? 0,
                TypeBreakdown = (Dictionary<string, int>)(_analytics?.GetHazardTypeBreakdown() ?? new Dictionary<string, int>()),
                AverageLifetime = _analytics?.GetAverageHazardLifetime() ?? 0f,
                EffectivenessScore = _analytics?.GetAverageEffectivenessScore() ?? 0f
            };
        }

        ///<summary>
        ///Gets hazard occupancy data for a zone.
        ///</summary>
        ///<param name="zoneId">Zone identifier.</param>
        ///<returns>Occupancy information.</returns>
        public ZoneOccupancyData GetHazardOccupancy(int zoneId)
        {
            if (!_isInitialized) return new ZoneOccupancyData();

            return new ZoneOccupancyData
            {
                ZoneId = zoneId,
                OccupancyLevel = _occupancy?.GetZoneOccupancy(zoneId) ?? 0f,
                IsOverloaded = _occupancy?.IsZoneOverloaded(zoneId) ?? false
            };
        }

        ///<summary>
        ///Gets hazard density data for an area.
        ///</summary>
        ///<param name="area">Area to analyze.</param>
        ///<returns>Density information.</returns>
        public HazardDensityData GetHazardDensity(Rectangle area)
        {
            if (!_isInitialized) return new HazardDensityData();

            var densityData = _density?.CalculateHazardDensity(area);
            return densityData ?? new HazardDensityData();
        }

        ///<summary>
        ///Triggers visual effects for hazards.
        ///</summary>
        ///<param name="hazard">The hazard to trigger visuals for.</param>
        ///<param name="effectType">Type of visual effect.</param>
        public void TriggerHazardVisuals(Hazard hazard, HazardVisualEffect effectType)
        {
            if (!_isInitialized || hazard == null) return;

            _visuals?.TriggerVisualEffect(hazard, (HazardVisualEffectType)effectType);
        }

        private int GetCurrentZone(Hazard hazard)
        {
            //Simple zone calculation - can be enhanced
            return (int)(hazard.Position.X / 100) + (int)(hazard.Position.Y / 100) * 10;
        }

        private IEnumerable<IHazardSubsystem> GetSubsystems()
        {
            yield return (IHazardSubsystem)_lifecycle;
            yield return (IHazardSubsystem)_registration;
            yield return (IHazardSubsystem)_cleanup;
            yield return (IHazardSubsystem)_analytics;
            yield return (IHazardSubsystem)_occupancy;
            yield return (IHazardSubsystem)_killAttribution;
            yield return (IHazardSubsystem)_density;
            yield return (IHazardSubsystem)_laneInteraction;
            yield return (IHazardSubsystem)_visuals;
        }
    }

    internal class HazardDensity : IHazardSubsystem
    {
        public void Init()
        {
            //No-op for now; hook up if density needs initialization.
        }

        public void Cleanup()
        {
            //No-op for now; hook up if density needs cleanup.
        }

        internal HazardDensityData CalculateHazardDensity(Rectangle area)
        {
            //Placeholder deterministic implementation; safe default.
            return new HazardDensityData();
        }

        internal void UpdateDensityForNewHazard(Hazard hazard)
        {
            //Placeholder deterministic implementation; safe no-op.
        }
    }

    public interface IHazardSubsystem
    {
        void Init();
        void Cleanup();
    }
}
