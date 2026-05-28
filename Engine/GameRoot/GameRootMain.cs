/*
File: GameRootMain.cs
Path: Engine/GameRoot/GameRootMain.cs

Purpose: P11-09-01 - Public API orchestrator for GameRoot system.
Contains only the public API and high-level flow that delegates
to other partial files. No deep logic lives here.

Role: Public entry point and high-level coordinator.
- Provides clean API surface for external systems
- Delegates to specialized partial files
- Maintains public interface compatibility
- High-level game start/shutdown functions

Notes: This is the main partial class that external systems interact with.
All complex logic is delegated to specialized partial files.
*/

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Managers;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Input;
using System;
using System.Threading;
using IRenderContext = SASZombieAssaultTD.Engine.Interfaces.IRenderContext;

namespace SASZombieAssaultTD.Engine
{
    public partial class GameRoot : IProgram
    {
        private readonly ISystemRegistry _systemRegistry;
        private readonly SystemManager _systemManager;
        private readonly UpdateManager _updateManager;
        private readonly RenderManager _renderManager;
        private readonly UIInputRouter _inputManager;
        private readonly IGameStateMachine _stateMachine;
        private readonly IRenderContext _renderContext;

        public SASZombieAssaultTD.Engine.Enemies.EnemySystem? EnemySystem =>
            GetService<SASZombieAssaultTD.Engine.Enemies.EnemySystem>();

        public object? RenderSystem => GetService<RenderManager>();

        public UIInputRouter? Input => _inputManager;

        private bool _isInitialized = false;
        private bool _isRunning = false;
        private readonly object _stateLock = new();

        private DateTime _lastFrameTime;
        private float _frameAccumulator = 0f;
        private const float TargetFrameTime = 1f / 60f;

        public GameRoot(
            ISystemRegistry systemRegistry,
            SystemManager systemManager,
            UpdateManager updateManager,
            RenderManager renderManager,
            UIInputRouter inputRouter,
            IGameStateMachine stateMachine,
            IRenderContext renderContext)
        {
            _systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));
            _systemManager = systemManager ?? throw new ArgumentNullException(nameof(systemManager));
            _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
            _renderManager = renderManager ?? throw new ArgumentNullException(nameof(renderManager));
            _inputManager = inputRouter;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));

            DebugLogger.LogInfo("GameRoot initialized with all managers");
        }

        public bool IsInitialized => _isInitialized;
        public bool IsRunning => _isRunning;

        public EngineState State =>
            _isInitialized
                ? (_isRunning ? EngineState.Running : EngineState.Stopped)
                : EngineState.Uninitialized;

        public void Initialize()
        {
            lock (_stateLock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    DebugLogger.LogInfo("Starting GameRoot initialization...");
                    DebugLogger.LogDebug("TRACE", "GameRoot.Initialize: Enter");

                    PerformInitialization();

                    _isInitialized = true;

                    DebugLogger.LogDebug("TRACE", "GameRoot.Initialize: Exit OK");
                    DebugLogger.LogInfo("GameRoot initialization completed successfully");
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError("GameRoot.Initialize: EXCEPTION");
                    DebugLogger.Exception(ex, "GameRoot.Initialize");

                    Shutdown();
                    throw;
                }
            }
        }

        public void Shutdown()
        {
            lock (_stateLock)
            {
                if (!_isInitialized)
                    return;

                _isRunning = false;

                try
                {
                    DebugLogger.LogInfo("Starting GameRoot shutdown...");
                    DebugLogger.LogDebug("TRACE", "GameRoot.Shutdown: Enter");

                    PerformShutdown();

                    _isInitialized = false;

                    DebugLogger.LogDebug("TRACE", "GameRoot.Shutdown: Exit OK");
                    DebugLogger.LogInfo("GameRoot shutdown completed successfully");
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError("GameRoot.Shutdown: EXCEPTION");
                    DebugLogger.Exception(ex, "GameRoot.Shutdown");
                }
            }
        }

        public void Run()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("GameRoot must be initialized before running.");

            lock (_stateLock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
            }

            _lastFrameTime = DateTime.Now;

            try
            {
                while (_isRunning)
                {
                    var currentTime = DateTime.Now;
                    var deltaTime = (float)(currentTime - _lastFrameTime).TotalSeconds;
                    _lastFrameTime = currentTime;

                    _frameAccumulator += deltaTime;

                    while (_frameAccumulator >= TargetFrameTime)
                    {
                        DebugLogger.LogDebug("TRACE", $"GameRoot.Run: Update({TargetFrameTime})");
                        Update(TargetFrameTime);
                        _frameAccumulator -= TargetFrameTime;
                    }

                    DebugLogger.LogDebug("TRACE", "GameRoot.Run: Render()");
                    Render();

                    var frameTime = (float)(DateTime.Now - currentTime).TotalSeconds;
                    if (frameTime < TargetFrameTime)
                    {
                        var sleepTime = (int)((TargetFrameTime - frameTime) * 1000);
                        Thread.Sleep(sleepTime);
                    }
                }
            }
            finally
            {
                _isRunning = false;
            }
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized || !_isRunning)
                return;

            try
            {
                DebugLogger.LogDebug("TRACE", $"GameRoot.Update: Enter (delta={deltaTime:F4})");

                PerformUpdate(deltaTime);

                DebugLogger.LogDebug("TRACE", "GameRoot.Update: Exit OK");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("GameRoot.Update: EXCEPTION");
                DebugLogger.Exception(ex, "GameRoot.Update");

                Shutdown();
            }
        }

        public void Render()
        {
            if (!_isInitialized || !_isRunning)
                return;

            try
            {
                DebugLogger.LogDebug("TRACE", "GameRoot.Render: Enter");

                PerformRender();

                DebugLogger.LogDebug("TRACE", "GameRoot.Render: Exit OK");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError("GameRoot.Render: EXCEPTION");
                DebugLogger.Exception(ex, "GameRoot.Render");
            }
        }

        public EngineDiagnostics GetDiagnostics()
        {
            lock (_stateLock)
            {
                return GetEngineDiagnostics();
            }
        }

        private EngineDiagnostics GetEngineDiagnostics()
        {
            // Intentionally still guarded until diagnostics pipeline is implemented
            DebugLogger.LogError("GameRoot.GetEngineDiagnostics: NOT IMPLEMENTED");
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        // ======================================================================================
        // IProgram IMPLEMENTATION — public API is the single source of truth
        // ======================================================================================

        void IProgram.Initialize()
        {
            // No guardrail here anymore: Initialize is now implemented.
            // This ensures the first pass succeeds and the system can move on
            // to the next NOT_IMPLEMENTED litmus point.
            DebugLogger.LogDebug("TRACE", "IProgram.Initialize: delegating to GameRoot.Initialize");
            Initialize();
        }

        void IProgram.Update(TimeSpan deltaTime)
        {
            NotImplementedGuard.Hit("IProgram.Update NOT IMPLEMENTED");
            throw new NotImplementedException();
        }

        void IProgram.Render()
        {
            NotImplementedGuard.Hit("IProgram.Render NOT IMPLEMENTED");
            throw new NotImplementedException();
        }

        void IProgram.Shutdown()
        {
            NotImplementedGuard.Hit("IProgram.Shutdown NOT IMPLEMENTED");
            throw new NotImplementedException();
        }
    }

    public enum EngineState
    {
        Uninitialized,
        Initializing,
        Running,
        Stopped,
        ShuttingDown,
        Error
    }

    public class EngineDiagnostics
    {
        public EngineState State { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsRunning { get; set; }
        public float FrameAccumulator { get; set; }
        public float TargetFrameTime { get; set; }

        public Diagnostics.ManagerDiagnostics SystemManagerDiagnostics { get; set; }
        public Diagnostics.ManagerDiagnostics UpdateManagerDiagnostics { get; set; }
        public Diagnostics.ManagerDiagnostics RenderManagerDiagnostics { get; set; }
        public Diagnostics.ManagerDiagnostics InputManagerDiagnostics { get; set; }

        internal static void Trace(string eventName, string details)
        {
            eventName ??= string.Empty;
            details ??= string.Empty;

            try
            {
                var message = $"EngineDiagnostics.Trace: {eventName} | {details}";
                DebugLogger.LogDebug(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"EngineDiagnostics.Trace: Diagnostics failure suppressed | {ex.Message}"
                );
            }
        }
    }
}
