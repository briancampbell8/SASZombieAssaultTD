// =====================================================================================================
//  FILE: PerformanceMetric.cs
//  PATH: Engine/Performance/PerformanceMetric.cs
//  SUBSYSTEM: Performance
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Performance
{
    public class PerformanceMetric
    {
        public string Name { get; }
        public float MinTime { get; private set; }
        public float MaxTime { get; private set; }
        public float AverageTime { get; private set; }
        public float TotalTime { get; private set; }
        public int SampleCount { get; private set; }

        public PerformanceMetric(string name)
        {
            Name = name;
            MinTime = float.MaxValue;
            MaxTime = 0f;
            AverageTime = 0f;
            TotalTime = 0f;
            SampleCount = 0;
        }

        public void AddMeasurement(float duration)
        {
            TotalTime += duration;
            SampleCount++;
            AverageTime = TotalTime / SampleCount;
            MinTime = System.Math.Min(MinTime, duration);
            MaxTime = System.Math.Max(MaxTime, duration);
        }

        public void Reset()
        {
            MinTime = float.MaxValue;
            MaxTime = 0f;
            AverageTime = 0f;
            TotalTime = 0f;
            SampleCount = 0;
        }

        public override string ToString()
        {
            return $"{Name}: Avg={AverageTime:F2}ms, Min={MinTime:F2}ms, Max={MaxTime:F2}ms, Samples={SampleCount}";
        }
    }
}
