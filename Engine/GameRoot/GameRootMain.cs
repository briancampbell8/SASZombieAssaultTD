// =====================================================================================================
//  FILE: GameRootMain.cs
//  PATH: Engine/GameRoot/GameRootMain.cs
//  SUBSYSTEM: Engine/GameRoot
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program. Implemented
//      by GameRootMain to provide a clean, engine-facing boundary for initialization, execution, update,
//      render dispatch, ticking, and shutdown.
//
//  RESPONSIBILITIES:
//      - Provide a strict lifecycle surface: Initialize → Run → Update → Render → Shutdown.
//      - Allow engine hosts (e.g., GameRootMain) to expose deterministic lifecycle entry points.
//      - Serve as the base contract for any future top-level engine program modules.
//      - Support both GPU-context rendering and generic object-based render forwarding.
//
//  NON-RESPONSIBILITIES:
//      - Implementing update or render logic internally (delegated to subsystems).
//      - Managing system registration, asset loading, or state-machine orchestration.
//      - Handling GPU device creation, swap-chain management, or windowing.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces legacy partial lifecycle methods.
//      - GameRootMain implements this interface and delegates lifecycle operations to:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
//      - Includes legacy compatibility signatures (Render(object), Tick(object,...)) for transitional
//        subsystem support, though the GPU-only pipeline uses Render(D3D11Adapter_Core).
// =====================================================================================================


using System;
using System.Threading;
using SASZombieAssaultTD.Engine.GameRoot;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Scenes;
using SASZombieAssaultTD.Engine.State;
using SASZombieAssaultTD.Engine.Systems;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;

namespace SASZombieAssaultTD.Engine
{
    public sealed class GameRootMain : IDisposable
    {
        private readonly ISystemRegistry _systemRegistry;
        private readonly SystemManager _systemManager;
        private readonly UpdateManager _updateManager;
        private readonly RenderManager _renderManager;
        private readonly UIInputRouter _inputRouter;
        private readonly StateMachine _stateMachine;
        private readonly D3D11Adapter_Core _renderContext;
        private readonly ModernUIRenderer _uiRenderer;

        private SceneManager _sceneManager;

        private readonly GameRootInitialization _initialization;
        private readonly GameRootUpdateLoop _updateLoop;
        private readonly GameRootStateController _stateController;
        private readonly GameRootSystemRegistration _systemRegistration;

        private bool _isInitialized;
        private bool _isRunning;
        private readonly object _stateLock = new();

        private DateTime _lastFrameTime;
        private float _frameAccumulator;
        private const float TargetFrameTime = 1f / 60f;

        public bool Initialized { get; internal set; }
        public bool Running { get; internal set; }
        public ISystemRegistry SystemRegistry { get; internal set; }
        public SystemManager SystemManager { get; internal set; }
        public DateTime LastFrameTime { get; internal set; }

        public GameRootMain(
            ISystemRegistry systemRegistry,
            SystemManager systemManager,
            UpdateManager updateManager,
            RenderManager renderManager,
            UIInputRouter inputRouter,
            StateMachine stateMachine,
            D3D11Adapter_Core renderContext,
            ModernUIRenderer uiRenderer)
        {
            _systemRegistry = systemRegistry ?? throw new ArgumentNullException(nameof(systemRegistry));
            _systemManager = systemManager;
            _updateManager = updateManager;
            _renderManager = renderManager;
            _inputRouter = inputRouter;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _renderContext = renderContext;
            _uiRenderer = uiRenderer;

            SystemRegistry = systemRegistry;
            SystemManager = systemManager;

            _systemRegistration = new GameRootSystemRegistration(
                _systemRegistry,
                _systemManager,
                _updateManager,
                _renderManager,
                _inputRouter,
                _stateMachine,
                _uiRenderer,
                _renderContext);

            _initialization = new GameRootInitialization(
                _systemRegistry,
                _systemManager,
                _updateManager,
                _renderManager,
                _inputRouter,
                _stateMachine,
                _uiRenderer,
                _renderContext);

            _updateLoop = new GameRootUpdateLoop(
                _systemRegistry,
                _systemManager,
                _updateManager,
                _renderManager,
                _stateMachine,
                _renderContext,
                _uiRenderer);

            _stateController = new GameRootStateController(
                _systemRegistry,
                _systemManager,
                _stateMachine);

            _lastFrameTime = DateTime.Now;
            LastFrameTime = _lastFrameTime;
        }

        public void Initialize()
        {
            lock (_stateLock)
            {
                if (_isInitialized)
                    return;

                if (_renderContext != null && _systemRegistry != null)
                {
                    _systemRegistry.Register<D3D11Adapter_Core>(_renderContext);

                    if (_renderContext.DeviceCore != null)
                    {
                        _systemRegistry.Register<D3D11DeviceCore>(_renderContext.DeviceCore);
                    }
                }

                _systemRegistration.RegisterCoreSystems();
                _initialization.Execute();

                _sceneManager = _systemRegistry.Resolve<SceneManager>();

                _isInitialized = true;
                Initialized = true;

                // FIX: DO NOT start the loop here.
                // Run() must be the ONLY place that sets _isRunning = true.

                _stateController.StartGame();
                _sceneManager.SetScene("MainMenu");
            }
        }

        public void Run()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("GameRootMain must be initialized before running.");

            lock (_stateLock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                Running = true;
            }

            _lastFrameTime = DateTime.Now;
            LastFrameTime = _lastFrameTime;

            while (_isRunning)
            {
                var now = DateTime.Now;
                var delta = (float)(now - _lastFrameTime).TotalSeconds;
                _lastFrameTime = now;
                LastFrameTime = _lastFrameTime;

                _frameAccumulator += delta;

                while (_frameAccumulator >= TargetFrameTime)
                {
                    _updateLoop.UpdateFrame(TargetFrameTime);
                    _frameAccumulator -= TargetFrameTime;
                }

                _updateLoop.RenderFrame();

                var frameTime = (float)(DateTime.Now - now).TotalSeconds;
                if (frameTime < TargetFrameTime)
                {
                    var sleepMs = (int)((TargetFrameTime - frameTime) * 1000f);
                    if (sleepMs > 0)
                        Thread.Sleep(sleepMs);
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
                Running = false;

                _stateController.GameOver();
                _stateMachine.Shutdown();

                _isInitialized = false;
                Initialized = false;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized || !_isRunning)
                return;

            lock (_stateLock)
            {
                _updateLoop.UpdateFrame(deltaTime);
            }
        }

        public void Render(D3D11Adapter_Core context)
        {
            if (!_isInitialized || !_isRunning)
                return;

            lock (_stateLock)
            {
                _updateLoop.RenderFrame();
            }
        }

        public void Tick(object gameTime, ElapsedGameTime elapsedGameTime)
        {
        }

        public void Render(object value)
        {
        }
    }
}
