using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Diagnostics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Core
{

    public class TimingController
    {
        // --- Public API consumed by GameLoop ---
           
        public float DeltaTime { get; private set; }
        public bool ShouldUpdate { get; private set; }
        public bool ShouldRender { get; private set; }
        // --- Fixed timestep configuration ---
        private const float FixedTimeStep = 1f / 60f; // 60 Hz update loop
                                                      // --- Internal state ---
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private long _lastTimestamp;
        private float _accumulator;
        public void Start()
        {
            DebugLogger.Log("BREAKPOINT", "Execution reached here");
            DebugLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DebugLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");

            Console.WriteLine("[TimingController] Start() called.");
            // your timing setup logic
            _stopwatch.Reset();
            _stopwatch.Start();
            _lastTimestamp = 0;
            _accumulator = 0f;
            DeltaTime = 0f;
            ShouldUpdate = false;
            ShouldRender = false;
        }
        public void Tick()
        {
            ShouldUpdate = false;
            ShouldRender = false;
            long current = _stopwatch.ElapsedTicks;
            if (_lastTimestamp == 0)
            {
                _lastTimestamp = current;
                return;
            }
            long deltaTicks = current - _lastTimestamp;
            _lastTimestamp = current;
            DeltaTime = (float)deltaTicks / Stopwatch.Frequency;
            // Accumulate time for fixed updates
            _accumulator += DeltaTime;
            // Determine if we should run Update()
            if (_accumulator >= FixedTimeStep)
            {
                ShouldUpdate = true;
                _accumulator -= FixedTimeStep;
            }
            // Render every frame
            ShouldRender = true;
        }
    }

}
