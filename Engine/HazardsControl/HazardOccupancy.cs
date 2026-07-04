/*
File:    HazardOccupancy.cs
Path:    Engine/HazardsControl/HazardOccupancy.cs
Purpose:  Zone/lane occupancy tracking for hazard systems.
          Monitors hazard distribution across game areas.

Role:     Occupancy manager for hazard systems.
          - Tracks zone and lane occupancy
          - Monitors hazard overload conditions
          - Provides spatial distribution data
          - Supports AI and pathing systems

Notes:    This file supports AI and pathing systems.
          All spatial tracking logic is centralized here.
          Single responsibility: occupancy management.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Occupancy manager for hazard systems.
    ///Tracks zone/lane occupancy and overload conditions.
    ///</summary>
    public class HazardOccupancy
    {
        private readonly Dictionary<int, ZoneOccupancyData> _zoneOccupancy = new();
        private readonly Dictionary<int, LaneOccupancyData> _laneOccupancy = new();
        private readonly Dictionary<int, List<HazardZoneTracking>> _hazardZoneTracking = new();
        private bool _isInitialized;

        ///<summary>
        ///Initializes the hazard occupancy system.
        ///</summary>
        public void Init()
        {
            _zoneOccupancy.Clear();
            _laneOccupancy.Clear();
            _hazardZoneTracking.Clear();
            _isInitialized = true;
        }

        ///<summary>
        ///Gets occupancy level for a specific zone.
        ///</summary>
        ///<param name="zoneId">The zone identifier.</param>
        ///<returns>Occupancy level (0-1).</returns>
        public float GetZoneOccupancy(int zoneId)
        {
            return _isInitialized && _zoneOccupancy.TryGetValue(zoneId, out var occupancy)
                ? occupancy.OccupancyLevel
                : 0f;
        }

        ///<summary>
        ///Gets exposure level for a specific lane.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<returns>Exposure level (0-1).</returns>
        public float GetLaneExposure(int laneId)
        {
            return _isInitialized && _laneOccupancy.TryGetValue(laneId, out var occupancy)
                ? occupancy.ExposureLevel
                : 0f;
        }

        ///<summary>
        ///Determines if a zone is overloaded.
        ///</summary>
        ///<param name="zoneId">The zone identifier.</param>
        ///<returns>True if zone is overloaded.</returns>
        public bool IsZoneOverloaded(int zoneId)
        {
            return _isInitialized && _zoneOccupancy.TryGetValue(zoneId, out var occupancy) && occupancy.IsOverloaded;
        }

        ///<summary>
        ///Tracks hazard entry into a zone.
        ///</summary>
        ///<param name="hazard">The hazard entering the zone.</param>
        ///<param name="zone">The zone being entered.</param>
        public void TrackHazardZoneEntry(Hazard hazard, int zone)
        {
            if (!_isInitialized || hazard == null) return;

            var zoneData = _zoneOccupancy.GetOrAdd(zone, () => new ZoneOccupancyData
            {
                ZoneId = zone,
                MaxCapacity = 10f
            });

            zoneData.HazardCount++;
            zoneData.OccupancyLevel = System.Math.Min(1.0f, zoneData.HazardCount / zoneData.MaxCapacity);
            zoneData.IsOverloaded = zoneData.OccupancyLevel >= 0.8f;

            _hazardZoneTracking.GetOrAdd(hazard.Id, () => new List<HazardZoneTracking>())
                .Add(new HazardZoneTracking
                {
                    HazardId = hazard.Id,
                    ZoneId = zone,
                    EntryTime = DateTime.Now
                });

            OnHazardZoneEntry?.Invoke(hazard, zone);
        }

        ///<summary>
        ///Tracks hazard exit from a zone.
        ///</summary>
        ///<param name="hazard">The hazard exiting the zone.</param>
        ///<param name="zone">The zone being exited.</param>
        public void TrackHazardZoneExit(Hazard hazard, int zone)
        {
            if (!_isInitialized || hazard == null) return;

            if (_zoneOccupancy.TryGetValue(zone, out var zoneData))
            {
                zoneData.HazardCount = System.Math.Max(0, zoneData.HazardCount - 1);
                zoneData.OccupancyLevel = System.Math.Min(1.0f, zoneData.HazardCount / zoneData.MaxCapacity);
                zoneData.IsOverloaded = zoneData.OccupancyLevel >= 0.8f;
            }

            if (_hazardZoneTracking.TryGetValue(hazard.Id, out var trackingList))
            {
                var currentTracking = trackingList.LastOrDefault(t => t.ExitTime == null);
                if (currentTracking != null)
                {
                    currentTracking.ExitTime = DateTime.Now;
                }
            }

            OnHazardZoneExit?.Invoke(hazard, zone);
        }

        ///<summary>
        ///Gets active hazards in a specific zone.
        ///</summary>
        ///<param name="zoneId">The zone identifier.</param>
        ///<returns>List of active hazards in the zone.</returns>
        public List<Hazard> GetActiveHazardsInZone(int zoneId)
        {
            if (!_isInitialized) return new List<Hazard>();

            //This would need access to the actual hazard list
            //For now, return empty list - implementation would need hazard reference
            return new List<Hazard>();
        }

        ///<summary>
        ///Gets occupancy data for a zone.
        ///</summary>
        ///<param name="zoneId">The zone identifier.</param>
        ///<returns>Complete occupancy data for the zone.</returns>
        public ZoneOccupancyData GetZoneOccupancyData(int zoneId)
        {
            return _isInitialized && _zoneOccupancy.TryGetValue(zoneId, out var occupancy)
                ? occupancy
                : new ZoneOccupancyData { ZoneId = zoneId };
        }

        ///<summary>
        ///Gets exposure data for a lane.
        ///</summary>
        ///<param name="laneId">The lane identifier.</param>
        ///<returns>Complete exposure data for the lane.</returns>
        public LaneOccupancyData GetLaneExposureData(int laneId)
        {
            return _isInitialized && _laneOccupancy.TryGetValue(laneId, out var exposure)
                ? exposure
                : new LaneOccupancyData { LaneId = laneId };
        }

        ///<summary>
        ///Updates lane exposure based on hazard positions.
        ///</summary>
        ///<param name="hazards">List of active hazards.</param>
        public void UpdateLaneExposure(List<Hazard> hazards)
        {
            if (!_isInitialized || hazards == null) return;

            foreach (var lane in _laneOccupancy.Values)
            {
                lane.HazardCount = 0;
                lane.ExposureLevel = 0f;
            }

            foreach (var hazard in hazards)
            {
                if (hazard == null) continue;

                var laneId = GetLaneFromPosition(hazard.Position);
                var laneData = _laneOccupancy.GetOrAdd(laneId, () => new LaneOccupancyData
                {
                    LaneId = laneId,
                    MaxExposure = 5f
                });

                laneData.HazardCount++;
                laneData.ExposureLevel = System.Math.Min(1.0f, laneData.HazardCount / laneData.MaxExposure);
            }
        }

        ///<summary>
        ///Gets zones with high occupancy.
        ///</summary>
        ///<param name="threshold">Occupancy threshold (0-1).</param>
        ///<returns>List of zone IDs with high occupancy.</returns>
        public List<int> GetHighOccupancyZones(float threshold = 0.7f)
        {
            return _isInitialized
                ? _zoneOccupancy.Where(kvp => kvp.Value.OccupancyLevel >= threshold).Select(kvp => kvp.Key).ToList()
                : new List<int>();
        }

        ///<summary>
        ///Gets lanes with high exposure.
        ///</summary>
        ///<param name="threshold">Exposure threshold (0-1).</param>
        ///<returns>List of lane IDs with high exposure.</returns>
        public List<int> GetHighExposureLanes(float threshold = 0.7f)
        {
            return _isInitialized
                ? _laneOccupancy.Where(kvp => kvp.Value.ExposureLevel >= threshold).Select(kvp => kvp.Key).ToList()
                : new List<int>();
        }

        ///<summary>
        ///Gets lane ID from position.
        ///</summary>
        ///<param name="position">The position to convert.</param>
        ///<returns>Lane ID.</returns>
        private int GetLaneFromPosition(Vector3 position)
        {
            //Simple lane calculation - can be enhanced
            return (int)(position.X / 200); //200 units per lane
        }

        ///<summary>
        ///Gets comprehensive occupancy summary.
        ///</summary>
        ///<returns>Complete occupancy summary.</returns>
        public OccupancySummary GetOccupancySummary()
        {
            if (!_isInitialized) return new OccupancySummary();

            return new OccupancySummary
            {
                TotalZones = _zoneOccupancy.Count,
                TotalLanes = _laneOccupancy.Count,
                OverloadedZones = _zoneOccupancy.Values.Count(z => z.IsOverloaded),
                HighExposureLanes = _laneOccupancy.Values.Count(l => l.ExposureLevel >= 0.7f),
                AverageZoneOccupancy = _zoneOccupancy.Values.Average(z => z.OccupancyLevel),
                AverageLaneExposure = _laneOccupancy.Values.Average(l => l.ExposureLevel)
            };
        }

        ///<summary>
        ///Cleans up the hazard occupancy system.
        ///</summary>
        public void Cleanup()
        {
            _zoneOccupancy.Clear();
            _laneOccupancy.Clear();
            _hazardZoneTracking.Clear();
            _isInitialized = false;
        }

        ///<summary>
        ///Event triggered when a hazard enters a zone.
        ///</summary>
        public event Action<Hazard, int> OnHazardZoneEntry;

        ///<summary>
        ///Event triggered when a hazard exits a zone.
        ///</summary>
        public event Action<Hazard, int> OnHazardZoneExit;
    }

    ///<summary>
    ///Occupancy data for a zone.
    ///</summary>
    public class ZoneOccupancyData
    {
        public int ZoneId { get; set; }
        public float OccupancyLevel { get; set; }
        public int HazardCount { get; set; }
        public bool IsOverloaded { get; set; }
        public float MaxCapacity { get; set; }
    }

    ///<summary>
    ///Exposure data for a lane.
    ///</summary>
    public class LaneOccupancyData
    {
        public int LaneId { get; set; }
        public float ExposureLevel { get; set; }
        public int HazardCount { get; set; }
        public float MaxExposure { get; set; }
    }

    ///<summary>
    ///Tracking data for hazard zone movement.
    ///</summary>
    internal class HazardZoneTracking
    {
        public int HazardId { get; set; }
        public int ZoneId { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }

    ///<summary>
    ///Summary of occupancy data.
    ///</summary>
    public class OccupancySummary
    {
        public int TotalZones { get; set; }
        public int TotalLanes { get; set; }
        public int OverloadedZones { get; set; }
        public int HighExposureLanes { get; set; }
        public float AverageZoneOccupancy { get; set; }
        public float AverageLaneExposure { get; set; }
    }

    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory) where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory();
                dictionary[key] = value;
            }
            return value;
        }
    }
}
