/*
File:    BaseScene.cs
Purpose: SceneManager and GameRoot references; protected accessors for systems.
*/
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Input;
using System.Collections.Generic;
using UISystem = SASZombieAssaultTD.Engine.UI.UISystem;
using SASZombieAssaultTD.Engine.Extensions;
// P11-08-01: Updated to work with decomposed WaveSystem structure

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Manages animations for entities.
    /// </summary>
    public class AnimationSystem
    {
        private readonly Dictionary<uint, string> _entityAnimations;
        private readonly Dictionary<string, float> _animationDurations;

        public AnimationSystem()
        {
            _entityAnimations = new Dictionary<uint, string>();
            _animationDurations = new Dictionary<string, float>();

            // Initialize default animations
            _animationDurations["idle"] = 1.0f;
            _animationDurations["walk"] = 0.8f;
            _animationDurations["attack"] = 0.5f;
            _animationDurations["death"] = 1.5f;
        }

        /// <summary>
        /// Sets an animation for an entity.
        /// </summary>
        public void SetAnimation(uint entityId, string animationName)
        {
            _entityAnimations[entityId] = animationName;
        }

        /// <summary>
        /// Gets the current animation for an entity.
        /// </summary>
        public string GetAnimation(uint entityId)
        {
            return _entityAnimations.TryGetValue(entityId, out var animation) ? animation : "idle";
        }

        /// <summary>
        /// Gets the duration of an animation.
        /// </summary>
        public float GetAnimationDuration(string animationName)
        {
            return _animationDurations.TryGetValue(animationName, out var duration) ? duration : 1.0f;
        }

        /// <summary>
        /// Updates animations for all entities.
        /// </summary>
        public void Update(float deltaTime)
        {
            // Animation update logic here
        }
    }

    /// <summary>
    /// Root game object that manages the overall game state.
    /// </summary>
    public class GameRoot
    {
        public bool IsInitialized { get; private set; }
        public bool IsRunning { get; private set; }
        public SASZombieAssaultTD.Engine.ECS.EntityManager? EntityManager { get; private set; }
        public UIInputRouter? Input { get; private set; }
        // public SASZombieAssaultTD.Engine.Gameplay.WaveController? WaveSystem { get; private set; } // Add this property

        public GameRoot()
        {
            IsInitialized = false;
            IsRunning = false;
        }

        /// <summary>
        /// Initializes the game root.
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;

                // Initialize WaveSystem
                // WaveSystem = new SASZombieAssaultTD.Engine.Gameplay.WaveController(); // Initialize the WaveSystem here

                // Other initialization logic here
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Start()
        {
            if (IsInitialized && !IsRunning)
            {
                IsRunning = true;
                // Game start logic here
            }
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                // Game stop logic here
            }
        }
    }

    public abstract class BaseScene
    {
        private GameRoot? _gameRoot;
        private SceneManager? _sceneManager;

        protected GameRoot? GameRoot => _gameRoot;
        protected SceneManager? SceneManager => _sceneManager;

        protected SASZombieAssaultTD.Engine.ECS.EntityManager? EntityManager => _gameRoot?.EntityManager;

        /// <summary>
        /// Gets of input module for input state access.
        /// P11-10-08: Replaces legacy InputSystem with new UIInputRouter.
        /// </summary>
        protected UIInputRouter? InputRouter => _gameRoot?.Input;

        protected UISystem? UISystem => _gameRoot?.UISystem;
        protected AnimationSystem? AnimationSystem => _gameRoot?.AnimationSystem;
        protected SASZombieAssaultTD.Engine.Enemies.EnemySystem? EnemySystem => _gameRoot?.EnemySystem;
        // P11-08-01: Updated to work with decomposed WaveSystem structure
        // protected SASZombieAssaultTD.Engine.Gameplay.WaveController? WaveSystem => _gameRoot?.WaveSystem;
        protected object? RenderSystem => _gameRoot?.RenderSystem;

        /// <summary>
        /// Sets the GameRoot reference for this scene.
        /// </summary>
        /// <param name="gameRoot">The GameRoot instance</param>
        public void SetGameRoot(GameRoot gameRoot)
        {
            _gameRoot = gameRoot;
        }

        /// <summary>
        /// Sets the SceneManager reference for this scene.
        /// </summary>
        /// <param name="sceneManager">The SceneManager instance</param>
        public void SetSceneManager(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        /// <summary>
        /// P11-11-02: Called when the scene becomes active.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// P11-11-02: Called when the scene becomes inactive.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// P11-11-02: Called when scene is initialized.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// P11-11-02: Called during scene update loop.
        /// Default implementation is a no-op.
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// P11-11-02: Called during scene render loop.
        /// Default implementation is a no-op.
        /// </summary>
        /// <param name="context">Render context</param>
        public virtual void Render(IRenderContext context) { }

        /// <summary>
        /// P11-11-02: Called during scene update loop.
        /// Default implementation is a no-op.
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// P11-11-02: Called during scene render loop.
        /// Default implementation is a no-op.
        /// </summary>
        /// <param name="context">Render context</param>
        public virtual void OnRender(IRenderContext context) { }

        /// <summary>
        /// P11-11-02: Called when scene content should be loaded.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// P11-11-02: Called when scene should be cleaned up.
        /// Used to release resources and clean up scene-specific data.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void Cleanup() { }
    }
}
