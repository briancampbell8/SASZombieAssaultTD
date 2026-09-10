/*
File:    HazardDensity.cs
Path:    Engine/HazardsControl/HazardDensity.cs
Purpose:  Spatial density and clustering for hazard systems.
          Calculates hazard distribution and risk areas.

Role:     Density manager for hazard systems.
          - Calculates spatial hazard density
          - Identifies hazard clusters
          - Provides proximity analysis
          - Supports AI and difficulty scaling

Notes:    This file supports AI, Waves, and difficulty scaling.
          All spatial analysis logic is centralized here.
          Single responsibility: density management.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.HazardsControl
{
    ///<summary>
    ///Density manager for hazard systems.
    ///Calculates spatial density and clustering analysis.
    ///</summary>
    public class HazardDensitySystem
    {
        public class HazardDensityData
        {
            public float Area { get; set; }
        }


        //A mutable list of hazards
        private List<Hazard> _hazards = new();

        //A mutable dictionary for cached density values
        private Dictionary<string, DensityCache> _densityCache = new();

        //Grid size in world units
        private float _gridSize = 100f;

        //Tracks whether the system has been initialized
        private bool _isInitialized = false;

        ///<summary>
        ///Initializes the hazard density system.
        ///</summary>
        public void Init()
        {
            if (_isInitialized)
                return;

            _hazards = new List<Hazard>();
            _densityCache = new Dictionary<string, DensityCache>();
            _gridSize = 100f; //100 units per grid cell
            _isInitialized = true;
        }


        ///<summary>
        ///Calculates hazard density for an area.
        ///</summary>
        ///<param name="area">The area to analyze.</param>
        ///<returns>Density value (hazards per unit area).</returns>
        public float CalculateHazardDensity(Rectangle area)
        {
            if (!_isInitialized) return 0f;

            var cacheKey = GetCacheKey(area);
            if (_densityCache.TryGetValue(cacheKey, out var cached) &&
                DateTime.Now - cached.CalculatedAt < TimeSpan.FromSeconds(1))
            {
                return cached.Density;
            }

            var hazardsInArea = GetHazardsInArea(area);
            var areaSize = area.Width * area.Height;
            var density = areaSize > 0 ? hazardsInArea.Count / areaSize : 0f;

            //Cache the result
            _densityCache[cacheKey] = new DensityCache
            {
                Density = density,
                CalculatedAt = DateTime.Now
            };

            return density;
        }

        ///<summary>
        ///Gets the number of hazard clusters.
        ///</summary>
        ///<returns>Number of hazard clusters found.</returns>
        public int GetHazardClusterCount()
        {
            if (!_isInitialized || _hazards.Count == 0) return 0;

            var clusters = new List<HashSet<Hazard>>();
            var visited = new HashSet<Hazard>();

            foreach (var hazard in _hazards)
            {
                if (hazard == null || visited.Contains(hazard)) continue;

                var cluster = FindCluster(hazard, visited);
                if (cluster.Count > 1) //Only count clusters with multiple hazards
                {
                    clusters.Add(cluster);
                }
            }

            return clusters.Count;
        }

        ///<summary>
        ///Gets nearest hazards to a position.
        ///</summary>
        ///<param name="position">The center position.</param>
        ///<param name="radius">Search radius.</param>
        ///<returns>List of nearest hazards within radius.</returns>
        public List<Hazard> GetNearestHazards(Vector3 position, float radius)
        {
            if (!_isInitialized) return new List<Hazard>();

            var nearbyHazards = new List<Hazard>();
            var radiusSquared = radius * radius;

            foreach (var hazard in _hazards)
            {
                if (hazard == null) continue;

                var distanceSquared = CalculateDistanceSquared(position, hazard.Position);
                if (distanceSquared <= radiusSquared)
                {
                    nearbyHazards.Add(hazard);
                }
            }

            return nearbyHazards
                .OrderBy(h => CalculateDistanceSquared(position, h.Position))
                .ToList();
        }

        ///<summary>
        ///Determines if an area is high risk.
        ///</summary>
        ///<param name="area">The area to analyze.</param>
        ///<returns>True if area is considered high risk.</returns>
        public bool IsAreaHighRisk(Rectangle area)
        {
            if (!_isInitialized) return false;

            var density = CalculateHazardDensity(area);
            var clusterCount = GetClusterCountInArea(area);

            //High risk criteria: high density or multiple clusters
            return density > 0.001f || clusterCount >= 2; //Thresholds can be adjusted
        }

        ///<summary>
        ///Updates density for a new hazard.
        ///</summary>
        ///<param name="hazard">The new hazard to add.</param>
        public void UpdateDensityForNewHazard(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return;

            _hazards.Add(hazard);

            //Clear relevant cache entries
            ClearCacheForArea(hazard.Position, hazard.Radius * 2);

            OnHazardAdded?.Invoke(hazard);
        }

        ///<summary>
        ///Updates density when a hazard is removed.
        ///</summary>
        ///<param name="hazard">The hazard being removed.</param>
        public void UpdateDensityForRemovedHazard(Hazard hazard)
        {
            if (!_isInitialized || hazard == null) return;

            _hazards.Remove(hazard);

            //Clear relevant cache entries
            ClearCacheForArea(hazard.Position, hazard.Radius * 2);

            OnHazardRemoved?.Invoke(hazard);
        }

        ///<summary>
        ///Gets density heatmap data.
        ///</summary>
        ///<param name="bounds">The bounds for the heatmap.</param>
        ///<param name="resolution">Grid resolution for the heatmap.</param>
        ///<returns>2D array representing density heatmap.</returns>
        public float[,] GetDensityHeatmap(SASZombieAssaultTD.Engine.HazardsControl.Rectangle bounds, int resolution)
        {
            if (!_isInitialized) return new float[0, 0];

            var cellWidth = bounds.Width / resolution;
            var cellHeight = bounds.Height / resolution;
            var heatmap = new float[resolution, resolution];

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    var cellArea = new SASZombieAssaultTD.Engine.HazardsControl.Rectangle(
                        bounds.X + x * cellWidth,
                        bounds.Y + y * cellHeight,
                        cellWidth,
                        cellHeight
                    );

                    heatmap[x, y] = CalculateHazardDensity(cellArea);
                }
            }

            return heatmap;
        }

        ///<summary>
        ///Gets hazards in a specific area.
        ///</summary>
        ///<param name="area">The area to search.</param>
        ///<returns>List of hazards in the area.</returns>
        private List<Hazard> GetHazardsInArea(SASZombieAssaultTD.Engine.HazardsControl.Rectangle area)
        {
            return _hazards.Where(h => h != null && IsHazardInArea(h, area)).ToList();
        }

        ///<summary>
        ///Determines if a hazard is within an area.
        ///</summary>
        ///<param name="hazard">The hazard to check.</param>
        ///<param name="area">The area to check against.</param>
        ///<returns>True if hazard is within the area.</returns>
        private bool IsHazardInArea(Hazard hazard, SASZombieAssaultTD.Engine.HazardsControl.Rectangle area)
        {
            var hazardLeft = hazard.Position.X - hazard.Radius;
            var hazardRight = hazard.Position.X + hazard.Radius;
            var hazardTop = hazard.Position.Y - hazard.Radius;
            var hazardBottom = hazard.Position.Y + hazard.Radius;

            var areaLeft = area.X;
            var areaRight = area.X + area.Width;
            var areaTop = area.Y;
            var areaBottom = area.Y + area.Height;

            return !(hazardRight < areaLeft || hazardLeft > areaRight ||
                     hazardBottom < areaTop || hazardTop > areaBottom);
        }

        ///<summary>
        ///Finds a cluster of nearby hazards.
        ///</summary>
        ///<param name="startHazard">The hazard to start clustering from.</param>
        ///<param name="visited">Set of already visited hazards.</param>
        ///<returns>Set of hazards in the cluster.</returns>
        private HashSet<Hazard> FindCluster(Hazard startHazard, HashSet<Hazard> visited)
        {
            var cluster = new HashSet<Hazard>();
            var toVisit = new Queue<Hazard>();
            toVisit.Enqueue(startHazard);

            var clusterRadius = 200f; //Hazards within 200 units are considered clustered
            var clusterRadiusSquared = clusterRadius * clusterRadius;

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();

                if (visited.Contains(current)) continue;

                visited.Add(current);
                cluster.Add(current);

                //Find nearby hazards
                foreach (var hazard in _hazards)
                {
                    if (hazard == null || visited.Contains(hazard)) continue;

                    var distanceSquared = CalculateDistanceSquared(current.Position, hazard.Position);
                    if (distanceSquared <= clusterRadiusSquared)
                    {
                        toVisit.Enqueue(hazard);
                    }
                }
            }

            return cluster;
        }

        ///<summary>
        ///Gets cluster count in a specific area.
        ///</summary>
        ///<param name="area">The area to analyze.</param>
        ///<returns>Number of clusters in the area.</returns>
        private int GetClusterCountInArea(Rectangle area)
        {
            var hazardsInArea = GetHazardsInArea(area);
            var visited = new HashSet<Hazard>();
            var clusterCount = 0;

            foreach (var hazard in hazardsInArea)
            {
                if (visited.Contains(hazard)) continue;

                var cluster = FindCluster(hazard, visited);
                if (cluster.Count > 1)
                {
                    clusterCount++;
                }
            }

            return clusterCount;
        }

        ///<summary>
        ///Calculates squared distance between two positions.
        ///</summary>
        ///<param name="pos1">First position.</param>
        ///<param name="pos2">Second position.</param>
        ///<returns>Squared distance.</returns>
        private float CalculateDistanceSquared(Vector3 pos1, Vector3 pos2)
        {
            var dx = pos1.X - pos2.X;
            var dy = pos1.Y - pos2.Y;
            var dz = pos1.Z - pos2.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        ///<summary>
        ///Gets cache key for an area.
        ///</summary>
        ///<param name="area">The area to cache.</param>
        ///<returns>Cache key string.</returns>
        private string GetCacheKey(Rectangle area)
        {
            return $"{area.X}_{area.Y}_{area.Width}_{area.Height}";
        }

        ///<summary>
        ///Clears cache entries for an area.
        ///</summary>
        ///<param name="position">Center position.</param>
        ///<param name="radius">Clear radius.</param>
        private void ClearCacheForArea(Vector3 position, float radius)
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _densityCache)
            {
                //Simple check - can be enhanced with actual area calculation
                if (kvp.Value.CalculatedAt < DateTime.Now.AddSeconds(-5)) //Remove old cache entries
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _densityCache.Remove(key);
            }
        }

        ///<summary>
        ///Gets comprehensive density statistics.
        ///</summary>
        ///<returns>Complete density statistics.</returns>
        public DensityStatistics GetDensityStatistics()
        {
            if (!_isInitialized) return new DensityStatistics();

            return new DensityStatistics
            {
                TotalHazards = _hazards.Count,
                ClusterCount = GetHazardClusterCount(),
                AverageDensity = CalculateAverageDensity(),
                HighRiskAreas = GetHighRiskAreaCount(),
                CacheSize = _densityCache.Count
            };
        }

        ///<summary>
        ///Calculates average density across all hazards.
        ///</summary>
        ///<returns>Average density value.</returns>
        private float CalculateAverageDensity()
        {
            if (_hazards.Count == 0) return 0f;

            //Simple average density calculation
            var totalArea = 10000f * 10000f; //Assuming 10k x 10k world
            return _hazards.Count / totalArea;
        }

        ///<summary>
        ///Gets count of high risk areas.
        ///</summary>
        ///<returns>Number of high risk areas.</returns>
        private int GetHighRiskAreaCount()
        {
            //This would require area grid analysis
            //For now, return based on cluster count
            return GetHazardClusterCount();
        }

        ///<summary>
        ///Cleans up the hazard density system.
        ///</summary>
        public void Cleanup()
        {
            _hazards?.Clear();
            _densityCache?.Clear();
            _isInitialized = false;
        }

        ///<summary>
        ///Event triggered when a hazard is added.
        ///</summary>
        public event Action<Hazard> OnHazardAdded;

        ///<summary>
        ///Event triggered when a hazard is removed.
        ///</summary>
        public event Action<Hazard> OnHazardRemoved;
    }

    ///<summary>
    ///Cache entry for density calculations.
    ///</summary>
    internal class DensityCache
    {
        public float Density { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    ///<summary>
    ///Statistics for density operations.
    ///</summary>
    public class DensityStatistics
    {
        public int TotalHazards { get; set; }
        public int ClusterCount { get; set; }
        public float AverageDensity { get; set; }
        public int HighRiskAreas { get; set; }
        public int CacheSize { get; set; }
    }
}
