using System.Diagnostics;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Timing
//
{
    ///<summary>
    ///High-precision timing module for delta time calculation and frame pacing.
    ///Implements P11-09-02: High-precision timing with frame pacing logic.
    ///</summary>
    public class TimingModule
    {
        //--- Public API consumed by GameLoop ---
        ///<summary>
        ///Gets the delta time in seconds since the last frame.
        ///</summary>
        public float DeltaTime { get; private set; }

        ///<summary>
        ///Gets whether an update should be performed this frame (fixed timestep).
        ///</summary>
        public bool ShouldUpdate { get; private set; }

        ///<summary>
        ///Gets whether a render should be performed this frame.
        ///</summary>
        public bool ShouldRender { get; private set; }

        ///<summary>
        ///Gets the current frames per second.
        ///</summary>
        public float CurrentFPS { get; private set; }

        ///<summary>
        ///Gets the average frame time over the last second.
        ///</summary>
        public float AverageFrameTime { get; private set; }

        //--- Fixed timestep configuration ---
        ///<summary>
        ///Fixed timestep for updates (60 Hz by default).
        ///</summary>
        private const float FixedTimeStep = 1f / 60f;

        ///<summary>
        ///Target frame rate for rendering (60 FPS by default).
        ///</summary>
        private const float TargetFrameRate = 60f;

        ///<summary>
        ///Target frame time in milliseconds.
        ///</summary>
        private const float TargetFrameTimeMs = 1000f / TargetFrameRate;

        //--- Internal state ---
        private readonly Stopwatch _highPrecisionClock = new Stopwatch();
        private long _lastTimestamp;
        private float _accumulator;
        private float _fpsAccumulator;
        private int _fpsFrameCount;
        private float _totalFrameTime;
        private int _frameTimeCount;

        ///<summary>
        ///Initializes the timing module and starts the high-precision clock.
        ///</summary>
        public void Start()
        {
            DLogger.Log(LogSubsystems.Timing,LogLevel.Info, "TimingModule started - P11-09-02: High-precision timing initialized");

            _highPrecisionClock.Reset();
            _highPrecisionClock.Start();
            _lastTimestamp = 0;
            _accumulator = 0f;
            _fpsAccumulator = 0f;
            _fpsFrameCount = 0;
            _totalFrameTime = 0f;
            _frameTimeCount = 0;

            DeltaTime = 0f;
            ShouldUpdate = false;
            ShouldRender = false;
            CurrentFPS = 0f;
            AverageFrameTime = 0f;
        }

        ///<summary>
        ///Processes timing for a single frame.
        ///Implements delta time calculation and frame pacing logic.
        ///</summary>
        public void Tick()
        {
            ShouldUpdate = false;
            ShouldRender = false;

            long currentTimestamp = _highPrecisionClock.ElapsedTicks;

            //Handle first frame
            if (_lastTimestamp == 0)
            {
                _lastTimestamp = currentTimestamp;
                DeltaTime = 0f;
                ShouldRender = true; //Always render first frame
                return;
            }

            //Calculate delta time using high-precision clock
            long deltaTicks = currentTimestamp - _lastTimestamp;
            _lastTimestamp = currentTimestamp;
            DeltaTime = (float)deltaTicks / Stopwatch.Frequency;

            //Clamp delta time to prevent spiral of death
            if (DeltaTime > 0.1f) //Cap at 100ms delta time
            {
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "WARNING", $"Large delta time detected: {DeltaTime:F3}s, clamping to 0.1s");
                DeltaTime = 0.1f;
            }

            //Accumulate time for fixed updates
            _accumulator += DeltaTime;

            //Determine if we should run Update() (fixed timestep)
            if (_accumulator >= FixedTimeStep)
            {
                ShouldUpdate = true;
                _accumulator -= FixedTimeStep;
            }

            //Render every frame (variable timestep)
            ShouldRender = true;

            //Update FPS calculation
            UpdateFPSMetrics(DeltaTime);

            //Frame pacing - sleep to maintain target frame rate
            PerformFramePacing();
        }

        ///<summary>
        ///Updates FPS and frame time metrics.
        ///</summary>
        ///<param name="frameTime">The current frame time.</param>
        private void UpdateFPSMetrics(float frameTime)
        {
            _fpsAccumulator += frameTime;
            _fpsFrameCount++;
            _totalFrameTime += frameTime;
            _frameTimeCount++;

            //Update FPS every second
            if (_fpsAccumulator >= 1.0f)
            {
                CurrentFPS = _fpsFrameCount / _fpsAccumulator;
                AverageFrameTime = _totalFrameTime / _frameTimeCount;

                //Reset accumulators
                _fpsAccumulator = 0f;
                _fpsFrameCount = 0;
                _totalFrameTime = 0f;
                _frameTimeCount = 0;

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Info, "DEBUG", $"Timing metrics - FPS: {CurrentFPS:F1}, Avg Frame Time: {AverageFrameTime:F3}ms");
            }
        }

        ///<summary>
        ///Performs frame pacing to maintain stable frame rates.
        ///Implements P11-09-02: Frame pacing logic.
        ///</summary>
        private void PerformFramePacing()
        {
            //Calculate how much time we should wait to hit target frame time
            long elapsedMs = _highPrecisionClock.ElapsedMilliseconds;
            long frameEndMs = (long)(elapsedMs / TargetFrameTimeMs * TargetFrameTimeMs + TargetFrameTimeMs);
            long sleepTimeMs = frameEndMs - elapsedMs;

            //Only sleep if we need to wait and the sleep time is reasonable
            if (sleepTimeMs > 0 && sleepTimeMs < TargetFrameTimeMs)
            {
                //Use high-precision sleep for better accuracy
                //Note: Thread.Sleep is not very precise, but it's the best we can do in pure C#
                //For production, consider using platform-specific high-precision timers
                System.Threading.Thread.Sleep((int)sleepTimeMs);
            }
        }

        ///<summary>
        ///Gets timing statistics for debugging and diagnostics.
        ///</summary>
        ///<returns>A string containing timing statistics.</returns>
        public string GetTimingStats()
        {
            return $"FPS: {CurrentFPS:F1} | Delta: {DeltaTime * 1000:F2}ms | Avg: {AverageFrameTime * 1000:F2}ms | Update: {ShouldUpdate} | Render: {ShouldRender}";
        }

        ///<summary>
        ///Resets the timing module to initial state.
        ///</summary>
        public void Reset()
        {
            DLogger.Log(LogSubsystems.Timing,LogLevel.Info, "TimingModule reset");
            Start();
        }
    }
}




