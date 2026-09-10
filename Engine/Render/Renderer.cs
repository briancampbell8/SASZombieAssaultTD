// =========================================================================================
// FILE: Renderer.cs
// PATH: ./Engine/Rendering/
// MODULE: Rendering
// ROLE: Driver program declaring fields, events, properties, and core structures.
// =========================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        // Core Fields
        private IntPtr _gpuContext;
        private IntPtr _renderTarget;
        private Color _clearColor;
        private bool _isInitialized;
        private Vector3 _viewportSize;
        private object _targetFPS;
        private bool _vsyncEnabled;
        private bool _adaptiveVSync;
        private float _renderScale;
        private bool _colorGradingEnabled;
        private Color _colorGradeTint;
        private float _colorGradeContrast;
        private float _colorGradeBrightness;
        private bool _screenshotEnabled;
        private string _screenshotPath;
        private bool _gpuTimingEnabled;
        private List<long> _gpuFrameTimes;
        private static object TheType;
        private static object TheMember;

        // Public Properties
        public IntPtr GpuContext => _gpuContext;
        public IntPtr RenderTarget => _renderTarget;

        public Color ClearColor
        {
            get => _clearColor;
            set => _clearColor = value;
        }

        public Vector3 ViewportSize => _viewportSize;
        public bool IsInitialized => _isInitialized;

        public bool VSyncEnabled
        {
            get => _vsyncEnabled;
            set => _vsyncEnabled = value;
        }

        public float RenderScale
        {
            get => _renderScale;
            set
            {
                _renderScale = System.MathF.Max(0.1f, System.MathF.Min(3.0f, value));
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Debug, $"Renderer: Render scale set to {_renderScale:F2}");
            }
        }

        public bool ColorGradingEnabled
        {
            get => _colorGradingEnabled;
            set
            {
                _colorGradingEnabled = value;
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Debug, $"Renderer: Color grading {(value ? "enabled" : "disabled")}");
            }
        }

        public Color ColorGradeTint
        {
            get => _colorGradeTint;
            set => _colorGradeTint = value;
        }

        public float ColorGradeContrast
        {
            get => _colorGradeContrast;
            set => _colorGradeContrast = System.MathF.Max(0.0f, System.MathF.Min(2.0f, value));
        }

        public float ColorGradeBrightness
        {
            get => _colorGradeBrightness;
            set => _colorGradeBrightness = System.MathF.Max(-1.0f, System.MathF.Min(1.0f, value));
        }

        public bool ScreenshotEnabled
        {
            get => _screenshotEnabled;
            set => _screenshotEnabled = value;
        }

        public bool GpuTimingEnabled
        {
            get => _gpuTimingEnabled;
            set => _gpuTimingEnabled = value;
        }

        // Public Events
        public event Action OnRendererInitialized;
        public event Action OnRendererShutdown;
        public event Action OnRenderTargetChanged;
    }

    internal class CachedFont
    {
        internal CachedFont() { }
    }
}
