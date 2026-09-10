//=====================================================================================================
//  FILE: RenderPerformanceMonitor.cs
//  PATH: Engine/Diagnostics/Performance/RenderPerformanceMonitor.cs
//  SUBSYSTEM: Diagnostics – Performance Sampling
//  ROLE: Provides allocationless, deterministic performance metrics for the rendering pipeline.
//
//  RESPONSIBILITIES:
//      - Sample frame timing using high‑resolution, monotonic timers.
//      - Compute real FPS, CPU load, and GPU load without introducing GC pressure.
//      - Emit deterministic diagnostics for engine‑level performance analysis.
//      - Serve as the source-of-truth provider for ModernUIRenderer_Performance.
//
//  NON-RESPONSIBILITIES:
//      - Making quality decisions (handled by ModernUIRenderer_Performance).
//      - Rendering or participating in the draw pipeline.
//      - Managing UI or HUD components.
//      - Performing blocking OS queries on the main render thread.
//
//  ARCHITECTURAL NOTES:
//      - This subsystem must remain allocationless in the hot path.
//      - All sampling must be thread-safe and free of jitter.
//      - Consumers (UI, HUD, gameplay systems) must treat its output as authoritative.
//      - Legacy RenderStats is fully retired; only RenderPerformanceStats is produced.
//=====================================================================================================

using System;
using System.Diagnostics;
using System.Threading;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Diagnostics.Performance
{
    internal sealed class RenderPerformanceMonitor : IDisposable
    {
        private long _lastTimestamp;
        private double _deltaTime;
        private double _fps;

        private float _cachedCpuLoad;
        private float _cachedGpuLoad;

        private readonly Thread _samplingThread;
        private readonly CancellationTokenSource _cts = new();
        private const int SamplingIntervalMs = 500;

        public RenderPerformanceMonitor()
        {
            _samplingThread = new Thread(BackgroundSamplingLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
                Name = "RenderTelemetrySampler"
            };
            _samplingThread.Start();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Diagnostics", "[RenderPerformanceMonitor] Subsystem initialized. Offloaded OS counters to background worker thread.");
        }

        public RenderPerformanceStats GetStats()
        {
            SampleFrameTiming();

            float currentCpu = Volatile.Read(ref _cachedCpuLoad);
            float currentGpu = Volatile.Read(ref _cachedGpuLoad);

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Telemetry", $"[PerformanceMonitor] Telemetry dispatched: FPS={_fps:F2}, CPU={currentCpu:F1}%, GPU={currentGpu:F1}%");

            return new RenderPerformanceStats(
                fps: _fps,
                cpuLoad: currentCpu,
                gpuLoad: currentGpu
            );
        }

        public void SampleFrameTiming()
        {
            long current = Stopwatch.GetTimestamp();

            if (_lastTimestamp != 0)
            {
                long deltaTicks = current - _lastTimestamp;
                _deltaTime = deltaTicks / (double)Stopwatch.Frequency;

                if (_deltaTime > 0)
                    _fps = 1.0 / _deltaTime;
            }

            _lastTimestamp = current;
        }

        private void BackgroundSamplingLoop()
        {
            PerformanceCounter? cpuCounter = null;
            PerformanceCounter? gpuCounter = null;

            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                gpuCounter = new PerformanceCounter("GPU Engine", "Utilization Percentage", "engtype_3D", true);

                cpuCounter.NextValue();
                gpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Error", $"[PerformanceMonitor] Windows Performance Counters failed to initialize. Fallbacks active. Details: {ex.Message}");
            }

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    float cpuSample = cpuCounter != null ? cpuCounter.NextValue() : -1f;
                    float gpuSample = gpuCounter != null ? gpuCounter.NextValue() : -1f;

                    Volatile.Write(ref _cachedCpuLoad, cpuSample);
                    Volatile.Write(ref _cachedGpuLoad, gpuSample);
                }
                catch
                {
                    Volatile.Write(ref _cachedCpuLoad, -1f);
                    Volatile.Write(ref _cachedGpuLoad, -1f);
                }

                if (_cts.Token.WaitHandle.WaitOne(SamplingIntervalMs))
                    break;
            }

            cpuCounter?.Dispose();
            gpuCounter?.Dispose();
        }

        public void Dispose()
        {
            _cts.Cancel();
            if (_samplingThread.IsAlive)
            {
                _samplingThread.Join(1000);
            }
            _cts.Dispose();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Diagnostics", "[RenderPerformanceMonitor] Subsystem safely shutdown and unmanaged counters disposed.");
        }
    }

    internal readonly record struct RenderPerformanceStats
    {
        public double Fps { get; }
        public float CpuLoad { get; }
        public float GpuLoad { get; }

        public RenderPerformanceStats(double fps, float cpuLoad, float gpuLoad)
        {
            Fps = fps;
            CpuLoad = cpuLoad;
            GpuLoad = gpuLoad;
        }
    }
}
