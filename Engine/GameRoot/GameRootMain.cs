/*
File:    GameRootMain.cs
Path:    Engine/GameRoot/GameRootMain.cs
Purpose: P11-09-01 - Public API orchestrator for GameRoot system.
         Contains only the public API and high-level flow that delegates
         to other partial files. No deep logic lives here.

Role:     Public entry point and high-level coordinator.
         - Provides clean API surface for external systems
         - Delegates to specialized partial files
         - Maintains public interface compatibility
         - High-level game start/shutdown functions

Notes:    This is the main partial class that external systems interact with.
         All complex logic is delegated to specialized partial files.
         No deep implementation details - pure orchestration.
*/

using System;
using SASZombieAssaultTD.Engine.Core;
using System.Threading;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Managers;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Input;
using IRenderContext = SASZombieAssaultTD.Engine.Interfaces.IRenderContext;

namespace SASZombieAssaultTD.Engine
{
    /// <summary>
    /// Central engine orchestrator responsible for coordinating all engine systems
    /// and managers. Provides the main game loop, initialization, and shutdown
    /// coordination for the entire engine.
    /// </summary>
    public partial class GameRoot
    {
        private readonly ISystemRegistry _systemRegistry;
        private readonly SystemManager _systemManager;
        private readonly UpdateManager _updateManager;
        private readonly RenderManager _renderManager;
        private readonly UIInputRouter _inputManager;
        private readonly IGameStateMachine _stateMachine;
        private readonly IRenderContext _renderContext;

        // System properties for external access
        // public GetUISystem? GetUISystem => null;
        // public AnimationSystem? AnimationSystem => null;
        public SASZombieAssaultTD.Engine.Enemies.EnemySystem? EnemySystem => GetService<SASZombieAssaultTD.Engine.Enemies.EnemySystem>();
        public object? RenderSystem => GetService<RenderManager>();
        public UIInputRouter? Input => _inputManager;

        // Engine state
        private bool _isInitialized = false;
        private bool _isRunning = false;
        private readonly object _stateLock = new();

        // Performance tracking
        private DateTime _lastFrameTime;
        private float _frameAccumulator = 0f;
        private const float TargetFrameTime = 1f / 60f; // 60 FPS target

        /// <summary>
        /// Initializes a new instance of GameRoot with all required managers.
        /// </summary>
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

            ModernLoggingSystem.LogInfo("GameRoot initialized with all managers");
        }

        /// <summary>
        /// Gets whether the engine is currently initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets whether the engine is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Gets the current engine state.
        /// </summary>
        public EngineState State => _isInitialized ? (_isRunning ? EngineState.Running : EngineState.Stopped) : EngineState.Uninitialized;

        /// <summary>
        /// Initializes all engine systems and managers in the correct order.
        /// </summary>
        public void Initialize()
        {
            lock (_stateLock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    ModernLoggingSystem.LogInfo("Starting GameRoot initialization...");

                    // Delegate to Initialization partial
                    PerformInitialization();

                    _isInitialized = true;
                    ModernLoggingSystem.LogInfo("GameRoot initialization completed successfully");
                }
                catch (Exception ex)
                {
                    ModernLoggingSystem.Log("ERROR", $"GameRoot initialization failed: {ex.Message}");
                    ModernLoggingSystem.Exception(ex, "GameRoot initialization");
                    Shutdown(); // Cleanup on failure
                    throw;
                }
            }
        }

        /// <summary>
        /// Shuts down all engine systems and managers in the correct order.
        /// </summary>
        public void Shutdown()
        {
            lock (_stateLock)
            {
                if (!_isInitialized)
                    return;

                _isRunning = false; // Stop game loop

                try
                {
                    ModernLoggingSystem.LogInfo("Starting GameRoot shutdown...");

                    // Delegate to Initialization partial
                    PerformShutdown();

                    _isInitialized = false;
                    ModernLoggingSystem.LogInfo("GameRoot shutdown completed successfully");
                }
                catch (Exception ex)
                {
                    ModernLoggingSystem.Log("ERROR", $"GameRoot shutdown failed: {ex.Message}");
                    ModernLoggingSystem.Exception(ex, "GameRoot shutdown");
                }
            }
        }

        /// <summary>
        /// Runs the main game loop until shutdown is requested.
        /// </summary>
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

                    // Fixed timestep with accumulator
                    _frameAccumulator += deltaTime;

                    while (_frameAccumulator >= TargetFrameTime)
                    {
                        Update(TargetFrameTime);
                        _frameAccumulator -= TargetFrameTime;
                    }

                    Render();

                    // Frame rate limiting
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

        /// <summary>
        /// Updates all engine systems for the current frame.
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds.</param>
        public void Update(float deltaTime)
        {
            if (!_isInitialized || !_isRunning)
                return;

            try
            {
                // Delegate to UpdateLoop partial
                PerformUpdate(deltaTime);
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Game update failed: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Game update");
                Shutdown();
            }
        }

        /// <summary>
        /// Renders all engine systems for the current frame.
        /// </summary>
        public void Render()
        {
            if (!_isInitialized || !_isRunning)
                return;

            try
            {
                // Delegate to UpdateLoop partial
                PerformRender();
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"Game render failed: {ex.Message}");
                ModernLoggingSystem.Exception(ex, "Game render");
                // Continue running even if render fails
            }
        }

        /// <summary>
        /// Gets diagnostic information about the engine.
        /// </summary>
        /// <returns>Engine diagnostic information.</returns>
        public EngineDiagnostics GetDiagnostics()
        {
            lock (_stateLock)
            {
                // Delegate to Debug partial
                return GetEngineDiagnostics();
            }
        }
    }

    /// <summary>
    /// Represents the current state of the engine.
    /// </summary>
    public enum EngineState
    {
        Uninitialized,
        Initializing,
        Running,
        Stopped,
        ShuttingDown,
        Error
    }

    /// <summary>
    /// Diagnostic information about the engine.
    /// </summary>
    public class EngineDiagnostics
    {
        public EngineState State { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsRunning { get; set; }
        public float FrameAccumulator { get; set; }
        public float TargetFrameTime { get; set; }
        public SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics SystemManagerDiagnostics { get; set; }
        public SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics UpdateManagerDiagnostics { get; set; }
        public SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics RenderManagerDiagnostics { get; set; }
        public SASZombieAssaultTD.Engine.Interfaces.ManagerDiagnostics InputManagerDiagnostics { get; set; }
    }
}
