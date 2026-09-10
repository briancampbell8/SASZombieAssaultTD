// =====================================================================================================
//  FILE: SpatialGridRuntime.cs
//  PATH: Engine/ECS/ECSRuntimeCore/SpatialGridRuntime.cs
//  SUBSYSTEM: ECS ECSRuntimeCore
//
//  ROLE:
//      Provides deterministic spatial‑partitioning support for the ECSRuntimeCore subsystem.
//      Supplies cell‑based ECSEntityCore organization, lookup, and pooling behaviors for collision and
//      spatial‑query subsystems. Ensures stable grid operations with diagnostic tracing.
//
//  RESPONSIBILITIES:
//      - Provide spatial cell allocation and retrieval behavior.
//      - Provide deterministic ECSEntityCore insertion and removal behavior.
//      - Provide cell pooling and reuse behavior.
//      - Provide grid‑coordinate resolution behavior.
//      - Integrate deterministic diagnostic tracing using DLogger.Log.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision resolution or physics simulation.
//      - Managing ECSEntityCore lifecycle or component storage.
//      - Executing world update sequencing or rendering logic.
//      - Performing multi‑component queries or system orchestration.
//
//  ARCHITECTURAL NOTES:
//      - SpatialGrid is a dedicated subsystem file extracted from ECSRuntimeCore.cs.
//      - Operates strictly on ECSRuntimeCore‑provided ECSEntityCore references.
//      - All diagnostic output uses DLogger.Log for deterministic engine tracing.
// =====================================================================================================
// Engine/Physics/SpatialGridRuntime.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Physics
{
    public sealed class SpatialGridRuntime
    {
        public Vector3 WorldMin { get; internal set; }
        private SpatialGridStats _stats;
        public SpatialGridStats GetStatsObject() => _stats;
        private readonly Dictionary<(int X, int Y), List<ECSEntityCore>> _cells;
        private readonly ConcurrentBag<List<ECSEntityCore>> _cellPool;
        private readonly int _cellSize;
        private (int X, int Y) ResolveCell(ECSEntityCore ECSEntityCore) { /* existing impl */ throw new NotImplementedException(); }
        private List<ECSEntityCore> GetOrCreateCell((int X, int Y) cell) { /* existing impl */ throw new NotImplementedException(); }
        public void InsertEntity(ECSEntityCore ECSEntityCore) { /* existing impl */ }
        public void RemoveEntity(ECSEntityCore ECSEntityCore) { /* existing impl */ }
        public IReadOnlyList<ECSEntityCore> GetEntitiesInSameCell(ECSEntityCore ECSEntityCore) { /* existing impl */ throw new NotImplementedException(); }

        /// <summary>
        /// Returns all entities whose cells intersect the axis-aligned rectangle defined by areaMin/areaMax.
        /// This is a lightweight read-only aggregation suitable for visualization and queries that span multiple cells.
        /// </summary>
        public IReadOnlyList<ECSEntityCore> GetEntitiesInArea(Vector3 areaMin, Vector3 areaMax)
        {
            // Normalize min/max in case caller passed them in reverse
            float minXf = System.Math.Min(areaMin.X, areaMax.X);
            float minYf = System.Math.Min(areaMin.Y, areaMax.Y);
            float maxXf = System.Math.Max(areaMin.X, areaMax.X);
            float maxYf = System.Math.Max(areaMin.Y, areaMax.Y);

            // Convert world coords into cell indices relative to WorldMin and cell size
            int minCellX = (int)System.Math.Floor((minXf - WorldMin.X) / _cellSize);
            int minCellY = (int)System.Math.Floor((minYf - WorldMin.Y) / _cellSize);
            int maxCellX = (int)System.Math.Floor((maxXf - WorldMin.X) / _cellSize);
            int maxCellY = (int)System.Math.Floor((maxYf - WorldMin.Y) / _cellSize);

            var resultSet = new HashSet<ECSEntityCore>();
            for (int x = minCellX; x <= maxCellX; x++)
            {
                for (int y = minCellY; y <= maxCellY; y++)
                {
                    if (_cells.TryGetValue((x, y), out var list) && list != null)
                    {
                        foreach (var e in list)
                        {
                            // avoid duplicates across adjacent cells
                            resultSet.Add(e);
                        }
                    }
                }
            }

            var result = new List<ECSEntityCore>(resultSet.Count);
            result.AddRange(resultSet);
            return result;
        }
    }
}