// =====================================================================================================
//  FILE: TC_PowerStats.cs
//  PATH: Engine/Towers/TowerControl/Upgrades/Stats/TC_PowerStats.cs
//  SUBSYSTEM: Towers TowerControl Upgrades Stats Subsystem
//
//  ROLE:
//      Computes deterministic statistical summaries for tower upgrade power data.
//      Provides aggregate metrics used by other TowerControl upgrade subsystems.
//      Supports lifecycle operations for initialization, execution entry, and shutdown.
//
//  RESPONSIBILITIES:
//      - Initialize with a read-only list of tower upgrades.
//      - Compute deterministic statistical values (min, max, average power).
//      - Provide access to computed statistics for downstream systems.
//      - Maintain lifecycle structure: Initialize → Execute → Shutdown.
//
//  NON-RESPONSIBILITIES:
//      - Performing power calculations for individual upgrades.
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
    internal sealed class TC_PowerStats
    {
        private IReadOnlyList<TowerUpgrade> _upgrades;

        private float _minPower;
        private float _maxPower;
        private float _averagePower;

        public void Initialize(IReadOnlyList<TowerUpgrade> upgrades)
        {
            // Trace lifecycle initialization
            Trace.WriteLine("TC_PowerStats: Initialize - Binding upgrade list and computing power statistics.");

            // Bind upgrade list
            _upgrades = upgrades;

            // Compute deterministic statistics
            ComputeStatistics();
        }

        public void Execute()
        {
            // Trace execution entry (no runtime operations required)
            Trace.WriteLine("TC_PowerStats: Execute - No active runtime operations required.");
        }

        public void Shutdown()
        {
            // Trace teardown
            Trace.WriteLine("TC_PowerStats: Shutdown - Clearing upgrade list and computed statistics.");

            // Clear internal state
            _upgrades = null;
            _minPower = 0f;
            _maxPower = 0f;
            _averagePower = 0f;
        }

        private void ComputeStatistics()
        {
            // Trace computation start
            Trace.WriteLine("TC_PowerStats: ComputeStatistics - Computing min, max, and average power.");

            // Guard against null or empty list
            if (_upgrades == null || _upgrades.Count == 0)
            {
                _minPower = 0f;
                _maxPower = 0f;
                _averagePower = 0f;

                Trace.WriteLine("TC_PowerStats: ComputeStatistics - No upgrades available; all statistics set to 0.");
                return;
            }

            float min = float.MaxValue;
            float max = float.MinValue;
            float total = 0f;

            // Iterate through upgrades to compute statistics
            for (int i = 0; i < _upgrades.Count; i++)
            {
                float power = _upgrades[i].Power;

                if (power < min) min = power;
                if (power > max) max = power;

                total += power;
            }

            _minPower = min;
            _maxPower = max;
            _averagePower = total / _upgrades.Count;

            // Trace final computed values
            Trace.WriteLine($"TC_PowerStats: ComputeStatistics - min={_minPower}, max={_maxPower}, avg={_averagePower}.");
        }

        public float GetMinPower() => _minPower;
        public float GetMaxPower() => _maxPower;
        public float GetAveragePower() => _averagePower;
    }
}
