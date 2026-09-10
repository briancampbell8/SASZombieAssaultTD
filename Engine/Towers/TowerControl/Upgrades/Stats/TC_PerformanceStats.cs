// =====================================================================================================
//  FILE: TC_PerformanceStats.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Stats/TC_PerformanceStats.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Stats Subsystem
//
//  ROLE:
//      Computes deterministic statistical summaries for tower upgrade performance data.
//      Provides aggregate metrics used by other TowerControl upgrade subsystems.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Compute deterministic statistical values (min, max, average performance).
//      - Provide access to computed statistics for downstream systems.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing performance calculations for individual upgrades.
//      - Managing persistence, file I/O, or external data loading.
//      - Validating upgrade correctness or enforcing upgrade rules.
//
//  ARCHITECTURAL NOTES:
//      - Operates strictly on provided upgrade data; does not mutate external systems.
//      - Shutdown clears internal references and computed values for deterministic teardown.
//      - Serves as a lightweight statistical layer for TowerControl upgrade subsystems.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Diagnostics;

namespace SASZombieAssaultTD.Engine.Towers.TowerControl.Upgrades.Stats
{
    internal sealed class TC_PerformanceStats
    {
        private IReadOnlyList<TowerUpgrade> _upgrades;

        private float _minPerformance;
        private float _maxPerformance;
        private float _averagePerformance;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_PerformanceStats: Initialize - Binding upgrade list and computing performance statistics.");

            // Bind upgrade list
            _upgrades = upgrades;

            // Compute deterministic statistics
            ComputeStatistics();
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_PerformanceStats: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_PerformanceStats: Shutdown - Clearing upgrade list and computed statistics.");

            // Clear internal state
            _upgrades = null;
            _minPerformance = 0f;
            _maxPerformance = 0f;
            _averagePerformance = 0f;
        }

        private void ComputeStatistics()
        {
            // Trace computation start
            Trace.WriteLine("TC_PerformanceStats: ComputeStatistics - Computing min, max, and average performance.");

            // Guard against null or empty list
            if (_upgrades == null || _upgrades.Count == 0)
            {
                _minPerformance = 0f;
                _maxPerformance = 0f;
                _averagePerformance = 0f;

                Trace.WriteLine("TC_PerformanceStats: ComputeStatistics - No upgrades available; all statistics set to 0.");
                return;
            }

            float min = float.MaxValue;
            float max = float.MinValue;
            float total = 0f;

            // Iterate through upgrades to compute statistics
            for (int i = 0; i < _upgrades.Count; i++)
            {
                float perf = _upgrades[i].Performance;

                if (perf < min) min = perf;
                if (perf > max) max = perf;

                total += perf;
            }

            _minPerformance = min;
            _maxPerformance = max;
            _averagePerformance = total / _upgrades.Count;

            // Trace final computed values
            Trace.WriteLine($"TC_PerformanceStats: ComputeStatistics - min={_minPerformance}, max={_maxPerformance}, avg={_averagePerformance}.");
        }

        public float GetMinPerformance() => _minPerformance;
        public float GetMaxPerformance() => _maxPerformance;
        public float GetAveragePerformance() => _averagePerformance;
    }
}
