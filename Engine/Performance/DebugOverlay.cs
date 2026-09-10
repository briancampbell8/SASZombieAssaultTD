using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextRendering;
using SASZombieAssaultTD.Engine.TextureRendering;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Performance
{
    /// <summary>
    /// In‑game debug overlay for performance monitoring.
    /// </summary>
    public class DebugOverlay
    {
        // ---------------------------------------------------------------------------------------------
        // Fields
        // ---------------------------------------------------------------------------------------------

        private readonly PerformanceProfiler _profiler;
        private readonly FramePacer _framePacer;

        private readonly List<float> _frameTimeHistory = new();
        private readonly List<float> _memoryHistory = new();
        private readonly List<float> _cpuHistory = new();
        private readonly List<float> _gpuHistory = new();

        private readonly int _maxHistorySize = 300;

        private bool _visible;
        private bool _showFrameTimeGraph = true;
        private bool _showMemoryGraph = true;
        private bool _showCPUGraph = true;
        private bool _showGPUGraph = false;
        private bool _showEntityCount = true;
        private bool _showDrawCalls = true;
        private bool _showPathfindingMetrics = true;
        private bool _showUIMetrics = true;

        private int _ECSEntityCoreCount;
        private string _debugInfo;

        private Font _debugFont = new Font("Consolas", 12);
        private Color _textColor = new Color(0, 255, 0, 255);
        private Color _graphColor = new Color(0, 255, 0, 255);
        private Color _backgroundColor = new Color(0, 0, 0, 128);

        private System.Numerics.Vector3 _position = new System.Numerics.Vector3(10f, 10f, 0f);
        private float _scale = 1f;

        // ---------------------------------------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------------------------------------

        public bool Visible { get => _visible; set => _visible = value; }
        public bool ShowFrameTimeGraph { get => _showFrameTimeGraph; set => _showFrameTimeGraph = value; }
        public bool ShowMemoryGraph { get => _showMemoryGraph; set => _showMemoryGraph = value; }
        public bool ShowCPUGraph { get => _showCPUGraph; set => _showCPUGraph = value; }
        public bool ShowGPUGraph { get => _showGPUGraph; set => _showGPUGraph = value; }
        public bool ShowEntityCount { get => _showEntityCount; set => _showEntityCount = value; }
        public bool ShowDrawCalls { get => _showDrawCalls; set => _showDrawCalls = value; }
        public bool ShowPathfindingMetrics { get => _showPathfindingMetrics; set => _showPathfindingMetrics = value; }
        public bool ShowUIMetrics { get => _showUIMetrics; set => _showUIMetrics = value; }

        public event Action<bool> OnVisibilityChanged;

        // ---------------------------------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------------------------------

        public DebugOverlay(PerformanceProfiler profiler = null, FramePacer framePacer = null)
        {
            _profiler = profiler;
            _framePacer = framePacer;

            DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "INFO", "DebugOverlay: Initialized");
        }

        // ---------------------------------------------------------------------------------------------
        // Draw (D3D11Adapter_Core path)
        // ---------------------------------------------------------------------------------------------

        public void Draw(D3D11Adapter_Core context)
        {
            if (!_visible || context == null)
                return;

            var white = new Color(255, 255, 255, 255);

            context.DrawText($"FPS: {System.Math.Round(_profiler?.CurrentFPS ?? 0)}",
                new System.Numerics.Vector3(10f, 10f, 0f), white);

            if (_memoryHistory.Count > 0)
            {
                context.DrawText($"Memory: {_memoryHistory[^1]:F1} MB",
                    new System.Numerics.Vector3(10f, 30f, 0f), white);
            }

            context.DrawText($"Entities: {_ECSEntityCoreCount}",
                new System.Numerics.Vector3(10f, 50f, 0f), white);

            var drawCalls = _profiler?.DrawCallCount ?? 0;
            context.DrawText($"Draw Calls: {drawCalls}",
                new System.Numerics.Vector3(10f, 70f, 0f), white);
        }

        // ---------------------------------------------------------------------------------------------
        // Update
        // ---------------------------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            if (!_visible)
                return;

            UpdateHistoryData();
        }

        private void UpdateHistoryData()
        {
            // Frame time
            if (_profiler != null)
            {
                var frameMetric = _profiler.GetMetric("Frame");
                if (frameMetric != null)
                {
                    _frameTimeHistory.Add(frameMetric.AverageTime);
                    Trim(_frameTimeHistory);
                }
            }

            // Memory
            var mem = GC.GetTotalMemory(false) / 1024f / 1024f;
            _memoryHistory.Add(mem);
            Trim(_memoryHistory);

            // CPU
            if (_profiler != null)
            {
                _cpuHistory.Add(_profiler.CPUUsage);
                Trim(_cpuHistory);
            }

            // GPU (placeholder)
            _gpuHistory.Add(0f);
            Trim(_gpuHistory);
        }

        private void Trim(List<float> list)
        {
            while (list.Count > _maxHistorySize)
                list.RemoveAt(0);
        }

        // ---------------------------------------------------------------------------------------------
        // Render (SpriteBatch path)
        // ---------------------------------------------------------------------------------------------

        public void Render(SpriteBatch spriteBatch)
        {
            if (!_visible || spriteBatch == null)
                return;

            try
            {
                RenderBackground(spriteBatch);
                RenderPerformanceMetrics(spriteBatch);

                if (_showFrameTimeGraph) RenderFrameTimeGraph(spriteBatch);
                if (_showMemoryGraph) RenderMemoryGraph(spriteBatch);
                if (_showCPUGraph) RenderCPUGraph(spriteBatch);
                if (_showGPUGraph) RenderGPUGraph(spriteBatch);

                if (_showEntityCount) RenderEntityCount(spriteBatch);
                if (_showDrawCalls) RenderDrawCalls(spriteBatch);
                if (_showPathfindingMetrics) RenderPathfindingMetrics(spriteBatch);
                if (_showUIMetrics) RenderUIMetrics(spriteBatch);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "ERROR",
                    $"DebugOverlay: Failed to render - {ex.Message}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Rendering helpers
        // ---------------------------------------------------------------------------------------------

        private void RenderBackground(SpriteBatch spriteBatch)
        {
            DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE",
                "DebugOverlay: Rendered background");
        }

        private void RenderPerformanceMetrics(SpriteBatch spriteBatch)
        {
            var lines = new List<string>();

            if (_profiler != null)
            {
                lines.Add($"FPS: {_profiler.CurrentFPS:F1}");
                lines.Add($"Frame Time: {_profiler.FrameTimeMs:F2}ms");
                lines.Add($"CPU: {_profiler.CPUUsage:F1}%");
                lines.Add($"Memory: {_memoryHistory[^1]:F1}MB");
            }

            if (_framePacer != null)
            {
                lines.Add($"Target FPS: {_framePacer.TargetFPS}");
                lines.Add($"Dropped Frames: {_framePacer.DroppedFrames}");
            }

            RenderTextLines(spriteBatch, lines, _position);
        }

        private void RenderFrameTimeGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _frameTimeHistory, "Frame Time (ms)", 16.67f,
                new Color(255, 0, 0, 255));
        }

        private void RenderMemoryGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _memoryHistory, "Memory (MB)", 0f,
                new Color(0, 0, 255, 255));
        }

        private void RenderCPUGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _cpuHistory, "CPU (%)", 0f,
                new Color(255, 255, 0, 255));
        }

        private void RenderGPUGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _gpuHistory, "GPU (%)", 0f,
                new Color(0, 255, 255, 255));
        }

        private void RenderGraph(SpriteBatch spriteBatch, List<float> data,
            string title, float targetValue, Color color)
        {
            if (data.Count < 2)
                return;

            DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE",
                $"DebugOverlay: Rendered {title} graph with {data.Count} points");
        }

        private void RenderEntityCount(SpriteBatch spriteBatch)
        {
            var lines = new List<string> { $"Entities: {_ECSEntityCoreCount}" };
            var pos = new System.Numerics.Vector3(_position.X, _position.Y + 200, 0f);
            RenderTextLines(spriteBatch, lines, pos);
        }

        private void RenderDrawCalls(SpriteBatch spriteBatch)
        {
            var drawCalls = _profiler?.DrawCallCount ?? 0;
            var lines = new List<string> { $"Draw Calls: {drawCalls}" };
            var pos = new System.Numerics.Vector3(_position.X, _position.Y + 220, 0f);
            RenderTextLines(spriteBatch, lines, pos);
        }

        private void RenderPathfindingMetrics(SpriteBatch spriteBatch)
        {
            var lines = new List<string>
            {
                "Pathfinders: 0",
                "Cache Hits: 0"
            };

            var pos = new System.Numerics.Vector3(_position.X, _position.Y + 240, 0f);
            RenderTextLines(spriteBatch, lines, pos);
        }

        private void RenderUIMetrics(SpriteBatch spriteBatch)
        {
            var lines = new List<string> { "UI Elements: 0" };
            var pos = new System.Numerics.Vector3(_position.X, _position.Y + 280, 0f);
            RenderTextLines(spriteBatch, lines, pos);
        }

        private void RenderTextLines(SpriteBatch spriteBatch,
            List<string> lines, System.Numerics.Vector3 position)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var linePos = new System.Numerics.Vector3(position.X,
                    position.Y + i * 15f * _scale, position.Z);

                DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE",
                    $"DebugOverlay: Rendered text '{lines[i]}' at {linePos}");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------

        public void ToggleVisibility()
        {
            _visible = !_visible;
            OnVisibilityChanged?.Invoke(_visible);

            DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "INFO",
                $"DebugOverlay: Visibility set to {_visible}");
        }

        public override string ToString()
        {
            return $"DebugOverlay: Visible={_visible}, Position={_position}, " +
                   $"Scale={_scale}, FrameHistory={_frameTimeHistory.Count}";
        }
    }
}
