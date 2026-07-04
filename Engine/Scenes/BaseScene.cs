// =========================================================
//  FILE: BaseScene.cs
//  PATH: Engine/Platform/BaseScene.cs
//  SUBSYSTEM: Platform Abstraction Layer
//  ROLE: Defines the deterministic lifecycle contract
//  =========================================================

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Input;
using UISystem = SASZombieAssaultTD.Engine.UI.UISystem;

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Root game object that manages the overall game state and core subsystems.
    /// </summary>
    public class GameRoot
    {
        public bool IsInitialized { get; private set; }
        public bool IsRunning { get; private set; }

        public EntityManager? EntityManager { get; private set; }
        public UIInputRouter? Input { get; private set; }
        public UISystem? UISystem { get; private set; }
        public AnimationSystem? AnimationSystem { get; private set; }
        public EnemySystem? EnemySystem { get; private set; }
        public RenderSystem? RenderSystem { get; private set; }

        public GameRoot()
        {
            IsInitialized = false;
            IsRunning = false;
        }

        /// <summary>
        /// Initializes the game root and its subsystems.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                return;

            // Basic subsystem wiring; replace with your actual factories/DI as needed.
            EntityManager = new EntityManager();
            Input = new UIInputRouter();
            UISystem = new UISystem();
            AnimationSystem = new AnimationSystem();
            EnemySystem = new EnemySystem();
            RenderSystem = new RenderSystem();
            RenderSystem.Initialize();

            IsInitialized = true;
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Start()
        {
            if (!IsInitialized || IsRunning)
                return;

            IsRunning = true;
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
        }
    }

    /// <summary>
    /// Manages animations for entities.
    /// </summary>
    public class AnimationSystem
    {
        private readonly System.Collections.Generic.Dictionary<uint, string> _entityAnimations;
        private readonly System.Collections.Generic.Dictionary<string, float> _animationDurations;

        public AnimationSystem()
        {
            _entityAnimations = new System.Collections.Generic.Dictionary<uint, string>();
            _animationDurations = new System.Collections.Generic.Dictionary<string, float>();

            _animationDurations["idle"] = 1.0f;
            _animationDurations["walk"] = 0.8f;
            _animationDurations["attack"] = 0.5f;
            _animationDurations["death"] = 1.5f;
        }

        public void SetAnimation(uint entityId, string animationName)
        {
            _entityAnimations[entityId] = animationName;
        }

        public string GetAnimation(uint entityId)
        {
            return _entityAnimations.TryGetValue(entityId, out var animation) ? animation : "idle";
        }

        public float GetAnimationDuration(string animationName)
        {
            return _animationDurations.TryGetValue(animationName, out var duration) ? duration : 1.0f;
        }

        public void Update(float deltaTime)
        {
            // Hook animation timing/state here if needed.
        }
    }

    public abstract class BaseScene
    {
        private GameRoot? _gameRoot;
        private SceneManager? _sceneManager;

        protected GameRoot? GameRoot => _gameRoot;
        protected SceneManager? SceneManager => _sceneManager;

        protected EntityManager? EntityManager => _gameRoot?.EntityManager;
        protected UIInputRouter? InputRouter => _gameRoot?.Input;
        protected UISystem? UISystem => _gameRoot?.UISystem;
        protected AnimationSystem? AnimationSystem => _gameRoot?.AnimationSystem;
        protected EnemySystem? EnemySystem => _gameRoot?.EnemySystem;
        protected RenderSystem? RenderSystem => _gameRoot?.RenderSystem;

        public void SetGameRoot(GameRoot gameRoot)
        {
            _gameRoot = gameRoot;
        }

        public void SetSceneManager(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Render(IRenderContext context) { }
        public virtual void OnUpdate(float deltaTime) { }
        public virtual void OnRender(IRenderContext context) { }
        public virtual void LoadContent() { }
        public virtual void Cleanup() { }

        /// <summary>
        /// IDrawingContext bridge; default implementation just wraps a frame around derived scene rendering.
        /// </summary>
        internal void Render(IDrawingContext context)
        {
            if (_gameRoot?.RenderSystem == null)
                return;

            _gameRoot.RenderSystem.BeginFrame();
            // Derived scenes are expected to use Render(IRenderContext) with the engine's render context.
            _gameRoot.RenderSystem.EndFrame();
        }

        internal virtual void OnUnload() { }

        internal abstract void OnLoad();
        internal abstract void OnStart();
    }
}
