// ====================================================================================================
// FILE: HazardsMain.cs
// PATH: Engine/HazardsControl/HazardsMain.cs
// MODULE: HazardsControl
//
// ROLE:
//     Central orchestrator for all hazard subsystems.
//     Provides the unified API surface for registering, updating, analyzing,
//     and visualizing hazards.
//
// RESPONSIBILITIES:
//     - Maintain the active hazard list.
//     - Route operations to lifecycle, registration, cleanup, occupancy,
//       density, kill attribution, lane interaction, analytics, and visuals.
//     - Manage subsystem initialization and cleanup.
//     - Provide deterministic hazard analytics and zone/density queries.
//     - Expose a clean external API with no embedded hazard logic.
//
// NON-RESPONSIBILITIES:
//     - Performing hazard logic (delegated to subsystems).
//     - Rendering, simulation, or persistence.
//     - Managing hazard types or definitions.
//
// NOTES:
//     Pure orchestration layer. All complex behavior is delegated.
//     Single responsibility: coordination and routing.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.HazardsControl.HazardDensitySystem;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
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

        public IEnumerable<Hazard> ActiveHazards { get; internal set; }

        public void InitHazards()
        {
            if (_isInitialized) return;

            foreach (var subsystem in GetSubsystems())
                subsystem?.Init();

            _isInitialized = true;
        }

        public void UpdateHazards(float deltaTime)
        {
            if (!_isInitialized) return;

            _lifecycle?.UpdateAllHazards(_activeHazards, deltaTime);
            _cleanup?.PerformCleanup(_activeHazards);
            _visuals?.UpdateHazardVisualState(_activeHazards);
        }

        public void CleanupHazards()
        {
            if (!_isInitialized) return;

            foreach (var subsystem in GetSubsystems())
                subsystem?.Cleanup();

            _activeHazards.Clear();
            _isInitialized = false;
        }

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

        public HazardAnalyticsSummary GetHazardAnalytics(
            int? zoneId = null,
            bool? isOverloaded = null,
            float? minIntensity = null,
            float? maxIntensity = null,
            float? minLifetime = null)
        {
            if (!_isInitialized) return new HazardAnalyticsSummary();

            return new HazardAnalyticsSummary
            {
                TotalCount = _analytics?.GetTotalHazardCount() ?? 0,
                ActiveCount = _analytics?.GetActiveHazardCount() ?? 0,
                TypeBreakdown = (Dictionary<string, int>)(_analytics?.GetHazardTypeBreakdown()
                    ?? new Dictionary<string, int>()),
                AverageLifetime = _analytics?.GetAverageHazardLifetime() ?? 0f,
                EffectivenessScore = _analytics?.GetAverageEffectivenessScore() ?? 0f,
                Active = _analytics?.GetHazardCountByState(HazardState.Active) ?? 0,
                Inactive = _analytics?.GetHazardCountByState(HazardState.Inactive) ?? 0,
                Dead = _analytics?.GetHazardCountByState(HazardState.Dead) ?? 0,
                Revived = _analytics?.GetHazardCountByState(HazardState.Revived) ?? 0,
                Killed = _analytics?.GetHazardCountByState(HazardState.Killed) ?? 0,
                Reviving = _analytics?.GetHazardCountByState(HazardState.Reviving) ?? 0
            };
        }

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

        public HazardDensityData GetHazardDensity(Rectangle area)
        {
            if (!_isInitialized) return new HazardDensityData();

            var densityData = _density?.CalculateHazardDensity(area);
            return densityData ?? new HazardDensityData();
        }

        public void TriggerHazardVisuals(Hazard hazard, HazardVisualEffect effectType)
        {
            if (!_isInitialized || hazard == null) return;

            _visuals?.TriggerVisualEffect(hazard, (HazardVisualEffectType)effectType);
        }

        private int GetCurrentZone(Hazard hazard)
        {
            return (int)(hazard.Position.X / 100) +
                   (int)(hazard.Position.Y / 100) * 10;
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
        public void Init() { }
        public void Cleanup() { }

        internal HazardDensityData CalculateHazardDensity(Rectangle area)
        {
            return new HazardDensityData();
        }

        internal void UpdateDensityForNewHazard(Hazard hazard)
        {
        }
    }

    public interface IHazardSubsystem
    {
        void Init();
        void Cleanup();
    }
}
