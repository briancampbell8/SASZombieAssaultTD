using System;
using System.Collections.Generic;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
using Font = SASZombieAssaultTD.Engine.Rendering.Font;
namespace SASZombieAssaultTD.Engine.Performance
//
{
    ///<summary>
    ///In-game debug overlay for performance monitoring.
    ///P30-09-01: Add in-game profiler overlay.
    ///P30-09-02: Add frame time graph.
    ///P30-09-03: Add memory usage graph.
    ///P30-09-04: Add CPU usage graph.
    ///P30-09-05: Add GPU usage graph.
    ///P30-09-06: Add entity count display.
    ///P30-09-07: Add draw call counter.
    ///P30-09-08: Add pathfinding metrics display.
    ///P30-09-09: Add UI performance metrics.
    ///</summary>
    public class DebugOverlay
    {
        private readonly PerformanceProfiler _profiler;
        private readonly FramePacer _framePacer;
        private readonly List<float> _frameTimeHistory;
        private readonly List<float> _memoryHistory;
        private readonly List<float> _cpuHistory;
        private readonly List<float> _gpuHistory;
        private readonly int _maxHistorySize;
        private bool _visible;
        private bool _showFrameTimeGraph;
        private bool _showMemoryGraph;
        private bool _showCPUGraph;
        private bool _showGPUGraph;
        private bool _showEntityCount;
        private bool _showDrawCalls;
        private int _entityCount;
        private string _debugInfo;
        private bool _showPathfindingMetrics;
        private bool _showUIMetrics;
        private Font _debugFont;
        private Color _textColor;
        private Color _graphColor;
        private Color _backgroundColor;
        private Vector3 _position;
        private float _scale;

        ///<summary>
        ///Gets or sets whether the debug overlay is visible.
        ///</summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        ///<summary>
        ///Gets or sets whether to show the frame time graph.
        ///</summary>
        public bool ShowFrameTimeGraph
        {
            get => _showFrameTimeGraph;
            set => _showFrameTimeGraph = value;
        }

        ///<summary>
        ///Gets or sets whether to show the memory graph.
        ///</summary>
        public bool ShowMemoryGraph
        {
            get => _showMemoryGraph;
            set => _showMemoryGraph = value;
        }

        ///<summary>
        ///Gets or sets whether to show the CPU graph.
        ///</summary>
        public bool ShowCPUGraph
        {
            get => _showCPUGraph;
            set => _showCPUGraph = value;
        }

        ///<summary>
        ///Gets or sets whether to show the GPU graph.
        ///</summary>
        public bool ShowGPUGraph
        {
            get => _showGPUGraph;
            set => _showGPUGraph = value;
        }

        ///<summary>
        ///Gets or sets whether to show entity count.
        ///</summary>
        public bool ShowEntityCount
        {
            get => _showEntityCount;
            set => _showEntityCount = value;
        }

        ///<summary>
        ///Gets or sets whether to show draw calls.
        ///</summary>
        public bool ShowDrawCalls
        {
            get => _showDrawCalls;
            set => _showDrawCalls = value;
        }

        ///<summary>
        ///Gets or sets whether to show pathfinding metrics.
        ///</summary>
        public bool ShowPathfindingMetrics
        {
            get => _showPathfindingMetrics;
            set => _showPathfindingMetrics = value;
        }

        ///<summary>
        ///Gets or sets whether to show UI metrics.
        ///</summary>
        public bool ShowUIMetrics
        {
            get => _showUIMetrics;
            set => _showUIMetrics = value;
        }

        ///<summary>
        ///Event fired when overlay visibility changes.
        ///</summary>
        public event Action<bool> OnVisibilityChanged;

        ///<summary>
        ///Initializes a new debug overlay.
        ///</summary>
        ///<param name="profiler">The performance profiler.</param>
        ///<param name="framePacer">The frame pacer.</param>
        public DebugOverlay(PerformanceProfiler profiler = null, FramePacer framePacer = null)
        {
            _profiler = profiler;
            _framePacer = framePacer;
            _frameTimeHistory = new List<float>();
            _memoryHistory = new List<float>();
            _cpuHistory = new List<float>();
            _gpuHistory = new List<float>();
            _maxHistorySize = 300;
            _visible = false;
            _showFrameTimeGraph = true;
            _showMemoryGraph = true;
            _showCPUGraph = true;
            _showGPUGraph = false;
            _showEntityCount = true;
            _showDrawCalls = true;
            _showPathfindingMetrics = true;
            _showUIMetrics = true;
            _debugFont = new Font("Consolas", 12);
            _textColor = new Color(0, 255, 0, 255); //Lime
            _graphColor = new Color(0, 255, 0, 255); //Lime
            _backgroundColor = new Color(0, 0, 0, 128); //Black with alpha
            _position = new Vector3(10f, 10f, 0f);
            _scale = 1f;

            DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "INFO", "DebugOverlay: Initialized");
        }

        ///<summary>
        ///Draws debug overlay information to the screen.
        ///</summary>
        ///<param name="context">The render context.</param>
        public void Draw(IRenderContext context)
        {
            //Draw FPS counter
            var fpsColor = new Color(255, 255, 255, 255);
            context.DrawText($"FPS: {System.Math.Round(_profiler.CurrentFPS)}", new SASZombieAssaultTD.Engine.VectorMath.Vector3(10f, 10f, 0f), fpsColor);

            //Draw memory usage
            var memColor = new Color(255, 255, 255, 255);
            context.DrawText($"Memory: {_memoryHistory[_memoryHistory.Count - 1] / 1024f:F1}KB", new SASZombieAssaultTD.Engine.VectorMath.Vector3(10f, 30f, 0f), memColor);

            //Draw entity count
            var entityColor = new Color(255, 255, 255, 255);
            context.DrawText($"Entities: {_entityCount}", new SASZombieAssaultTD.Engine.VectorMath.Vector3(10f, 50f, 0f), entityColor);

            //Draw draw calls
            var drawColor = new Color(255, 255, 255, 255);
            var drawCallCount = _profiler?.GetType().GetProperty("DrawCallCount")?.GetValue(_profiler) ?? 0;
            context.DrawText($"Draw Calls: {drawCallCount}", new SASZombieAssaultTD.Engine.VectorMath.Vector3(10f, 70f, 0f), drawColor);
        }

        ///<summary>
        ///Updates the debug overlay.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        public void Update(float deltaTime)
        {
            if (!_visible)
                return;

            //Update history data
            UpdateHistoryData();
        }

        ///<summary>
        ///Renders the debug overlay.
        ///</summary>
        ///<param name="spriteBatch">The sprite batch to use for rendering.</param>
        public void Render(SpriteBatch spriteBatch)
        {
            if (!_visible || spriteBatch == null)
                return;

            try
            {
                //Render background
                RenderBackground(spriteBatch);

                //Render performance metrics
                RenderPerformanceMetrics(spriteBatch);

                //Render graphs
                if (_showFrameTimeGraph)
                    RenderFrameTimeGraph(spriteBatch);

                if (_showMemoryGraph)
                    RenderMemoryGraph(spriteBatch);

                if (_showCPUGraph)
                    RenderCPUGraph(spriteBatch);

                if (_showGPUGraph)
                    RenderGPUGraph(spriteBatch);

                //Render additional metrics
                if (_showEntityCount)
                    RenderEntityCount(spriteBatch);

                if (_showDrawCalls)
                    RenderDrawCalls(spriteBatch);

                if (_showPathfindingMetrics)
                    RenderPathfindingMetrics(spriteBatch);

                if (_showUIMetrics)
                    RenderUIMetrics(spriteBatch);
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "ERROR", $"DebugOverlay: Failed to render - {ex.Message}");
            }
        }

        ///<summary>
        ///Toggles overlay visibility.
        ///</summary>
        public void ToggleVisibility()
        {
            _visible = !_visible;
            OnVisibilityChanged?.Invoke(_visible);
            DLogger.Log(LogSubsystems.Performance, LogLevel.Info, "INFO", $"DebugOverlay: Visibility set to {_visible}");
        }

        ///<summary>
        ///Updates history data.
        ///</summary>
        private void UpdateHistoryData()
        {
            //Update frame time history
            if (_profiler != null)
            {
                var frameMetric = _profiler.GetMetric("Frame");
                if (frameMetric != null)
                {
                    _frameTimeHistory.Add(frameMetric.AverageTime);
                    while (_frameTimeHistory.Count > _maxHistorySize)
                    {
                        _frameTimeHistory.RemoveAt(0);
                    }
                }
            }

            //Update memory history
            var currentMemory = GC.GetTotalMemory(false) / 1024f / 1024f; //MB
            _memoryHistory.Add(currentMemory);
            while (_memoryHistory.Count > _maxHistorySize)
            {
                _memoryHistory.RemoveAt(0);
            }

            //Update CPU history
            if (_profiler != null)
            {
                _cpuHistory.Add(_profiler.CPUUsage);
                while (_cpuHistory.Count > _maxHistorySize)
                {
                    _cpuHistory.RemoveAt(0);
                }
            }

            //Update GPU history (placeholder)
            _gpuHistory.Add(0f); //Would get actual GPU usage
            while (_gpuHistory.Count > _maxHistorySize)
            {
                _gpuHistory.RemoveAt(0);
            }
        }

        ///<summary>
        ///Renders the background.
        ///</summary>
        private void RenderBackground(SpriteBatch spriteBatch)
        {
            //This would render a semi-transparent background
            //For now, we'll just log the operation
            DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", "DebugOverlay: Rendered background");
        }

        ///<summary>
        ///Renders performance metrics.
        ///</summary>
        private void RenderPerformanceMetrics(SpriteBatch spriteBatch)
        {
            var lines = new List<string>();

            if (_profiler != null)
            {
                lines.Add($"FPS: {_profiler.CurrentFPS:F1}");
                lines.Add($"Frame Time: {_profiler.FrameTimeMs:F2}ms");
                lines.Add($"CPU: {_profiler.CPUUsage:F1}%");
                lines.Add($"Memory: {GC.GetTotalMemory(false) / 1024f / 1024f:F1}MB");
            }

            if (_framePacer != null)
            {
                lines.Add($"Target FPS: {_framePacer.TargetFPS}");
                lines.Add($"Dropped Frames: {_framePacer.DroppedFrames}");
            }

            RenderTextLines(spriteBatch, lines, _position);
        }

        ///<summary>
        ///Renders the frame time graph.
        ///</summary>
        private void RenderFrameTimeGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _frameTimeHistory, "Frame Time (ms)", 16.67f, new Color(255, 0, 0, 255)); //Red
        }

        ///<summary>
        ///Renders the memory graph.
        ///</summary>
        private void RenderMemoryGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _memoryHistory, "Memory (MB)", 0f, new Color(0, 0, 255, 255)); //Blue
        }

        ///<summary>
        ///Renders the CPU graph.
        ///</summary>
        private void RenderCPUGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _cpuHistory, "CPU (%)", 0f, new Color(255, 255, 0, 255)); //Yellow
        }

        ///<summary>
        ///Renders the GPU graph.
        ///</summary>
        private void RenderGPUGraph(SpriteBatch spriteBatch)
        {
            RenderGraph(spriteBatch, _gpuHistory, "GPU (%)", 0f, new Color(0, 255, 255, 255)); //Cyan
        }

        ///<summary>
        ///Renders a generic graph.
        ///</summary>
        private void RenderGraph(SpriteBatch spriteBatch, List<float> data, string title, float targetValue, Color color)
        {
            if (data.Count < 2)
                return;

            var graphPosition = new Vector3(_position.X, _position.Y + 120, 0f);
            var graphSize = new Vector3(200f, 60f, 0f);

            //This would render the actual graph
            //For now, we'll just log the operation
            DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", $"DebugOverlay: Rendered {title} graph with {data.Count} data points");
        }

        ///<summary>
        ///Renders entity count.
        ///</summary>
        private void RenderEntityCount(SpriteBatch spriteBatch)
        {
            //This would get actual entity count
            var entityCount = 0; //Would get from entity system
            var lines = new List<string> { $"Entities: {entityCount}" };
            var position = new Vector3(_position.X, _position.Y + 200, 0f);
            RenderTextLines(spriteBatch, lines, position);
        }

        ///<summary>
        ///Renders draw calls.
        ///</summary>
        private void RenderDrawCalls(SpriteBatch spriteBatch)
        {
            //This would get actual draw call count
            var drawCalls = 0; //Would get from renderer
            var lines = new List<string> { $"Draw Calls: {drawCalls}" };
            var position = new Vector3(_position.X, _position.Y + 220, 0f);
            RenderTextLines(spriteBatch, lines, position);
        }

        ///<summary>
        ///Renders pathfinding metrics.
        ///</summary>
        private void RenderPathfindingMetrics(SpriteBatch spriteBatch)
        {
            //This would get actual pathfinding metrics
            var pathfinders = 0; //Would get from pathfinding system
            var cacheHits = 0; //Would get from pathfinding system
            var lines = new List<string>
            {
                $"Pathfinders: {pathfinders}",
                $"Cache Hits: {cacheHits}"
            };
            var position = new Vector3(_position.X, _position.Y + 240, 0f);
            RenderTextLines(spriteBatch, lines, position);
        }

        ///<summary>
        ///Renders UI metrics.
        ///</summary>
        private void RenderUIMetrics(SpriteBatch spriteBatch)
        {
            //This would get actual UI metrics
            var uiElements = 0; //Would get from UI system
            var lines = new List<string> { $"UI Elements: {uiElements}" };
            var position = new Vector3(_position.X, _position.Y + 280, 0f);
            RenderTextLines(spriteBatch, lines, position);
        }

        ///<summary>
        ///Renders text lines.
        ///</summary>
        private void RenderTextLines(SpriteBatch spriteBatch, List<string> lines, Vector3 position)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var linePosition = new Vector3(position.X, position.Y + i * 15f * _scale, position.Z);
                //This would render the text using the sprite batch
                DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", $"DebugOverlay: Rendered text '{lines[i]}' at {linePosition}");
            }
        }

        ///<summary>
        ///Gets debug overlay information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"DebugOverlay: Visible={_visible}, Position={_position}, " +
            $"Scale={_scale}, FrameHistory={_frameTimeHistory.Count}";
        }
    }
}




