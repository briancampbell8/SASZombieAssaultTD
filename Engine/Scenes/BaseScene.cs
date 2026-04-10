/*
File:    BaseScene.cs
Purpose: SceneManager and GameRoot references; protected accessors for systems.
*/
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Input;
using System.Collections.Generic;
using System;
using ModernLoggingSystem = SASZombieAssaultTD.Engine.Core.ModernLoggingSystem;
using UISystem = SASZombieAssaultTD.Engine.UI.UISystem;

// P11-08-01: Updated to work with decomposed WaveSystem structure

namespace SASZombieAssaultTD.Engine.Scenes
{
    /// <summary>
    /// Manages animations for entities.
    /// </summary>
    public class AnimationSystem
    {
        readonly Dictionary<uint, string> _entityAnimations;
        readonly Dictionary<string, float> _animationDurations;

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
        internal UISystem UISystem;

        public bool IsInitialized { get; private set; }
        public bool IsRunning { get; private set; }
        public SASZombieAssaultTD.Engine.ECS.EntityManager? EntityManager { get; private set; }
        public Input.UIInputRouter? Input { get; private set; }
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
        GameRoot? _gameRoot;
        SceneManager? _sceneManager;
        bool _resumeRequested;
        private object previousGameScene;

        protected GameRoot? GameRoot => _gameRoot;
        protected SceneManager? SceneManager => _sceneManager;

        protected SASZombieAssaultTD.Engine.ECS.EntityManager? EntityManager => _gameRoot?.EntityManager;

        /// <summary>
        /// Gets of input module for input state access.
        /// P11-10-08: Replaces legacy InputSystem with new UIInputRouter.
        /// </summary>
        protected SASZombieAssaultTD.Engine.Input.UIInputRouter? InputRouter => _gameRoot?.Input;

        protected UISystem? UISystem => _gameRoot?.UISystem;

        /// <summary>
        /// Gets the animation system for the scene.
        /// Returns a shared instance or creates a new one if needed.
        /// </summary>
        protected AnimationSystem? AnimationSystem 
        { 
            get 
            {
                if (_gameRoot == null) return null;
                // In a full implementation, this would return a shared instance
                // For now, create a new instance per scene
                return _animationSystem ??= new AnimationSystem();
            } 
        }
        
        /// <summary>
        /// Gets the enemy system for the scene.
        /// Returns a shared instance or creates a new one if needed.
        /// </summary>
        protected SASZombieAssaultTD.Engine.Enemies.EnemySystem? EnemySystem 
        { 
            get 
            {
                if (_gameRoot == null) return null;
                // In a full implementation, this would return a shared instance
                // For now, create a new instance per scene
                return _enemySystem ??= new SASZombieAssaultTD.Engine.Enemies.EnemySystem();
            } 
        }
        
        /// <summary>
        /// Gets the render system for the scene.
        /// Returns the static RenderSystem class.
        /// </summary>
        protected object RenderSystem 
        { 
            get 
            {
                // RenderSystem is a static class, return the class itself
                return typeof(SASZombieAssaultTD.Engine.Rendering.RenderSystem);
            } 
        }

        // Private backing fields for lazy-loaded systems
        private AnimationSystem? _animationSystem;
        private SASZombieAssaultTD.Engine.Enemies.EnemySystem? _enemySystem;

        /// <summary>
        /// Sets the GameRoot reference for this scene.
        /// </summary>
        /// <param name="gameRoot">The GameRoot instance</param>
        public void SetGameRoot(GameRoot gameRoot) => _gameRoot = gameRoot;

        /// <summary>
        /// Sets the SceneManager reference for this scene.
        /// </summary>
        /// <param name="sceneManager">The SceneManager instance</param>
        public void SetSceneManager(SceneManager sceneManager) => _sceneManager = sceneManager;

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
        public virtual void Render(SASZombieAssaultTD.Engine.Rendering.IRenderContext context) { }

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
        ///      public virtual void Render(SASZombieAssaultTD.Engine.Rendering.IRenderContext context) { }
        public virtual void OnRender(SASZombieAssaultTD.Engine.Rendering.IRenderContext context) { }

        /// <summary>
        /// P11-11-02: Called when scene content should be loaded.
        /// Loads scene-specific assets and initializes resources.
        /// </summary>
        public virtual void LoadContent()
        {
            ModernLoggingSystem.Log("DEBUG", $"BaseScene: Loading content for {GetType().Name}");
            
            try
            {
                // Load scene-specific assets
                LoadSceneAssets();
                
                // Initialize scene systems
                InitializeSceneSystems();
                
                // Create initial entities
                CreateInitialEntities();
                
                ModernLoggingSystem.Log("INFO", $"BaseScene: Content loading completed for {GetType().Name}");
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("ERROR", $"BaseScene: Error loading content for {GetType().Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads scene-specific assets.
        /// Override in derived classes to load custom assets.
        /// </summary>
        protected virtual void LoadSceneAssets()
        {
            // Default implementation - override in derived classes
            // Examples:
            // - Load textures
            // - Load sounds
            // - Load models
            // - Load UI elements
        }

        /// <summary>
        /// Initializes scene-specific systems.
        /// Override in derived classes to setup custom systems.
        /// </summary>
        protected virtual void InitializeSceneSystems()
        {
            // Default implementation - override in derived classes
            // Examples:
            // - Initialize physics system
            // - Setup audio system
            // - Configure rendering system
            // - Initialize game logic systems
        }

        /// <summary>
        /// Creates initial entities for the scene.
        /// Override in derived classes to create scene-specific entities.
        /// </summary>
        protected virtual void CreateInitialEntities()
        {
            // Default implementation - override in derived classes
            // Examples:
            // - Create player entities
            // - Create environment objects
            // - Spawn initial enemies
            // - Setup UI elements
        }

        /// <summary>
        /// P11-11-02: Called when scene should be cleaned up.
        /// Used to release resources and clean up scene-specific data.
        /// Default implementation is a no-op.
        /// </summary>
        public virtual void Cleanup() { }

        /// <summary>
        /// P11-11-02: Called every frame to update pause menu logic.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        // Default behavior for scenes that don't override it

        public virtual void OnUpdate(float deltaTime, InputData inputData)
        {
            // Handle pause menu input - check if the user has requested to resume.
            // P11-11-06: Input is now accessible through the Input property
            if (inputData != null)
            {
                // Check for ESC key press to resume game
                if (SASZombieAssaultTD.Engine.Input.InputSystem.IsKeyPressed(KeyCode.Escape))
                {
                    _resumeRequested = true;
                }
            }

            if (_resumeRequested)
            {
                ModernLoggingSystem.Log("Info", "[PauseScene] Resume requested.");
                // Queue transition back to previous scene via SceneManager
                SceneManager?.QueueScene(name: previousGameScene);
                
            }
        }
    }
}